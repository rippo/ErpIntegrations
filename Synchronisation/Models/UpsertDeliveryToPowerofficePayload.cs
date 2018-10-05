using System.Collections.Generic;
using Webcrm.ErpIntegrations.ApiClients.WebcrmApiClient;

namespace Webcrm.ErpIntegrations.Synchronisation.Models
{
    internal class UpsertDeliveryToPowerofficePayload : BasePowerofficePayload
    {
        public UpsertDeliveryToPowerofficePayload(
            DeliveryDto webcrmDelivery,
            List<QuotationLineDto> webcrmDeliveryLines,
            string webcrmSystemId)
            : base(webcrmSystemId)
        {
            WebcrmDelivery = webcrmDelivery;
            WebcrmDeliveryLines = webcrmDeliveryLines;
        }

        public DeliveryDto WebcrmDelivery { get; }
        public List<QuotationLineDto> WebcrmDeliveryLines { get; }
    }
}