namespace EMK.Save.API.Controllers;

/// <summary>
/// Receives Plaid webhooks (item/transaction lifecycle events). Anonymous by design — Plaid's
/// servers call this directly, not through our JWT auth. See PlaidWebhookRequest for the
/// production signature-verification caveat.
/// </summary>
[Route("api/plaid")]
[ApiController]
public class PlaidWebhookController : ControllerBase
{
    private readonly DbContextOptions<SaveEntities>  options;
    private readonly ILogger<PlaidWebhookController> logger;
    private readonly IPlaidSyncService               syncService;

    public PlaidWebhookController(ILogger<PlaidWebhookController> logger,
                                  DbContextOptions<SaveEntities>  options,
                                  IPlaidSyncService               syncService)
    {
        this.logger      = logger;
        this.options     = options;
        this.syncService = syncService;
    }

    [HttpPost("webhook")]
    public async Task<ActionResult> Webhook([FromBody] PlaidWebhookRequest payload)
    {
        try
        {
            logger.LogInformation("Plaid webhook received: {Type}/{Code} for item {ItemId}",
                payload.WebhookType, payload.WebhookCode, payload.ItemId);

            var accountManager = new PlaidAccountManager(options, logger);
            List<PlaidAccount> accounts = await accountManager.LoadByItemAsync(payload.ItemId);

            if (accounts.Count == 0)
            {
                logger.LogWarning("Plaid webhook for unknown item {ItemId} — ignoring", payload.ItemId);
                return Ok();
            }

            switch (payload.WebhookType, payload.WebhookCode)
            {
                case ("TRANSACTIONS", "SYNC_UPDATES_AVAILABLE"):
                case ("TRANSACTIONS", "INITIAL_UPDATE"):
                case ("TRANSACTIONS", "HISTORICAL_UPDATE"):
                case ("TRANSACTIONS", "DEFAULT_UPDATE"):
                    await syncService.SyncAsync(accounts[0].Id);
                    break;

                case ("ITEM", "ERROR"):
                {
                    var notificationManager = new PushNotificationManager(options, logger);
                    foreach (Guid userId in accounts.Select(a => a.UserId).Distinct())
                    {
                        await notificationManager.QueueAccountErrorNotificationAsync(
                            accounts[0].SharedBudgetId, userId, accounts[0].InstitutionName);
                    }
                    logger.LogWarning("Plaid ITEM_ERROR for item {ItemId} ({Institution})",
                        payload.ItemId, accounts[0].InstitutionName);
                    break;
                }

                default:
                    logger.LogInformation("Unhandled Plaid webhook {Type}/{Code} — no action taken",
                        payload.WebhookType, payload.WebhookCode);
                    break;
            }

            return Ok();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing Plaid webhook {Type}/{Code} for item {ItemId}",
                payload.WebhookType, payload.WebhookCode, payload.ItemId);
            // Plaid retries on non-2xx, but retrying a poison payload forever isn't useful — ack it.
            return Ok();
        }
    }
}
