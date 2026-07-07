namespace EMK.Save.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PlaidAccountController : ControllerBase
{
    private readonly DbContextOptions<SaveEntities>  options;
    private readonly ILogger<PlaidAccountController> logger;

    public PlaidAccountController(ILogger<PlaidAccountController> logger,
                                  DbContextOptions<SaveEntities>  options)
    {
        this.logger  = logger;
        this.options = options;
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
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
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
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    /// <summary>
    /// Creates a Plaid Link token and returns it to the PWA to initialize Plaid Link.
    /// In production this calls the Plaid API; here it returns a mock token structure.
    /// </summary>
    [Authorize]
    [HttpGet("linktoken/{userId}")]
    public ActionResult<PlaidLinkToken> GetLinkToken(Guid userId)
    {
        try
        {
            // TODO: replace with real Plaid client.LinkTokenCreate() call
            var linkToken = new PlaidLinkToken
            {
                LinkToken  = $"link-sandbox-{Guid.NewGuid()}",
                Expiration = DateTime.UtcNow.AddMinutes(30),
                RequestId  = Guid.NewGuid().ToString()
            };
            logger.LogInformation("Link token created for user {UserId}", userId);
            return Ok(linkToken);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    /// <summary>
    /// Exchanges the public_token returned by Plaid Link for an access_token,
    /// then persists the linked account(s).
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
            // TODO: call Plaid API to exchange public_token → access_token
            // For now store a placeholder encrypted token
            string encryptedToken = $"ENCRYPTED:{exchange.PublicToken}";

            var account = new PlaidAccount
            {
                UserId           = userId,
                SharedBudgetId   = sharedBudgetId,
                PlaidAccountId   = exchange.SelectedAccountIds.FirstOrDefault() ?? Guid.NewGuid().ToString(),
                PlaidItemId      = Guid.NewGuid().ToString(),
                InstitutionName  = exchange.InstitutionName,
                InstitutionLogoUrl = exchange.InstitutionLogoUrl,
                AccountName      = "Checking",
                Mask             = "0000",
                AccountType      = "depository",
                AccountSubtype   = "checking",
                CurrentBalance   = 0m,
                AvailableBalance = 0m,
                IsoCurrencyCode  = "USD",
                IsActive         = true,
                LastSynced       = DateTime.Now,
                DateLinked       = DateTime.Now
            };

            var manager = new PlaidAccountManager(options, logger);
            Guid id     = await manager.InsertAsync(account, encryptedToken, rollback);
            logger.LogInformation("Plaid account linked for user {UserId}: {AccountId}", userId, id);
            return Ok(new Dictionary<string, string> { { "id", id.ToString() } });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
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
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
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
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }
}
