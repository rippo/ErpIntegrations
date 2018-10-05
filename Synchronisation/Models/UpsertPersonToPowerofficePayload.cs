using Webcrm.ErpIntegrations.ApiClients.WebcrmApiClient;

namespace Webcrm.ErpIntegrations.Synchronisation.Models
{
    internal class UpsertPersonToPowerofficePayload : BasePowerofficePayload
    {
        public UpsertPersonToPowerofficePayload(
            PersonDto webcrmPerson,
            string webcrmSystemId)
            : base(webcrmSystemId)
        {
            WebcrmPerson = webcrmPerson;
        }

        public PersonDto WebcrmPerson { get; }
    }
}