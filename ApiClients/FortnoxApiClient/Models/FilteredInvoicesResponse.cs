using System.Collections.Generic;

namespace Webcrm.ErpIntegrations.ApiClients.FortnoxApiClient.Models
{
    public class FilteredInvoicesResponse
    {
        public List<FilteredInvoice> Invoices { get; set; }
        public MetaInformation MetaInformation { get; set; }
    }
}