

namespace EMK.Save.BL.Models
{
    /// <summary>
    /// Stores a user's push-notification opt-in/out settings and
    /// their Web Push subscription keys.
    /// Maps to tblNotificationPreference in the PL.
    /// </summary>
    public class NotificationPreference
    {
        public Guid Id { get; set; }

        [DisplayName("User")]
        public Guid UserId { get; set; }

        // ── Web Push VAPID subscription ──────────────────────────────────────
        [DisplayName("Push Endpoint")]
        public string PushEndpoint { get; set; } = string.Empty;

        [DisplayName("P256dh Key")]
        public string P256dhKey { get; set; } = string.Empty;

        [DisplayName("Auth Key")]
        public string AuthKey { get; set; } = string.Empty;

        // ── Per-event toggles ────────────────────────────────────────────────
        [DisplayName("New Transactions")]
        public bool NotifyOnNewTransaction { get; set; } = true;

        [DisplayName("Budget Overages")]
        public bool NotifyOnBudgetOverage { get; set; } = true;

        [DisplayName("Budget Warnings (80%)")]
        public bool NotifyOnBudgetWarning { get; set; } = true;

        [DisplayName("Large Transactions")]
        public bool NotifyOnLargeTransaction { get; set; } = true;

        [DisplayName("Weekly Summary")]
        public bool NotifyWeeklySummary { get; set; } = false;

        [DisplayName("Monthly Summary")]
        public bool NotifyMonthlySummary { get; set; } = true;

        [DisplayName("Account Sync Errors")]
        public bool NotifyOnSyncError { get; set; } = true;

        [DisplayName("Large Transaction Threshold")]
        public decimal LargeTransactionThreshold { get; set; } = 100.00m;

        [DisplayName("Quiet Hours Start")]
        public TimeOnly QuietHoursStart { get; set; } = new TimeOnly(22, 0);

        [DisplayName("Quiet Hours End")]
        public TimeOnly QuietHoursEnd { get; set; } = new TimeOnly(7, 0);

        [DisplayName("Is Push Enabled")]
        public bool IsPushEnabled { get; set; } = false;

        [DisplayName("Last Updated")]
        public DateTime LastUpdated { get; set; }

        // ── Computed ─────────────────────────────────────────────────────────
        [DisplayName("Has Valid Subscription")]
        public bool HasValidSubscription =>
            IsPushEnabled
            && !string.IsNullOrWhiteSpace(PushEndpoint)
            && !string.IsNullOrWhiteSpace(P256dhKey)
            && !string.IsNullOrWhiteSpace(AuthKey);
    }
}
