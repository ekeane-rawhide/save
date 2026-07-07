public class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // ── Database ──────────────────────────────────────────────────────────
        string? connectionString =
            builder.Configuration.GetConnectionString("DefaultConnection");

        builder.Services.AddDbContextPool<SaveEntities>(options =>
        {
            options.UseSqlServer(connectionString);
            options.UseLazyLoadingProxies();
        });

        // ── JWT settings ──────────────────────────────────────────────────────
        builder.Services.Configure<AppSettings>(
            builder.Configuration.GetSection("AppSettings"));

        // ── DI ────────────────────────────────────────────────────────────────
        builder.Services.AddScoped<IUserService, UserService>();

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

        // ── Controllers & Swagger ─────────────────────────────────────────────
        builder.Services.AddControllers()
            .AddJsonOptions(opts =>
            {
                // keep PascalCase so the PWA can deserialize without extra config
                opts.JsonSerializerOptions.PropertyNamingPolicy = null;
            });

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        // ── Logging ───────────────────────────────────────────────────────────
        builder.Services
            .AddLogging(c => c.AddDebug())
            .AddLogging(c => c.AddConsole());

        // ─────────────────────────────────────────────────────────────────────
        var app = builder.Build();
        // ─────────────────────────────────────────────────────────────────────

        app.Logger.LogInformation("Starting Save API…");

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseCors("PWAPolicy");

        // Custom JWT middleware — attaches user to HttpContext on every request
        app.UseMiddleware<JwtMiddleware>();

        app.UseRouting();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
