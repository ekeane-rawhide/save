namespace EMK.Save.BL.Models
{
    public class PlaidAccount
    {
        public Guid Id { get; set; }

        [DisplayName("User")]
        public Guid UserId { get; set; }

        [DisplayName("Shared Budget")]
        public Guid SharedBudgetId { get; set; }

        [DisplayName("Plaid Account ID")]
        public string PlaidAccountId { get; set; } = string.Empty;

        [DisplayName("Plaid Item ID")]
        public string PlaidItemId { get; set; } = string.Empty;

        [DisplayName("Institution Name")]
        public string InstitutionName { get; set; } = string.Empty;

        [DisplayName("Institution Logo")]
        public string InstitutionLogoUrl { get; set; } = string.Empty;

        [DisplayName("Account Name")]
        public string AccountName { get; set; } = string.Empty;

        [DisplayName("Mask")]
        public string Mask { get; set; } = string.Empty;

        [DisplayName("Account Type")]
        public string AccountType { get; set; } = string.Empty;

        [DisplayName("Account Subtype")]
        public string AccountSubtype { get; set; } = string.Empty;

        [DisplayName("Current Balance")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal CurrentBalance { get; set; }

        [DisplayName("Available Balance")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal AvailableBalance { get; set; }

        [DisplayName("Currency")]
        public string IsoCurrencyCode { get; set; } = "USD";

        [DisplayName("Last Synced")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy h:mm tt}")]
        public DateTime LastSynced { get; set; }

        [DisplayName("Is Active")]
        public bool IsActive { get; set; } = true;

        [DisplayName("Date Linked")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime DateLinked { get; set; }

        [DisplayName("Display Name")]
        public string DisplayName => $"{InstitutionName} – {AccountName} ••••{Mask}";
    }
}
