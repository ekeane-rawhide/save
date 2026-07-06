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
                row.NotificationType = (int)notification.NotificationType;
                row.Status           = (int)NotificationStatus.Pending;
                row.ScheduledFor     = notification.ScheduledFor == default
                    ? DateTime.Now
                    : notification.ScheduledFor;
                return await base.InsertAsync(row, null, rollback);
            }
            catch (Exception) { throw; }
        }

        public async Task<int> UpdateAsync(PushNotification notification, bool rollback = false)
        {
            try
            {
                tblPushNotification row = Map<PushNotification, tblPushNotification>(notification);
                row.NotificationType = (int)notification.NotificationType;
                row.Status           = (int)notification.Status;
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
                        var n = Map<tblPushNotification, PushNotification>(e);
                        n.NotificationType = (NotificationType)e.NotificationType;
                        n.Status           = (NotificationStatus)e.Status;
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
                IDbContextTransaction? txn = null;
                if (rollback) txn = dc.Database.BeginTransaction();

                var row = dc.tblPushNotifications.FirstOrDefault(n => n.Id == id)
                          ?? throw new Exception("Notification not found.");
                row.IsRead = true;
                var result = dc.SaveChanges();
                if (rollback) txn?.Rollback();
                return result;
            }
            catch (Exception) { throw; }
        }

        /// <summary>
        /// Creates and queues a budget-overage push notification.
        /// Called by TrackingInsightManager after generating insights.
        /// </summary>
        public async Task<Guid> QueueOverageNotificationAsync(
            Guid userId, Guid categoryId, string categoryName, decimal overageAmount, bool rollback = false)
        {
            try
            {
                var pref = await GetPreferenceAsync(userId);
                if (pref == null || !pref.NotifyOnBudgetOverage || !pref.IsPushEnabled)
                    return Guid.Empty;

                if (!IsOutsideQuietHours(pref)) return Guid.Empty;

                var n = new tblPushNotification
                {
                    Id               = Guid.NewGuid(),
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
                IDbContextTransaction? txn = null;
                if (rollback) txn = dc.Database.BeginTransaction();
                dc.tblPushNotifications.Add(n);
                dc.SaveChanges();
                if (rollback) txn?.Rollback();

                return n.Id;
            }
            catch (Exception) { throw; }
        }

        /// <summary>
        /// Creates and queues a new-transaction push notification.
        /// </summary>
        public async Task<Guid> QueueTransactionNotificationAsync(
            Guid userId, Guid transactionId, string merchantName, decimal amount, bool rollback = false)
        {
            try
            {
                var pref = await GetPreferenceAsync(userId);
                if (pref == null || !pref.NotifyOnNewTransaction || !pref.IsPushEnabled)
                    return Guid.Empty;

                if (!IsOutsideQuietHours(pref)) return Guid.Empty;

                var n = new tblPushNotification
                {
                    Id               = Guid.NewGuid(),
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
                IDbContextTransaction? txn = null;
                if (rollback) txn = dc.Database.BeginTransaction();
                dc.tblPushNotifications.Add(n);
                dc.SaveChanges();
                if (rollback) txn?.Rollback();

                return n.Id;
            }
            catch (Exception) { throw; }
        }

        // ── Helpers ───────────────────────────────────────────────────────────
        private async Task<tblNotificationPreference?> GetPreferenceAsync(Guid userId)
        {
            using var dc = new SaveEntities(options);
            return dc.tblNotificationPreferences.FirstOrDefault(p => p.UserId == userId);
        }

        private static bool IsOutsideQuietHours(tblNotificationPreference pref)
        {
            var now   = TimeOnly.FromDateTime(DateTime.Now);
            var start = pref.QuietHoursStart;
            var end   = pref.QuietHoursEnd;

            // Quiet window spans midnight
            if (start > end)
                return now < start && now > end;

            return now < start || now > end;
        }
    }
}
