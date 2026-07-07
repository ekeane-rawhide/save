namespace EMK.Save.BL.Models
{
    /// <summary>Short-lived link_token sent from API to PWA to initialize Plaid Link.</summary>
    public class PlaidLinkToken
    {
        public string LinkToken   { get; set; } = string.Empty;
        public DateTime Expiration { get; set; }
        public string RequestId   { get; set; } = string.Empty;
    }

    /// <summary>Sent by the PWA after the user completes Plaid Link. API exchanges it for an access_token.</summary>
    public class PlaidPublicTokenExchange
    {
        public string PublicToken        { get; set; } = string.Empty;
        public string InstitutionId      { get; set; } = string.Empty;
        public string InstitutionName    { get; set; } = string.Empty;
        public string InstitutionLogoUrl { get; set; } = string.Empty;
        public List<string> SelectedAccountIds { get; set; } = new List<string>();
    }
}
