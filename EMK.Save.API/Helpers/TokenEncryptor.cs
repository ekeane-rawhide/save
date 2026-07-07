namespace EMK.Save.API.Helpers;

using Microsoft.Extensions.Options;
using System.Security.Cryptography;

public interface ITokenEncryptor
{
    string Encrypt(string plaintext);
    string Decrypt(string encoded);
}

/// <summary>AES-GCM encryption for secrets at rest (e.g. Plaid access tokens). Format: base64(nonce || ciphertext || tag).</summary>
public class TokenEncryptor : ITokenEncryptor
{
    private const int NonceSize = 12;
    private const int TagSize = 16;
    private readonly byte[] _key;

    public TokenEncryptor(IOptions<EncryptionSettings> settings)
    {
        string key = settings.Value.PlaidTokenKey;
        if (string.IsNullOrWhiteSpace(key))
            throw new InvalidOperationException(
                "Encryption:PlaidTokenKey is not configured. Set it via `dotnet user-secrets set \"Encryption:PlaidTokenKey\" \"<base64-32-bytes>\"`.");

        _key = Convert.FromBase64String(key);
    }

    public string Encrypt(string plaintext)
    {
        byte[] nonce = RandomNumberGenerator.GetBytes(NonceSize);
        byte[] plaintextBytes = Encoding.UTF8.GetBytes(plaintext);
        byte[] ciphertext = new byte[plaintextBytes.Length];
        byte[] tag = new byte[TagSize];

        using var aes = new AesGcm(_key, TagSize);
        aes.Encrypt(nonce, plaintextBytes, ciphertext, tag);

        byte[] result = new byte[NonceSize + ciphertext.Length + TagSize];
        Buffer.BlockCopy(nonce, 0, result, 0, NonceSize);
        Buffer.BlockCopy(ciphertext, 0, result, NonceSize, ciphertext.Length);
        Buffer.BlockCopy(tag, 0, result, NonceSize + ciphertext.Length, TagSize);

        return Convert.ToBase64String(result);
    }

    public string Decrypt(string encoded)
    {
        byte[] data = Convert.FromBase64String(encoded);
        byte[] nonce = data[..NonceSize];
        byte[] tag = data[^TagSize..];
        byte[] ciphertext = data[NonceSize..^TagSize];
        byte[] plaintextBytes = new byte[ciphertext.Length];

        using var aes = new AesGcm(_key, TagSize);
        aes.Decrypt(nonce, ciphertext, tag, plaintextBytes);

        return Encoding.UTF8.GetString(plaintextBytes);
    }
}
