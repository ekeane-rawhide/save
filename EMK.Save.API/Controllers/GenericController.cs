namespace EMK.Save.API.Controllers;

// T = BL Model type  (e.g. BudgetCategory)
// U = Manager type   (e.g. BudgetCategoryManager)

[Route("api/[controller]")]
[ApiController]
public class GenericController<T, U> : ControllerBase
    where U : class
{
    protected DbContextOptions<SaveEntities> options;
    protected readonly ILogger              logger;
    dynamic manager;

    public GenericController(ILogger logger, DbContextOptions<SaveEntities> options)
    {
        this.options = options;
        this.logger  = logger;
        this.manager = (U)Activator.CreateInstance(typeof(U), options, logger)!;
    }

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<T>>> Get()
    {
        try
        {
            return Ok(await manager.LoadAsync());
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<ActionResult<T>> Get(Guid id)
    {
        try
        {
            return Ok(await manager.LoadByIdAsync(id));
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    [Authorize]
    [HttpPost("{rollback?}")]
    public async Task<ActionResult> Post([FromBody] T entity, bool rollback = false)
    {
        try
        {
            Guid id = await manager.InsertAsync(entity, rollback);
            var result = new Dictionary<string, string> { { "id", id.ToString() } };
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    [Authorize]
    [HttpPut("{id}/{rollback?}")]
    public async Task<ActionResult> Put(Guid id, [FromBody] T entity, bool rollback = false)
    {
        try
        {
            int rowsAffected = await manager.UpdateAsync(entity, rollback);
            var result = new Dictionary<string, string> { { "rowsaffected", rowsAffected.ToString() } };
            return Ok(result);
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
            int rowsAffected = await manager.DeleteAsync(id, rollback);
            var result = new Dictionary<string, string> { { "rowsaffected", rowsAffected.ToString() } };
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }
}
