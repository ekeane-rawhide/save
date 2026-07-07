namespace EMK.Save.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DashboardController : ControllerBase
{
    private readonly DbContextOptions<SaveEntities> options;
    private readonly ILogger<DashboardController>   logger;

    public DashboardController(ILogger<DashboardController>   logger,
                               DbContextOptions<SaveEntities> options)
    {
        this.logger  = logger;
        this.options = options;
    }

    /// <summary>
    /// Returns the complete DashboardSummary for a SharedBudget and month.
    /// All sub-collections are loaded in parallel by DashboardManager.
    /// </summary>
    [Authorize]
    [HttpGet("{sharedBudgetId}/{userId}/{month}/{year}")]
    public async Task<ActionResult<DashboardSummary>> Get(
        Guid sharedBudgetId, Guid userId, int month, int year)
    {
        try
        {
            var manager   = new DashboardManager(options, logger);
            var dashboard = await manager.LoadAsync(sharedBudgetId, userId, month, year);
            return Ok(dashboard);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled error");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An unexpected error occurred." });
        }
    }
}
