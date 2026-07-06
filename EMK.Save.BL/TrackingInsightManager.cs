namespace EMK.Save.BL
{
    public class TrackingInsightManager : GenericManager<tblTrackingInsight>
    {
        public TrackingInsightManager(DbContextOptions<SaveEntities> options, ILogger logger) : base(options, logger) { }

        public async Task<Guid> InsertAsync(TrackingInsight insight, bool rollback = false)
        {
            try
            {
                tblTrackingInsight row = Map<TrackingInsight, tblTrackingInsight>(insight);
                row.InsightType = (int)insight.InsightType;
                row.Severity    = (int)insight.Severity;
                return await base.InsertAsync(row, null, rollback);
            }
            catch (Exception) { throw; }
        }

        public async Task<int> UpdateAsync(TrackingInsight insight, bool rollback = false)
        {
            try
            {
                tblTrackingInsight row = Map<TrackingInsight, tblTrackingInsight>(insight);
                row.InsightType = (int)insight.InsightType;
                row.Severity    = (int)insight.Severity;
                return await base.UpdateAsync(row, null, rollback);
            }
            catch (Exception) { throw; }
        }

        public async Task<List<TrackingInsight>> LoadAsync(Guid userId, int month, int year)
        {
            try
            {
                var rows = new List<TrackingInsight>();
                (await base.LoadAsync(e =>
                    e.UserId == userId
                    && e.Month == month
                    && e.Year  == year
                    && !e.IsDismissed))
                .OrderByDescending(e => e.Severity)
                .ThenByDescending(e => e.GeneratedOn)
                .ToList()
                .ForEach(e =>
                {
                    var insight = Map<tblTrackingInsight, TrackingInsight>(e);
                    insight.InsightType = (InsightType)e.InsightType;
                    insight.Severity    = (InsightSeverity)e.Severity;
                    rows.Add(insight);
                });
                return rows;
            }
            catch (Exception) { throw; }
        }

        public async Task<int> DismissAsync(Guid id, bool rollback = false)
        {
            try
            {
                using var dc = new SaveEntities(options);
                IDbContextTransaction? txn = null;
                if (rollback) txn = dc.Database.BeginTransaction();

                var row = dc.tblTrackingInsights.FirstOrDefault(i => i.Id == id)
                          ?? throw new Exception("Insight not found.");
                row.IsDismissed = true;
                var result = dc.SaveChanges();
                if (rollback) txn?.Rollback();
                return result;
            }
            catch (Exception) { throw; }
        }

        /// <summary>
        /// Runs all insight rules for the month and persists any new ones.
        /// Designed to be called after a Plaid sync or by a background job.
        /// </summary>
        public async Task<List<TrackingInsight>> GenerateInsightsAsync(Guid userId, int month, int year)
        {
            try
            {
                using var dc = new SaveEntities(options);

                var transactions = dc.tblTransactions
                    .Where(t => t.PlaidAccount.UserId == userId
                             && t.TransactionDate.Month == month
                             && t.TransactionDate.Year  == year
                             && !t.IsExcluded)
                    .ToList();

                var budgets = dc.tblBudgets
                    .Include(b => b.Category)
                    .Where(b => b.UserId == userId && b.Month == month && b.Year == year)
                    .ToList();

                var newInsights = new List<tblTrackingInsight>();
                var now         = DateTime.Now;

                // Rule 1 — Over-budget categories
                foreach (var budget in budgets)
                {
                    decimal actual = transactions
                        .Where(t => t.CategoryId == budget.CategoryId && t.Amount < 0)
                        .Sum(t => Math.Abs(t.Amount));

                    if (actual > budget.PlannedAmount)
                    {
                        decimal over    = actual - budget.PlannedAmount;
                        double  pctOver = budget.PlannedAmount > 0
                            ? Math.Round((double)(over / budget.PlannedAmount) * 100, 1)
                            : 0;

                        newInsights.Add(new tblTrackingInsight
                        {
                            Id          = Guid.NewGuid(), UserId = userId,
                            Month       = month, Year = year,
                            InsightType = (int)InsightType.OverBudget,
                            Severity    = (int)InsightSeverity.Warning,
                            CategoryId  = budget.CategoryId,
                            Title       = $"{budget.Category.Name} over budget",
                            Message     = $"You've spent {over:C} more than planned on {budget.Category.Name} ({pctOver}% over).",
                            Amount      = over,
                            ChangePercent = pctOver,
                            GeneratedOn = now
                        });
                    }
                    // Rule 1b — Budget warning at 80%
                    else if (budget.PlannedAmount > 0 && actual / budget.PlannedAmount >= 0.80m)
                    {
                        newInsights.Add(new tblTrackingInsight
                        {
                            Id          = Guid.NewGuid(), UserId = userId,
                            Month       = month, Year = year,
                            InsightType = (int)InsightType.OverBudget,
                            Severity    = (int)InsightSeverity.Warning,
                            CategoryId  = budget.CategoryId,
                            Title       = $"{budget.Category.Name} at 80%",
                            Message     = $"You've used 80% of your {budget.Category.Name} budget.",
                            Amount      = actual,
                            GeneratedOn = now
                        });
                    }
                }

                // Rule 2 — Unassigned transactions
                int unassigned = transactions.Count(t => t.CategoryId == null && !t.IsPending);
                if (unassigned > 0)
                {
                    newInsights.Add(new tblTrackingInsight
                    {
                        Id          = Guid.NewGuid(), UserId = userId,
                        Month       = month, Year = year,
                        InsightType = (int)InsightType.UnassignedTransactions,
                        Severity    = (int)InsightSeverity.Info,
                        Title       = $"{unassigned} unassigned transaction{(unassigned > 1 ? "s" : "")}",
                        Message     = $"You have {unassigned} transaction{(unassigned > 1 ? "s" : "")} not assigned to a budget category.",
                        GeneratedOn = now
                    });
                }

                // Rule 3 — Large transactions (>$100 default)
                var large = transactions.Where(t => Math.Abs(t.Amount) >= 100m && t.Amount < 0).ToList();
                foreach (var t in large)
                {
                    newInsights.Add(new tblTrackingInsight
                    {
                        Id          = Guid.NewGuid(), UserId = userId,
                        Month       = month, Year = year,
                        InsightType = (int)InsightType.LargeTransaction,
                        Severity    = (int)InsightSeverity.Info,
                        Title       = $"Large charge: {t.MerchantName}",
                        Message     = $"A charge of {Math.Abs(t.Amount):C} at {t.MerchantName} was posted on {t.TransactionDate:MM/dd}.",
                        Amount      = Math.Abs(t.Amount),
                        GeneratedOn = now
                    });
                }

                // Persist — remove old auto-generated insights for the month first
                var old = dc.tblTrackingInsights
                    .Where(i => i.UserId == userId && i.Month == month && i.Year == year)
                    .ToList();
                dc.tblTrackingInsights.RemoveRange(old);
                dc.tblTrackingInsights.AddRange(newInsights);
                dc.SaveChanges();

                return await LoadAsync(userId, month, year);
            }
            catch (Exception) { throw; }
        }
    }
}
