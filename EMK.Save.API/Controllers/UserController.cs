namespace EMK.Save.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : GenericController<User, UserManager>
{
    private readonly IUserService                    _userService;
    private new readonly ILogger<UserController>     logger;
    private new readonly DbContextOptions<SaveEntities> options;

    public UserController(IUserService                       userService,
                          ILogger<UserController>            logger,
                          DbContextOptions<SaveEntities>     options)
        : base(logger, options)
    {
        _userService  = userService;
        this.logger   = logger;
        this.options  = options;
    }

    /// <summary>Authenticates a user and returns a JWT token.</summary>
    [EnableRateLimiting("auth")]
    [HttpPost("authenticate")]
    public IActionResult Authenticate([FromBody] AuthenticateRequest model)
    {
        var response = _userService.Authenticate(model);

        if (response == null)
        {
            logger.LogWarning("Authentication failed for {UserId}", model.UserId);
            return BadRequest(new { message = "Username or password is incorrect." });
        }

        logger.LogInformation("Authentication successful for {UserId}", model.UserId);
        return Ok(response);
    }

    /// <summary>Creates a new account and returns a JWT token (auto-login on signup).</summary>
    [EnableRateLimiting("auth")]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest model)
    {
        try
        {
            var response = await _userService.RegisterAsync(model);
            return Ok(response);
        }
        catch (Exception ex)
        {
            logger.LogWarning("Registration failed for {UserId}: {Message}", model.UserId, ex.Message);
            return BadRequest(new { message = ex.Message });
        }
    }

    [Authorize]
    [HttpGet("GetAll")]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _userService.GetAllAsync());
    }
}
