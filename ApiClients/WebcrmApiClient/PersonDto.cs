using Microsoft.Extensions.Logging;
using System;
using Webcrm.ErpIntegrations.ApiClients.PowerofficeApiClient.Models;
using Webcrm.ErpIntegrations.Configurations;
using Webcrm.ErpIntegrations.Configurations.Models;
using Webcrm.ErpIntegrations.GeneralUtilities;

namespace Webcrm.ErpIntegrations.ApiClients.WebcrmApiClient
{
    public partial class PersonDto
    {
        public PowerofficePersonKey GetPowerofficePersonKey(
            ILogger logger,
            string personIdFieldName)
        {
            string personKeyString = this.GetPropertyValue(personIdFieldName);
            if (string.IsNullOrWhiteSpace(personKeyString))
                return null;

            return PowerofficePersonKey.Create(logger, personKeyString);
        }

        private const string LastNamePlaceholder = "-";

        public void SetPowerofficePersonKey(
            string personIdFieldName,
            PowerofficePersonKey personKey)
        {
            this.SetPropertyValue(personIdFieldName, personKey.ToString());
        }

        public void Update(
            ILogger logger,
            ContactPerson sourcePowerofficeContactPerson,
            long sourcePowerofficeCustomerId,
            int webcrmOrganisationId,
            PowerofficeConfiguration configuration)
        {
            PersonDirectPhone = sourcePowerofficeContactPerson.PhoneNumber;
            PersonEmail = sourcePowerofficeContactPerson.EmailAddress;
            PersonFirstName = sourcePowerofficeContactPerson.FirstName;
            PersonAdjustedLastName = sourcePowerofficeContactPerson.LastName;

            // It is not allowed to change the organisation ID of a person.
            if (PersonOrganisationId == null)
            {
                PersonOrganisationId = webcrmOrganisationId;
            }
            else if (PersonOrganisationId != webcrmOrganisationId)
            {
                logger.LogWarning($"The PowerOffice contact person with ID '{sourcePowerofficeContactPerson.Id}' has changed webCRM organisation from '{PersonOrganisationId}' to '{webcrmOrganisationId}'. We cannot change the organisation ID through the API, so this is not synchronised.");
            }

            var personKey = new PowerofficePersonKey(sourcePowerofficeCustomerId, sourcePowerofficeContactPerson.Id);
            SetPowerofficePersonKey(configuration.PersonIdFieldName, personKey);

            MarkAsPrimaryContact(configuration.PrimaryContactCheckboxFieldName);
        }

        /// <summary>It is mandatory to define last name in webCRM. Since last name might be undefined in other systems we work around the requirement by setting a placeholder string in webCRM. Use this property instead of `PersonLastString` to handle placeholder strings properly.</summary>
        /// <remarks>Last names are also mandatory in PowerOffice, but it is still possible to create persons with empty last names by entering a space. These persons will be created with a dash as the last name in webCRM. When updating such a person in PowerOffice we set last name to `null`, so that the value of `LastName` is ignored by PowerOffice. Creating a person with `LastName` set to `null` is however not allowed, so the synchronisation will fail if a person is created in webCRM with the last name set to a dash.</remarks>
        public string PersonAdjustedLastName
        {
            get => PersonLastName.Equals(LastNamePlaceholder, StringComparison.InvariantCulture)
                ? null
                : PersonLastName;

            set => PersonLastName = string.IsNullOrWhiteSpace(value)
                ? LastNamePlaceholder
                : value;
        }

        private void MarkAsPrimaryContact(string primaryContactCheckboxFieldName)
        {
            this.SetPropertyValue(primaryContactCheckboxFieldName, Constants.CustomCheckboxFieldCheckedValue);
        }
    }
}