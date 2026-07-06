namespace EMK.Save.PL.Entities;

public partial class tblTransaction : IEntity
{
    public Guid Id { get; set; }

    public Guid PlaidAccountId { get; set; }            // FK → tblPlaidAccount

    public Guid? CategoryId { get; set; }               // FK → tblBudgetCategory (nullable — unassigned)

    public string PlaidTransactionId { get; set; } = null!;  // Plaid stable ID — unique index

    public DateTime TransactionDate { get; set; }

    public DateTime? PostedDate { get; set; }

    public string MerchantName { get; set; } = null!;

    public string Description { get; set; } = null!;

    public decimal Amount { get; set; }                 // negative = debit, positive = credit

    public string IsoCurrencyCode { get; set; } = null!;

    public string PlaidCategory { get; set; } = null!;

    public string PlaidSubcategory { get; set; } = null!;

    public string Notes { get; set; } = null!;

    public bool IsExcluded { get; set; }

    public bool IsPending { get; set; }

    public bool IsReviewed { get; set; }

    // ── Navigation ────────────────────────────────────────────────────────
    public virtual tblPlaidAccount PlaidAccount { get; set; } = null!;
    public virtual tblBudgetCategory? Category { get; set; }
    public virtual ICollection<tblPushNotification> PushNotifications { get; set; } = null!;
}
