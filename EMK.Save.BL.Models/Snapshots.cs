namespace EMK.Save.BL.Models
{
    public class MonthlySnapshot
    {
        public Guid Id { get; set; }

        [DisplayName("Shared Budget")]
        public Guid SharedBudgetId { get; set; }

        public int Month { get; set; }
        public int Year  { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}")] public decimal TotalIncome    { get; set; }
        [DisplayFormat(DataFormatString = "{0:C}")] public decimal TotalExpenses  { get; set; }
        [DisplayFormat(DataFormatString = "{0:C}")] public decimal TotalBudgeted  { get; set; }
        [DisplayFormat(DataFormatString = "{0:C}")] public decimal TotalSavings   { get; set; }

        public int TransactionCount        { get; set; }
        public int OverBudgetCategoryCount { get; set; }

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime SnapshotDate { get; set; }

        public List<CategorySummary> CategorySummaries { get; set; } = new List<CategorySummary>();

        [DisplayFormat(DataFormatString = "{0:C}")] public decimal NetCashFlow    => TotalIncome - TotalExpenses;
        [DisplayFormat(DataFormatString = "{0:C}")] public decimal BudgetVariance => TotalBudgeted - TotalExpenses;
        public double SavingsRate => TotalIncome == 0 ? 0 : Math.Round((double)(TotalSavings / TotalIncome) * 100, 1);
        public bool   IsUnderBudget => BudgetVariance >= 0;
        public string MonthLabel    => new DateTime(Year, Month, 1).ToString("MMMM yyyy");
    }

    public class CategorySummary
    {
        public Guid Id { get; set; }

        public Guid SnapshotId  { get; set; }
        public Guid CategoryId  { get; set; }

        [DisplayName("Category Name")]  public string CategoryName  { get; set; } = string.Empty;
        [DisplayName("Category Icon")]  public string CategoryIcon  { get; set; } = string.Empty;
        [DisplayName("Category Color")] public string CategoryColor { get; set; } = string.Empty;
        [DisplayName("Category Type")]  public CategoryType CategoryType { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}")] public decimal PlannedAmount { get; set; }
        [DisplayFormat(DataFormatString = "{0:C}")] public decimal ActualAmount  { get; set; }
        public int TransactionCount { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}")] public decimal Variance    => PlannedAmount - ActualAmount;
        public double PercentUsed  => PlannedAmount == 0 ? 0 : Math.Min((double)(ActualAmount / PlannedAmount) * 100, 999.9);
        public bool   IsOverBudget => ActualAmount > PlannedAmount;
        public string VarianceLabel => IsOverBudget ? $"{Math.Abs(Variance):C} over" : $"{Variance:C} remaining";
    }

    public class CashFlowEntry
    {
        public Guid Id { get; set; }

        [DisplayName("Shared Budget")]
        public Guid SharedBudgetId { get; set; }

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime EntryDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}")] public decimal DayIncome        { get; set; }
        [DisplayFormat(DataFormatString = "{0:C}")] public decimal DayExpenses      { get; set; }
        [DisplayFormat(DataFormatString = "{0:C}")] public decimal RunningBalance   { get; set; }
        [DisplayFormat(DataFormatString = "{0:C}")] public decimal ProjectedBalance { get; set; }
        public int TransactionCount { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}")] public decimal DayNet  => DayIncome - DayExpenses;
        public bool   IsFuture  => EntryDate.Date > DateTime.Today;
        public string DayLabel  => EntryDate.ToString("MMM d");
        public int    Month     => EntryDate.Month;
        public int    Year      => EntryDate.Year;
    }
}
