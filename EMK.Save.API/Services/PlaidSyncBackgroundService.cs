namespace EMK.Save.API.Services;

/// <summary>
/// Periodically syncs all Plaid accounts every 5 minutes to keep transaction data fresh.
/// </summary>
public class PlaidSyncBackgroundService : BackgroundService
{
    private readonly ILogger<PlaidSyncBackgroundService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly TimeSpan _syncInterval = TimeSpan.FromMinutes(5);

    public PlaidSyncBackgroundService(ILogger<PlaidSyncBackgroundService> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Plaid sync background service started. Sync interval: {Interval} minutes", _syncInterval.TotalMinutes);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(_syncInterval, stoppingToken);

                using (var scope = _serviceProvider.CreateScope())
                {
                    var dbOptions = scope.ServiceProvider.GetRequiredService<DbContextOptions<SaveEntities>>();
                    var syncService = scope.ServiceProvider.GetRequiredService<IPlaidSyncService>();
                    var scopedLogger = scope.ServiceProvider.GetRequiredService<ILogger<PlaidSyncBackgroundService>>();

                    var manager = new PlaidAccountManager(dbOptions, scopedLogger);
                    var accounts = await manager.GetAllActiveAccountsAsync();

                    if (!accounts.Any())
                    {
                        scopedLogger.LogDebug("No active Plaid accounts to sync");
                        continue;
                    }

                    scopedLogger.LogInformation("Starting sync for {Count} Plaid account(s)", accounts.Count);

                    foreach (var account in accounts)
                    {
                        try
                        {
                            int syncedCount = await syncService.SyncAsync(account.Id);
                            scopedLogger.LogInformation("Synced {Count} transactions for account {AccountId}", syncedCount, account.Id);
                        }
                        catch (Exception ex)
                        {
                            scopedLogger.LogError(ex, "Failed to sync account {AccountId}", account.Id);
                        }
                    }

                    scopedLogger.LogInformation("Completed Plaid sync batch");
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Plaid sync background service is shutting down");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in Plaid sync background service");
            }
        }
    }
}
