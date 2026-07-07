namespace EMK.Save.PL.Entities;

public partial class tblBudget : IEntity
{
    public Guid    Id             { get; set; }
    public Guid    SharedBudgetId { get; set; }
    public Guid    CategoryId     { get; set; }
    public int     Month          { get; set; }
    public int     Year           { get; set; }
    public decimal PlannedAmount  { get; set; }
    public decimal RolloverAmount { get; set; }
    public string  Notes          { get; set; } = null!;

    public virtual tblSharedBudget            SharedBudget { get; set; } = null!;
    public virtual tblBudgetCategory          Category     { get; set; } = null!;
    public virtual ICollection<tblTransaction> Transactions { get; set; } = null!;
}

public partial class tblPlaidAccount : IEntity
{
    public Guid    Id                    { get; set; }
    public Guid    UserId                { get; set; }
    public Guid    SharedBudgetId        { get; set; }
    public string  PlaidAccountId        { get; set; } = null!;
    public string  PlaidItemId           { get; set; } = null!;
    public string  AccessTokenEncrypted  { get; set; } = null!;  // never exposed to client
    public string  InstitutionName       { get; set; } = null!;
    public string  InstitutionLogoUrl    { get; set; } = null!;
    public string  AccountName           { get; set; } = null!;
    public string  Mask                  { get; set; } = null!;
    public string  AccountType           { get; set; } = null!;
    public string  AccountSubtype        { get; set; } = null!;
    public decimal CurrentBalance        { get; set; }
    public decimal AvailableBalance      { get; set; }
    public string  IsoCurrencyCode       { get; set; } = null!;
    public DateTime LastSynced           { get; set; }
    public bool    IsActive              { get; set; }
    public DateTime DateLinked           { get; set; }

    public virtual tblUser                       User         { get; set; } = null!;
    public virtual tblSharedBudget               SharedBudget { get; set; } = null!;
    public virtual ICollection<tblTransaction>   Transactions { get; set; } = null!;
}

public partial class tblTransaction : IEntity
{
    public Guid    Id                  { get; set; }
    public Guid    PlaidAccountId      { get; set; }
    public Guid    SharedBudgetId      { get; set; }
    public Guid?   CategoryId          { get; set; }
    public string  PlaidTransactionId  { get; set; } = null!;
    public DateTime  TransactionDate   { get; set; }
    public DateTime? PostedDate        { get; set; }
    public string  MerchantName        { get; set; } = null!;
    public string  Description         { get; set; } = null!;
    public decimal Amount              { get; set; }
    public string  IsoCurrencyCode     { get; set; } = null!;
    public string  PlaidCategory       { get; set; } = null!;
    public string  PlaidSubcategory    { get; set; } = null!;
    public string  Notes               { get; set; } = null!;
    public bool    IsExcluded          { get; set; }
    public bool    IsPending           { get; set; }
    public bool    IsReviewed          { get; set; }

    public virtual tblPlaidAccount               PlaidAccount      { get; set; } = null!;
    public virtual tblSharedBudget               SharedBudget      { get; set; } = null!;
    public virtual tblBudgetCategory?            Category          { get; set; }
    public virtual ICollection<tblPushNotification> PushNotifications { get; set; } = null!;
}
