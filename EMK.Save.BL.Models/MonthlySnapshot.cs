
using System.ComponentModel.DataAnnotations;

namespace EMK.Save.BL.Models
{
    /// <summary>
    /// A pre-aggregated monthly summary stored after each month closes.
    /// Drives the monthly history view and year-over-year trend charts.
    /// Maps to tblMonthlySnapshot in the PL.
    /// </summary>
    public class MonthlySnapshot
    {
        public Guid Id { get; set; }

        [DisplayName("User")]
        public Guid UserId { get; set; }

        [DisplayName("Month")]
        public int Month { get; set; }

        [DisplayName("Year")]
        public int Year { get; set; }

        [DisplayName("Total Income")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal TotalIncome { get; set; }

        [DisplayName("Total Expenses")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal TotalExpenses { get; set; }

        [DisplayName("Total Budgeted")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal TotalBudgeted { get; set; }

        [DisplayName("Total Savings")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal TotalSavings { get; set; }

        [DisplayName("Transaction Count")]
        public int TransactionCount { get; set; }

        [DisplayName("Over-Budget Categories")]
        public int OverBudgetCategoryCount { get; set; }

        [DisplayName("Snapshot Date")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime SnapshotDate { get; set; }

        // ── Category breakdowns (populated by manager) ───────────────────────
        [DisplayName("Category Summaries")]
        public List<CategorySummary> CategorySummaries { get; set; } = new List<CategorySummary>();

        // ── Computed ─────────────────────────────────────────────────────────
        [DisplayName("Net Cash Flow")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal NetCashFlow => TotalIncome - TotalExpenses;

        [DisplayName("Budget Variance")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal BudgetVariance => TotalBudgeted - TotalExpenses;

        [DisplayName("Savings Rate")]
        public double SavingsRate =>
            TotalIncome == 0 ? 0 : (double)(TotalSavings / TotalIncome) * 100.0;

        [DisplayName("Month Label")]
        public string MonthLabel =>
            new DateTime(Year, Month, 1).ToString("MMMM yyyy");

        [DisplayName("Under Budget")]
        public bool IsUnderBudget => BudgetVariance >= 0;
    }
}
