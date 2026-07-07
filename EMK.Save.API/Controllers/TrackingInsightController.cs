using EMK.Save.API.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace EMK.Save.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TrackingInsightController : ControllerBase
{
    private readonly DbContextOptions<SaveEntities>   options;
    private readonly ILogger<TrackingInsightController> logger;
    private readonly IHubContext<SaveHub>              hub;

    public TrackingInsightController(ILogger<TrackingInsightController> logger,
                                     DbContextOptions<SaveEntities>     options,
                                     IHubContext<SaveHub>               hub)
    {
        this.logger  = logger;
        this.options = options;
        this.hub     = hub;
    }

    /// <summary>Returns active (non-dismissed) insights for a SharedBudget in a given month.</summary>
    [Authorize]
    [HttpGet("{sharedBudgetId}/{month}/{year}")]
    public async Task<ActionResult<IEnumerable<TrackingInsight>>> Get(
        Guid sharedBudgetId, int month, int year)
    {
        try
        {
            var manager = new TrackingInsightManager(options, logger);
            return Ok(await manager.LoadAsync(sharedBudgetId, month, year));
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    /// <summary>Runs all insight rules and replaces existing insights for the period.</summary>
    [Authorize]
    [HttpPost("generate/{sharedBudgetId}/{month}/{year}")]
    public async Task<ActionResult<IEnumerable<TrackingInsight>>> Generate(
        Guid sharedBudgetId, int month, int year)
    {
        try
        {
            var manager  = new TrackingInsightManager(options, logger);
            var insights = await manager.GenerateInsightsAsync(sharedBudgetId, month, year);

            await hub.Clients.Group(SaveHub.BudgetGroup(sharedBudgetId))
                .SendAsync("InsightsGenerated", insights);

            return Ok(insights);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    /// <summary>Dismisses a single insight so it no longer appears in the UI.</summary>
    [Authorize]
    [HttpPut("dismiss/{id}/{rollback?}")]
    public async Task<ActionResult> Dismiss(Guid id, bool rollback = false)
    {
        try
        {
            var manager      = new TrackingInsightManager(options, logger);
            int rowsAffected = await manager.DismissAsync(id, rollback);
            return Ok(new Dictionary<string, string> { { "rowsaffected", rowsAffected.ToString() } });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }
}
