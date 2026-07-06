
using System.ComponentModel.DataAnnotations;

namespace EMK.Save.BL.Models
{
    /// <summary>
    /// A monthly budget plan for a single category.
    /// One Budget row = one category's planned amount for one calendar month.
    /// Maps to tblBudget in the PL.
    /// </summary>
    public class Budget
    {
        public Guid Id { get; set; }

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
        public int Month { get; set; }           // 1–12

        [DisplayName("Year")]
        public int Year { get; set; }

        [DisplayName("Planned Amount")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal PlannedAmount { get; set; }

        [DisplayName("Rollover from Previous Month")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal RolloverAmount { get; set; }

        [DisplayName("Notes")]
        public string Notes { get; set; } = string.Empty;

        // ── Computed / populated by manager joins ────────────────────────────
        [DisplayName("Transactions")]
        public List<Transaction> Transactions { get; set; } = new List<Transaction>();

        [DisplayName("Spent")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal AmountSpent
        {
            get
            {
                decimal total = 0m;
                foreach (var t in Transactions)
                    total += Math.Abs(t.Amount);
                return total;
            }
        }

        [DisplayName("Remaining")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal AmountRemaining => PlannedAmount + RolloverAmount - AmountSpent;

        [DisplayName("% Used")]
        public double PercentUsed
        {
            get
            {
                decimal denom = PlannedAmount + RolloverAmount;
                if (denom == 0) return 0;
                return (double)(AmountSpent / denom) * 100.0;
            }
        }

        [DisplayName("Over Budget")]
        public bool IsOverBudget => AmountRemaining < 0;

        [DisplayName("Month Label")]
        public string MonthLabel =>
            new DateTime(Year, Month, 1).ToString("MMMM yyyy");
    }
}
