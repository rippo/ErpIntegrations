namespace Webcrm.ErpIntegrations.ApiClients.FortnoxApiClient.Models
{
    /// <summary>
    /// Fortnox returns a subset of the full customer class when filtering the request using query parameters.
    /// </summary>
    public class FilteredCustomer : BaseCustomer
    {
        public string OrganisationNumber { get; set; }
        public string Phone { get; set; }
    }
}