namespace EMK.Save.API.Models;

public class AuthenticateResponse
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string UserId { get; set; }
    public string Email { get; set; }
    public Guid? SharedBudgetId { get; set; }
    public EMK.Save.BL.Models.BudgetRole? BudgetRole { get; set; }
    public string Token { get; set; }


    public AuthenticateResponse(EMK.Save.BL.Models.User user, string token)
    {
        Id = user.Id;
        FirstName = user.FirstName;
        LastName = user.LastName;
        UserId = user.UserId;
        Email = user.Email;
        SharedBudgetId = user.SharedBudgetId;
        BudgetRole = user.BudgetRole;
        Token = token;
    }
}
