namespace EMK.Save.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CashFlowController : ControllerBase
{
    private readonly DbContextOptions<SaveEntities> options;
    private readonly ILogger<CashFlowController>    logger;

    public CashFlowController(ILogger<CashFlowController>    logger,
                              DbContextOptions<SaveEntities> options)
    {
        this.logger  = logger;
        this.options = options;
    }

    /// <summary>Returns daily cash-flow entries for a SharedBudget in a given month.</summary>
    [Authorize]
    [HttpGet("{sharedBudgetId}/{month}/{year}")]
    public async Task<ActionResult<IEnumerable<CashFlowEntry>>> Get(
        Guid sharedBudgetId, int month, int year)
    {
        try
        {
            var manager = new CashFlowManager(options, logger);
            return Ok(await manager.LoadAsync(sharedBudgetId, month, year));
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    /// <summary>
    /// Rebuilds cash-flow entries for the month from live transactions,
    /// including forward projections for remaining days.
    /// </summary>
    [Authorize]
    [HttpPost("build/{sharedBudgetId}/{month}/{year}")]
    public async Task<ActionResult<IEnumerable<CashFlowEntry>>> Build(
        Guid sharedBudgetId, int month, int year)
    {
        try
        {
            var manager = new CashFlowManager(options, logger);
            return Ok(await manager.BuildCashFlowAsync(sharedBudgetId, month, year));
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }
}
