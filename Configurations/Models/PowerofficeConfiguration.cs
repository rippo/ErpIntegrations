using System;
using System.Linq;

namespace Webcrm.ErpIntegrations.Configurations.Models
{
    public class PowerofficeConfiguration : BaseConfiguration
    {
        public PowerofficeConfiguration()
        {
            AcceptedOrganisationStatuses = new string[0];
            AcceptedOrganisationTypes = new string[0];
        }

        /// <summary>Optional. The name of the field in webCRM that is used to store the customer number that PowerOffice generates. This is typically a number between 10.000 and 19.999. The PowerOffice API calls this for the `Code`.</summary>
        public string OrganisationNumberFieldName { get; set; }

        /// <summary>Optional. The name of the field in webCRM that is used to store the VAT number (CVR-nummer) of the corresponding organisation in the ERP system, e.g. "OrganisationExtraCustom1". Note that webCRM's property named OrganisationVatNumber is not used.</summary>
        public string OrganisationVatNumberFieldName { get; set; }

        /// <summary>The name of the field in webCRM that is used to store the ID of the corresponding person in the ERP system, e.g. "PersonCustom5".</summary>
        public string PersonIdFieldName { get; set; }

        /// <summary>A GUID used to authenticate towards the PowerOffice API.</summary>
        public Guid PowerofficeClientKey { get; set; }

        /// <summary>Optional. E.g. "OrganisationCustom1".</summary>
        public string PrimaryDeliveryAddressFieldName { get; set; }

        /// <summary>The name of the field in webCRM that is used to store the product code of the corresponding product in the ERP system, e.g. "QuotationLineLinkedDataItemData3".</summary>
        public string ProductCodeFieldName { get; set; }

        /// <summary>The name of the field in webCRM that is used to store the ID of the corresponding product in the ERP system, e.g. "QuotationLineLinkedDataItemData1".</summary>
        public string ProductIdFieldName { get; set; }

        /// <summary>The name of the field in webCRM that is used ot store the name of the corresponding product in the ERP system, e.g. "QuotationLineLinkedDataItemData2".</summary>
        public string ProductNameFieldName { get; set; }

        /// <summary>Optional. The name of the field in webCRM that is used to store the sales account from the ERP system, e.g. "QuotationLineLinkedDataItemData4".</summary>
        public string ProductSalesAccountFieldName { get; set; }

        /// <summary>Optional. the name of the field in webCRM that is used to store the unit of the product ("kilo", "a piece", etc.), e.g. "QuotationLineLinkedDataItem5".</summary>
        public string ProductUnitFieldName { get; set; }

        /// <summary>Throws an `ApplicationException` if the configuration isn't valid.</summary>
        public void Validate()
        {
            var errorMessages = GetBaseValidationErrorMessages();

            if (string.IsNullOrWhiteSpace(PersonIdFieldName))
                errorMessages.Add("PersonIdFieldName isn't defined.");

            if (string.IsNullOrWhiteSpace(ProductCodeFieldName))
                errorMessages.Add("ProductCodeFieldName isn't defined.");

            if (string.IsNullOrWhiteSpace(ProductNameFieldName))
                errorMessages.Add("ProductNameFieldName isn't defined.");

            if (string.IsNullOrWhiteSpace(ProductIdFieldName))
                errorMessages.Add("ProductIdFieldName isn't defined.");

            if (errorMessages.Any())
            {
                string joinedMessages = string.Join(" ", errorMessages);
                throw new ApplicationException($"The PowerOffice configuration isn't valid. Error(s): {joinedMessages}");
            }
        }
    }
}