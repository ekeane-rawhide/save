namespace EMK.Save.BL
{
    public class BudgetManager : GenericManager<tblBudget>
    {
        public BudgetManager(DbContextOptions<SaveEntities> options, ILogger logger) : base(options, logger) { }

        public async Task<Guid> InsertAsync(Budget budget, bool rollback = false)
        {
            try
            {
                tblBudget row = Map<Budget, tblBudget>(budget);
                return await base.InsertAsync(row,
                    e => e.SharedBudgetId == budget.SharedBudgetId
                      && e.CategoryId     == budget.CategoryId
                      && e.Month          == budget.Month
                      && e.Year           == budget.Year,
                    rollback);
            }
            catch (Exception) { throw; }
        }

        public async Task<int> UpdateAsync(Budget budget, bool rollback = false)
        {
            try
            {
                tblBudget row = Map<Budget, tblBudget>(budget);
                return await base.UpdateAsync(row, null, rollback);
            }
            catch (Exception) { throw; }
        }

        public async Task<List<Budget>> LoadAsync(Guid sharedBudgetId, int month, int year)
        {
            try
            {
                Expression<Func<tblBudget, object>>[] includes =
                [
                    x => x.Category,
                    x => x.Transactions
                ];

                var rows = new List<Budget>();

                (await base.LoadAsync(
                    e => e.SharedBudgetId == sharedBudgetId
                      && e.Month          == month
                      && e.Year           == year,
                    includes))
                .ForEach(e =>
                {
                    Budget b        = Map<tblBudget, Budget>(e);
                    b.CategoryName  = e.Category.Name;
                    b.CategoryIcon  = e.Category.Icon;
                    b.CategoryColor = e.Category.Color;
                    b.CategoryType  = (CategoryType)e.Category.CategoryType;
                    b.Transactions  = e.Transactions
                        .Select(t => Map<tblTransaction, Transaction>(t))
                        .ToList();
                    rows.Add(b);
                });

                return rows;
            }
            catch (Exception) { throw; }
        }

        public new async Task<Budget> LoadByIdAsync(Guid id)
        {
            try
            {
                Expression<Func<tblBudget, object>>[] includes =
                [
                    x => x.Category,
                    x => x.Transactions
                ];

                tblBudget e0 = (await base.LoadAsync(e => e.Id == id, includes)).FirstOrDefault()
                               ?? throw new Exception("Budget not found.");

                Budget b        = Map<tblBudget, Budget>(e0);
                b.CategoryName  = e0.Category.Name;
                b.CategoryIcon  = e0.Category.Icon;
                b.CategoryColor = e0.Category.Color;
                b.CategoryType  = (CategoryType)e0.Category.CategoryType;
                b.Transactions  = e0.Transactions
                    .Select(t => Map<tblTransaction, Transaction>(t))
                    .ToList();
                return b;
            }
            catch (Exception) { throw; }
        }
    }
}
