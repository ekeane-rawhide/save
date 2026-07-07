namespace EMK.Save.API.Services;

using System.Collections.Concurrent;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Going.Plaid.Entity;
using Going.Plaid.WebhookVerificationKey;
using Microsoft.IdentityModel.Tokens;

public interface IPlaidWebhookVerifier
{
    Task<bool> VerifyAsync(string? signatureJwt, string rawBody);
}

/// <summary>
/// Verifies the `Plaid-Verification` JWT header per Plaid's webhook verification spec:
/// https://plaid.com/docs/api/webhooks/webhook-verification/
/// The JWT is ES256-signed with a key fetched from /webhook_verification_key/get (cached by kid),
/// and its `request_body_sha256` claim must match the raw request body's SHA-256 hash.
/// </summary>
public class PlaidWebhookVerifier : IPlaidWebhookVerifier
{
    private static readonly ConcurrentDictionary<string, (JWKPublicKey Key, DateTime CachedAt)> KeyCache = new();
    private static readonly TimeSpan KeyCacheDuration = TimeSpan.FromHours(24);
    private static readonly TimeSpan MaxSignatureAge = TimeSpan.FromMinutes(5);

    private readonly PlaidClient _plaidClient;
    private readonly ILogger<PlaidWebhookVerifier> _logger;

    public PlaidWebhookVerifier(PlaidClient plaidClient, ILogger<PlaidWebhookVerifier> logger)
    {
        _plaidClient = plaidClient;
        _logger = logger;
    }

    public async Task<bool> VerifyAsync(string? signatureJwt, string rawBody)
    {
        if (string.IsNullOrEmpty(signatureJwt))
        {
            _logger.LogWarning("Plaid webhook rejected: missing Plaid-Verification header");
            return false;
        }

        try
        {
            var handler = new JwtSecurityTokenHandler();
            string? kid = handler.ReadJwtToken(signatureJwt).Header.Kid;
            if (string.IsNullOrEmpty(kid))
            {
                _logger.LogWarning("Plaid webhook rejected: no kid in signature header");
                return false;
            }

            JWKPublicKey key = await GetKeyAsync(kid);
            if (key.ExpiredAt.HasValue)
            {
                _logger.LogWarning("Plaid webhook rejected: signing key {Kid} has been rotated/expired", kid);
                return false;
            }

            using ECDsa ecdsa = ECDsa.Create(new ECParameters
            {
                Curve = ECCurve.NamedCurves.nistP256,
                Q = new ECPoint { X = Base64UrlDecode(key.X), Y = Base64UrlDecode(key.Y) },
            });

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false, // we enforce our own 5-minute freshness window below
                IssuerSigningKey = new ECDsaSecurityKey(ecdsa),
                ValidAlgorithms = [SecurityAlgorithms.EcdsaSha256],
            };

            ClaimsPrincipal principal = handler.ValidateToken(signatureJwt, validationParameters, out _);

            if (long.TryParse(principal.FindFirst("iat")?.Value, out long iat))
            {
                var issuedAt = DateTimeOffset.FromUnixTimeSeconds(iat);
                if (DateTimeOffset.UtcNow - issuedAt > MaxSignatureAge)
                {
                    _logger.LogWarning("Plaid webhook rejected: signature is older than {Minutes} minutes",
                        MaxSignatureAge.TotalMinutes);
                    return false;
                }
            }

            string? expectedHash = principal.FindFirst("request_body_sha256")?.Value;
            if (string.IsNullOrEmpty(expectedHash))
            {
                _logger.LogWarning("Plaid webhook rejected: no request_body_sha256 claim");
                return false;
            }

            string actualHash = Convert.ToHexString(
                SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(rawBody))).ToLowerInvariant();

            if (!string.Equals(actualHash, expectedHash, StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogWarning("Plaid webhook rejected: request body hash mismatch");
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Plaid webhook signature verification threw");
            return false;
        }
    }

    private async Task<JWKPublicKey> GetKeyAsync(string kid)
    {
        if (KeyCache.TryGetValue(kid, out var cached) && DateTime.UtcNow - cached.CachedAt < KeyCacheDuration)
            return cached.Key;

        var response = await _plaidClient.WebhookVerificationKeyGetAsync(
            new WebhookVerificationKeyGetRequest { KeyId = kid });

        KeyCache[kid] = (response.Key, DateTime.UtcNow);
        return response.Key;
    }

    private static byte[] Base64UrlDecode(string input)
    {
        string s = input.Replace('-', '+').Replace('_', '/');
        s = s.PadRight(s.Length + ((4 - (s.Length % 4)) % 4), '=');
        return Convert.FromBase64String(s);
    }
}
