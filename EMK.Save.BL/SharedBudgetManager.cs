namespace EMK.Save.BL
{
    public class SharedBudgetManager : GenericManager<tblSharedBudget>
    {
        public SharedBudgetManager(DbContextOptions<SaveEntities> options, ILogger logger) : base(options, logger) { }

        // ── Create ────────────────────────────────────────────────────────────
        /// <summary>
        /// Creates a new SharedBudget and makes the requesting user the Owner.
        /// Throws if the user already belongs to any budget.
        /// Seeds default categories automatically.
        /// </summary>
        public async Task<SharedBudget> CreateBudgetAsync(
            Guid ownerId, string name, string description, bool rollback = false)
        {
            try
            {
                using var dc = new SaveEntities(options);
                IDbContextTransaction? txn = rollback ? dc.Database.BeginTransaction() : null;

                tblUser owner = dc.tblUsers.FirstOrDefault(u => u.Id == ownerId)
                                ?? throw new Exception("User not found.");

                if (owner.SharedBudgetId.HasValue)
                    throw new InvalidOperationException(
                        "You already belong to a budget. Leave it before creating a new one.");

                var budget = new tblSharedBudget
                {
                    Id          = Guid.NewGuid(),
                    Name        = name,
                    Description = description,
                    OwnerId     = ownerId,
                    InviteCode  = GenerateUniqueCode(dc),
                    IsActive    = true,
                    DateCreated = DateTime.Now,
                    MaxMembers  = 10
                };

                dc.tblSharedBudgets.Add(budget);

                owner.SharedBudgetId = budget.Id;
                owner.BudgetRole     = (int)BudgetRole.Owner;

                SeedDefaultCategories(dc, budget.Id);
                EnsureNotificationPreference(dc, ownerId);

                dc.SaveChanges();
                txn?.Rollback();

                return await LoadByIdAsync(budget.Id);
            }
            catch (Exception) { throw; }
        }

        // ── Join ──────────────────────────────────────────────────────────────
        /// <summary>
        /// Joins an existing SharedBudget via the 6-character invite code.
        /// Throws if the user already belongs to any budget.
        /// </summary>
        public async Task<SharedBudget> JoinBudgetAsync(
            Guid userId, string inviteCode, bool rollback = false)
        {
            try
            {
                using var dc = new SaveEntities(options);
                IDbContextTransaction? txn = rollback ? dc.Database.BeginTransaction() : null;

                tblUser user = dc.tblUsers.FirstOrDefault(u => u.Id == userId)
                               ?? throw new Exception("User not found.");

                if (user.SharedBudgetId.HasValue)
                    throw new InvalidOperationException(
                        "You already belong to a budget. Leave it before joining another.");

                tblSharedBudget budget = dc.tblSharedBudgets
                                           .Include(b => b.Members)
                                           .FirstOrDefault(b => b.InviteCode == inviteCode.ToUpper()
                                                             && b.IsActive)
                                         ?? throw new Exception("Invite code not found or budget is inactive.");

                if (budget.Members.Count >= budget.MaxMembers)
                    throw new Exception("This budget has reached its member limit.");

                user.SharedBudgetId = budget.Id;
                user.BudgetRole     = (int)BudgetRole.Member;

                EnsureNotificationPreference(dc, userId);

                dc.SaveChanges();
                txn?.Rollback();

                return await LoadByIdAsync(budget.Id);
            }
            catch (Exception) { throw; }
        }

        // ── Leave ─────────────────────────────────────────────────────────────
        /// <summary>
        /// Removes the user from their current budget.
        /// Owners must remove all members first; the budget is then deactivated.
        /// </summary>
        public async Task<int> LeaveBudgetAsync(Guid userId, bool rollback = false)
        {
            try
            {
                using var dc = new SaveEntities(options);
                IDbContextTransaction? txn = rollback ? dc.Database.BeginTransaction() : null;

                tblUser user = dc.tblUsers.FirstOrDefault(u => u.Id == userId)
                               ?? throw new Exception("User not found.");

                if (!user.SharedBudgetId.HasValue)
                    throw new Exception("You are not a member of any budget.");

                if (user.BudgetRole == (int)BudgetRole.Owner)
                {
                    int otherMembers = dc.tblUsers
                        .Count(u => u.SharedBudgetId == user.SharedBudgetId && u.Id != userId);

                    if (otherMembers > 0)
                        throw new InvalidOperationException(
                            "Remove all members before leaving as owner, or transfer ownership first.");

                    // Last person out — deactivate the budget
                    var budget = dc.tblSharedBudgets.Find(user.SharedBudgetId);
                    if (budget != null) budget.IsActive = false;
                }

                user.SharedBudgetId = null;
                user.BudgetRole     = null;

                int result = dc.SaveChanges();
                txn?.Rollback();
                return result;
            }
            catch (Exception) { throw; }
        }

        // ── Load ──────────────────────────────────────────────────────────────
        public async Task<SharedBudget> LoadByIdAsync(Guid id)
        {
            try
            {
                Expression<Func<tblSharedBudget, object>>[] includes =
                    [x => x.Members];

                tblSharedBudget row = (await base.LoadAsync(e => e.Id == id, includes))
                                      .FirstOrDefault()
                                      ?? throw new Exception("SharedBudget not found.");

                SharedBudget sb = Map<tblSharedBudget, SharedBudget>(row);
                sb.Members = row.Members
                    .Select(u =>
                    {
                        var m = Map<tblUser, User>(u);
                        m.BudgetRole = u.BudgetRole.HasValue ? (BudgetRole?)u.BudgetRole.Value : null;
                        return m;
                    })
                    .ToList();
                return sb;
            }
            catch (Exception) { throw; }
        }

        public async Task<SharedBudget?> LoadByInviteCodeAsync(string code)
        {
            try
            {
                Expression<Func<tblSharedBudget, object>>[] includes =
                    [x => x.Members];

                tblSharedBudget? row = (await base.LoadAsync(
                    e => e.InviteCode == code.ToUpper() && e.IsActive, includes))
                    .FirstOrDefault();

                if (row == null) return null;

                SharedBudget sb = Map<tblSharedBudget, SharedBudget>(row);
                sb.Members = row.Members
                    .Select(u => Map<tblUser, User>(u))
                    .ToList();
                return sb;
            }
            catch (Exception) { throw; }
        }

        // ── Helpers ───────────────────────────────────────────────────────────
        private static string GenerateUniqueCode(SaveEntities dc)
        {
            const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
            var rng = new Random();
            string code;
            do
            {
                code = new string(Enumerable.Range(0, 6)
                    .Select(_ => chars[rng.Next(chars.Length)])
                    .ToArray());
            }
            while (dc.tblSharedBudgets.Any(b => b.InviteCode == code));
            return code;
        }

        private static void SeedDefaultCategories(SaveEntities dc, Guid sharedBudgetId)
        {
            var defaults = new (string Name, string Icon, string Color, int Type, int Sort)[]
            {
                ("Housing",       "ti-home",          "#2a78d6", 1, 1),
                ("Groceries",     "ti-shopping-cart", "#1baf7a", 1, 2),
                ("Dining Out",    "ti-tools-kitchen", "#eda100", 1, 3),
                ("Transport",     "ti-car",           "#4a3aa7", 1, 4),
                ("Entertainment", "ti-device-tv",     "#e34948", 1, 5),
                ("Healthcare",    "ti-heart",         "#e87ba4", 1, 6),
                ("Savings",       "ti-piggy-bank",    "#008300", 2, 7),
                ("Income",        "ti-wallet",        "#1baf7a", 0, 0),
            };

            foreach (var d in defaults)
            {
                dc.tblBudgetCategories.Add(new tblBudgetCategory
                {
                    Id             = Guid.NewGuid(),
                    SharedBudgetId = sharedBudgetId,
                    Name           = d.Name,
                    Icon           = d.Icon,
                    Color          = d.Color,
                    CategoryType   = d.Type,
                    SortOrder      = d.Sort,
                    IsActive       = true
                });
            }
        }

        private static void EnsureNotificationPreference(SaveEntities dc, Guid userId)
        {
            if (!dc.tblNotificationPreferences.Any(p => p.UserId == userId))
            {
                dc.tblNotificationPreferences.Add(new tblNotificationPreference
                {
                    Id                         = Guid.NewGuid(),
                    UserId                     = userId,
                    PushEndpoint               = string.Empty,
                    P256dhKey                  = string.Empty,
                    AuthKey                    = string.Empty,
                    NotifyOnNewTransaction     = true,
                    NotifyOnBudgetOverage      = true,
                    NotifyOnBudgetWarning      = true,
                    NotifyOnLargeTransaction   = true,
                    NotifyWeeklySummary        = false,
                    NotifyMonthlySummary       = true,
                    NotifyOnSyncError          = true,
                    LargeTransactionThreshold  = 100m,
                    QuietHoursStart            = new TimeOnly(22, 0),
                    QuietHoursEnd              = new TimeOnly(7, 0),
                    IsPushEnabled              = false,
                    LastUpdated                = DateTime.Now
                });
            }
        }
    }
}
