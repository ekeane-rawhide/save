namespace EMK.Save.PL.Entities;

public partial class tblSharedBudget : IEntity
{
    public Guid    Id          { get; set; }
    public string  Name        { get; set; } = null!;
    public string  Description { get; set; } = null!;
    public Guid    OwnerId     { get; set; }
    public string  InviteCode  { get; set; } = null!;   // unique 6-char alphanumeric
    public bool    IsActive    { get; set; }
    public DateTime DateCreated { get; set; }
    public int     MaxMembers  { get; set; }

    // ── Navigation ────────────────────────────────────────────────────────
    public virtual ICollection<tblUser>             Members          { get; set; } = null!;
    public virtual ICollection<tblBudgetCategory>   BudgetCategories { get; set; } = null!;
    public virtual ICollection<tblBudget>           Budgets          { get; set; } = null!;
    public virtual ICollection<tblPlaidAccount>     PlaidAccounts    { get; set; } = null!;
    public virtual ICollection<tblMonthlySnapshot>  MonthlySnapshots { get; set; } = null!;
    public virtual ICollection<tblCashFlowEntry>    CashFlowEntries  { get; set; } = null!;
    public virtual ICollection<tblTrackingInsight>  TrackingInsights { get; set; } = null!;
    public virtual ICollection<tblPushNotification> PushNotifications{ get; set; } = null!;
}
