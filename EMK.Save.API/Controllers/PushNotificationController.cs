using EMK.Save.API.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace EMK.Save.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PushNotificationController : ControllerBase
{
    private readonly DbContextOptions<SaveEntities>    options;
    private readonly ILogger<PushNotificationController> logger;
    private readonly IHubContext<SaveHub>              hub;
    private readonly INotificationDispatchService      dispatch;

    public PushNotificationController(ILogger<PushNotificationController> logger,
                                      DbContextOptions<SaveEntities>      options,
                                      IHubContext<SaveHub>                hub,
                                      INotificationDispatchService        dispatch)
    {
        this.logger   = logger;
        this.options  = options;
        this.hub      = hub;
        this.dispatch = dispatch;
    }

    /// <summary>Returns all notifications for a user.</summary>
    [Authorize]
    [HttpGet("{userId}")]
    public async Task<ActionResult<IEnumerable<PushNotification>>> Get(Guid userId)
    {
        try
        {
            var manager = new PushNotificationManager(options, logger);
            return Ok(await manager.LoadAsync(userId));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled error");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An unexpected error occurred." });
        }
    }

    /// <summary>Returns only unread notifications for a user.</summary>
    [Authorize]
    [HttpGet("unread/{userId}")]
    public async Task<ActionResult<IEnumerable<PushNotification>>> GetUnread(Guid userId)
    {
        try
        {
            var manager = new PushNotificationManager(options, logger);
            return Ok(await manager.LoadAsync(userId, unreadOnly: true));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled error");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An unexpected error occurred." });
        }
    }

    /// <summary>Marks a notification as read.</summary>
    [Authorize]
    [HttpPut("markread/{id}/{rollback?}")]
    public async Task<ActionResult> MarkRead(Guid id, bool rollback = false)
    {
        try
        {
            var manager      = new PushNotificationManager(options, logger);
            int rowsAffected = await manager.MarkReadAsync(id, rollback);
            return Ok(new Dictionary<string, string> { { "rowsaffected", rowsAffected.ToString() } });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled error");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An unexpected error occurred." });
        }
    }

    /// <summary>Queues a budget overage push notification.</summary>
    [Authorize]
    [HttpPost("overage/{rollback?}")]
    public async Task<ActionResult> QueueOverage(
        [FromBody] OverageNotificationRequest request, bool rollback = false)
    {
        try
        {
            Guid id = await dispatch.DispatchOverageAsync(
                request.SharedBudgetId, request.UserId,
                request.CategoryId, request.CategoryName,
                request.OverageAmount, rollback);

            if (id != Guid.Empty)
            {
                var manager = new PushNotificationManager(options, logger);
                var notification = await manager.LoadByIdAsync(id);
                await hub.Clients.Group(SaveHub.BudgetGroup(request.SharedBudgetId))
                    .SendAsync("NotificationReceived", notification);
            }

            return Ok(new Dictionary<string, string> { { "id", id.ToString() } });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled error");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An unexpected error occurred." });
        }
    }

    /// <summary>Queues a new-transaction push notification.</summary>
    [Authorize]
    [HttpPost("transaction/{rollback?}")]
    public async Task<ActionResult> QueueTransaction(
        [FromBody] TransactionNotificationRequest request, bool rollback = false)
    {
        try
        {
            Guid id = await dispatch.DispatchTransactionAsync(
                request.SharedBudgetId, request.UserId,
                request.TransactionId, request.MerchantName,
                request.Amount, rollback);

            if (id != Guid.Empty)
            {
                var manager = new PushNotificationManager(options, logger);
                var notification = await manager.LoadByIdAsync(id);
                await hub.Clients.Group(SaveHub.BudgetGroup(request.SharedBudgetId))
                    .SendAsync("NotificationReceived", notification);
            }

            return Ok(new Dictionary<string, string> { { "id", id.ToString() } });
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
            var manager      = new PushNotificationManager(options, logger);
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

// ── Request models ────────────────────────────────────────────────────────────
public class OverageNotificationRequest
{
    public Guid    SharedBudgetId { get; set; }
    public Guid    UserId         { get; set; }
    public Guid    CategoryId     { get; set; }
    public string  CategoryName   { get; set; } = null!;
    public decimal OverageAmount  { get; set; }
}

public class TransactionNotificationRequest
{
    public Guid    SharedBudgetId { get; set; }
    public Guid    UserId         { get; set; }
    public Guid    TransactionId  { get; set; }
    public string  MerchantName   { get; set; } = null!;
    public decimal Amount         { get; set; }
}
