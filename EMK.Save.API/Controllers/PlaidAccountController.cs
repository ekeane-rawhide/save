using Going.Plaid.Accounts;
using Going.Plaid.Entity;
using Going.Plaid.Item;
using Going.Plaid.Link;

namespace EMK.Save.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PlaidAccountController : ControllerBase
{
    private readonly DbContextOptions<SaveEntities>  options;
    private readonly ILogger<PlaidAccountController> logger;
    private readonly PlaidClient                     plaidClient;
    private readonly ITokenEncryptor                 tokenEncryptor;
    private readonly IPlaidSyncService               syncService;
    private readonly IConfiguration                  configuration;

    public PlaidAccountController(ILogger<PlaidAccountController> logger,
                                  DbContextOptions<SaveEntities>  options,
                                  PlaidClient                     plaidClient,
                                  ITokenEncryptor                 tokenEncryptor,
                                  IPlaidSyncService               syncService,
                                  IConfiguration                  configuration)
    {
        this.logger         = logger;
        this.options        = options;
        this.plaidClient    = plaidClient;
        this.tokenEncryptor = tokenEncryptor;
        this.syncService    = syncService;
        this.configuration  = configuration;
    }

    /// <summary>Returns all active linked accounts for a SharedBudget.</summary>
    [Authorize]
    [HttpGet("{sharedBudgetId}")]
    public async Task<ActionResult<IEnumerable<PlaidAccount>>> Get(Guid sharedBudgetId)
    {
        try
        {
            var manager = new PlaidAccountManager(options, logger);
            return Ok(await manager.LoadAsync(sharedBudgetId));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled error");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An unexpected error occurred." });
        }
    }

    [Authorize]
    [HttpGet("byid/{id}")]
    public async Task<ActionResult<PlaidAccount>> GetById(Guid id)
    {
        try
        {
            var manager = new PlaidAccountManager(options, logger);
            return Ok(await manager.LoadByIdAsync(id));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled error");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An unexpected error occurred." });
        }
    }

    /// <summary>Creates a real Plaid Link token so the PWA can initialize Plaid Link (react-plaid-link).</summary>
    [Authorize]
    [HttpGet("linktoken/{userId}")]
    public async Task<ActionResult<PlaidLinkToken>> GetLinkToken(Guid userId)
    {
        try
        {
            string? webhookUrl = configuration["Plaid:WebhookUrl"];

            var request = new LinkTokenCreateRequest
            {
                User = new LinkTokenCreateRequestUser { ClientUserId = userId.ToString() },
                ClientName = "Save",
                Products = [Products.Transactions],
                CountryCodes = [CountryCode.Us],
                Language = Language.English,
                Webhook = string.IsNullOrWhiteSpace(webhookUrl) ? null : webhookUrl,
            };

            var response = await plaidClient.LinkTokenCreateAsync(request);

            var linkToken = new PlaidLinkToken
            {
                LinkToken  = response.LinkToken,
                Expiration = response.Expiration.DateTime,
                RequestId  = response.RequestId ?? string.Empty,
            };
            logger.LogInformation("Link token created for user {UserId}", userId);
            return Ok(linkToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to create Plaid link token for user {UserId}", userId);
            logger.LogError(ex, "Unhandled error");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An unexpected error occurred." });
        }
    }

    /// <summary>
    /// Exchanges the public_token returned by Plaid Link for a real access_token,
    /// fetches the linked accounts from Plaid, and persists one row per account.
    /// </summary>
    [Authorize]
    [HttpPost("exchange/{sharedBudgetId}/{userId}/{rollback?}")]
    public async Task<ActionResult> Exchange(
        Guid sharedBudgetId, Guid userId,
        [FromBody] PlaidPublicTokenExchange exchange,
        bool rollback = false)
    {
        try
        {
            var exchangeResponse = await plaidClient.ItemPublicTokenExchangeAsync(
                new ItemPublicTokenExchangeRequest { PublicToken = exchange.PublicToken });

            string accessToken = exchangeResponse.AccessToken;
            string encryptedToken = tokenEncryptor.Encrypt(accessToken);

            var accountsResponse = await plaidClient.AccountsGetAsync(
                new AccountsGetRequest { AccessToken = accessToken });

            var manager = new PlaidAccountManager(options, logger);
            var insertedIds = new List<string>();

            foreach (var plaidAccount in accountsResponse.Accounts)
            {
                var account = new PlaidAccount
                {
                    UserId              = userId,
                    SharedBudgetId      = sharedBudgetId,
                    PlaidAccountId      = plaidAccount.AccountId,
                    PlaidItemId         = exchangeResponse.ItemId,
                    InstitutionName     = exchange.InstitutionName,
                    InstitutionLogoUrl  = exchange.InstitutionLogoUrl,
                    AccountName         = plaidAccount.Name,
                    Mask                = plaidAccount.Mask ?? "0000",
                    AccountType         = plaidAccount.Type.ToString(),
                    AccountSubtype      = plaidAccount.Subtype?.ToString() ?? "checking",
                    CurrentBalance      = plaidAccount.Balances.Current ?? 0m,
                    AvailableBalance    = plaidAccount.Balances.Available ?? 0m,
                    IsoCurrencyCode     = plaidAccount.Balances.IsoCurrencyCode ?? "USD",
                    IsActive            = true,
                    LastSynced          = DateTime.Now,
                    DateLinked          = DateTime.Now
                };

                Guid id = await manager.InsertAsync(account, encryptedToken, rollback);
                insertedIds.Add(id.ToString());

                // Auto-sync transactions for newly linked account
                try
                {
                    int syncedCount = await syncService.SyncAsync(id, rollback);
                    logger.LogInformation("Auto-synced {Count} transactions for new account {AccountId}", syncedCount, id);
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "Failed to auto-sync transactions for new account {AccountId}", id);
                }
            }

            logger.LogInformation(
                "Plaid item linked for user {UserId}: {Count} account(s)", userId, insertedIds.Count);
            return Ok(new Dictionary<string, string> { { "id", insertedIds.FirstOrDefault() ?? "" } });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to exchange Plaid public token for user {UserId}", userId);
            logger.LogError(ex, "Unhandled error");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An unexpected error occurred." });
        }
    }

    /// <summary>Syncs all Plaid accounts for a shared budget.</summary>
    [Authorize]
    [HttpPost("refresh/{sharedBudgetId}")]
    public async Task<ActionResult> RefreshAll(Guid sharedBudgetId)
    {
        try
        {
            var manager = new PlaidAccountManager(options, logger);
            var accounts = await manager.LoadAsync(sharedBudgetId);

            if (!accounts.Any())
                return Ok(new { syncedCount = 0, message = "No linked accounts" });

            int totalSynced = 0;
            foreach (var account in accounts)
            {
                try
                {
                    int syncedCount = await syncService.SyncAsync(account.Id);
                    totalSynced += syncedCount;
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "Failed to sync account {AccountId}", account.Id);
                }
            }

            logger.LogInformation("Refreshed {Count} transactions for SharedBudget {SharedBudgetId}", totalSynced, sharedBudgetId);
            return Ok(new { syncedCount = totalSynced, message = $"Synced {totalSynced} transactions" });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to refresh transactions");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Failed to refresh transactions" });
        }
    }

    /// <summary>
    /// Pulls real transactions from Plaid via /transactions/sync (cursor-based) for the Item
    /// backing this account, upserts them, and advances the stored cursor.
    /// </summary>
    [Authorize]
    [HttpPost("sync/{accountId}/{rollback?}")]
    public async Task<ActionResult> Sync(Guid accountId, bool rollback = false)
    {
        try
        {
            int newCount = await syncService.SyncAsync(accountId, rollback);
            return Ok(new Dictionary<string, string> { { "newcount", newCount.ToString() } });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to sync transactions for account {AccountId}", accountId);
            logger.LogError(ex, "Unhandled error");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An unexpected error occurred." });
        }
    }

    [Authorize]
    [HttpPut("{id}/{rollback?}")]
    public async Task<ActionResult> Put(Guid id, [FromBody] PlaidAccount account, bool rollback = false)
    {
        try
        {
            var manager      = new PlaidAccountManager(options, logger);
            int rowsAffected = await manager.UpdateAsync(account, rollback);
            return Ok(new Dictionary<string, string> { { "rowsaffected", rowsAffected.ToString() } });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled error");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An unexpected error occurred." });
        }
    }

    [Authorize]
    [HttpDelete("{id}/{rollback?}")]
    public async Task<ActionResult> Delete(Guid id, bool rollback = false)
    {
        try
        {
            var manager      = new PlaidAccountManager(options, logger);
            int rowsAffected = await manager.DeleteAsync(id, rollback);
            return Ok(new Dictionary<string, string> { { "rowsaffected", rowsAffected.ToString() } });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled error");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An unexpected error occurred." });
        }
    }
}
