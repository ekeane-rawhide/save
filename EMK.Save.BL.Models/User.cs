

namespace EMK.Save.BL.Models
{
    /// <summary>
    /// Application user.  Mirrors the DVDCentral User pattern.
    /// Maps to tblUser in the PL.
    /// </summary>
    public class User
    {
        public Guid Id { get; set; }

        [DisplayName("Username")]
        public string UserId { get; set; } = string.Empty;      // login handle

        [DisplayName("First Name")]
        public string FirstName { get; set; } = string.Empty;

        [DisplayName("Last Name")]
        public string LastName { get; set; } = string.Empty;

        [DisplayName("Email")]
        public string Email { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;    // stored as SHA-1 hash

        [DisplayName("Time Zone")]
        public string TimeZone { get; set; } = "America/Chicago";

        [DisplayName("Currency")]
        public string CurrencyCode { get; set; } = "USD";

        [DisplayName("Date Registered")]
        public DateTime DateRegistered { get; set; }

        [DisplayName("Last Login")]
        public DateTime? LastLogin { get; set; }

        // ── Computed ─────────────────────────────────────────────────────────
        [DisplayName("Full Name")]
        public string FullName => $"{FirstName} {LastName}".Trim();

        [DisplayName("Initials")]
        public string Initials =>
            $"{(FirstName.Length > 0 ? FirstName[0] : ' ')}{(LastName.Length > 0 ? LastName[0] : ' ')}".Trim().ToUpper();
    }
}
