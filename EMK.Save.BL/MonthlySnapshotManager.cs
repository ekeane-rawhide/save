namespace EMK.Save.BL
{
    public class MonthlySnapshotManager : GenericManager<tblMonthlySnapshot>
    {
        public MonthlySnapshotManager(DbContextOptions<SaveEntities> options, ILogger logger) : base(options, logger) { }

        public async Task<Guid> InsertAsync(MonthlySnapshot snapshot, bool rollback = false)
        {
            try
            {
                tblMonthlySnapshot row = Map<MonthlySnapshot, tblMonthlySnapshot>(snapshot);
                return await base.InsertAsync(row,
                    e => e.SharedBudgetId == snapshot.SharedBudgetId
                      && e.Month          == snapshot.Month
                      && e.Year           == snapshot.Year,
                    rollback);
            }
            catch (Exception) { throw; }
        }

        public async Task<int> UpdateAsync(MonthlySnapshot snapshot, bool rollback = false)
        {
            try
            {
                tblMonthlySnapshot row = Map<MonthlySnapshot, tblMonthlySnapshot>(snapshot);
                return await base.UpdateAsync(row, null, rollback);
            }
            catch (Exception) { throw; }
        }

        public async Task<List<MonthlySnapshot>> LoadAsync(Guid sharedBudgetId, int? limitMonths = null)
        {
            try
            {
                Expression<Func<tblMonthlySnapshot, object>>[] includes =
                    [x => x.CategorySummaries];

                var all = (await base.LoadAsync(e => e.SharedBudgetId == sharedBudgetId, includes))
                    .OrderByDescending(e => e.Year)
                    .ThenByDescending(e => e.Month)
                    .ToList();

                if (limitMonths.HasValue) all = all.Take(limitMonths.Value).ToList();

                return all.Select(e =>
                {
                    MonthlySnapshot s = Map<tblMonthlySnapshot, MonthlySnapshot>(e);
                    s.CategorySummaries = e.CategorySummaries
                        .Select(cs => Map<tblCategorySummary, CategorySummary>(cs))
                        .ToList();
                    return s;
                }).ToList();
            }
            catch (Exception) { throw; }
        }

        public async Task<MonthlySnapshot> LoadByMonthAsync(Guid sharedBudgetId, int month, int year)
        {
            try
            {
                Expression<Func<tblMonthlySnapshot, object>>[] includes =
                    [x => x.CategorySummaries];

                tblMonthlySnapshot e0 = (await base.LoadAsync(
                    e => e.SharedBudgetId == sharedBudgetId && e.Month == month && e.Year == year,
                    includes)).FirstOrDefault()
                    ?? throw new Exception("Snapshot not found.");

                MonthlySnapshot s = Map<tblMonthlySnapshot, MonthlySnapshot>(e0);
                s.CategorySummaries = e0.CategorySummaries
                    .Select(cs => Map<tblCategorySummary, CategorySummary>(cs))
                    .ToList();
                return s;
            }
            catch (Exception) { throw; }
        }

        /// <summary>
        /// Aggregates live transaction data and upserts a MonthlySnapshot for the given period.
        /// </summary>
        public async Task<MonthlySnapshot> BuildSnapshotAsync(
            Guid sharedBudgetId, int month, int year)
        {
            try
            {
                using var dc = new SaveEntities(options);

                var transactions = dc.tblTransactions
                    .Where(t => t.SharedBudgetId       == sharedBudgetId
                             && t.TransactionDate.Month == month
                             && t.TransactionDate.Year  == year
                             && !t.IsExcluded)
                    .ToList();

                var budgets = dc.tblBudgets
                    .Include(b => b.Category)
                    .Where(b => b.SharedBudgetId == sharedBudgetId
                             && b.Month == month && b.Year == year)
                    .ToList();

                decimal income   = transactions.Where(t => t.Amount > 0).Sum(t => t.Amount);
                decimal expenses = transactions.Where(t => t.Amount < 0).Sum(t => Math.Abs(t.Amount));
                decimal budgeted = budgets.Sum(b => b.PlannedAmount);
                decimal savings  = budgets.Where(b => b.Category.CategoryType == 2)
                                          .Sum(b => b.PlannedAmount);

                List<tblCategorySummary> summaries = budgets.Select(b =>
                {
                    decimal actual = transactions
                        .Where(t => t.CategoryId == b.CategoryId && t.Amount < 0)
                        .Sum(t => Math.Abs(t.Amount));

                    return new tblCategorySummary
                    {
                        Id               = Guid.NewGuid(),
                        CategoryId       = b.CategoryId,
                        PlannedAmount    = b.PlannedAmount,
                        ActualAmount     = actual,
                        TransactionCount = transactions.Count(t => t.CategoryId == b.CategoryId)
                    };
                }).ToList();

                int overBudget = summaries.Count(s => s.ActualAmount > s.PlannedAmount);

                tblMonthlySnapshot? existing = dc.tblMonthlySnapshots
                    .FirstOrDefault(s => s.SharedBudgetId == sharedBudgetId
                                      && s.Month == month && s.Year == year);

                if (existing != null)
                {
                    existing.TotalIncome             = income;
                    existing.TotalExpenses            = expenses;
                    existing.TotalBudgeted            = budgeted;
                    existing.TotalSavings             = savings;
                    existing.TransactionCount         = transactions.Count;
                    existing.OverBudgetCategoryCount  = overBudget;
                    existing.SnapshotDate             = DateTime.Now;

                    dc.tblCategorySummaries.RemoveRange(
                        dc.tblCategorySummaries.Where(cs => cs.SnapshotId == existing.Id));
                    summaries.ForEach(s => s.SnapshotId = existing.Id);
                    dc.tblCategorySummaries.AddRange(summaries);
                }
                else
                {
                    var snap = new tblMonthlySnapshot
                    {
                        Id                      = Guid.NewGuid(),
                        SharedBudgetId          = sharedBudgetId,
                        Month                   = month,
                        Year                    = year,
                        TotalIncome             = income,
                        TotalExpenses           = expenses,
                        TotalBudgeted           = budgeted,
                        TotalSavings            = savings,
                        TransactionCount        = transactions.Count,
                        OverBudgetCategoryCount = overBudget,
                        SnapshotDate            = DateTime.Now
                    };
                    summaries.ForEach(s => s.SnapshotId = snap.Id);
                    snap.CategorySummaries = summaries;
                    dc.tblMonthlySnapshots.Add(snap);
                }

                dc.SaveChanges();
                return await LoadByMonthAsync(sharedBudgetId, month, year);
            }
            catch (Exception) { throw; }
        }
    }
}
