namespace EMK.Save.API.Models;

public class AuthenticateResponse
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string UserId { get; set; }
    public string Token { get; set; }


    public AuthenticateResponse(EMK.Save.BL.Models.User user, string token)
    {
        Id = user.Id;
        FirstName = user.FirstName;
        LastName = user.LastName;
        UserId = user.UserId;
        Token = token;
    }
}
