namespace EMK.Save.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BudgetController : ControllerBase
{
    private readonly DbContextOptions<SaveEntities> options;
    private readonly ILogger<BudgetController>      logger;

    public BudgetController(ILogger<BudgetController>      logger,
                            DbContextOptions<SaveEntities> options)
    {
        this.logger  = logger;
        this.options = options;
    }

    /// <summary>Returns all budget plan rows for a SharedBudget in a given month.</summary>
    [Authorize]
    [HttpGet("{sharedBudgetId}/{month}/{year}")]
    public async Task<ActionResult<IEnumerable<Budget>>> Get(Guid sharedBudgetId, int month, int year)
    {
        try
        {
            var manager = new BudgetManager(options, logger);
            return Ok(await manager.LoadAsync(sharedBudgetId, month, year));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled error");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An unexpected error occurred." });
        }
    }

    [Authorize]
    [HttpGet("byid/{id}")]
    public async Task<ActionResult<Budget>> GetById(Guid id)
    {
        try
        {
            var manager = new BudgetManager(options, logger);
            return Ok(await manager.LoadByIdAsync(id));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled error");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An unexpected error occurred." });
        }
    }

    [Authorize]
    [HttpPost("{rollback?}")]
    public async Task<ActionResult> Post([FromBody] Budget budget, bool rollback = false)
    {
        try
        {
            var manager = new BudgetManager(options, logger);
            Guid id     = await manager.InsertAsync(budget, rollback);
            return Ok(new Dictionary<string, string> { { "id", id.ToString() } });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled error");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An unexpected error occurred." });
        }
    }

    [Authorize]
    [HttpPut("{id}/{rollback?}")]
    public async Task<ActionResult> Put(Guid id, [FromBody] Budget budget, bool rollback = false)
    {
        try
        {
            var manager      = new BudgetManager(options, logger);
            int rowsAffected = await manager.UpdateAsync(budget, rollback);
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
            var manager      = new BudgetManager(options, logger);
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
