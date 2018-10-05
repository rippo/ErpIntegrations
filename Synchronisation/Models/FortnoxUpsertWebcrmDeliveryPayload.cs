namespace Webcrm.ErpIntegrations.Synchronisation.Models
{
    public class FortnoxUpsertWebcrmDeliveryPayload : BaseFortnoxPayload
    {
        public string FortnoxCustomerNumber { get; set; }
        public string FortnoxDocumentNumber { get; set; }
    }
}