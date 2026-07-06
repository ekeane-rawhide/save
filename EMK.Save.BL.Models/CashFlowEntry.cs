
using System.ComponentModel.DataAnnotations;

namespace EMK.Save.BL.Models
{
    /// <summary>
    /// A single day's aggregated cash-flow data point.
    /// The manager builds a list of these for the current month
    /// to drive the running-balance / waterfall chart.
    /// Maps to tblCashFlowEntry in the PL.
    /// </summary>
    public class CashFlowEntry
    {
        public Guid Id { get; set; }

        [DisplayName("User")]
        public Guid UserId { get; set; }

        [DisplayName("Date")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime EntryDate { get; set; }

        [DisplayName("Month")]
        public int Month => EntryDate.Month;

        [DisplayName("Year")]
        public int Year => EntryDate.Year;

        [DisplayName("Total Income")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal DayIncome { get; set; }

        [DisplayName("Total Expenses")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal DayExpenses { get; set; }

        [DisplayName("Running Balance")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal RunningBalance { get; set; }

        [DisplayName("Projected Balance")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal ProjectedBalance { get; set; }

        [DisplayName("Transaction Count")]
        public int TransactionCount { get; set; }

        // ── Computed ─────────────────────────────────────────────────────────
        [DisplayName("Net")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal DayNet => DayIncome - DayExpenses;

        [DisplayName("Is Future")]
        public bool IsFuture => EntryDate.Date > DateTime.Today;

        [DisplayName("Day Label")]
        public string DayLabel => EntryDate.ToString("MMM d");
    }
}
