namespace EMK.Save.API.Helpers;

using Microsoft.Extensions.Diagnostics.HealthChecks;

/// <summary>
/// AddDbContextCheck&lt;SaveEntities&gt;() doesn't work here: SaveEntities overrides OnConfiguring,
/// which EF Core forbids on a pooled context (AddDbContextPool). This check instead constructs a
/// fresh, unpooled SaveEntities — the same pattern every Manager class already uses.
/// </summary>
public class DatabaseHealthCheck : IHealthCheck
{
    private readonly DbContextOptions<SaveEntities> _options;

    public DatabaseHealthCheck(DbContextOptions<SaveEntities> options)
    {
        _options = options;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            using var db = new SaveEntities(_options);
            return await db.Database.CanConnectAsync(cancellationToken)
                ? HealthCheckResult.Healthy()
                : HealthCheckResult.Unhealthy("Cannot connect to the database.");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Database health check failed.", ex);
        }
    }
}
