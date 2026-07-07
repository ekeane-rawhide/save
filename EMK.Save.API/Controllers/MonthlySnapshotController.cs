namespace EMK.Save.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MonthlySnapshotController : ControllerBase
{
    private readonly DbContextOptions<SaveEntities>   options;
    private readonly ILogger<MonthlySnapshotController> logger;

    public MonthlySnapshotController(ILogger<MonthlySnapshotController> logger,
                                     DbContextOptions<SaveEntities>     options)
    {
        this.logger  = logger;
        this.options = options;
    }

    /// <summary>Returns all monthly snapshots for a SharedBudget, newest first.</summary>
    [Authorize]
    [HttpGet("{sharedBudgetId}")]
    public async Task<ActionResult<IEnumerable<MonthlySnapshot>>> Get(Guid sharedBudgetId)
    {
        try
        {
            var manager = new MonthlySnapshotManager(options, logger);
            return Ok(await manager.LoadAsync(sharedBudgetId));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled error");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An unexpected error occurred." });
        }
    }

    /// <summary>Returns the last N monthly snapshots for history charts.</summary>
    [Authorize]
    [HttpGet("{sharedBudgetId}/last/{months}")]
    public async Task<ActionResult<IEnumerable<MonthlySnapshot>>> GetLast(
        Guid sharedBudgetId, int months)
    {
        try
        {
            var manager = new MonthlySnapshotManager(options, logger);
            return Ok(await manager.LoadAsync(sharedBudgetId, limitMonths: months));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled error");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An unexpected error occurred." });
        }
    }

    /// <summary>Returns a specific month's snapshot.</summary>
    [Authorize]
    [HttpGet("{sharedBudgetId}/{month}/{year}")]
    public async Task<ActionResult<MonthlySnapshot>> GetByMonth(
        Guid sharedBudgetId, int month, int year)
    {
        try
        {
            var manager = new MonthlySnapshotManager(options, logger);
            return Ok(await manager.LoadByMonthAsync(sharedBudgetId, month, year));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled error");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An unexpected error occurred." });
        }
    }

    /// <summary>
    /// Builds (or rebuilds) the snapshot for a month by aggregating live transaction data.
    /// </summary>
    [Authorize]
    [HttpPost("build/{sharedBudgetId}/{month}/{year}")]
    public async Task<ActionResult<MonthlySnapshot>> Build(
        Guid sharedBudgetId, int month, int year)
    {
        try
        {
            var manager  = new MonthlySnapshotManager(options, logger);
            var snapshot = await manager.BuildSnapshotAsync(sharedBudgetId, month, year);
            return Ok(snapshot);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled error");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An unexpected error occurred." });
        }
    }
}
