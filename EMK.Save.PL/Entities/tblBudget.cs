namespace EMK.Save.PL.Entities;

public partial class tblBudget : IEntity
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public Guid CategoryId { get; set; }

    public int Month { get; set; }

    public int Year { get; set; }

    public decimal PlannedAmount { get; set; }

    public decimal RolloverAmount { get; set; }

    public string Notes { get; set; } = null!;

    // ── Navigation ────────────────────────────────────────────────────────
    public virtual tblUser User { get; set; } = null!;
    public virtual tblBudgetCategory Category { get; set; } = null!;
    public virtual ICollection<tblTransaction> Transactions { get; set; } = null!;
}
