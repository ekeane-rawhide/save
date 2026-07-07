namespace EMK.Save.API.Services;

using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Options;
using WebPush;

public record WebPushResult(bool Success, bool SubscriptionGone, string? ErrorMessage);

public interface IWebPushSender
{
    Task<WebPushResult> SendAsync(NotificationPreference preference, PushNotification notification, CancellationToken ct = default);
}

/// <summary>Sends a real Web Push message (VAPID-signed) to the browser's push service — see sw.ts for the client-side `push` handler.</summary>
public class WebPushSender : IWebPushSender
{
    private readonly WebPushClient _client;
    private readonly VapidDetails _vapidDetails;
    private readonly ILogger<WebPushSender> _logger;

    public WebPushSender(HttpClient httpClient, IOptions<WebPushSettings> settings, ILogger<WebPushSender> logger)
    {
        _logger = logger;
        var s = settings.Value;

        if (string.IsNullOrWhiteSpace(s.PublicKey) || string.IsNullOrWhiteSpace(s.PrivateKey))
        {
            _logger.LogWarning(
                "WebPush:PublicKey/PrivateKey are not configured — push notifications will be queued but never sent. " +
                "Set them via `dotnet user-secrets set \"WebPush:PrivateKey\" \"...\"`.");
        }

        _client = new WebPushClient(httpClient);
        _vapidDetails = new VapidDetails(s.Subject, s.PublicKey, s.PrivateKey);
    }

    public async Task<WebPushResult> SendAsync(NotificationPreference preference, PushNotification notification, CancellationToken ct = default)
    {
        if (!preference.HasValidSubscription)
            return new WebPushResult(false, false, "No valid push subscription for this user.");

        if (string.IsNullOrWhiteSpace(_vapidDetails.PrivateKey))
            return new WebPushResult(false, false, "WebPush VAPID keys are not configured on the server.");

        var subscription = new PushSubscription(preference.PushEndpoint, preference.P256dhKey, preference.AuthKey);
        string payload = JsonSerializer.Serialize(new
        {
            title = notification.Title,
            body = notification.Body,
            icon = notification.Icon,
            actionUrl = notification.ActionUrl,
        });

        try
        {
            await _client.SendNotificationAsync(subscription, payload, _vapidDetails, ct);
            return new WebPushResult(true, false, null);
        }
        catch (WebPushException ex) when (ex.StatusCode is HttpStatusCode.NotFound or HttpStatusCode.Gone)
        {
            _logger.LogInformation("Push subscription gone (endpoint no longer valid) for user notification {Id}", notification.Id);
            return new WebPushResult(false, true, ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to send web push for notification {Id}", notification.Id);
            return new WebPushResult(false, false, ex.Message);
        }
    }
}
