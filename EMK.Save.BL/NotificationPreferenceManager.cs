namespace EMK.Save.BL
{
    public class NotificationPreferenceManager : GenericManager<tblNotificationPreference>
    {
        public NotificationPreferenceManager(DbContextOptions<SaveEntities> options, ILogger logger) : base(options, logger) { }

        public async Task<Guid> InsertAsync(NotificationPreference pref, bool rollback = false)
        {
            try
            {
                tblNotificationPreference row = Map<NotificationPreference, tblNotificationPreference>(pref);
                row.LastUpdated = DateTime.Now;
                return await base.InsertAsync(row, e => e.UserId == pref.UserId, rollback);
            }
            catch (Exception) { throw; }
        }

        public async Task<int> UpdateAsync(NotificationPreference pref, bool rollback = false)
        {
            try
            {
                tblNotificationPreference row = Map<NotificationPreference, tblNotificationPreference>(pref);
                row.LastUpdated = DateTime.Now;
                return await base.UpdateAsync(row, null, rollback);
            }
            catch (Exception) { throw; }
        }

        public async Task<NotificationPreference> LoadByUserAsync(Guid userId)
        {
            try
            {
                var row = (await base.LoadAsync(e => e.UserId == userId)).FirstOrDefault()
                          ?? throw new Exception("Notification preferences not found.");
                return Map<tblNotificationPreference, NotificationPreference>(row);
            }
            catch (Exception) { throw; }
        }

        /// <summary>
        /// Saves the Web Push subscription keys received from the browser after
        /// the user grants notification permission.
        /// </summary>
        public async Task<int> SaveSubscriptionAsync(
            Guid userId, string endpoint, string p256dh, string auth, bool rollback = false)
        {
            try
            {
                using var dc = new SaveEntities(options);
                IDbContextTransaction? txn = null;
                if (rollback) txn = dc.Database.BeginTransaction();

                var row = dc.tblNotificationPreferences.FirstOrDefault(p => p.UserId == userId)
                          ?? throw new Exception("Preferences not found.");

                row.PushEndpoint   = endpoint;
                row.P256dhKey      = p256dh;
                row.AuthKey        = auth;
                row.IsPushEnabled  = true;
                row.LastUpdated    = DateTime.Now;

                var result = dc.SaveChanges();
                if (rollback) txn?.Rollback();
                return result;
            }
            catch (Exception) { throw; }
        }

        /// <summary>Disables push for the user and clears VAPID keys.</summary>
        public async Task<int> RevokeSubscriptionAsync(Guid userId, bool rollback = false)
        {
            try
            {
                using var dc = new SaveEntities(options);
                IDbContextTransaction? txn = null;
                if (rollback) txn = dc.Database.BeginTransaction();

                var row = dc.tblNotificationPreferences.FirstOrDefault(p => p.UserId == userId)
                          ?? throw new Exception("Preferences not found.");

                row.PushEndpoint  = string.Empty;
                row.P256dhKey     = string.Empty;
                row.AuthKey       = string.Empty;
                row.IsPushEnabled = false;
                row.LastUpdated   = DateTime.Now;

                var result = dc.SaveChanges();
                if (rollback) txn?.Rollback();
                return result;
            }
            catch (Exception) { throw; }
        }
    }
}
