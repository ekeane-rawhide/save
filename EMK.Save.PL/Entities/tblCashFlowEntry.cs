namespace EMK.Save.PL.Entities;

public partial class tblCashFlowEntry : IEntity
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public DateTime EntryDate { get; set; }

    public decimal DayIncome { get; set; }

    public decimal DayExpenses { get; set; }

    public decimal RunningBalance { get; set; }

    public decimal ProjectedBalance { get; set; }

    public int TransactionCount { get; set; }

    // ── Navigation ────────────────────────────────────────────────────────
    public virtual tblUser User { get; set; } = null!;
}
