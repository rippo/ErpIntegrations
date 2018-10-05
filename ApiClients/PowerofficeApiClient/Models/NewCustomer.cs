using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Webcrm.ErpIntegrations.ApiClients.WebcrmApiClient;
using Webcrm.ErpIntegrations.Configurations.Models;
using Webcrm.ErpIntegrations.GeneralUtilities;

namespace Webcrm.ErpIntegrations.ApiClients.PowerofficeApiClient.Models
{
    /// <remarks>https://api.poweroffice.net/Web/docs/index.html#Reference/Rest/Type_Customer.md</remarks>
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class NewCustomer
    {
        public NewCustomer()
        {
            // Setting these to null and having NullValueHandling.Ignore on the properties should also work, but it doesn't. Don't know why.
            ContactGroups = new string[0];
            MailAddress = new Address();
            StreetAddresses = new Address[0];
        }

        public NewCustomer(
            OrganisationDto webcrmOrganisation)
        {
            Update(webcrmOrganisation);
        }

        public long? Code { get; set; }
        public IEnumerable<string> ContactGroups { get; set; }
        public long? ContactPersonId { get; set; }
        public string CurrencyCode { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public int? DiscountPercent { get; set; }
        public string EmailAddress { get; set; }
        public long? ExternalCode { get; set; }
        public string FirstName { get; set; }
        public decimal? HourlyRate { get; set; }
        public string InvoiceBrandingThemeCode { get; set; }
        public InvoiceDeliveryType? InvoiceDeliveryType { get; set; }
        public string InvoiceEmailAddress { get; set; }
        public bool IsArchived { get; set; }
        public bool IsPerson { get; set; }
        public bool? IsVatFree { get; set; }
        public string LastName { get; set; }
        public string LegalName { get; set; }
        public Address MailAddress { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime? Since { get; set; }
        public string SocialSecurityNumber { get; set; }
        public IEnumerable<Address> StreetAddresses { get; set; }
        public string VatNumber { get; set; }
        public string WebsiteUrl { get; set; }

        [JsonIgnore]
        public string ConcatenatedPrimaryStreetAddress
        {
            get
            {
                if (PrimaryStreetAddress == null)
                    return string.Empty;

                // Country is not included because PowerOffice uses country codes whereas webCRM uses full names.
                return StringUtilities.JoinDefined(
                    StringUtilities.WindowsNewline,
                    PrimaryStreetAddress.Address1,
                    PrimaryStreetAddress.Address2,
                    PrimaryStreetAddress.Address3,
                    PrimaryStreetAddress.ZipCode,
                    PrimaryStreetAddress.City);
            }
        }

        [JsonIgnore]
        private Address PrimaryStreetAddress => StreetAddresses.FirstOrDefault(address => address.IsPrimary);

        /// <summary>Check if this PowerOffice organisation contains any updates that should be copied to webCRM.</summary>
        public bool HasChangesRelevantToWebcrm(
            OrganisationDto webcrmOrganisation,
            PowerofficeConfiguration configuration)
        {
            if (PropertyHasChanged(webcrmOrganisation, configuration.OrganisationNumberFieldName, Code.ToString()))
                return true;

            if (PropertyHasChanged(webcrmOrganisation, configuration.PrimaryDeliveryAddressFieldName, ConcatenatedPrimaryStreetAddress))
                return true;

            return HasRelevantChanges(webcrmOrganisation, configuration);
        }

        private static bool PropertyHasChanged(OrganisationDto webcrmOrganisation, string propertyName, string powerofficeValue)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
                return false;

            string webcrmValue = webcrmOrganisation.GetPropertyValue(propertyName);
            return !StringUtilities.AreEquivalent(powerofficeValue, webcrmValue);
        }

        internal bool HasRelevantChanges(
            OrganisationDto webcrmOrganisation,
            PowerofficeConfiguration configuration)
        {
            if (!StringUtilities.AreEquivalent(MailAddress.Address1, webcrmOrganisation.OrganisationAddress1))
                return true;

            if (!StringUtilities.AreEquivalent(MailAddress.Address2, webcrmOrganisation.OrganisationAddress2))
                return true;

            if (!StringUtilities.AreEquivalent(MailAddress.Address3, webcrmOrganisation.OrganisationAddress3))
                return true;

            if (!StringUtilities.AreEquivalent(MailAddress.City, webcrmOrganisation.OrganisationCity))
                return true;

            // MailAddress.Country is not included because PowerOffice uses country codes whereas webCRM uses full names.

            if (!StringUtilities.AreEquivalent(MailAddress.ZipCode, webcrmOrganisation.OrganisationPostCode))
                return true;

            if (!StringUtilities.AreEquivalent(Name, webcrmOrganisation.OrganisationName))
                return true;

            if (!StringUtilities.AreEquivalent(PhoneNumber, webcrmOrganisation.OrganisationTelephone))
                return true;

            if (!string.IsNullOrWhiteSpace(configuration.OrganisationVatNumberFieldName))
            {
                string webcrmValue = webcrmOrganisation.GetPropertyValue(configuration.OrganisationVatNumberFieldName);

                if (!StringUtilities.AreEquivalent(VatNumber, webcrmValue))
                    return true;
            }

            if (!StringUtilities.AreEquivalent(WebsiteUrl, webcrmOrganisation.OrganisationWww))
                return true;

            return false;
        }

        public void Update(
            OrganisationDto webcrmOrganisation)
        {
            if (MailAddress == null)
                MailAddress = new Address();

            MailAddress.Address1 = webcrmOrganisation.OrganisationAddress1;
            MailAddress.Address2 = webcrmOrganisation.OrganisationAddress2;
            MailAddress.Address3 = webcrmOrganisation.OrganisationAddress3;
            MailAddress.City = webcrmOrganisation.OrganisationCity;
            // MailAddress.Country is not included because PowerOffice uses country codes whereas webCRM uses full names.
            MailAddress.ZipCode = webcrmOrganisation.OrganisationPostCode;
            Name = webcrmOrganisation.OrganisationName;
            PhoneNumber = webcrmOrganisation.OrganisationTelephone;
            VatNumber = webcrmOrganisation.OrganisationVatNumber;
            WebsiteUrl = webcrmOrganisation.OrganisationWww;

            // Setting delivery type to Print to avoid an error about a missing email address. Setting the value to None still requires an email address.
            if (string.IsNullOrWhiteSpace(EmailAddress))
                InvoiceDeliveryType = Models.InvoiceDeliveryType.Print;
        }
    }
}