namespace EMK.Save.BL
{
    /// <summary>
    /// Assembles the DashboardSummary composite model used by the /api/Dashboard endpoint.
    /// Not backed by a tbl entity — read-only aggregation across all managers.
    /// Fires all sub-loads in parallel via Task.WhenAll.
    /// </summary>
    public class DashboardManager
    {
        private readonly DbContextOptions<SaveEntities> options;
        private readonly ILogger? logger;

        public DashboardManager(DbContextOptions<SaveEntities> options, ILogger logger)
        {
            this.options = options;
            this.logger  = logger;
        }

        public async Task<DashboardSummary> LoadAsync(
            Guid sharedBudgetId, Guid userId, int month, int year)
        {
            try
            {
                var budgetMgr   = new BudgetManager(options, logger!);
                var txMgr       = new TransactionManager(options, logger!);
                var cashMgr     = new CashFlowManager(options, logger!);
                var insightMgr  = new TrackingInsightManager(options, logger!);
                var accountMgr  = new PlaidAccountManager(options, logger!);
                var notifMgr    = new PushNotificationManager(options, logger!);
                var sbMgr       = new SharedBudgetManager(options, logger!);

                Task<List<Budget>>           t1 = budgetMgr.LoadAsync(sharedBudgetId, month, year);
                Task<List<Transaction>>      t2 = txMgr.LoadAsync(sharedBudgetId, month, year);
                Task<List<Transaction>>      t3 = txMgr.LoadAsync(sharedBudgetId, month, year, unassignedOnly: true);
                Task<List<CashFlowEntry>>    t4 = cashMgr.LoadAsync(sharedBudgetId, month, year);
                Task<List<TrackingInsight>>  t5 = insightMgr.LoadAsync(sharedBudgetId, month, year);
                Task<List<PlaidAccount>>     t6 = accountMgr.LoadAsync(sharedBudgetId);
                Task<List<PushNotification>> t7 = notifMgr.LoadAsync(userId, unreadOnly: true);
                Task<SharedBudget>           t8 = sbMgr.LoadByIdAsync(sharedBudgetId);

                await Task.WhenAll(t1, t2, t3, t4, t5, t6, t7, t8);

                List<Budget>      budgets = t1.Result;
                List<Transaction> allTx   = t2.Result;

                decimal totalIncome   = allTx.Where(t => t.IsIncome).Sum(t => t.Amount);
                decimal totalExpenses = allTx.Where(t => t.IsExpense).Sum(t => t.DisplayAmount);
                decimal totalBudgeted = budgets.Sum(b => b.PlannedAmount);

                List<CategorySummary> summaries = budgets
                    .Select(b => new CategorySummary
                    {
                        CategoryId       = b.CategoryId,
                        CategoryName     = b.CategoryName,
                        CategoryIcon     = b.CategoryIcon,
                        CategoryColor    = b.CategoryColor,
                        CategoryType     = b.CategoryType,
                        PlannedAmount    = b.PlannedAmount,
                        ActualAmount     = b.AmountSpent,
                        TransactionCount = b.Transactions.Count
                    })
                    .ToList();

                logger?.LogInformation(
                    "Dashboard loaded for SharedBudget {Id} {Month}/{Year}: {TxCount} tx, {BudgetCount} budgets",
                    sharedBudgetId, month, year, allTx.Count, budgets.Count);

                return new DashboardSummary
                {
                    SharedBudgetId         = sharedBudgetId,
                    Month                  = month,
                    Year                   = year,
                    TotalIncome            = totalIncome,
                    TotalExpenses          = totalExpenses,
                    TotalBudgeted          = totalBudgeted,
                    Budgets                = budgets,
                    RecentTransactions     = allTx.Take(10).ToList(),
                    UnassignedTransactions = t3.Result,
                    CategorySummaries      = summaries,
                    CashFlow               = t4.Result,
                    Insights               = t5.Result,
                    LinkedAccounts         = t6.Result,
                    UnreadNotifications    = t7.Result,
                    SharedBudget           = t8.Result
                };
            }
            catch (Exception) { throw; }
        }
    }
}
