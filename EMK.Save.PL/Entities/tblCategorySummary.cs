namespace EMK.Save.PL.Entities;

public partial class tblCategorySummary : IEntity
{
    public Guid    Id               { get; set; }
    public Guid    SnapshotId       { get; set; }
    public Guid    CategoryId       { get; set; }
    public decimal PlannedAmount    { get; set; }
    public decimal ActualAmount     { get; set; }
    public int     TransactionCount { get; set; }

    // ── Navigation ────────────────────────────────────────────────────────────
    public virtual tblMonthlySnapshot Snapshot { get; set; } = null!;
    public virtual tblBudgetCategory  Category { get; set; } = null!;
}
