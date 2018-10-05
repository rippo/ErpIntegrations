using Webcrm.ErpIntegrations.ApiClients.PowerofficeApiClient.Models;

namespace Webcrm.ErpIntegrations.Synchronisation.Models
{
    /// <remarks>OutgoingInvoiceWithoutLines does not contain any delivery lines, so we need to fetch the full invoice using the Id. Still enqueuing the whole OutgoingInvoiceWithoutLines because that might make it easier to identity the delivery in case the synchronisation fails.</remarks>
    class UpsertDeliveryFromPowerofficePayload : BasePowerofficePayload
    {
        public UpsertDeliveryFromPowerofficePayload(
            OutgoingInvoiceWithoutLines powerofficeDelivery,
            string webcrmSystemId) : base(webcrmSystemId)
        {
            PowerofficeDelivery = powerofficeDelivery;
        }

        public OutgoingInvoiceWithoutLines PowerofficeDelivery { get; }
    }
}