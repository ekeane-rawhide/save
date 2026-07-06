namespace EMK.Save.BL
{
    public class PlaidAccountManager : GenericManager<tblPlaidAccount>
    {
        public PlaidAccountManager(DbContextOptions<SaveEntities> options, ILogger logger) : base(options, logger) { }

        public async Task<Guid> InsertAsync(PlaidAccount account, string accessTokenEncrypted, bool rollback = false)
        {
            try
            {
                tblPlaidAccount row = Map<PlaidAccount, tblPlaidAccount>(account);
                // access token never lives in the BL model — injected here only
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
                // Load existing row to preserve the encrypted token
                using var dc  = new SaveEntities(options);
                var existing  = dc.tblPlaidAccounts.FirstOrDefault(a => a.Id == account.Id)
                                ?? throw new Exception("PlaidAccount not found.");

                existing.AccountName      = account.AccountName;
                existing.CurrentBalance   = account.CurrentBalance;
                existing.AvailableBalance = account.AvailableBalance;
                existing.LastSynced       = DateTime.Now;
                existing.IsActive         = account.IsActive;

                return dc.SaveChanges();
            }
            catch (Exception) { throw; }
        }

        public async Task<List<PlaidAccount>> LoadAsync(Guid userId)
        {
            try
            {
                var rows = new List<PlaidAccount>();
                (await base.LoadAsync(e => e.UserId == userId && e.IsActive))
                    .ForEach(e => rows.Add(Map<tblPlaidAccount, PlaidAccount>(e)));
                return rows;
            }
            catch (Exception) { throw; }
        }

        public async Task<PlaidAccount> LoadByIdAsync(Guid id)
        {
            try
            {
                var row = (await base.LoadAsync(e => e.Id == id)).FirstOrDefault()
                          ?? throw new Exception("PlaidAccount not found.");
                return Map<tblPlaidAccount, PlaidAccount>(row);
            }
            catch (Exception) { throw; }
        }

        /// <summary>Returns the encrypted access token for server-side Plaid API calls.</summary>
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
