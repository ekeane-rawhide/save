namespace EMK.Save.API.Helpers;

using EMK.Save.API.Services;

/// <summary>
/// Verifies the Plaid-Verification signature on POST /api/plaid/webhook before the request
/// reaches MVC model binding — must read the raw body ahead of the controller, since the
/// signature covers the exact bytes Plaid sent, not the deserialized object.
/// </summary>
public class PlaidWebhookVerificationMiddleware
{
    private readonly RequestDelegate _next;

    public PlaidWebhookVerificationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IPlaidWebhookVerifier verifier, ILogger<PlaidWebhookVerificationMiddleware> logger)
    {
        if (context.Request.Method != HttpMethods.Post ||
            !context.Request.Path.StartsWithSegments("/api/plaid/webhook"))
        {
            await _next(context);
            return;
        }

        context.Request.EnableBuffering();
        string body;
        using (var reader = new StreamReader(context.Request.Body, leaveOpen: true))
        {
            body = await reader.ReadToEndAsync();
        }
        context.Request.Body.Position = 0;

        string? signature = context.Request.Headers["Plaid-Verification"];
        if (!await verifier.VerifyAsync(signature, body))
        {
            logger.LogWarning("Rejected Plaid webhook: signature verification failed");
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return;
        }

        await _next(context);
    }
}
