using System.Collections.Generic;

namespace Webcrm.ErpIntegrations.ApiClients.FortnoxApiClient.Models
{
    public class FilteredCustomersResponse
    {
        public List<FilteredCustomer> Customers { get; set; }
        public MetaInformation MetaInformation { get; set; }
    }
}