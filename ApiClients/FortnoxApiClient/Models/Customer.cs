using Webcrm.ErpIntegrations.ApiClients.WebcrmApiClient;
using Webcrm.ErpIntegrations.GeneralUtilities;

namespace Webcrm.ErpIntegrations.ApiClients.FortnoxApiClient.Models
{
    // For a full list of available fields see
    // https://github.com/FortnoxAB/csharp-api-sdk/blob/master/FortnoxAPILibrary/Entities/Customer.cs
    // https://developer.fortnox.se/documentation/resources/customers/
    public class Customer : BaseCustomer
    {
        public Customer()
        {
            Active = true;
        }

        public bool Active { get; set; }

        // NOTE: Fortnox uses ISO two letter codes and webCRM uses strings. Therefore we would need to implement a mapping function or use a service https://restcountries.eu/.
        // public string Country { get; set; }
        public string Phone1 { get; set; }
        // ReSharper disable once InconsistentNaming
        public string WWW { get; set; }
        public string ZipCode { get; set; }

        // TODO FORTNOX: Move some of the comparisons to `BaseCustomer`.
        public bool HasRelevantChanges(
            OrganisationDto webcrmOrganisation)
        {
            if (!StringUtilities.AreEquivalent(City, webcrmOrganisation.OrganisationCity))
                return true;

            if (!StringUtilities.AreEquivalent(Name, webcrmOrganisation.OrganisationName))
                return true;

            if (!StringUtilities.AreEquivalent(Phone1, webcrmOrganisation.OrganisationTelephone))
                return true;

            if (!StringUtilities.AreEquivalent(WWW, webcrmOrganisation.OrganisationWww))
                return true;

            if (!StringUtilities.AreEquivalent(ZipCode, webcrmOrganisation.OrganisationPostCode))
                return true;

            return false;
        }
    }
}