namespace EMK.Save.PL.Entities;

public partial class tblBudgetCategory : IEntity
{
    public Guid   Id             { get; set; }
    public Guid   SharedBudgetId { get; set; }
    public string Name           { get; set; } = null!;
    public string Icon           { get; set; } = null!;
    public string Color          { get; set; } = null!;
    public int    CategoryType   { get; set; }   // 0=Income 1=Expense 2=Savings 3=Transfer
    public int    SortOrder      { get; set; }
    public bool   IsActive       { get; set; }

    // ── Navigation ────────────────────────────────────────────────────────────
    public virtual tblSharedBudget                SharedBudget      { get; set; } = null!;
    public virtual ICollection<tblBudget>         Budgets           { get; set; } = null!;
    public virtual ICollection<tblTransaction>    Transactions      { get; set; } = null!;
    public virtual ICollection<tblCategorySummary>   CategorySummaries { get; set; } = null!;
    public virtual ICollection<tblTrackingInsight>   TrackingInsights  { get; set; } = null!;
    public virtual ICollection<tblPushNotification>  PushNotifications { get; set; } = null!;
}
