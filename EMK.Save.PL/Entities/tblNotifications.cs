namespace EMK.Save.PL.Entities;

public partial class tblTrackingInsight : IEntity
{
    public Guid    Id             { get; set; }
    public Guid    SharedBudgetId { get; set; }
    public int     Month          { get; set; }
    public int     Year           { get; set; }
    public int     InsightType    { get; set; }
    public int     Severity       { get; set; }
    public Guid?   CategoryId     { get; set; }
    public string  Title          { get; set; } = null!;
    public string  Message        { get; set; } = null!;
    public decimal? Amount        { get; set; }
    public double?  ChangePercent { get; set; }
    public DateTime GeneratedOn   { get; set; }
    public bool    IsDismissed    { get; set; }
    public bool    IsRead         { get; set; }

    public virtual tblSharedBudget   SharedBudget { get; set; } = null!;
    public virtual tblBudgetCategory? Category    { get; set; }
}

public partial class tblPushNotification : IEntity
{
    public Guid    Id               { get; set; }
    public Guid    SharedBudgetId   { get; set; }
    public Guid    UserId           { get; set; }
    public int     NotificationType { get; set; }
    public string  Title            { get; set; } = null!;
    public string  Body             { get; set; } = null!;
    public string  Icon             { get; set; } = null!;
    public string  ActionUrl        { get; set; } = null!;
    public string  PushEndpoint     { get; set; } = null!;
    public Guid?   TransactionId    { get; set; }
    public Guid?   CategoryId       { get; set; }
    public decimal? Amount          { get; set; }
    public DateTime? SentOn         { get; set; }
    public DateTime  ScheduledFor   { get; set; }
    public int     Status           { get; set; }
    public bool    IsRead           { get; set; }
    public string  ErrorMessage     { get; set; } = null!;

    public virtual tblSharedBudget    SharedBudget { get; set; } = null!;
    public virtual tblUser            User         { get; set; } = null!;
    public virtual tblTransaction?    Transaction  { get; set; }
    public virtual tblBudgetCategory? Category     { get; set; }
}

public partial class tblNotificationPreference : IEntity
{
    public Guid    Id                          { get; set; }
    public Guid    UserId                      { get; set; }   // 1-to-1
    public string  PushEndpoint                { get; set; } = null!;
    public string  P256dhKey                   { get; set; } = null!;
    public string  AuthKey                     { get; set; } = null!;
    public bool    NotifyOnNewTransaction      { get; set; }
    public bool    NotifyOnBudgetOverage       { get; set; }
    public bool    NotifyOnBudgetWarning       { get; set; }
    public bool    NotifyOnLargeTransaction    { get; set; }
    public bool    NotifyWeeklySummary         { get; set; }
    public bool    NotifyMonthlySummary        { get; set; }
    public bool    NotifyOnSyncError           { get; set; }
    public decimal LargeTransactionThreshold   { get; set; }
    public TimeOnly QuietHoursStart            { get; set; }
    public TimeOnly QuietHoursEnd              { get; set; }
    public bool    IsPushEnabled               { get; set; }
    public DateTime LastUpdated                { get; set; }

    public virtual tblUser User { get; set; } = null!;
}
