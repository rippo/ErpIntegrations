namespace Webcrm.ErpIntegrations.ApiClients.FortnoxApiClient.Models
{
    /// <summary>
    /// Fortnox returns a subset of the full invoice class when filtering the request using query parameters.
    /// </summary>
    public class FilteredInvoice
    {
        public string CustomerName { get; set; }
        public string CustomerNumber { get; set; }
        public string DocumentNumber { get; set; }
    }
}