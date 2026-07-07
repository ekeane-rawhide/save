namespace EMK.Save.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SharedBudgetController : ControllerBase
{
    private readonly DbContextOptions<SaveEntities> options;
    private readonly ILogger<SharedBudgetController> logger;

    public SharedBudgetController(ILogger<SharedBudgetController> logger,
                                  DbContextOptions<SaveEntities>  options)
    {
        this.logger  = logger;
        this.options = options;
    }

    /// <summary>Returns a SharedBudget by ID, including its member list.</summary>
    [Authorize]
    [HttpGet("{id}")]
    public async Task<ActionResult<SharedBudget>> Get(Guid id)
    {
        try
        {
            var manager = new SharedBudgetManager(options, logger);
            return Ok(await manager.LoadByIdAsync(id));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled error");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An unexpected error occurred." });
        }
    }

    /// <summary>Previews a SharedBudget by invite code — does not join.</summary>
    [Authorize]
    [HttpGet("preview/{code}")]
    public async Task<ActionResult<SharedBudget>> Preview(string code)
    {
        try
        {
            var manager = new SharedBudgetManager(options, logger);
            var budget  = await manager.LoadByInviteCodeAsync(code);
            if (budget == null) return NotFound(new { message = "Invite code not found." });
            return Ok(budget);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled error");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An unexpected error occurred." });
        }
    }

    /// <summary>Creates a new SharedBudget. Owner cannot already belong to one.</summary>
    [Authorize]
    [HttpPost("create/{rollback?}")]
    public async Task<ActionResult<SharedBudget>> Create(
        [FromBody] CreateBudgetRequest request, bool rollback = false)
    {
        try
        {
            var manager = new SharedBudgetManager(options, logger);
            var budget  = await manager.CreateBudgetAsync(
                request.OwnerId, request.Name, request.Description, rollback);
            return Ok(budget);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled error");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An unexpected error occurred." });
        }
    }

    /// <summary>Joins an existing SharedBudget via invite code. User cannot already belong to one.</summary>
    [Authorize]
    [HttpPost("join/{rollback?}")]
    public async Task<ActionResult<SharedBudget>> Join(
        [FromBody] JoinBudgetRequest request, bool rollback = false)
    {
        try
        {
            var manager = new SharedBudgetManager(options, logger);
            var budget  = await manager.JoinBudgetAsync(
                request.UserId, request.InviteCode, rollback);
            return Ok(budget);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled error");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An unexpected error occurred." });
        }
    }

    /// <summary>Removes the user from their current SharedBudget.</summary>
    [Authorize]
    [HttpPost("leave/{userId}/{rollback?}")]
    public async Task<ActionResult> Leave(Guid userId, bool rollback = false)
    {
        try
        {
            var manager      = new SharedBudgetManager(options, logger);
            int rowsAffected = await manager.LeaveBudgetAsync(userId, rollback);
            return Ok(new Dictionary<string, string> { { "rowsaffected", rowsAffected.ToString() } });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled error");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An unexpected error occurred." });
        }
    }
}

// ── Request models (transport-only) ──────────────────────────────────────────
public class CreateBudgetRequest
{
    public Guid   OwnerId     { get; set; }
    public string Name        { get; set; } = null!;
    public string Description { get; set; } = string.Empty;
}

public class JoinBudgetRequest
{
    public Guid   UserId     { get; set; }
    public string InviteCode { get; set; } = null!;
}
