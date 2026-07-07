namespace EMK.Save.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BudgetCategoryController : ControllerBase
{
    private readonly DbContextOptions<SaveEntities>     options;
    private readonly ILogger<BudgetCategoryController>  logger;

    public BudgetCategoryController(ILogger<BudgetCategoryController> logger,
                                    DbContextOptions<SaveEntities>    options)
    {
        this.logger  = logger;
        this.options = options;
    }

    /// <summary>Returns all active categories for a SharedBudget.</summary>
    [Authorize]
    [HttpGet("{sharedBudgetId}")]
    public async Task<ActionResult<IEnumerable<BudgetCategory>>> Get(Guid sharedBudgetId)
    {
        try
        {
            var manager = new BudgetCategoryManager(options, logger);
            return Ok(await manager.LoadAsync(sharedBudgetId));
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    [Authorize]
    [HttpGet("byid/{id}")]
    public async Task<ActionResult<BudgetCategory>> GetById(Guid id)
    {
        try
        {
            var manager = new BudgetCategoryManager(options, logger);
            return Ok(await manager.LoadByIdAsync(id));
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    [Authorize]
    [HttpPost("{rollback?}")]
    public async Task<ActionResult> Post([FromBody] BudgetCategory category, bool rollback = false)
    {
        try
        {
            var manager = new BudgetCategoryManager(options, logger);
            Guid id     = await manager.InsertAsync(category, rollback);
            return Ok(new Dictionary<string, string> { { "id", id.ToString() } });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    [Authorize]
    [HttpPut("{id}/{rollback?}")]
    public async Task<ActionResult> Put(Guid id, [FromBody] BudgetCategory category, bool rollback = false)
    {
        try
        {
            var manager      = new BudgetCategoryManager(options, logger);
            int rowsAffected = await manager.UpdateAsync(category, rollback);
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
            var manager      = new BudgetCategoryManager(options, logger);
            int rowsAffected = await manager.DeleteAsync(id, rollback);
            return Ok(new Dictionary<string, string> { { "rowsaffected", rowsAffected.ToString() } });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }
}
