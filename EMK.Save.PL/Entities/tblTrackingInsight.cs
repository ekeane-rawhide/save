namespace EMK.Save.PL.Entities;

public partial class tblTrackingInsight : IEntity
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public int Month { get; set; }

    public int Year { get; set; }

    public int InsightType { get; set; }        // maps to InsightType enum

    public int Severity { get; set; }           // maps to InsightSeverity enum

    public Guid? CategoryId { get; set; }       // nullable — some insights are account-wide

    public string Title { get; set; } = null!;

    public string Message { get; set; } = null!;

    public decimal? Amount { get; set; }

    public double? ChangePercent { get; set; }

    public DateTime GeneratedOn { get; set; }

    public bool IsDismissed { get; set; }

    public bool IsRead { get; set; }

    // ── Navigation ────────────────────────────────────────────────────────
    public virtual tblUser User { get; set; } = null!;
    public virtual tblBudgetCategory? Category { get; set; }
}
