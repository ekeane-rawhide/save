
using System.ComponentModel.DataAnnotations;

namespace EMK.Save.BL.Models
{
    /// <summary>
    /// A rule-generated or AI-generated spending insight surfaced on the dashboard.
    /// Examples: "Dining is 30% over budget this month",
    ///           "You saved $200 more than last month".
    /// Maps to tblTrackingInsight in the PL.
    /// </summary>
    public class TrackingInsight
    {
        public Guid Id { get; set; }

        [DisplayName("User")]
        public Guid UserId { get; set; }

        [DisplayName("Month")]
        public int Month { get; set; }

        [DisplayName("Year")]
        public int Year { get; set; }

        [DisplayName("Insight Type")]
        public InsightType InsightType { get; set; }

        [DisplayName("Severity")]
        public InsightSeverity Severity { get; set; }

        [DisplayName("Category")]
        public Guid? CategoryId { get; set; }

        [DisplayName("Category Name")]
        public string CategoryName { get; set; } = string.Empty;

        [DisplayName("Title")]
        public string Title { get; set; } = string.Empty;

        [DisplayName("Message")]
        public string Message { get; set; } = string.Empty;

        [DisplayName("Amount")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal? Amount { get; set; }

        [DisplayName("Change %")]
        public double? ChangePercent { get; set; }

        [DisplayName("Generated On")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy h:mm tt}")]
        public DateTime GeneratedOn { get; set; }

        [DisplayName("Is Dismissed")]
        public bool IsDismissed { get; set; }

        [DisplayName("Is Read")]
        public bool IsRead { get; set; }

        // ── Computed ─────────────────────────────────────────────────────────
        [DisplayName("Severity Label")]
        public string SeverityLabel => Severity.ToString();

        [DisplayName("Type Label")]
        public string TypeLabel => InsightType.ToString();

        [DisplayName("Month Label")]
        public string MonthLabel =>
            new DateTime(Year, Month, 1).ToString("MMMM yyyy");
    }

    public enum InsightType
    {
        OverBudget = 0,
        UnderBudget = 1,
        SpendingSpike = 2,
        SpendingDrop = 3,
        SavingsGoalMet = 4,
        SavingsGoalMissed = 5,
        LargeTransaction = 6,
        UnassignedTransactions = 7,
        CashFlowWarning = 8,
        MonthlyRecap = 9
    }

    public enum InsightSeverity
    {
        Info = 0,
        Success = 1,
        Warning = 2,
        Critical = 3
    }
}
