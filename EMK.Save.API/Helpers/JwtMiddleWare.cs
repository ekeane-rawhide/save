namespace EMK.Save.API.Helpers;

using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using EMK.Save.API.Services;

public class JwtMiddleware
{
    private readonly RequestDelegate _next;
    private readonly AppSettings _appSettings;

    public JwtMiddleware(RequestDelegate next, IOptions<AppSettings> appSettings)
    {
        _next = next;
        _appSettings = appSettings.Value;
    }

    public async Task Invoke(HttpContext context, IUserService userService)
    {
        // SignalR websocket connections can't set custom headers, so the JS client
        // sends the token via ?access_token=... on the /hubs/* negotiate request instead.
        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

        if (string.IsNullOrEmpty(token) && context.Request.Path.StartsWithSegments("/hubs"))
            token = context.Request.Query["access_token"];

        if (!string.IsNullOrEmpty(token))
            await AttachUserToContextAsync(context, userService, token);

        await _next(context);
    }

    private async Task AttachUserToContextAsync(HttpContext context, IUserService userService, string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            var userId = Guid.Parse(jwtToken.Claims.First(x => x.Type == "id").Value);

            context.Items["User"] = await userService.GetByIdAsync(userId);
        }
        catch
        {
            // jwt validation failed
        }
    }
}