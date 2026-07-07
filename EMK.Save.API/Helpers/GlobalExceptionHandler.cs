namespace EMK.Save.API.Helpers;

using Microsoft.AspNetCore.Diagnostics;

/// <summary>
/// Defense-in-depth catch-all for anything that escapes a controller's own try/catch
/// (middleware failures, model binding, etc.). Logs full details server-side and never
/// leaks exception messages/stack traces to the client.
/// </summary>
public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Unhandled exception on {Method} {Path}",
            httpContext.Request.Method, httpContext.Request.Path);

        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        httpContext.Response.ContentType = "application/json";

        await httpContext.Response.WriteAsJsonAsync(
            new { message = "An unexpected error occurred." }, cancellationToken);

        return true;
    }
}
