namespace EMK.Save.PL.Entities;

public partial class tblTrackingInsight : IEntity
{
    public Guid     Id             { get; set; }
    public Guid     SharedBudgetId { get; set; }
    public int      Month          { get; set; }
    public int      Year           { get; set; }
    public int      InsightType    { get; set; }   // maps to InsightType enum in BL
    public int      Severity       { get; set; }   // maps to InsightSeverity enum in BL
    public Guid?    CategoryId     { get; set; }   // null = budget-wide insight
    public string   Title          { get; set; } = null!;
    public string   Message        { get; set; } = null!;
    public decimal? Amount         { get; set; }
    public double?  ChangePercent  { get; set; }
    public DateTime GeneratedOn    { get; set; }
    public bool     IsDismissed    { get; set; }
    public bool     IsRead         { get; set; }

    // ── Navigation ────────────────────────────────────────────────────────────
    public virtual tblSharedBudget    SharedBudget { get; set; } = null!;
    public virtual tblBudgetCategory? Category     { get; set; }
}
