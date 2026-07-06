namespace EMK.Save.BL
{
    /// <summary>
    /// Assembles the DashboardSummary composite model used by the
    /// /api/Dashboard endpoint.  Not backed by a tbl entity — read-only aggregation.
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

        public async Task<DashboardSummary> LoadAsync(Guid userId, int month, int year)
        {
            try
            {
                // Parallel loads using individual managers
                var budgetMgr    = new BudgetManager(options, logger!);
                var txMgr        = new TransactionManager(options, logger!);
                var cashMgr      = new CashFlowManager(options, logger!);
                var insightMgr   = new TrackingInsightManager(options, logger!);
                var accountMgr   = new PlaidAccountManager(options, logger!);
                var notifMgr     = new PushNotificationManager(options, logger!);
                var snapshotMgr  = new MonthlySnapshotManager(options, logger!);

                var budgetsTask      = budgetMgr.LoadAsync(userId, month, year);
                var allTxTask        = txMgr.LoadAsync(userId, month, year);
                var unassignedTxTask = txMgr.LoadAsync(userId, month, year, unassignedOnly: true);
                var cashTask         = cashMgr.LoadAsync(userId, month, year);
                var insightsTask     = insightMgr.LoadAsync(userId, month, year);
                var accountsTask     = accountMgr.LoadAsync(userId);
                var notifTask        = notifMgr.LoadAsync(userId, unreadOnly: true);

                await Task.WhenAll(budgetsTask, allTxTask, unassignedTxTask,
                                   cashTask, insightsTask, accountsTask, notifTask);

                var budgets      = budgetsTask.Result;
                var allTx        = allTxTask.Result;
                var unassignedTx = unassignedTxTask.Result;
                var cashFlow     = cashTask.Result;
                var insights     = insightsTask.Result;
                var accounts     = accountsTask.Result;
                var notifs       = notifTask.Result;

                // Build category summaries from live budget + transaction data
                var summaries = budgets.Select(b => new CategorySummary
                {
                    CategoryId       = b.CategoryId,
                    CategoryName     = b.CategoryName,
                    CategoryIcon     = b.CategoryIcon,
                    CategoryColor    = b.CategoryColor,
                    CategoryType     = b.CategoryType,
                    PlannedAmount    = b.PlannedAmount,
                    ActualAmount     = b.AmountSpent,
                    TransactionCount = b.Transactions.Count
                }).ToList();

                decimal totalIncome   = allTx.Where(t => t.IsIncome).Sum(t => t.Amount);
                decimal totalExpenses = allTx.Where(t => t.IsExpense).Sum(t => t.DisplayAmount);
                decimal totalBudgeted = budgets.Sum(b => b.PlannedAmount);

                logger?.LogInformation(
                    "Dashboard loaded for user {UserId} {Month}/{Year}: {TxCount} tx, {BudgetCount} budgets",
                    userId, month, year, allTx.Count, budgets.Count);

                return new DashboardSummary
                {
                    UserId               = userId,
                    Month                = month,
                    Year                 = year,
                    TotalIncome          = totalIncome,
                    TotalExpenses        = totalExpenses,
                    TotalBudgeted        = totalBudgeted,
                    Budgets              = budgets,
                    RecentTransactions   = allTx.Take(10).ToList(),
                    UnassignedTransactions = unassignedTx,
                    CategorySummaries    = summaries,
                    CashFlow             = cashFlow,
                    Insights             = insights,
                    LinkedAccounts       = accounts,
                    UnreadNotifications  = notifs
                };
            }
            catch (Exception) { throw; }
        }
    }
}
