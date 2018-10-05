namespace Webcrm.ErpIntegrations.ApiClients.PowerofficeApiClient.Models.Responses
{
    public abstract class Response<TData>
    {
        public TData Data { get; set; }
        public bool Success { get; set; }
    }
}