namespace EMK.Save.BL
{
    public class BudgetCategoryManager : GenericManager<tblBudgetCategory>
    {
        public BudgetCategoryManager(DbContextOptions<SaveEntities> options, ILogger logger) : base(options, logger) { }

        public async Task<Guid> InsertAsync(BudgetCategory category, bool rollback = false)
        {
            try
            {
                tblBudgetCategory row = Map<BudgetCategory, tblBudgetCategory>(category);
                row.CategoryType = (int)category.CategoryType;
                return await base.InsertAsync(row,
                    e => e.UserId == category.UserId && e.Name == category.Name,
                    rollback);
            }
            catch (Exception) { throw; }
        }

        public async Task<int> UpdateAsync(BudgetCategory category, bool rollback = false)
        {
            try
            {
                tblBudgetCategory row = Map<BudgetCategory, tblBudgetCategory>(category);
                row.CategoryType = (int)category.CategoryType;
                return await base.UpdateAsync(row, null, rollback);
            }
            catch (Exception) { throw; }
        }

        public async Task<List<BudgetCategory>> LoadAsync(Guid? userId = null)
        {
            try
            {
                Expression<Func<tblBudgetCategory, bool>>? filter =
                    userId.HasValue ? e => e.UserId == userId.Value && e.IsActive : e => e.IsActive;

                var rows = new List<BudgetCategory>();
                (await base.LoadAsync(filter))
                    .OrderBy(e => e.SortOrder)
                    .ToList()
                    .ForEach(e =>
                    {
                        var cat = Map<tblBudgetCategory, BudgetCategory>(e);
                        cat.CategoryType = (CategoryType)e.CategoryType;
                        rows.Add(cat);
                    });
                return rows;
            }
            catch (Exception) { throw; }
        }

        public async Task<BudgetCategory> LoadByIdAsync(Guid id)
        {
            try
            {
                var row = (await base.LoadAsync(e => e.Id == id)).FirstOrDefault()
                          ?? throw new Exception("BudgetCategory not found.");
                var cat = Map<tblBudgetCategory, BudgetCategory>(row);
                cat.CategoryType = (CategoryType)row.CategoryType;
                return cat;
            }
            catch (Exception) { throw; }
        }
    }
}
