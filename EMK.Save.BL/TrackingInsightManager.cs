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

        public async Task<List<TrackingInsight>> LoadAsync(Guid sharedBudgetId, int month, int year)
        {
            try
            {
                var rows = new List<TrackingInsight>();

                (await base.LoadAsync(e => e.SharedBudgetId == sharedBudgetId
                                        && e.Month          == month
                                        && e.Year           == year
                                        && !e.IsDismissed))
                    .OrderByDescending(e => e.Severity)
                    .ThenByDescending(e => e.GeneratedOn)
                    .ToList()
                    .ForEach(e =>
                    {
                        TrackingInsight insight = Map<tblTrackingInsight, TrackingInsight>(e);
                        insight.InsightType     = (InsightType)e.InsightType;
                        insight.Severity        = (InsightSeverity)e.Severity;
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
                IDbContextTransaction? txn = rollback ? dc.Database.BeginTransaction() : null;

                tblTrackingInsight row = dc.tblTrackingInsights.Find(id)
                                         ?? throw new Exception("Insight not found.");
                row.IsDismissed = true;

                int result = dc.SaveChanges();
                txn?.Rollback();
                return result;
            }
            catch (Exception) { throw; }
        }

        /// <summary>
        /// Runs all rule-based insight checks and replaces existing insights for the period.
        /// Rules: over-budget, 80% warning, unassigned transactions, large transactions.
        /// </summary>
        public async Task<List<TrackingInsight>> GenerateInsightsAsync(
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

                DateTime now = DateTime.Now;
                var newInsights = new List<tblTrackingInsight>();

                // Rule 1 — Over-budget & 80% warning per category
                foreach (tblBudget budget in budgets)
                {
                    decimal actual = transactions
                        .Where(t => t.CategoryId == budget.CategoryId && t.Amount < 0)
                        .Sum(t => Math.Abs(t.Amount));

                    if (actual > budget.PlannedAmount)
                    {
                        decimal over = actual - budget.PlannedAmount;
                        double  pct  = budget.PlannedAmount > 0
                            ? Math.Round((double)(over / budget.PlannedAmount) * 100, 1) : 0;

                        newInsights.Add(new tblTrackingInsight
                        {
                            Id             = Guid.NewGuid(),
                            SharedBudgetId = sharedBudgetId,
                            Month          = month,
                            Year           = year,
                            InsightType    = (int)InsightType.OverBudget,
                            Severity       = (int)InsightSeverity.Warning,
                            CategoryId     = budget.CategoryId,
                            Title          = $"{budget.Category.Name} over budget",
                            Message        = $"You've spent {over:C} more than planned on {budget.Category.Name} ({pct}% over).",
                            Amount         = over,
                            ChangePercent  = pct,
                            GeneratedOn    = now
                        });
                    }
                    else if (budget.PlannedAmount > 0
                          && actual / budget.PlannedAmount >= 0.80m)
                    {
                        newInsights.Add(new tblTrackingInsight
                        {
                            Id             = Guid.NewGuid(),
                            SharedBudgetId = sharedBudgetId,
                            Month          = month,
                            Year           = year,
                            InsightType    = (int)InsightType.OverBudget,
                            Severity       = (int)InsightSeverity.Warning,
                            CategoryId     = budget.CategoryId,
                            Title          = $"{budget.Category.Name} at 80%",
                            Message        = $"You've used 80% of your {budget.Category.Name} budget.",
                            Amount         = actual,
                            GeneratedOn    = now
                        });
                    }
                }

                // Rule 2 — Unassigned transactions
                int unassigned = transactions.Count(t => t.CategoryId == null && !t.IsPending);
                if (unassigned > 0)
                {
                    newInsights.Add(new tblTrackingInsight
                    {
                        Id             = Guid.NewGuid(),
                        SharedBudgetId = sharedBudgetId,
                        Month          = month,
                        Year           = year,
                        InsightType    = (int)InsightType.UnassignedTransactions,
                        Severity       = (int)InsightSeverity.Info,
                        Title          = $"{unassigned} unassigned transaction{(unassigned > 1 ? "s" : "")}",
                        Message        = $"Assign them to a category to keep your budget accurate.",
                        GeneratedOn    = now
                    });
                }

                // Rule 3 — Large transactions (≥ $100)
                foreach (tblTransaction t in transactions.Where(t => Math.Abs(t.Amount) >= 100m && t.Amount < 0))
                {
                    newInsights.Add(new tblTrackingInsight
                    {
                        Id             = Guid.NewGuid(),
                        SharedBudgetId = sharedBudgetId,
                        Month          = month,
                        Year           = year,
                        InsightType    = (int)InsightType.LargeTransaction,
                        Severity       = (int)InsightSeverity.Info,
                        Title          = $"Large charge: {t.MerchantName}",
                        Message        = $"{Math.Abs(t.Amount):C} at {t.MerchantName} on {t.TransactionDate:MM/dd}.",
                        Amount         = Math.Abs(t.Amount),
                        GeneratedOn    = now
                    });
                }

                // Replace old auto-generated insights for the period
                dc.tblTrackingInsights.RemoveRange(
                    dc.tblTrackingInsights.Where(i => i.SharedBudgetId == sharedBudgetId
                                                    && i.Month == month && i.Year == year));
                dc.tblTrackingInsights.AddRange(newInsights);
                dc.SaveChanges();

                return await LoadAsync(sharedBudgetId, month, year);
            }
            catch (Exception) { throw; }
        }
    }
}
