namespace EMK.Save.PL.Entities;

public partial class tblMonthlySnapshot : IEntity
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public int Month { get; set; }

    public int Year { get; set; }

    public decimal TotalIncome { get; set; }

    public decimal TotalExpenses { get; set; }

    public decimal TotalBudgeted { get; set; }

    public decimal TotalSavings { get; set; }

    public int TransactionCount { get; set; }

    public int OverBudgetCategoryCount { get; set; }

    public DateTime SnapshotDate { get; set; }

    // ── Navigation ────────────────────────────────────────────────────────
    public virtual tblUser User { get; set; } = null!;
    public virtual ICollection<tblCategorySummary> CategorySummaries { get; set; } = null!;
}
