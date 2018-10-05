namespace Webcrm.ErpIntegrations.ApiClients.FortnoxApiClient.Models.Responses
{
    public abstract class Response<TData>
    {
        public TData Data { get; set; }
        public bool Success { get; set; }
    }
}