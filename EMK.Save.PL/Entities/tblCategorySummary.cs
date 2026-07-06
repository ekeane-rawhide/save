namespace EMK.Save.PL.Entities;

public partial class tblCategorySummary : IEntity
{
    public Guid Id { get; set; }

    public Guid SnapshotId { get; set; }            // FK → tblMonthlySnapshot

    public Guid CategoryId { get; set; }            // FK → tblBudgetCategory

    public decimal PlannedAmount { get; set; }

    public decimal ActualAmount { get; set; }

    public int TransactionCount { get; set; }

    // ── Navigation ────────────────────────────────────────────────────────
    public virtual tblMonthlySnapshot Snapshot { get; set; } = null!;
    public virtual tblBudgetCategory Category { get; set; } = null!;
}
