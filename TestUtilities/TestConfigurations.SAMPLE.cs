using System;
using Webcrm.ErpIntegrations.ApiClients.FortnoxApiClient.Models;
using Webcrm.ErpIntegrations.Configurations.Models;

namespace Webcrm.ErpIntegrations.Test
{
    /// <summary>Configuration settings for test systems. Only use this class in test projects.</summary>
    public static class TestConfigurations
    {
        public const string FortnoxAccessToken = "XXX";
        public const string FortnoxClientSecret = "XXX";
        public const string FortnoxOrganisationIdFieldName = "OrganisationCustom11";

        public const string WebcrmApiKey = "XXX";
        public const string WebcrmSystemId = "XXX";

        public static readonly FortnoxApiKeys FortnoxApiKeys
            = new FortnoxApiKeys(FortnoxAccessToken, FortnoxClientSecret);

        public static readonly FortnoxConfiguration FortnoxConfiguration
            = new FortnoxConfiguration
            {
                DeliveryIdFieldName = "DeliveryCustom6",
                Disabled = true, // TODO 1458: Enable Fortnox.
                FortnoxAccessToken = FortnoxAccessToken,
                FortnoxClientSecret = FortnoxClientSecret,
                LastSuccessfulCopyFromErpHeartbeat = new DateTime(2018, 05, 16),
                LastSuccessfulCopyToErpHeartbeat = new DateTime(2018, 05, 16),
                OrganisationIdFieldName = FortnoxOrganisationIdFieldName,
                WebcrmApiKey = WebcrmApiKey,
                WebcrmSystemId = WebcrmSystemId
            };

        public static readonly PowerofficeConfiguration PowerofficeConfiguration
            = new PowerofficeConfiguration
            {
                AcceptedOrganisationStatuses = new string[0],
                AcceptedOrganisationTypes = new[] { "Kunde" },
                DeliveryIdFieldName = "DeliveryCustom5",
                Disabled = false,
                LastSuccessfulCopyFromErpHeartbeat = new DateTime(2018, 05, 16),
                LastSuccessfulCopyToErpHeartbeat = new DateTime(2018, 05, 16),
                OrganisationIdFieldName = "OrganisationExtraCustom8",
                OrganisationNumberFieldName = "OrganisationExtraCustom2",
                OrganisationVatNumberFieldName = "OrganisationExtraCustom1",
                PersonIdFieldName = "PersonCustom5",
                PowerofficeClientKey = new Guid("XXX"),
                PrimaryContactCheckboxFieldName = "PersonCustom4",
                ProductCodeFieldName = "QuotationLineLinkedDataItemData1",
                ProductIdFieldName = "QuotationLineLinkedDataItemData3",
                ProductNameFieldName = "QuotationLineLinkedDataItemData2",
                ProductSalesAccountFieldName = "QuotationLineLinkedDataItemData4",
                ProductUnitFieldName = "QuotationLineLinkedDataItemData5",
                SynchroniseProducts = true,
                SynchroniseDeliveries = SynchroniseDeliveries.FromErp,
                WebcrmApiKey = WebcrmApiKey,
                WebcrmSystemId = WebcrmSystemId
            };
    }
}