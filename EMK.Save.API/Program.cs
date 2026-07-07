using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;
using Serilog;

public class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // ── Serilog — structured logging to console + rolling file + Seq ────────
        builder.Host.UseSerilog((context, services, configuration) => configuration
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext()
            .WriteTo.Seq("http://localhost:5341"));

        // ── Database ──────────────────────────────────────────────────────────
        string? connectionString =
            builder.Configuration.GetConnectionString("DefaultConnection");

        builder.Services.AddDbContextPool<SaveEntities>(options =>
        {
            options.UseSqlServer(connectionString, sql => sql.EnableRetryOnFailure(
                maxRetryCount: 5, maxRetryDelay: TimeSpan.FromSeconds(10), errorNumbersToAdd: null));
            options.UseLazyLoadingProxies();
        });

        // ── JWT settings ──────────────────────────────────────────────────────
        builder.Services.Configure<AppSettings>(
            builder.Configuration.GetSection("AppSettings"));

        // Fail fast on a missing/weak signing key rather than issuing tokens no one can validate later.
        string? jwtSecret = builder.Configuration["AppSettings:Secret"];
        if (string.IsNullOrWhiteSpace(jwtSecret) || jwtSecret.Length < 32)
        {
            throw new InvalidOperationException(
                "AppSettings:Secret must be configured and at least 32 characters. " +
                "Set it via an environment variable or secret store in production — never commit a real value.");
        }

        // ── Plaid ─────────────────────────────────────────────────────────────
        builder.Services.AddPlaid(builder.Configuration.GetSection("Plaid"));
        builder.Services.Configure<EncryptionSettings>(
            builder.Configuration.GetSection("Encryption"));
        builder.Services.AddSingleton<ITokenEncryptor, TokenEncryptor>();
        builder.Services.AddScoped<IPlaidSyncService, PlaidSyncService>();
        builder.Services.AddSingleton<IPlaidWebhookVerifier, PlaidWebhookVerifier>();

        // ── Web Push (VAPID) ──────────────────────────────────────────────────
        builder.Services.Configure<WebPushSettings>(
            builder.Configuration.GetSection("WebPush"));
        builder.Services.AddHttpClient<IWebPushSender, WebPushSender>();
        builder.Services.AddScoped<INotificationDispatchService, NotificationDispatchService>();

        // ── DI ────────────────────────────────────────────────────────────────
        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddHostedService<PlaidSyncBackgroundService>();

        // ── CORS — allow the React PWA origin ─────────────────────────────────
        builder.Services.AddCors(o => o.AddPolicy("PWAPolicy", policy =>
        {
            policy.WithOrigins(
                    builder.Configuration["AllowedOrigins"]
                        ?? "http://localhost:5173")   // Vite dev default
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        }));

        // ── Rate limiting — blunt brute-force/credential-stuffing on auth ─────
        builder.Services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            options.AddPolicy("auth", httpContext => RateLimitPartition.GetFixedWindowLimiter(
                partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                factory: _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 10,
                    Window = TimeSpan.FromMinutes(1),
                    QueueLimit = 0,
                }));
        });

        // ── Global exception handling — never leak stack traces to clients ────
        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
        builder.Services.AddProblemDetails();

        // ── Health checks ─────────────────────────────────────────────────────
        builder.Services.AddHealthChecks()
            .AddCheck<DatabaseHealthCheck>("database");

        // ── Response compression ──────────────────────────────────────────────
        builder.Services.AddResponseCompression(options =>
        {
            options.EnableForHttps = true;
        });

        // ── Controllers & Swagger ─────────────────────────────────────────────
        builder.Services.AddControllers()
            .AddJsonOptions(opts =>
            {
                // keep PascalCase so the PWA can deserialize without extra config
                opts.JsonSerializerOptions.PropertyNamingPolicy = null;
            });

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        // ── SignalR — real-time push for the React PWA ────────────────────────
        builder.Services.AddSignalR();

        // ─────────────────────────────────────────────────────────────────────
        var app = builder.Build();
        // ─────────────────────────────────────────────────────────────────────

        app.Logger.LogInformation("Starting Save API…");

        app.UseExceptionHandler();

        // ── CORS must come before HTTPS redirect to avoid blocking preflight requests ──
        app.UseCors("PWAPolicy");

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        else
        {
            app.UseHsts();
            app.UseHttpsRedirection();
        }
        app.UseResponseCompression();

        // ── Security headers ──────────────────────────────────────────────────
        app.Use(async (context, next) =>
        {
            context.Response.Headers["X-Content-Type-Options"] = "nosniff";
            context.Response.Headers["X-Frame-Options"] = "DENY";
            context.Response.Headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
            await next();
        });

        // Verifies Plaid's webhook signature before the request reaches model binding/controllers
        app.UseMiddleware<PlaidWebhookVerificationMiddleware>();

        // Custom JWT middleware — attaches user to HttpContext on every request
        app.UseMiddleware<JwtMiddleware>();

        app.UseRouting();
        app.UseRateLimiter();
        app.UseAuthorization();

        app.MapControllers();
        app.MapHub<EMK.Save.API.Hubs.SaveHub>("/hubs/save");
        app.MapHealthChecks("/health");

        app.Run();
    }
}
