namespace EMK.Save.BL.Models
{
    public class User
    {
        public Guid Id { get; set; }

        [DisplayName("Username")]
        public string UserId { get; set; } = string.Empty;

        [DisplayName("First Name")]
        public string FirstName { get; set; } = string.Empty;

        [DisplayName("Last Name")]
        public string LastName { get; set; } = string.Empty;

        [DisplayName("Email")]
        public string Email { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        [DisplayName("Time Zone")]
        public string TimeZone { get; set; } = "America/Chicago";

        [DisplayName("Currency")]
        public string CurrencyCode { get; set; } = "USD";

        [DisplayName("Date Registered")]
        public DateTime DateRegistered { get; set; }

        [DisplayName("Last Login")]
        public DateTime? LastLogin { get; set; }

        // ── Budget membership ─────────────────────────────────────────────────
        /// <summary>
        /// Null  = user has not yet created or joined a shared budget.
        /// HasValue = the SharedBudget this user belongs to.
        /// A user can belong to at most one SharedBudget (owner OR member).
        /// </summary>
        [DisplayName("Shared Budget")]
        public Guid? SharedBudgetId { get; set; }

        [DisplayName("Budget Role")]
        public BudgetRole? BudgetRole { get; set; }

        // ── Computed ─────────────────────────────────────────────────────────
        [DisplayName("Full Name")]
        public string FullName => $"{FirstName} {LastName}".Trim();

        [DisplayName("Initials")]
        public string Initials =>
            $"{(FirstName.Length > 0 ? FirstName[0] : ' ')}{(LastName.Length > 0 ? LastName[0] : ' ')}".Trim().ToUpper();

        [DisplayName("Has Budget")]
        public bool HasBudget => SharedBudgetId.HasValue;

        [DisplayName("Is Owner")]
        public bool IsOwner => BudgetRole == Models.BudgetRole.Owner;
    }
}
