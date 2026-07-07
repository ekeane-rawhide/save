namespace EMK.Save.API.Models;

using System.ComponentModel.DataAnnotations;

public class RegisterRequest
{
    [Required]
    public string UserId { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;

    [Required]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    public string LastName { get; set; } = string.Empty;

    [Required]
    public string Email { get; set; } = string.Empty;

    public string TimeZone { get; set; } = "America/Chicago";

    public string CurrencyCode { get; set; } = "USD";
}
