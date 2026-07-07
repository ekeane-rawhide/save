namespace EMK.Save.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class NotificationPreferenceController : ControllerBase
{
    private readonly DbContextOptions<SaveEntities>        options;
    private readonly ILogger<NotificationPreferenceController> logger;

    public NotificationPreferenceController(
        ILogger<NotificationPreferenceController> logger,
        DbContextOptions<SaveEntities>            options)
    {
        this.logger  = logger;
        this.options = options;
    }

    /// <summary>Returns notification preferences for a user.</summary>
    [Authorize]
    [HttpGet("{userId}")]
    public async Task<ActionResult<NotificationPreference>> Get(Guid userId)
    {
        try
        {
            var manager = new NotificationPreferenceManager(options, logger);
            return Ok(await manager.LoadByUserAsync(userId));
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    [Authorize]
    [HttpPut("{id}/{rollback?}")]
    public async Task<ActionResult> Put(
        Guid id, [FromBody] NotificationPreference pref, bool rollback = false)
    {
        try
        {
            var manager      = new NotificationPreferenceManager(options, logger);
            int rowsAffected = await manager.UpdateAsync(pref, rollback);
            return Ok(new Dictionary<string, string> { { "rowsaffected", rowsAffected.ToString() } });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    /// <summary>Saves the Web Push VAPID subscription received from the browser.</summary>
    [Authorize]
    [HttpPost("subscribe/{userId}/{rollback?}")]
    public async Task<ActionResult> Subscribe(
        Guid userId, [FromBody] PushSubscriptionRequest request, bool rollback = false)
    {
        try
        {
            var manager      = new NotificationPreferenceManager(options, logger);
            int rowsAffected = await manager.SaveSubscriptionAsync(
                userId, request.Endpoint, request.P256dh, request.Auth, rollback);
            return Ok(new Dictionary<string, string> { { "rowsaffected", rowsAffected.ToString() } });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    /// <summary>Revokes the Web Push subscription — disables all push for this user.</summary>
    [Authorize]
    [HttpPost("unsubscribe/{userId}/{rollback?}")]
    public async Task<ActionResult> Unsubscribe(Guid userId, bool rollback = false)
    {
        try
        {
            var manager      = new NotificationPreferenceManager(options, logger);
            int rowsAffected = await manager.RevokeSubscriptionAsync(userId, rollback);
            return Ok(new Dictionary<string, string> { { "rowsaffected", rowsAffected.ToString() } });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }
}

// ── Request model ─────────────────────────────────────────────────────────────
public class PushSubscriptionRequest
{
    public string Endpoint { get; set; } = null!;
    public string P256dh   { get; set; } = null!;
    public string Auth     { get; set; } = null!;
}
