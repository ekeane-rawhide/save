namespace EMK.Save.PL.Entities;

public partial class tblUser : IEntity
{
    public Guid    Id           { get; set; }
    public string  UserId       { get; set; } = null!;
    public string  FirstName    { get; set; } = null!;
    public string  LastName     { get; set; } = null!;
    public string  Email        { get; set; } = null!;
    public string  Password     { get; set; } = null!;
    public string  TimeZone     { get; set; } = null!;
    public string  CurrencyCode { get; set; } = null!;
    public DateTime  DateRegistered { get; set; }
    public DateTime? LastLogin      { get; set; }

    // Budget membership — FK to tblSharedBudget (nullable; null = no budget yet)
    public Guid? SharedBudgetId { get; set; }
    public int?  BudgetRole     { get; set; }   // 0 = Owner, 1 = Member

    // ── Navigation ────────────────────────────────────────────────────────
    public virtual tblSharedBudget?          SharedBudget           { get; set; }
    public virtual ICollection<tblPlaidAccount>         PlaidAccounts          { get; set; } = null!;
    public virtual ICollection<tblPushNotification>     PushNotifications      { get; set; } = null!;
    public virtual tblNotificationPreference?           NotificationPreference { get; set; }
}
