using Webcrm.ErpIntegrations.ApiClients.WebcrmApiClient;

namespace Webcrm.ErpIntegrations.Synchronisation.Models
{
    public class WebcrmUpsertFortnoxOrganisationPayload : BaseFortnoxPayload
    {
        public OrganisationDto Organisation { get; set; }
    }
}