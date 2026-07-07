namespace EMK.Save.BL.Models
{
    /// <summary>
    /// Composite read model assembled by DashboardManager for a single API call.
    /// Not persisted — built fresh on each request.
    /// </summary>
    public class DashboardSummary
    {
        public Guid SharedBudgetId { get; set; }
        public int  Month { get; set; }
        public int  Year  { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}")] public decimal TotalIncome   { get; set; }
        [DisplayFormat(DataFormatString = "{0:C}")] public decimal TotalExpenses { get; set; }
        [DisplayFormat(DataFormatString = "{0:C}")] public decimal TotalBudgeted { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}")] public decimal NetCashFlow    => TotalIncome - TotalExpenses;
        [DisplayFormat(DataFormatString = "{0:C}")] public decimal BudgetRemaining => TotalBudgeted - TotalExpenses;

        public double SavingsRate =>
            TotalIncome == 0 ? 0 : Math.Round((double)(NetCashFlow / TotalIncome) * 100.0, 1);

        public List<Budget>           Budgets                { get; set; } = new();
        public List<Transaction>      RecentTransactions     { get; set; } = new();
        public List<Transaction>      UnassignedTransactions { get; set; } = new();
        public List<CategorySummary>  CategorySummaries      { get; set; } = new();
        public List<CashFlowEntry>    CashFlow               { get; set; } = new();
        public List<TrackingInsight>  Insights               { get; set; } = new();
        public List<PlaidAccount>     LinkedAccounts         { get; set; } = new();
        public List<PushNotification> UnreadNotifications    { get; set; } = new();
        public SharedBudget?          SharedBudget           { get; set; }

        public bool   HasOverBudgetCategories => CategorySummaries.Any(c => c.IsOverBudget);
        public int    UnassignedCount         => UnassignedTransactions.Count;
        public string MonthLabel              => new DateTime(Year, Month, 1).ToString("MMMM yyyy");

        public List<CategorySummary> OverBudgetCategories =>
            CategorySummaries.Where(c => c.IsOverBudget).ToList();
    }
}
