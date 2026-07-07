using EMK.Save.API.Helpers;
using EMK.Save.API.Hubs;
using EMK.Save.BL;
using EMK.Save.BL.Models;
using EMK.Save.PL.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace EMK.Save.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TransactionController : ControllerBase
{
    private readonly DbContextOptions<SaveEntities> options;
    private readonly ILogger<TransactionController> logger;
    private readonly IHubContext<SaveHub>           hub;

    public TransactionController(ILogger<TransactionController> logger,
                                 DbContextOptions<SaveEntities> options,
                                 IHubContext<SaveHub>           hub)
    {
        this.logger  = logger;
        this.options = options;
        this.hub     = hub;
    }

    /// <summary>Returns all non-excluded transactions for a SharedBudget in a given month.</summary>
    [Authorize]
    [HttpGet("{sharedBudgetId}/{month}/{year}")]
    public async Task<ActionResult<IEnumerable<Transaction>>> Get(
        Guid sharedBudgetId, int month, int year)
    {
        try
        {
            var manager = new TransactionManager(options, logger);
            return Ok(await manager.LoadAsync(sharedBudgetId, month, year));
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    /// <summary>Returns only unassigned transactions for a SharedBudget in a given month.</summary>
    [Authorize]
    [HttpGet("unassigned/{sharedBudgetId}/{month}/{year}")]
    public async Task<ActionResult<IEnumerable<Transaction>>> GetUnassigned(
        Guid sharedBudgetId, int month, int year)
    {
        try
        {
            var manager = new TransactionManager(options, logger);
            return Ok(await manager.LoadAsync(sharedBudgetId, month, year, unassignedOnly: true));
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    [Authorize]
    [HttpGet("byid/{id}")]
    public async Task<ActionResult<Transaction>> GetById(Guid id)
    {
        try
        {
            var manager = new TransactionManager(options, logger);
            return Ok(await manager.LoadByIdAsync(id));
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    /// <summary>Assigns (or clears) a budget category on a single transaction.</summary>
    [Authorize]
    [HttpPut("assign/{transactionId}/{rollback?}")]
    public async Task<ActionResult> Assign(
        Guid transactionId,
        [FromBody] AssignCategoryRequest request,
        bool rollback = false)
    {
        try
        {
            var manager      = new TransactionManager(options, logger);
            int rowsAffected = await manager.AssignCategoryAsync(
                transactionId, request.CategoryId, rollback);

            var updated = await manager.LoadByIdAsync(transactionId);
            await hub.Clients.Group(SaveHub.BudgetGroup(updated.SharedBudgetId))
                .SendAsync("TransactionUpdated", updated);

            return Ok(new Dictionary<string, string> { { "rowsaffected", rowsAffected.ToString() } });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    /// <summary>Upserts a batch of Plaid transactions. Returns count of new rows.</summary>
    [Authorize]
    [HttpPost("sync/{rollback?}")]
    public async Task<ActionResult> Sync(
        [FromBody] List<Transaction> transactions, bool rollback = false)
    {
        try
        {
            var manager  = new TransactionManager(options, logger);
            int newCount = await manager.UpsertFromPlaidAsync(transactions, rollback);

            foreach (var sharedBudgetId in transactions.Select(t => t.SharedBudgetId).Distinct())
                await hub.Clients.Group(SaveHub.BudgetGroup(sharedBudgetId))
                    .SendAsync("TransactionsSynced", new { sharedBudgetId, newCount });

            return Ok(new Dictionary<string, string> { { "newcount", newCount.ToString() } });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    [Authorize]
    [HttpPost("{rollback?}")]
    public async Task<ActionResult> Post([FromBody] Transaction transaction, bool rollback = false)
    {
        try
        {
            var manager = new TransactionManager(options, logger);
            Guid id     = await manager.InsertAsync(transaction, rollback);
            return Ok(new Dictionary<string, string> { { "id", id.ToString() } });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    [Authorize]
    [HttpPut("{id}/{rollback?}")]
    public async Task<ActionResult> Put(Guid id, [FromBody] Transaction transaction, bool rollback = false)
    {
        try
        {
            var manager      = new TransactionManager(options, logger);
            int rowsAffected = await manager.UpdateAsync(transaction, rollback);
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
            var manager      = new TransactionManager(options, logger);
            int rowsAffected = await manager.DeleteAsync(id, rollback);
            return Ok(new Dictionary<string, string> { { "rowsaffected", rowsAffected.ToString() } });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }
}

// ── Request model ─────────────────────────────────────────────────────────────
public class AssignCategoryRequest
{
    public Guid? CategoryId { get; set; }   // null = unassign
}
