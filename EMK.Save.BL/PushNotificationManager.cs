namespace EMK.Save.BL
{
    public class PushNotificationManager : GenericManager<tblPushNotification>
    {
        public PushNotificationManager(DbContextOptions<SaveEntities> options, ILogger logger) : base(options, logger) { }

        public async Task<Guid> InsertAsync(PushNotification notification, bool rollback = false)
        {
            try
            {
                tblPushNotification row = Map<PushNotification, tblPushNotification>(notification);
                row.NotificationType    = (int)notification.NotificationType;
                row.Status              = (int)NotificationStatus.Pending;
                row.ScheduledFor        = notification.ScheduledFor == default
                    ? DateTime.Now : notification.ScheduledFor;
                return await base.InsertAsync(row, null, rollback);
            }
            catch (Exception) { throw; }
        }

        public async Task<int> UpdateAsync(PushNotification notification, bool rollback = false)
        {
            try
            {
                tblPushNotification row = Map<PushNotification, tblPushNotification>(notification);
                row.NotificationType    = (int)notification.NotificationType;
                row.Status              = (int)notification.Status;
                return await base.UpdateAsync(row, null, rollback);
            }
            catch (Exception) { throw; }
        }

        public async Task<List<PushNotification>> LoadAsync(Guid userId, bool unreadOnly = false)
        {
            try
            {
                Expression<Func<tblPushNotification, bool>> filter = unreadOnly
                    ? e => e.UserId == userId && !e.IsRead
                    : e => e.UserId == userId;

                var rows = new List<PushNotification>();

                (await base.LoadAsync(filter))
                    .OrderByDescending(e => e.ScheduledFor)
                    .ToList()
                    .ForEach(e =>
                    {
                        PushNotification n  = Map<tblPushNotification, PushNotification>(e);
                        n.NotificationType  = (NotificationType)e.NotificationType;
                        n.Status            = (NotificationStatus)e.Status;
                        rows.Add(n);
                    });

                return rows;
            }
            catch (Exception) { throw; }
        }

        public async Task<int> MarkReadAsync(Guid id, bool rollback = false)
        {
            try
            {
                using var dc = new SaveEntities(options);
                IDbContextTransaction? txn = rollback ? dc.Database.BeginTransaction() : null;

                tblPushNotification row = dc.tblPushNotifications.Find(id)
                                          ?? throw new Exception("Notification not found.");
                row.IsRead = true;

                int result = dc.SaveChanges();
                txn?.Rollback();
                return result;
            }
            catch (Exception) { throw; }
        }

        // ── Queue helpers ─────────────────────────────────────────────────────

        public async Task<Guid> QueueOverageNotificationAsync(
            Guid sharedBudgetId, Guid userId,
            Guid categoryId, string categoryName, decimal overageAmount,
            bool rollback = false)
        {
            try
            {
                tblNotificationPreference? pref = GetPreference(userId);
                if (pref == null
                 || !pref.NotifyOnBudgetOverage
                 || !pref.IsPushEnabled
                 || !IsOutsideQuietHours(pref))
                    return Guid.Empty;

                tblPushNotification n = new tblPushNotification
                {
                    Id               = Guid.NewGuid(),
                    SharedBudgetId   = sharedBudgetId,
                    UserId           = userId,
                    NotificationType = (int)NotificationType.BudgetOverage,
                    Title            = $"{categoryName} over budget!",
                    Body             = $"You've exceeded your {categoryName} budget by {overageAmount:C}.",
                    Icon             = "/icons/icon-192.png",
                    ActionUrl        = "/budget",
                    PushEndpoint     = pref.PushEndpoint,
                    CategoryId       = categoryId,
                    Amount           = overageAmount,
                    ScheduledFor     = DateTime.Now,
                    Status           = (int)NotificationStatus.Pending,
                    IsRead           = false,
                    ErrorMessage     = string.Empty
                };

                using var dc = new SaveEntities(options);
                IDbContextTransaction? txn = rollback ? dc.Database.BeginTransaction() : null;
                dc.tblPushNotifications.Add(n);
                dc.SaveChanges();
                txn?.Rollback();
                return n.Id;
            }
            catch (Exception) { throw; }
        }

        public async Task<Guid> QueueTransactionNotificationAsync(
            Guid sharedBudgetId, Guid userId,
            Guid transactionId, string merchantName, decimal amount,
            bool rollback = false)
        {
            try
            {
                tblNotificationPreference? pref = GetPreference(userId);
                if (pref == null
                 || !pref.NotifyOnNewTransaction
                 || !pref.IsPushEnabled
                 || !IsOutsideQuietHours(pref))
                    return Guid.Empty;

                tblPushNotification n = new tblPushNotification
                {
                    Id               = Guid.NewGuid(),
                    SharedBudgetId   = sharedBudgetId,
                    UserId           = userId,
                    NotificationType = (int)NotificationType.NewTransaction,
                    Title            = $"New transaction: {merchantName}",
                    Body             = $"A {Math.Abs(amount):C} charge at {merchantName} was posted.",
                    Icon             = "/icons/icon-192.png",
                    ActionUrl        = "/transactions",
                    PushEndpoint     = pref.PushEndpoint,
                    TransactionId    = transactionId,
                    Amount           = Math.Abs(amount),
                    ScheduledFor     = DateTime.Now,
                    Status           = (int)NotificationStatus.Pending,
                    IsRead           = false,
                    ErrorMessage     = string.Empty
                };

                using var dc = new SaveEntities(options);
                IDbContextTransaction? txn = rollback ? dc.Database.BeginTransaction() : null;
                dc.tblPushNotifications.Add(n);
                dc.SaveChanges();
                txn?.Rollback();
                return n.Id;
            }
            catch (Exception) { throw; }
        }

        public async Task<Guid> QueueAccountErrorNotificationAsync(
            Guid sharedBudgetId, Guid userId, string institutionName, bool rollback = false)
        {
            try
            {
                tblNotificationPreference? pref = GetPreference(userId);
                if (pref == null || !pref.NotifyOnSyncError || !pref.IsPushEnabled)
                    return Guid.Empty;

                tblPushNotification n = new tblPushNotification
                {
                    Id               = Guid.NewGuid(),
                    SharedBudgetId   = sharedBudgetId,
                    UserId           = userId,
                    NotificationType = (int)NotificationType.AccountSyncError,
                    Title            = $"{institutionName} needs attention",
                    Body             = $"We couldn't sync {institutionName} — please reconnect this account.",
                    Icon             = "/icons/icon-192.png",
                    ActionUrl        = "/accounts",
                    PushEndpoint     = pref.PushEndpoint,
                    ScheduledFor     = DateTime.Now,
                    Status           = (int)NotificationStatus.Pending,
                    IsRead           = false,
                    ErrorMessage     = string.Empty
                };

                using var dc = new SaveEntities(options);
                IDbContextTransaction? txn = rollback ? dc.Database.BeginTransaction() : null;
                dc.tblPushNotifications.Add(n);
                dc.SaveChanges();
                txn?.Rollback();
                return n.Id;
            }
            catch (Exception) { throw; }
        }

        // ── Private helpers ───────────────────────────────────────────────────
        private tblNotificationPreference? GetPreference(Guid userId)
        {
            using var dc = new SaveEntities(options);
            return dc.tblNotificationPreferences.FirstOrDefault(p => p.UserId == userId);
        }

        private static bool IsOutsideQuietHours(tblNotificationPreference pref)
        {
            TimeOnly now   = TimeOnly.FromDateTime(DateTime.Now);
            TimeOnly start = pref.QuietHoursStart;
            TimeOnly end   = pref.QuietHoursEnd;

            // Window spans midnight
            if (start > end) return now < start && now > end;
            return now < start || now > end;
        }
    }
}
