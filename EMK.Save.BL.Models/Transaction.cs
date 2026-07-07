namespace EMK.Save.BL.Models
{
    public class Transaction
    {
        public Guid Id { get; set; }

        [DisplayName("Account")]
        public Guid PlaidAccountId { get; set; }

        [DisplayName("Account Name")]
        public string AccountDisplayName { get; set; } = string.Empty;

        [DisplayName("Shared Budget")]
        public Guid SharedBudgetId { get; set; }

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

        /// <summary>Negative = expense/debit.  Positive = income/credit.</summary>
        [DisplayName("Amount")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal Amount { get; set; }

        [DisplayName("Currency")]
        public string IsoCurrencyCode { get; set; } = "USD";

        [DisplayName("Plaid Category")]
        public string PlaidCategory { get; set; } = string.Empty;

        [DisplayName("Plaid Subcategory")]
        public string PlaidSubcategory { get; set; } = string.Empty;

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
        public bool IsExcluded { get; set; }

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

        [DisplayName("Assigned")]
        public bool IsAssigned => CategoryId.HasValue && CategoryId != Guid.Empty;

        public int Month => TransactionDate.Month;
        public int Year  => TransactionDate.Year;
    }
}
