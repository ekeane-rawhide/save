namespace EMK.Save.PL.Entities;

public partial class tblPlaidAccount : IEntity
{
    public Guid     Id                   { get; set; }
    public Guid     UserId               { get; set; }
    public Guid     SharedBudgetId       { get; set; }
    public string   PlaidAccountId       { get; set; } = null!;   // Plaid's account_id
    public string   PlaidItemId          { get; set; } = null!;   // Plaid's item_id
    public string   AccessTokenEncrypted { get; set; } = null!;   // never exposed to client
    public string   InstitutionName      { get; set; } = null!;
    public string   InstitutionLogoUrl   { get; set; } = null!;
    public string   AccountName          { get; set; } = null!;
    public string   Mask                 { get; set; } = null!;   // last 4 digits
    public string   AccountType          { get; set; } = null!;
    public string   AccountSubtype       { get; set; } = null!;
    public decimal  CurrentBalance       { get; set; }
    public decimal  AvailableBalance     { get; set; }
    public string   IsoCurrencyCode      { get; set; } = null!;
    public DateTime LastSynced           { get; set; }
    public bool     IsActive             { get; set; }
    public DateTime DateLinked           { get; set; }

    // ── Navigation ────────────────────────────────────────────────────────────
    public virtual tblUser                      User         { get; set; } = null!;
    public virtual tblSharedBudget              SharedBudget { get; set; } = null!;
    public virtual ICollection<tblTransaction>  Transactions { get; set; } = null!;
}
