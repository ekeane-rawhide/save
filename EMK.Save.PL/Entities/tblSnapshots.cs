namespace EMK.Save.PL.Entities;

public partial class tblMonthlySnapshot : IEntity
{
    public Guid    Id                      { get; set; }
    public Guid    SharedBudgetId          { get; set; }
    public int     Month                   { get; set; }
    public int     Year                    { get; set; }
    public decimal TotalIncome             { get; set; }
    public decimal TotalExpenses           { get; set; }
    public decimal TotalBudgeted           { get; set; }
    public decimal TotalSavings            { get; set; }
    public int     TransactionCount        { get; set; }
    public int     OverBudgetCategoryCount { get; set; }
    public DateTime SnapshotDate           { get; set; }

    public virtual tblSharedBudget                      SharedBudget      { get; set; } = null!;
    public virtual ICollection<tblCategorySummary>      CategorySummaries { get; set; } = null!;
}

public partial class tblCategorySummary : IEntity
{
    public Guid    Id               { get; set; }
    public Guid    SnapshotId       { get; set; }
    public Guid    CategoryId       { get; set; }
    public decimal PlannedAmount    { get; set; }
    public decimal ActualAmount     { get; set; }
    public int     TransactionCount { get; set; }

    public virtual tblMonthlySnapshot Snapshot { get; set; } = null!;
    public virtual tblBudgetCategory  Category { get; set; } = null!;
}

public partial class tblCashFlowEntry : IEntity
{
    public Guid    Id               { get; set; }
    public Guid    SharedBudgetId   { get; set; }
    public DateTime EntryDate       { get; set; }
    public decimal DayIncome        { get; set; }
    public decimal DayExpenses      { get; set; }
    public decimal RunningBalance   { get; set; }
    public decimal ProjectedBalance { get; set; }
    public int     TransactionCount { get; set; }

    public virtual tblSharedBudget SharedBudget { get; set; } = null!;
}
