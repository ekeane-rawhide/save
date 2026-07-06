namespace EMK.Save.BL
{
    // ── BudgetCategoryManager ─────────────────────────────────────────────────
    public class BudgetCategoryManager : GenericManager<tblBudgetCategory>
    {
        public BudgetCategoryManager(DbContextOptions<SaveEntities> options, ILogger logger) : base(options, logger) { }

        public async Task<Guid> InsertAsync(BudgetCategory cat, bool rollback = false)
        {
            try
            {
                var row = Map<BudgetCategory, tblBudgetCategory>(cat);
                row.CategoryType = (int)cat.CategoryType;
                return await base.InsertAsync(row, e => e.SharedBudgetId == cat.SharedBudgetId && e.Name == cat.Name, rollback);
            }
            catch (Exception) { throw; }
        }

        public async Task<int> UpdateAsync(BudgetCategory cat, bool rollback = false)
        {
            try
            {
                var row = Map<BudgetCategory, tblBudgetCategory>(cat);
                row.CategoryType = (int)cat.CategoryType;
                return await base.UpdateAsync(row, null, rollback);
            }
            catch (Exception) { throw; }
        }

        public async Task<List<BudgetCategory>> LoadAsync(Guid sharedBudgetId)
        {
            try
            {
                var rows = new List<BudgetCategory>();
                (await base.LoadAsync(e => e.SharedBudgetId == sharedBudgetId && e.IsActive))
                    .OrderBy(e => e.SortOrder).ToList()
                    .ForEach(e => { var c = Map<tblBudgetCategory, BudgetCategory>(e); c.CategoryType = (CategoryType)e.CategoryType; rows.Add(c); });
                return rows;
            }
            catch (Exception) { throw; }
        }

        public async Task<BudgetCategory> LoadByIdAsync(Guid id)
        {
            try
            {
                var row = (await base.LoadAsync(e => e.Id == id)).FirstOrDefault() ?? throw new Exception("BudgetCategory not found.");
                var c = Map<tblBudgetCategory, BudgetCategory>(row);
                c.CategoryType = (CategoryType)row.CategoryType;
                return c;
            }
            catch (Exception) { throw; }
        }
    }

    // ── BudgetManager ─────────────────────────────────────────────────────────
    public class BudgetManager : GenericManager<tblBudget>
    {
        public BudgetManager(DbContextOptions<SaveEntities> options, ILogger logger) : base(options, logger) { }

        public async Task<Guid> InsertAsync(Budget budget, bool rollback = false)
        {
            try { return await base.InsertAsync(Map<Budget, tblBudget>(budget), e => e.SharedBudgetId == budget.SharedBudgetId && e.CategoryId == budget.CategoryId && e.Month == budget.Month && e.Year == budget.Year, rollback); }
            catch (Exception) { throw; }
        }

        public async Task<int> UpdateAsync(Budget budget, bool rollback = false)
        {
            try { return await base.UpdateAsync(Map<Budget, tblBudget>(budget), null, rollback); }
            catch (Exception) { throw; }
        }

        public async Task<List<Budget>> LoadAsync(Guid sharedBudgetId, int month, int year)
        {
            try
            {
                Expression<Func<tblBudget, object>>[] inc = [x => x.Category, x => x.Transactions];
                var rows = new List<Budget>();
                (await base.LoadAsync(e => e.SharedBudgetId == sharedBudgetId && e.Month == month && e.Year == year, inc))
                    .ForEach(e =>
                    {
                        var b = Map<tblBudget, Budget>(e);
                        b.CategoryName  = e.Category.Name;
                        b.CategoryIcon  = e.Category.Icon;
                        b.CategoryColor = e.Category.Color;
                        b.CategoryType  = (CategoryType)e.Category.CategoryType;
                        b.Transactions  = e.Transactions.Select(t => Map<tblTransaction, Transaction>(t)).ToList();
                        rows.Add(b);
                    });
                return rows;
            }
            catch (Exception) { throw; }
        }

        public async Task<Budget> LoadByIdAsync(Guid id)
        {
            try
            {
                Expression<Func<tblBudget, object>>[] inc = [x => x.Category, x => x.Transactions];
                var e0 = (await base.LoadAsync(e => e.Id == id, inc)).FirstOrDefault() ?? throw new Exception("Budget not found.");
                var b  = Map<tblBudget, Budget>(e0);
                b.CategoryName  = e0.Category.Name;
                b.CategoryIcon  = e0.Category.Icon;
                b.CategoryColor = e0.Category.Color;
                b.CategoryType  = (CategoryType)e0.Category.CategoryType;
                return b;
            }
            catch (Exception) { throw; }
        }
    }

    // ── PlaidAccountManager ───────────────────────────────────────────────────
    public class PlaidAccountManager : GenericManager<tblPlaidAccount>
    {
        public PlaidAccountManager(DbContextOptions<SaveEntities> options, ILogger logger) : base(options, logger) { }

        public async Task<Guid> InsertAsync(PlaidAccount account, string accessTokenEncrypted, bool rollback = false)
        {
            try
            {
                var row = Map<PlaidAccount, tblPlaidAccount>(account);
                row.AccessTokenEncrypted = accessTokenEncrypted;
                row.DateLinked = row.LastSynced = DateTime.Now;
                return await base.InsertAsync(row, e => e.PlaidAccountId == account.PlaidAccountId, rollback);
            }
            catch (Exception) { throw; }
        }

        public async Task<int> UpdateAsync(PlaidAccount account, bool rollback = false)
        {
            try
            {
                using var dc = new SaveEntities(options);
                var row = dc.tblPlaidAccounts.Find(account.Id) ?? throw new Exception("Account not found.");
                row.CurrentBalance   = account.CurrentBalance;
                row.AvailableBalance = account.AvailableBalance;
                row.AccountName      = account.AccountName;
                row.IsActive         = account.IsActive;
                row.LastSynced       = DateTime.Now;
                return dc.SaveChanges();
            }
            catch (Exception) { throw; }
        }

        public async Task<List<PlaidAccount>> LoadAsync(Guid sharedBudgetId)
        {
            try
            {
                var rows = new List<PlaidAccount>();
                (await base.LoadAsync(e => e.SharedBudgetId == sharedBudgetId && e.IsActive))
                    .ForEach(e => rows.Add(Map<tblPlaidAccount, PlaidAccount>(e)));
                return rows;
            }
            catch (Exception) { throw; }
        }

        public async Task<PlaidAccount> LoadByIdAsync(Guid id)
        {
            try { return Map<tblPlaidAccount, PlaidAccount>((await base.LoadAsync(e => e.Id == id)).FirstOrDefault() ?? throw new Exception("Account not found.")); }
            catch (Exception) { throw; }
        }

        public string GetEncryptedToken(Guid accountId)
        {
            try
            {
                using var dc = new SaveEntities(options);
                return dc.tblPlaidAccounts.Where(a => a.Id == accountId).Select(a => a.AccessTokenEncrypted).FirstOrDefault() ?? throw new Exception("Account not found.");
            }
            catch (Exception) { throw; }
        }
    }
}
