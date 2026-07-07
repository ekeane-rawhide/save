namespace EMK.Save.API.Services;

public interface IUserService
{
    AuthenticateResponse? Authenticate(AuthenticateRequest model);
    Task<AuthenticateResponse> RegisterAsync(RegisterRequest model);
    Task<IEnumerable<User>> GetAllAsync();
    Task<User> GetByIdAsync(Guid id);
}

public class UserService : IUserService
{
    private readonly AppSettings                     _appSettings;
    private readonly DbContextOptions<SaveEntities>  _dbOptions;
    private readonly ILogger<UserService>            _logger;

    public UserService(ILogger<UserService>           logger,
                       IOptions<AppSettings>          appSettings,
                       DbContextOptions<SaveEntities> options)
    {
        _logger      = logger;
        _appSettings = appSettings.Value;
        _dbOptions   = options;
    }

    public AuthenticateResponse? Authenticate(AuthenticateRequest model)
    {
        var user = new UserManager(_dbOptions)
                       .LoadAsync()
                       .Result
                       .SingleOrDefault(u => u.UserId == model.UserId);

        if (user == null || !UserManager.VerifyPassword(model.Password, user.Password)) return null;

        string token = GenerateJwtToken(user);
        _logger.LogInformation("Authentication successful for {UserId}", model.UserId);
        return new AuthenticateResponse(user, token);
    }

    public async Task<AuthenticateResponse> RegisterAsync(RegisterRequest model)
    {
        var manager = new UserManager(_dbOptions);

        var user = new User
        {
            UserId         = model.UserId,
            Password       = model.Password,
            FirstName      = model.FirstName,
            LastName       = model.LastName,
            Email          = model.Email,
            TimeZone       = model.TimeZone,
            CurrencyCode   = model.CurrencyCode,
            DateRegistered = DateTime.Now
        };

        Guid id = await manager.InsertAsync(user);
        var created = await manager.LoadByIdAsync(id);

        string token = GenerateJwtToken(created);
        _logger.LogInformation("Registration successful for {UserId}", model.UserId);
        return new AuthenticateResponse(created, token);
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return await new UserManager(_dbOptions).LoadAsync();
    }

    public async Task<User> GetByIdAsync(Guid id)
    {
        return await new UserManager(_dbOptions).LoadByIdAsync(id);
    }

    private string GenerateJwtToken(User user)
    {
        var tokenHandler    = new JwtSecurityTokenHandler();
        var key             = Encoding.ASCII.GetBytes(_appSettings.Secret);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim("id", user.Id.ToString())
            }),
            Expires            = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        _logger.LogInformation("JWT token generated for {UserId}", user.UserId);
        return tokenHandler.WriteToken(token);
    }
}
