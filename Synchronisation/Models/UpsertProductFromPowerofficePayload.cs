using Webcrm.ErpIntegrations.ApiClients.PowerofficeApiClient.Models;

namespace Webcrm.ErpIntegrations.Synchronisation.Models
{
    internal class UpsertProductFromPowerofficePayload : BasePowerofficePayload
    {
        public UpsertProductFromPowerofficePayload(
            Product powerofficeProduct,
            string webcrmSystemId) : base(webcrmSystemId)
        {
            PowerofficeProduct = powerofficeProduct;
        }

        public Product PowerofficeProduct { get; }
    }
}