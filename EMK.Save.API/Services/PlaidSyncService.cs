using Going.Plaid;
using Going.Plaid.Transactions;
using EMK.Save.API.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace EMK.Save.API.Services;

public interface IPlaidSyncService
{
    Task<int> SyncAsync(Guid accountId, bool rollback = false);
}

/// <summary>
/// Pulls real transactions from Plaid via /transactions/sync (cursor-based) for the Item backing
/// an account, upserts them, advances the stored cursor, and broadcasts the update over SignalR.
/// Shared by the manual "sync now" endpoint and the SYNC_UPDATES_AVAILABLE webhook handler.
/// </summary>
public class PlaidSyncService : IPlaidSyncService
{
    private readonly DbContextOptions<SaveEntities> _options;
    private readonly ILogger<PlaidSyncService>       _logger;
    private readonly PlaidClient                     _plaidClient;
    private readonly ITokenEncryptor                 _tokenEncryptor;
    private readonly IHubContext<SaveHub>            _hub;

    public PlaidSyncService(ILogger<PlaidSyncService> logger, DbContextOptions<SaveEntities> options,
        PlaidClient plaidClient, ITokenEncryptor tokenEncryptor, IHubContext<SaveHub> hub)
    {
        _logger         = logger;
        _options        = options;
        _plaidClient    = plaidClient;
        _tokenEncryptor = tokenEncryptor;
        _hub            = hub;
    }

    public async Task<int> SyncAsync(Guid accountId, bool rollback = false)
    {
        var accountManager = new PlaidAccountManager(_options, _logger);
        SyncContext ctx = accountManager.GetSyncContext(accountId);
        string accessToken = _tokenEncryptor.Decrypt(ctx.EncryptedAccessToken);
        Dictionary<string, Guid> accountIdMap = accountManager.GetAccountIdMap(ctx.PlaidItemId);

        var transactionManager = new TransactionManager(_options, _logger);
        string? cursor = ctx.SyncCursor;
        int newCount = 0;
        bool hasMore = true;

        while (hasMore)
        {
            var syncResponse = await _plaidClient.TransactionsSyncAsync(new TransactionsSyncRequest
            {
                AccessToken = accessToken,
                Cursor = cursor,
            });

            var toUpsert = new List<Transaction>();
            foreach (var t in syncResponse.Added.Concat(syncResponse.Modified))
            {
                if (string.IsNullOrEmpty(t.AccountId) || !accountIdMap.TryGetValue(t.AccountId, out Guid ourAccountId))
                    continue;

                DateTime? postedDate = t.AuthorizedDate.HasValue
                    ? t.AuthorizedDate.Value.ToDateTime(TimeOnly.MinValue)
                    : null;

                toUpsert.Add(new Transaction
                {
                    PlaidAccountId     = ourAccountId,
                    SharedBudgetId     = ctx.SharedBudgetId,
                    PlaidTransactionId = t.TransactionId ?? string.Empty,
                    TransactionDate    = t.Date.GetValueOrDefault().ToDateTime(TimeOnly.MinValue),
                    PostedDate         = postedDate,
                    MerchantName       = t.MerchantName ?? t.OriginalDescription ?? string.Empty,
                    Description        = t.OriginalDescription ?? string.Empty,
                    Amount             = -(t.Amount ?? 0m), // Plaid: positive = money out; ours: negative = expense
                    IsoCurrencyCode    = t.IsoCurrencyCode ?? "USD",
                    PlaidCategory      = t.PersonalFinanceCategory?.Primary.ToString() ?? string.Empty,
                    PlaidSubcategory   = t.PersonalFinanceCategory?.Detailed.ToString() ?? string.Empty,
                    IsPending          = t.Pending ?? false,
                    Notes              = string.Empty,
                });
            }

            newCount += await transactionManager.UpsertFromPlaidAsync(toUpsert, rollback);

            foreach (var removed in syncResponse.Removed)
                await transactionManager.RemoveByPlaidTransactionIdAsync(removed.TransactionId);

            cursor = syncResponse.NextCursor;
            hasMore = syncResponse.HasMore;
        }

        await accountManager.UpdateSyncCursorAsync(ctx.PlaidItemId, cursor ?? string.Empty);

        await _hub.Clients.Group(SaveHub.BudgetGroup(ctx.SharedBudgetId))
            .SendAsync("TransactionsSynced", new { sharedBudgetId = ctx.SharedBudgetId, newCount });

        _logger.LogInformation("Synced {Count} new transaction(s) for account {AccountId}", newCount, accountId);
        return newCount;
    }
}
