namespace EMK.Save.BL
{
    public class PlaidAccountManager : GenericManager<tblPlaidAccount>
    {
        public PlaidAccountManager(DbContextOptions<SaveEntities> options, ILogger logger) : base(options, logger) { }

        /// <summary>
        /// Inserts a new linked account.
        /// accessTokenEncrypted is injected here and never travels through the BL model.
        /// </summary>
        public async Task<Guid> InsertAsync(
            PlaidAccount account, string accessTokenEncrypted, bool rollback = false)
        {
            try
            {
                tblPlaidAccount row      = Map<PlaidAccount, tblPlaidAccount>(account);
                row.AccessTokenEncrypted = accessTokenEncrypted;
                row.DateLinked           = DateTime.Now;
                row.LastSynced           = DateTime.Now;
                return await base.InsertAsync(row,
                    e => e.PlaidAccountId == account.PlaidAccountId,
                    rollback);
            }
            catch (Exception) { throw; }
        }

        public async Task<int> UpdateAsync(PlaidAccount account, bool rollback = false)
        {
            try
            {
                // Preserve the encrypted token — only update balance/status fields
                using var dc     = new SaveEntities(options);
                tblPlaidAccount existing = dc.tblPlaidAccounts.Find(account.Id)
                                           ?? throw new Exception("PlaidAccount not found.");
                existing.AccountName      = account.AccountName;
                existing.CurrentBalance   = account.CurrentBalance;
                existing.AvailableBalance = account.AvailableBalance;
                existing.IsActive         = account.IsActive;
                existing.LastSynced       = DateTime.Now;
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

        public new async Task<PlaidAccount> LoadByIdAsync(Guid id)
        {
            try
            {
                tblPlaidAccount row = (await base.LoadAsync(e => e.Id == id)).FirstOrDefault()
                                      ?? throw new Exception("PlaidAccount not found.");
                return Map<tblPlaidAccount, PlaidAccount>(row);
            }
            catch (Exception) { throw; }
        }

        /// <summary>Returns the encrypted access token for server-side Plaid API calls only.</summary>
        public string GetEncryptedAccessToken(Guid accountId)
        {
            try
            {
                using var dc = new SaveEntities(options);
                return dc.tblPlaidAccounts
                         .Where(a => a.Id == accountId)
                         .Select(a => a.AccessTokenEncrypted)
                         .FirstOrDefault()
                       ?? throw new Exception("Account not found.");
            }
            catch (Exception) { throw; }
        }
    }
}
