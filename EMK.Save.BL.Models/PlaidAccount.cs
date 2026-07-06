
using System.ComponentModel.DataAnnotations;

namespace EMK.Save.BL.Models
{
    /// <summary>
    /// Represents a bank account connected via Plaid Link.
    /// Stores Plaid metadata; sensitive tokens live server-side only.
    /// Maps to tblPlaidAccount in the PL.
    /// </summary>
    public class PlaidAccount
    {
        public Guid Id { get; set; }

        [DisplayName("User")]
        public Guid UserId { get; set; }

        /// <summary>Plaid's stable account_id for this account.</summary>
        [DisplayName("Plaid Account ID")]
        public string PlaidAccountId { get; set; } = string.Empty;

        /// <summary>Plaid's item_id (one item = one institution connection).</summary>
        [DisplayName("Plaid Item ID")]
        public string PlaidItemId { get; set; } = string.Empty;

        [DisplayName("Institution Name")]
        public string InstitutionName { get; set; } = string.Empty;

        [DisplayName("Institution Logo URL")]
        public string InstitutionLogoUrl { get; set; } = string.Empty;

        [DisplayName("Account Name")]
        public string AccountName { get; set; } = string.Empty;

        [DisplayName("Account Mask")]
        public string Mask { get; set; } = string.Empty;    // last-4 digits

        [DisplayName("Account Type")]
        public string AccountType { get; set; } = string.Empty;     // depository, credit, etc.

        [DisplayName("Account Subtype")]
        public string AccountSubtype { get; set; } = string.Empty;  // checking, savings, etc.

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

        // ── Computed ─────────────────────────────────────────────────────────
        [DisplayName("Account Display Name")]
        public string DisplayName =>
            $"{InstitutionName} – {AccountName} ••••{Mask}";

        [DisplayName("Account Type Label")]
        public string TypeLabel =>
            $"{AccountType} / {AccountSubtype}";
    }
}
