using Microsoft.Extensions.Logging;

namespace Webcrm.ErpIntegrations.ApiClients.WebcrmApiClient
{
    /// <summary>Both the ID of the organisation and the person is required to identify a person in PowerOffice.</summary>
    public sealed class PowerofficePersonKey
    {
        public PowerofficePersonKey(
            long powerofficeOrganisationId,
            long powerofficePersonId)
        {
            PowerofficeOrganisationId = powerofficeOrganisationId;
            PowerofficePersonId = powerofficePersonId;
        }

        /// <summary>Creates a `PowerofficePersonKey` based on a string of "{organisationId}/{personId}". Returns `null` if something goes wrong.</summary>
        public static PowerofficePersonKey Create(
            ILogger logger,
            string personKeyString)
        {
            string[] personKeyArray = personKeyString.Split(OrganisationIdPersonIdSeparator);
            if (personKeyArray.Length != 2)
            {
                logger.LogWarning($"Got {personKeyArray.Length} parts when splitting string with organsation id and person id. Expected exact two.");
                return null;
            }

            if (!long.TryParse(personKeyArray[0], out long organisationId))
            {
                logger.LogWarning($"Could not parse '{personKeyArray[0]}' into a long.");
                return null;
            }

            if (!long.TryParse(personKeyArray[1], out long personId))
            {
                logger.LogWarning($"Could not parse '{personKeyArray[1]}' into a long.");
                return null;
            }

            return new PowerofficePersonKey(organisationId, personId);
        }

        public long PowerofficeOrganisationId { get; }
        public long PowerofficePersonId { get; }

        private const char OrganisationIdPersonIdSeparator = '/';

        public override string ToString()
        {
            return $"{PowerofficeOrganisationId}{OrganisationIdPersonIdSeparator}{PowerofficePersonId}";
        }
    }
}