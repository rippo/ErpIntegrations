using System.Collections.Generic;

namespace Webcrm.ErpIntegrations.ApiClients.PowerofficeApiClient.Models.Responses
{
    internal class ListResponse<TData> : Response<List<TData>>
    {
        public int Count { get; set; }
    }
}