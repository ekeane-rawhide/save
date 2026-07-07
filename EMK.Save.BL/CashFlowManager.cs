namespace EMK.Save.BL
{
    public class CashFlowManager : GenericManager<tblCashFlowEntry>
    {
        public CashFlowManager(DbContextOptions<SaveEntities> options, ILogger logger) : base(options, logger) { }

        public async Task<Guid> InsertAsync(CashFlowEntry entry, bool rollback = false)
        {
            try
            {
                tblCashFlowEntry row = Map<CashFlowEntry, tblCashFlowEntry>(entry);
                return await base.InsertAsync(row, null, rollback);
            }
            catch (Exception) { throw; }
        }

        public async Task<int> UpdateAsync(CashFlowEntry entry, bool rollback = false)
        {
            try
            {
                tblCashFlowEntry row = Map<CashFlowEntry, tblCashFlowEntry>(entry);
                return await base.UpdateAsync(row, null, rollback);
            }
            catch (Exception) { throw; }
        }

        public async Task<List<CashFlowEntry>> LoadAsync(Guid sharedBudgetId, int month, int year)
        {
            try
            {
                var rows = new List<CashFlowEntry>();

                (await base.LoadAsync(e => e.SharedBudgetId    == sharedBudgetId
                                        && e.EntryDate.Month   == month
                                        && e.EntryDate.Year    == year))
                    .OrderBy(e => e.EntryDate)
                    .ToList()
                    .ForEach(e => rows.Add(Map<tblCashFlowEntry, CashFlowEntry>(e)));

                return rows;
            }
            catch (Exception) { throw; }
        }

        /// <summary>
        /// Rebuilds daily cash-flow entries for the month from live transactions.
        /// Future days are projected using average daily spend from past days.
        /// </summary>
        public async Task<List<CashFlowEntry>> BuildCashFlowAsync(
            Guid sharedBudgetId, int month, int year)
        {
            try
            {
                using var dc = new SaveEntities(options);

                var transactions = dc.tblTransactions
                    .Where(t => t.SharedBudgetId       == sharedBudgetId
                             && t.TransactionDate.Month == month
                             && t.TransactionDate.Year  == year
                             && !t.IsExcluded
                             && !t.IsPending)
                    .OrderBy(t => t.TransactionDate)
                    .ToList();

                var grouped = transactions
                    .GroupBy(t => t.TransactionDate.Date)
                    .ToDictionary(
                        g => g.Key,
                        g => (
                            income:   g.Where(t => t.Amount > 0).Sum(t => t.Amount),
                            expenses: g.Where(t => t.Amount < 0).Sum(t => Math.Abs(t.Amount)),
                            count:    g.Count()));

                decimal startBalance = dc.tblPlaidAccounts
                    .Where(a => a.SharedBudgetId == sharedBudgetId && a.IsActive)
                    .Sum(a => a.CurrentBalance);

                DateTime today      = DateTime.Today;
                int      daysInMonth = DateTime.DaysInMonth(year, month);

                decimal avgDailySpend = grouped
                    .Where(g => g.Key.Date <= today.Date)
                    .Select(g => g.Value.expenses)
                    .DefaultIfEmpty(0m)
                    .Average();

                // Remove stale entries
                dc.tblCashFlowEntries.RemoveRange(
                    dc.tblCashFlowEntries.Where(e => e.SharedBudgetId    == sharedBudgetId
                                                  && e.EntryDate.Month   == month
                                                  && e.EntryDate.Year    == year));

                var entries = new List<CashFlowEntry>();
                decimal running = startBalance;

                for (int day = 1; day <= daysInMonth; day++)
                {
                    DateTime date     = new DateTime(year, month, day);
                    bool     isFuture = date.Date > today.Date;

                    decimal dayIncome = 0m, dayExpenses = 0m;
                    int     count     = 0;

                    if (!isFuture && grouped.TryGetValue(date, out var g))
                    {
                        dayIncome   = g.income;
                        dayExpenses = g.expenses;
                        count       = g.count;
                        running    += dayIncome - dayExpenses;
                    }

                    decimal projected = isFuture
                        ? running - avgDailySpend * (day - today.Day)
                        : running;

                    var row = new tblCashFlowEntry
                    {
                        Id               = Guid.NewGuid(),
                        SharedBudgetId   = sharedBudgetId,
                        EntryDate        = date,
                        DayIncome        = dayIncome,
                        DayExpenses      = dayExpenses,
                        RunningBalance   = isFuture ? 0m : running,
                        ProjectedBalance = projected,
                        TransactionCount = count
                    };

                    dc.tblCashFlowEntries.Add(row);
                    entries.Add(Map<tblCashFlowEntry, CashFlowEntry>(row));
                }

                dc.SaveChanges();
                return entries;
            }
            catch (Exception) { throw; }
        }
    }
}
