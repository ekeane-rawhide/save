namespace EMK.Save.PL.Entities;

public partial class tblNotificationPreference : IEntity
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }                // 1-to-1 with tblUser

    public string PushEndpoint { get; set; } = null!;

    public string P256dhKey { get; set; } = null!;

    public string AuthKey { get; set; } = null!;

    public bool NotifyOnNewTransaction { get; set; }

    public bool NotifyOnBudgetOverage { get; set; }

    public bool NotifyOnBudgetWarning { get; set; }

    public bool NotifyOnLargeTransaction { get; set; }

    public bool NotifyWeeklySummary { get; set; }

    public bool NotifyMonthlySummary { get; set; }

    public bool NotifyOnSyncError { get; set; }

    public decimal LargeTransactionThreshold { get; set; }

    public TimeOnly QuietHoursStart { get; set; }

    public TimeOnly QuietHoursEnd { get; set; }

    public bool IsPushEnabled { get; set; }

    public DateTime LastUpdated { get; set; }

    // ── Navigation ────────────────────────────────────────────────────────
    public virtual tblUser User { get; set; } = null!;
}
