using Webcrm.ErpIntegrations.ApiClients.WebcrmApiClient;

namespace Webcrm.ErpIntegrations.Synchronisation.Models
{
    internal class UpsertOrganisationToPowerofficePayload : BasePowerofficePayload
    {
        public UpsertOrganisationToPowerofficePayload(
            OrganisationDto webcrmOrganisation,
            string webcrmSystemId)
            : base(webcrmSystemId)
        {
            WebcrmOrganisation = webcrmOrganisation;
        }

        public OrganisationDto WebcrmOrganisation { get; }
    }
}