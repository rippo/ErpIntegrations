using Webcrm.ErpIntegrations.ApiClients.PowerofficeApiClient.Models;

namespace Webcrm.ErpIntegrations.Synchronisation.Models
{
    internal class UpsertOrganisationFromPowerofficePayload : BasePowerofficePayload
    {
        public UpsertOrganisationFromPowerofficePayload(
            Customer powerofficeOrganisation,
            string webcrmSystemId)
            : base(webcrmSystemId)
        {
            PowerofficeOrganisation = powerofficeOrganisation;
        }

        public Customer PowerofficeOrganisation { get; }
    }
}