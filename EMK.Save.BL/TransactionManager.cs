namespace EMK.Save.BL
{
    public class TransactionManager : GenericManager<tblTransaction>
    {
        public TransactionManager(DbContextOptions<SaveEntities> options, ILogger logger) : base(options, logger) { }

        public async Task<Guid> InsertAsync(Transaction transaction, bool rollback = false)
        {
            try
            {
                tblTransaction row = Map<Transaction, tblTransaction>(transaction);
                return await base.InsertAsync(row,
                    e => e.PlaidTransactionId == transaction.PlaidTransactionId,
                    rollback);
            }
            catch (Exception) { throw; }
        }

        public async Task<int> UpdateAsync(Transaction transaction, bool rollback = false)
        {
            try
            {
                tblTransaction row = Map<Transaction, tblTransaction>(transaction);
                return await base.UpdateAsync(row, null, rollback);
            }
            catch (Exception) { throw; }
        }

        /// <summary>
        /// Upserts a batch of Plaid transactions — insert new, update pending→posted.
        /// Returns count of new rows inserted.
        /// </summary>
        public async Task<int> UpsertFromPlaidAsync(List<Transaction> transactions, bool rollback = false)
        {
            try
            {
                int newCount = 0;
                using var dc = new SaveEntities(options);
                IDbContextTransaction? txn = null;
                if (rollback) txn = dc.Database.BeginTransaction();

                foreach (var t in transactions)
                {
                    var existing = dc.tblTransactions
                        .FirstOrDefault(r => r.PlaidTransactionId == t.PlaidTransactionId);

                    if (existing == null)
                    {
                        var row = Map<Transaction, tblTransaction>(t);
                        row.Id = Guid.NewGuid();
                        dc.tblTransactions.Add(row);
                        newCount++;
                    }
                    else
                    {
                        // Update pending → posted status and amount
                        existing.IsPending       = t.IsPending;
                        existing.PostedDate       = t.PostedDate;
                        existing.Amount           = t.Amount;
                        existing.MerchantName     = t.MerchantName;
                    }
                }

                dc.SaveChanges();
                if (rollback) txn?.Rollback();
                return newCount;
            }
            catch (Exception) { throw; }
        }

        /// <summary>Assigns a transaction to a budget category.</summary>
        public async Task<int> AssignCategoryAsync(Guid transactionId, Guid? categoryId, bool rollback = false)
        {
            try
            {
                using var dc = new SaveEntities(options);
                IDbContextTransaction? txn = null;
                if (rollback) txn = dc.Database.BeginTransaction();

                var row = dc.tblTransactions.FirstOrDefault(t => t.Id == transactionId)
                          ?? throw new Exception("Transaction not found.");

                row.CategoryId  = categoryId;
                row.IsReviewed  = true;
                var result      = dc.SaveChanges();

                if (rollback) txn?.Rollback();
                return result;
            }
            catch (Exception) { throw; }
        }

        public async Task<List<Transaction>> LoadAsync(
            Guid userId, int month, int year, bool unassignedOnly = false)
        {
            try
            {
                Expression<Func<tblTransaction, object>>[] includes =
                [
                    x => x.PlaidAccount,
                    x => x.Category!
                ];

                // Filter: transactions belonging to accounts of this user, in the month/year
                var rows = new List<Transaction>();

                (await base.LoadAsync(
                    t => t.PlaidAccount.UserId == userId
                      && t.TransactionDate.Month == month
                      && t.TransactionDate.Year  == year
                      && (!unassignedOnly || t.CategoryId == null)
                      && !t.IsExcluded,
                    includes))
                .ForEach(t =>
                {
                    var tx = Map<tblTransaction, Transaction>(t);
                    tx.AccountDisplayName = t.PlaidAccount.AccountName + " ••••" + t.PlaidAccount.Mask;
                    tx.CategoryName       = t.Category?.Name   ?? string.Empty;
                    tx.CategoryIcon       = t.Category?.Icon   ?? string.Empty;
                    tx.CategoryColor      = t.Category?.Color  ?? string.Empty;
                    rows.Add(tx);
                });

                return rows.OrderByDescending(t => t.TransactionDate).ToList();
            }
            catch (Exception) { throw; }
        }

        public async Task<Transaction> LoadByIdAsync(Guid id)
        {
            try
            {
                Expression<Func<tblTransaction, object>>[] includes =
                [
                    x => x.PlaidAccount,
                    x => x.Category!
                ];

                var rows = await base.LoadAsync(e => e.Id == id, includes);
                var t0   = rows.FirstOrDefault() ?? throw new Exception("Transaction not found.");

                var tx = Map<tblTransaction, Transaction>(t0);
                tx.AccountDisplayName = t0.PlaidAccount.AccountName + " ••••" + t0.PlaidAccount.Mask;
                tx.CategoryName       = t0.Category?.Name  ?? string.Empty;
                tx.CategoryIcon       = t0.Category?.Icon  ?? string.Empty;
                tx.CategoryColor      = t0.Category?.Color ?? string.Empty;
                return tx;
            }
            catch (Exception) { throw; }
        }
    }
}
