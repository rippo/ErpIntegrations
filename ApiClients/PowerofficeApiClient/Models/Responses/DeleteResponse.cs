namespace Webcrm.ErpIntegrations.ApiClients.PowerofficeApiClient.Models.Responses
{
    internal class DeleteResponse : Response<object>
    {
        public int Count { get; set; }
        public object Validation { get; set; }
    }
}