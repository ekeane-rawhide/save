namespace EMK.Save.API.Services;

public interface INotificationDispatchService
{
    Task<Guid> DispatchOverageAsync(
        Guid sharedBudgetId, Guid userId, Guid categoryId, string categoryName, decimal overageAmount, bool rollback = false);

    Task<Guid> DispatchTransactionAsync(
        Guid sharedBudgetId, Guid userId, Guid transactionId, string merchantName, decimal amount, bool rollback = false);

    Task<Guid> DispatchAccountErrorAsync(
        Guid sharedBudgetId, Guid userId, string institutionName, bool rollback = false);
}

/// <summary>
/// Queues a PushNotification row (via PushNotificationManager, which already gates on the user's
/// preferences) AND actually sends it as a real Web Push message, recording the delivery outcome.
/// Centralized here so every call site — controllers and the Plaid webhook handler alike — gets
/// real delivery for free instead of duplicating the queue-then-send dance.
/// </summary>
public class NotificationDispatchService : INotificationDispatchService
{
    private readonly DbContextOptions<SaveEntities> _options;
    private readonly ILogger<NotificationDispatchService> _logger;
    private readonly IWebPushSender _webPushSender;

    public NotificationDispatchService(
        ILogger<NotificationDispatchService> logger, DbContextOptions<SaveEntities> options, IWebPushSender webPushSender)
    {
        _logger = logger;
        _options = options;
        _webPushSender = webPushSender;
    }

    public async Task<Guid> DispatchOverageAsync(
        Guid sharedBudgetId, Guid userId, Guid categoryId, string categoryName, decimal overageAmount, bool rollback = false)
    {
        var manager = new PushNotificationManager(_options, _logger);
        Guid id = await manager.QueueOverageNotificationAsync(sharedBudgetId, userId, categoryId, categoryName, overageAmount, rollback);
        if (id != Guid.Empty) await SendAsync(manager, userId, id, rollback);
        return id;
    }

    public async Task<Guid> DispatchTransactionAsync(
        Guid sharedBudgetId, Guid userId, Guid transactionId, string merchantName, decimal amount, bool rollback = false)
    {
        var manager = new PushNotificationManager(_options, _logger);
        Guid id = await manager.QueueTransactionNotificationAsync(sharedBudgetId, userId, transactionId, merchantName, amount, rollback);
        if (id != Guid.Empty) await SendAsync(manager, userId, id, rollback);
        return id;
    }

    public async Task<Guid> DispatchAccountErrorAsync(
        Guid sharedBudgetId, Guid userId, string institutionName, bool rollback = false)
    {
        var manager = new PushNotificationManager(_options, _logger);
        Guid id = await manager.QueueAccountErrorNotificationAsync(sharedBudgetId, userId, institutionName, rollback);
        if (id != Guid.Empty) await SendAsync(manager, userId, id, rollback);
        return id;
    }

    private async Task SendAsync(PushNotificationManager manager, Guid userId, Guid notificationId, bool rollback)
    {
        try
        {
            var prefManager = new NotificationPreferenceManager(_options, _logger);
            NotificationPreference preference = await prefManager.LoadByUserAsync(userId);
            PushNotification notification = await manager.LoadByIdAsync(notificationId);

            WebPushResult result = await _webPushSender.SendAsync(preference, notification);
            await manager.MarkSentAsync(notificationId, result.Success, result.ErrorMessage ?? string.Empty, rollback);

            if (result.SubscriptionGone)
                await prefManager.RevokeSubscriptionAsync(userId, rollback);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to dispatch web push for notification {Id}", notificationId);
        }
    }
}
