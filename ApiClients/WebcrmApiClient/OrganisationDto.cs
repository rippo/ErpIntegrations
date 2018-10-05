using Newtonsoft.Json;
using System.Linq;
using Webcrm.ErpIntegrations.Configurations.Models;
using Webcrm.ErpIntegrations.GeneralUtilities;

namespace Webcrm.ErpIntegrations.ApiClients.WebcrmApiClient
{
    public partial class OrganisationDto
    {
        public OrganisationDto()
        { }

        public OrganisationDto(
            PowerofficeApiClient.Models.Customer powerofficeOrganisation,
            PowerofficeConfiguration configuration)
        {
            Update(powerofficeOrganisation, configuration);

            OrganisationStatus = configuration.FirstAcceptedOrganisationStatus;
            OrganisationType = configuration.FirstAcceptedOrganisationType;
        }

        [JsonIgnore]
        public string OrganisationAddress1 => SplitAddress.Length >= 1
            ? SplitAddress[0]
            : string.Empty;

        [JsonIgnore]
        public string OrganisationAddress2 => SplitAddress.Length >= 2
            ? SplitAddress[1]
            : string.Empty;

        [JsonIgnore]
        public string OrganisationAddress3 => SplitAddress.Length >= 3
            ? SplitAddress[2]
            : string.Empty;

        public long? GetPowerofficeOrganisationId(
            string organisationIdFieldName)
        {
            string stringValue = this.GetPropertyValue(organisationIdFieldName);
            if (!long.TryParse(stringValue, out long powerofficeOrganisationId))
                return null;

            return powerofficeOrganisationId;
        }

        public void SetPowerofficeOrganisationId(
            string organisationIdFieldName,
            long powerofficeOrganisationId)
        {
            this.SetPropertyValue(organisationIdFieldName, powerofficeOrganisationId.ToString());
        }

        public bool ShouldSynchronise(BaseConfiguration configuration)
        {
            bool statusOkay = !configuration.AcceptedOrganisationStatuses.Any()
                || configuration.AcceptedOrganisationStatuses.Contains(OrganisationStatus);

            bool typeOkay = !configuration.AcceptedOrganisationTypes.Any()
                || configuration.AcceptedOrganisationTypes.Contains(OrganisationType);

            return statusOkay && typeOkay;
        }

        /// <summary>Check if this webCRM organisation contains any updates that should be copied to PowerOffice.</summary>
        public bool HasChangesRelevantToPoweroffice(
            PowerofficeApiClient.Models.Customer powerofficeOrganisation,
            PowerofficeConfiguration configuration)
        {
            return powerofficeOrganisation.HasRelevantChanges(this, configuration);
        }

        public void Update(
            PowerofficeApiClient.Models.Customer powerofficeOrganisation,
            PowerofficeConfiguration configuration)
        {
            const int webcrmOrganisationNameMaximumLength = 50;

            OrganisationAddress = powerofficeOrganisation.MailAddress.ConcatenatedAddress;
            OrganisationCity = powerofficeOrganisation.MailAddress.City;
            OrganisationName = powerofficeOrganisation.Name.Truncate(webcrmOrganisationNameMaximumLength);
            OrganisationPostCode = powerofficeOrganisation.MailAddress.ZipCode;
            OrganisationTelephone = powerofficeOrganisation.PhoneNumber;
            OrganisationWww = powerofficeOrganisation.WebsiteUrl;

            if (!string.IsNullOrWhiteSpace(configuration.OrganisationNumberFieldName))
                this.SetPropertyValue(configuration.OrganisationNumberFieldName, powerofficeOrganisation.Code.ToString());

            if (!string.IsNullOrWhiteSpace(configuration.OrganisationVatNumberFieldName))
                this.SetPropertyValue(configuration.OrganisationVatNumberFieldName, powerofficeOrganisation.VatNumber);

            if (!string.IsNullOrWhiteSpace(configuration.PrimaryDeliveryAddressFieldName))
                this.SetPropertyValue(configuration.PrimaryDeliveryAddressFieldName, powerofficeOrganisation.ConcatenatedPrimaryStreetAddress);

            SetPowerofficeOrganisationId(configuration.OrganisationIdFieldName, powerofficeOrganisation.Id);
        }

        private string[] SplitAddress
            => OrganisationAddress.Split('\n')
                .Select(line => line.Trim())
                .ToArray();
    }
}