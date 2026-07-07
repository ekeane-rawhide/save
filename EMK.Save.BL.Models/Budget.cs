namespace EMK.Save.BL.Models
{
    public class Budget
    {
        public Guid Id { get; set; }

        [DisplayName("Shared Budget")]
        public Guid SharedBudgetId { get; set; }

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

        [DisplayName("Month")]
        public int Month { get; set; }

        [DisplayName("Year")]
        public int Year { get; set; }

        [DisplayName("Planned Amount")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal PlannedAmount { get; set; }

        [DisplayName("Rollover")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal RolloverAmount { get; set; }

        [DisplayName("Notes")]
        public string Notes { get; set; } = string.Empty;

        // ── Populated by manager ──────────────────────────────────────────────
        [DisplayName("Transactions")]
        public List<Transaction> Transactions { get; set; } = new List<Transaction>();

        // ── Computed ─────────────────────────────────────────────────────────
        [DisplayName("Spent")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal AmountSpent => Transactions.Sum(t => Math.Abs(t.Amount));

        [DisplayName("Remaining")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal AmountRemaining => PlannedAmount + RolloverAmount - AmountSpent;

        [DisplayName("% Used")]
        public double PercentUsed
        {
            get
            {
                decimal denom = PlannedAmount + RolloverAmount;
                return denom == 0 ? 0 : Math.Min((double)(AmountSpent / denom) * 100.0, 999.9);
            }
        }

        [DisplayName("Over Budget")]
        public bool IsOverBudget => AmountRemaining < 0;

        [DisplayName("Month Label")]
        public string MonthLabel => new DateTime(Year, Month, 1).ToString("MMMM yyyy");
    }
}
