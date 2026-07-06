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

        public async Task<List<CashFlowEntry>> LoadAsync(Guid userId, int month, int year)
        {
            try
            {
                var rows = new List<CashFlowEntry>();
                (await base.LoadAsync(e =>
                    e.UserId == userId
                    && e.EntryDate.Month == month
                    && e.EntryDate.Year  == year))
                .OrderBy(e => e.EntryDate)
                .ToList()
                .ForEach(e => rows.Add(Map<tblCashFlowEntry, CashFlowEntry>(e)));
                return rows;
            }
            catch (Exception) { throw; }
        }

        /// <summary>
        /// Rebuilds daily cash-flow entries for the given month from live transaction data.
        /// Projects remaining days using average daily spend.
        /// </summary>
        public async Task<List<CashFlowEntry>> BuildCashFlowAsync(Guid userId, int month, int year)
        {
            try
            {
                using var dc = new SaveEntities(options);

                var transactions = dc.tblTransactions
                    .Where(t => t.PlaidAccount.UserId == userId
                             && t.TransactionDate.Month == month
                             && t.TransactionDate.Year  == year
                             && !t.IsExcluded
                             && !t.IsPending)
                    .OrderBy(t => t.TransactionDate)
                    .ToList();

                // Group by day
                var grouped = transactions
                    .GroupBy(t => t.TransactionDate.Date)
                    .ToDictionary(
                        g => g.Key,
                        g => (
                            income:   g.Where(t => t.Amount > 0).Sum(t => t.Amount),
                            expenses: g.Where(t => t.Amount < 0).Sum(t => Math.Abs(t.Amount)),
                            count:    g.Count()
                        ));

                // Starting balance: first account balance before the month
                decimal startBalance = dc.tblPlaidAccounts
                    .Where(a => a.UserId == userId && a.IsActive)
                    .Sum(a => a.CurrentBalance);

                int daysInMonth = DateTime.DaysInMonth(year, month);
                var entries     = new List<CashFlowEntry>();
                decimal running = startBalance;
                today: DateTime today = DateTime.Today;

                // Average daily spend for projection (past days only)
                var pastDays = grouped.Where(g => g.Key.Date <= today.Date).ToList();
                decimal avgDailySpend = pastDays.Count > 0
                    ? pastDays.Average(g => g.Value.expenses)
                    : 0m;

                // Remove existing entries for the month
                var oldEntries = dc.tblCashFlowEntries
                    .Where(e => e.UserId == userId
                             && e.EntryDate.Month == month
                             && e.EntryDate.Year  == year)
                    .ToList();
                dc.tblCashFlowEntries.RemoveRange(oldEntries);

                for (int day = 1; day <= daysInMonth; day++)
                {
                    var date    = new DateTime(year, month, day);
                    bool isFuture = date.Date > today.Date;

                    decimal dayIncome   = 0m;
                    decimal dayExpenses = 0m;
                    int     count       = 0;

                    if (!isFuture && grouped.TryGetValue(date, out var g))
                    {
                        dayIncome   = g.income;
                        dayExpenses = g.expenses;
                        count       = g.count;
                        running    += dayIncome - dayExpenses;
                    }

                    decimal projected = isFuture
                        ? running - (avgDailySpend * (day - today.Day))
                        : running;

                    var entry = new tblCashFlowEntry
                    {
                        Id               = Guid.NewGuid(),
                        UserId           = userId,
                        EntryDate        = date,
                        DayIncome        = dayIncome,
                        DayExpenses      = dayExpenses,
                        RunningBalance   = isFuture ? 0m : running,
                        ProjectedBalance = projected,
                        TransactionCount = count
                    };

                    dc.tblCashFlowEntries.Add(entry);
                    entries.Add(Map<tblCashFlowEntry, CashFlowEntry>(entry));
                }

                dc.SaveChanges();
                return entries;
            }
            catch (Exception) { throw; }
        }
    }
}
