namespace EMK.Save.BL.Models
{
    /// <summary>
    /// A per-category spending roll-up for a given month.
    /// Embedded in MonthlySnapshot and returned by the dashboard endpoint.
    /// Maps to tblCategorySummary in the PL.
    /// </summary>
    public class CategorySummary
    {
        public Guid Id { get; set; }

        [DisplayName("Snapshot")]
        public Guid SnapshotId { get; set; }

        [DisplayName("Category")]
        public Guid CategoryId { get; set; }

        [DisplayName("Category Name")]
        public string CategoryName { get; set; } = string.Empty;

        [DisplayName("Category Icon")]
        public string CategoryIcon { get; set; } = string.Empty;

        [DisplayName("Category Color")]
        public string CategoryColor { get; set; } = string.Empty;

        [DisplayName("Category Type")]
        public CategoryType CategoryType { get; set; }

        [DisplayName("Planned Amount")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal PlannedAmount { get; set; }

        [DisplayName("Actual Amount")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal ActualAmount { get; set; }

        [DisplayName("Transaction Count")]
        public int TransactionCount { get; set; }

        // ── Computed ─────────────────────────────────────────────────────────
        [DisplayName("Variance")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal Variance => PlannedAmount - ActualAmount;

        [DisplayName("% Used")]
        public double PercentUsed =>
            PlannedAmount == 0 ? 0
            : Math.Min((double)(ActualAmount / PlannedAmount) * 100.0, 999.9);

        [DisplayName("Over Budget")]
        public bool IsOverBudget => ActualAmount > PlannedAmount;

        [DisplayName("Variance Label")]
        public string VarianceLabel =>
            IsOverBudget
                ? $"{Math.Abs(Variance):C} over"
                : $"{Variance:C} remaining";
    }
}
