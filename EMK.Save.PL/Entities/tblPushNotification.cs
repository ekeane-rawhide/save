namespace EMK.Save.PL.Entities;

public partial class tblPushNotification : IEntity
{
    public Guid      Id               { get; set; }
    public Guid      SharedBudgetId   { get; set; }
    public Guid      UserId           { get; set; }
    public int       NotificationType { get; set; }   // maps to NotificationType enum in BL
    public string    Title            { get; set; } = null!;
    public string    Body             { get; set; } = null!;
    public string    Icon             { get; set; } = null!;
    public string    ActionUrl        { get; set; } = null!;
    public string    PushEndpoint     { get; set; } = null!;
    public Guid?     TransactionId    { get; set; }   // null if not transaction-related
    public Guid?     CategoryId       { get; set; }   // null if not category-related
    public decimal?  Amount           { get; set; }
    public DateTime? SentOn           { get; set; }
    public DateTime  ScheduledFor     { get; set; }
    public int       Status           { get; set; }   // maps to NotificationStatus enum in BL
    public bool      IsRead           { get; set; }
    public string    ErrorMessage     { get; set; } = null!;

    // ── Navigation ────────────────────────────────────────────────────────────
    public virtual tblSharedBudget    SharedBudget { get; set; } = null!;
    public virtual tblUser            User         { get; set; } = null!;
    public virtual tblTransaction?    Transaction  { get; set; }
    public virtual tblBudgetCategory? Category     { get; set; }
}
