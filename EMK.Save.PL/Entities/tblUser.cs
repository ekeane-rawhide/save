namespace EMK.Save.PL.Entities;

public partial class tblUser : IEntity
{
    public Guid Id { get; set; }

    public string UserId { get; set; } = null!;         // login handle

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;       // SHA-1 hash

    public string TimeZone { get; set; } = null!;

    public string CurrencyCode { get; set; } = null!;

    public DateTime DateRegistered { get; set; }

    public DateTime? LastLogin { get; set; }

    // ── Navigation ────────────────────────────────────────────────────────
    public virtual ICollection<tblBudgetCategory> BudgetCategories { get; set; } = null!;
    public virtual ICollection<tblBudget> Budgets { get; set; } = null!;
    public virtual ICollection<tblPlaidAccount> PlaidAccounts { get; set; } = null!;
    public virtual ICollection<tblMonthlySnapshot> MonthlySnapshots { get; set; } = null!;
    public virtual ICollection<tblCashFlowEntry> CashFlowEntries { get; set; } = null!;
    public virtual ICollection<tblTrackingInsight> TrackingInsights { get; set; } = null!;
    public virtual ICollection<tblPushNotification> PushNotifications { get; set; } = null!;
    public virtual tblNotificationPreference NotificationPreference { get; set; } = null!;
}
