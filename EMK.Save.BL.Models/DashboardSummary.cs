
using System.ComponentModel.DataAnnotations;

namespace EMK.Save.BL.Models
{
    /// <summary>
    /// Composite read model returned by the dashboard API endpoint.
    /// Aggregates all data needed to render the monthly dashboard in a single call.
    /// Not persisted — assembled by DashboardManager on each request.
    /// </summary>
    public class DashboardSummary
    {
        [DisplayName("User")]
        public Guid UserId { get; set; }

        [DisplayName("Month")]
        public int Month { get; set; }

        [DisplayName("Year")]
        public int Year { get; set; }

        // ── Totals ───────────────────────────────────────────────────────────
        [DisplayName("Total Income")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal TotalIncome { get; set; }

        [DisplayName("Total Expenses")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal TotalExpenses { get; set; }

        [DisplayName("Total Budgeted")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal TotalBudgeted { get; set; }

        [DisplayName("Net Cash Flow")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal NetCashFlow => TotalIncome - TotalExpenses;

        [DisplayName("Budget Remaining")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal BudgetRemaining => TotalBudgeted - TotalExpenses;

        [DisplayName("Savings Rate")]
        public double SavingsRate =>
            TotalIncome == 0 ? 0
            : Math.Round((double)(NetCashFlow / TotalIncome) * 100.0, 1);

        // ── Child collections ─────────────────────────────────────────────────
        [DisplayName("Budget Categories")]
        public List<Budget> Budgets { get; set; } = new List<Budget>();

        [DisplayName("Recent Transactions")]
        public List<Transaction> RecentTransactions { get; set; } = new List<Transaction>();

        [DisplayName("Unassigned Transactions")]
        public List<Transaction> UnassignedTransactions { get; set; } = new List<Transaction>();

        [DisplayName("Category Summaries")]
        public List<CategorySummary> CategorySummaries { get; set; } = new List<CategorySummary>();

        [DisplayName("Cash Flow")]
        public List<CashFlowEntry> CashFlow { get; set; } = new List<CashFlowEntry>();

        [DisplayName("Insights")]
        public List<TrackingInsight> Insights { get; set; } = new List<TrackingInsight>();

        [DisplayName("Linked Accounts")]
        public List<PlaidAccount> LinkedAccounts { get; set; } = new List<PlaidAccount>();

        [DisplayName("Unread Notifications")]
        public List<PushNotification> UnreadNotifications { get; set; } = new List<PushNotification>();

        // ── Computed flags ────────────────────────────────────────────────────
        [DisplayName("Has Over-Budget Categories")]
        public bool HasOverBudgetCategories =>
            CategorySummaries.Any(c => c.IsOverBudget);

        [DisplayName("Unassigned Count")]
        public int UnassignedCount => UnassignedTransactions.Count;

        [DisplayName("Month Label")]
        public string MonthLabel =>
            new DateTime(Year, Month, 1).ToString("MMMM yyyy");

        [DisplayName("Over-Budget Categories")]
        public List<CategorySummary> OverBudgetCategories =>
            CategorySummaries.Where(c => c.IsOverBudget).ToList();
    }
}
