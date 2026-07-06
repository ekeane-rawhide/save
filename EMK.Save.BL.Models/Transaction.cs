
using System.ComponentModel.DataAnnotations;

namespace EMK.Save.BL.Models
{
    /// <summary>
    /// A single bank transaction pulled from Plaid.
    /// Can be assigned to a BudgetCategory by the user.
    /// Maps to tblTransaction in the PL.
    /// </summary>
    public class Transaction
    {
        public Guid Id { get; set; }

        [DisplayName("Account")]
        public Guid PlaidAccountId { get; set; }

        [DisplayName("Account Name")]
        public string AccountDisplayName { get; set; } = string.Empty;

        /// <summary>Plaid's stable transaction_id – used to prevent duplicate imports.</summary>
        [DisplayName("Plaid Transaction ID")]
        public string PlaidTransactionId { get; set; } = string.Empty;

        [DisplayName("Date")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime TransactionDate { get; set; }

        [DisplayName("Posted Date")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime? PostedDate { get; set; }

        [DisplayName("Merchant")]
        public string MerchantName { get; set; } = string.Empty;

        [DisplayName("Description")]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Negative = debit/expense.  Positive = credit/income.
        /// Stored exactly as Plaid returns it.
        /// </summary>
        [DisplayName("Amount")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal Amount { get; set; }

        [DisplayName("Currency")]
        public string IsoCurrencyCode { get; set; } = "USD";

        /// <summary>Plaid's primary category string (e.g. "Food and Drink").</summary>
        [DisplayName("Plaid Category")]
        public string PlaidCategory { get; set; } = string.Empty;

        /// <summary>Plaid's detailed category string (e.g. "Restaurants").</summary>
        [DisplayName("Plaid Subcategory")]
        public string PlaidSubcategory { get; set; } = string.Empty;

        // ── User assignment ──────────────────────────────────────────────────
        [DisplayName("Budget Category")]
        public Guid? CategoryId { get; set; }

        [DisplayName("Category Name")]
        public string CategoryName { get; set; } = string.Empty;

        [DisplayName("Category Icon")]
        public string CategoryIcon { get; set; } = string.Empty;

        [DisplayName("Category Color")]
        public string CategoryColor { get; set; } = string.Empty;

        [DisplayName("Notes")]
        public string Notes { get; set; } = string.Empty;

        [DisplayName("Is Excluded")]
        public bool IsExcluded { get; set; }       // e.g. transfers, splits

        [DisplayName("Is Pending")]
        public bool IsPending { get; set; }

        [DisplayName("Is Reviewed")]
        public bool IsReviewed { get; set; }

        // ── Computed ─────────────────────────────────────────────────────────
        [DisplayName("Is Expense")]
        public bool IsExpense => Amount < 0;

        [DisplayName("Is Income")]
        public bool IsIncome => Amount > 0;

        [DisplayName("Display Amount")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal DisplayAmount => Math.Abs(Amount);

        [DisplayName("Display Name")]
        public string DisplayName =>
            !string.IsNullOrWhiteSpace(MerchantName) ? MerchantName : Description;

        [DisplayName("Month")]
        public int Month => TransactionDate.Month;

        [DisplayName("Year")]
        public int Year => TransactionDate.Year;

        [DisplayName("Assigned")]
        public bool IsAssigned => CategoryId.HasValue && CategoryId != Guid.Empty;
    }
}
