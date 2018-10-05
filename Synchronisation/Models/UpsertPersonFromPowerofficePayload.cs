using Webcrm.ErpIntegrations.ApiClients.PowerofficeApiClient.Models;

namespace Webcrm.ErpIntegrations.Synchronisation.Models
{
    internal class UpsertPersonFromPowerofficePayload : BasePowerofficePayload
    {
        public UpsertPersonFromPowerofficePayload(
            long powerofficeOrganisationId,
            ContactPerson powerofficePerson,
            string webcrmSystemId)
            : base(webcrmSystemId)
        {
            PowerofficeOrganisationId = powerofficeOrganisationId;
            PowerofficePerson = powerofficePerson;
        }

        public long PowerofficeOrganisationId { get; }
        public ContactPerson PowerofficePerson { get; }
    }
}