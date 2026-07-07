namespace EMK.Save.API.Models;

using System.Text.Json.Serialization;

/// <summary>
/// Common envelope for all Plaid webhooks. Plaid sends snake_case JSON, hence the explicit names.
/// The `Plaid-Verification` signature is checked by PlaidWebhookVerificationMiddleware before this
/// ever reaches model binding — see PlaidWebhookVerifier for the ES256/JWK verification itself.
/// </summary>
public class PlaidWebhookRequest
{
    [JsonPropertyName("webhook_type")]
    public string WebhookType { get; set; } = string.Empty;

    [JsonPropertyName("webhook_code")]
    public string WebhookCode { get; set; } = string.Empty;

    [JsonPropertyName("item_id")]
    public string ItemId { get; set; } = string.Empty;
}
