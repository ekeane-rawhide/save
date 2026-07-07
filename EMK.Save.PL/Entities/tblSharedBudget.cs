namespace EMK.Save.PL.Entities;

public partial class tblSharedBudget : IEntity
{
    public DateTime DateCreated { get; set; }

    public virtual ICollection<tblUser>             Members          { get; set; } = null!;
    public virtual ICollection<tblBudgetCategory>   BudgetCategories { get; set; } = null!;
    public virtual ICollection<tblBudget>           Budgets          { get; set; } = null!;
    public virtual ICollection<tblPlaidAccount>     PlaidAccounts    { get; set; } = null!;
    public virtual ICollection<tblMonthlySnapshot>  MonthlySnapshots { get; set; } = null!;
    public virtual ICollection<tblCashFlowEntry>    CashFlowEntries  { get; set; } = null!;
    public virtual ICollection<tblTrackingInsight>  TrackingInsights { get; set; } = null!;
    public virtual ICollection<tblPushNotification> PushNotifications{ get; set; } = null!;
}
