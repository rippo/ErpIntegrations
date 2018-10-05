using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using Webcrm.ErpIntegrations.Configurations;
using Webcrm.ErpIntegrations.Synchronisation.Models;
using Webcrm.ErpIntegrations.Test;
using Xunit;
using Xunit.Abstractions;

namespace Webcrm.ErpIntegrations.Synchronisation.Test
{
    public class PowerofficeMessageDispatcherTester : BaseTester
    {
        public PowerofficeMessageDispatcherTester(ITestOutputHelper output) : base(output)
        { }

        [Theory]
        [Trait(Traits.Execution, Traits.Manual)]
        [InlineData(InsertPowerofficeDeliveryQueueItem)]
        [InlineData(UpdatePowerofficeDeliveryQueueItem)]
        [InlineData(UpdatePowerofficeOrganisationQueueItem)]
        [InlineData(UpdateWebcrmOrganisationQueueItem)]
        [InlineData(UpdateWebcrmPersonQueueItem)]
        [InlineData(UpdateWebcrmProductQueueItem1)]
        [InlineData(UpdateWebcrmProductQueueItem2)]
        public async Task TestHandleMessage(string queueItem)
        {
            string formattedQueueItem = queueItem.Replace("{TIMESTAMP}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            var upsertPersonMessage = JsonConvert.DeserializeObject<PowerofficeQueueMessage>(formattedQueueItem);
            var dispatcher = await PowerofficeMessageDispatcher.Create(
                OutputLogger,
                TestWebcrmClientFactory,
                TestTypedEnvironment.DatabaseCredentials,
                TestPowerofficeClientFactory);
            await dispatcher.HandleDequeuedMessage(upsertPersonMessage);
        }

        private const string InsertPowerofficeDeliveryQueueItem = @"{
            ""Action"": ""UpsertPowerofficeDelivery"",
            ""SerialisedPayload"": ""{
                \""WebcrmDelivery\"": {
                    \""DeliveryAssignedTo\"": 5,
                    \""DeliveryAssignedTo2\"": 0,
                    \""DeliveryComment\"": \""\"",
                    \""DeliveryCreatedAt\"": \""2018-06-12T16:33:42+02:00\"",
                    \""DeliveryCreatedBy\"": \""Jan Aagaard\"",
                    \""DeliveryCurrencyName\"": \""00\"",
                    \""DeliveryDescription\"": \""Delivery\"",
                    \""DeliveryDiscount\"": 0.0,
                    \""DeliveryErpId\"": \""\"",
                    \""DeliveryErpReadOnly\"": \""\"",
                    \""DeliveryErpStatus\"": \""NotReadyForSynchronization\"",
                    \""DeliveryErpSyncDateTime\"": \""0001-01-01T01:00:00+01:00\"",
                    \""DeliveryGmRevenue1\"": 0.0,
                    \""DeliveryGmRevenue2\"": 0.0,
                    \""DeliveryGmRevenue3\"": 0.0,
                    \""DeliveryGmRevenue4\"": 0.0,
                    \""DeliveryGmRevenue5\"": 0.0,
                    \""DeliveryGmRevenue6\"": 0.0,
                    \""DeliveryGmRevenue7\"": 0.0,
                    \""DeliveryGmRevenue8\"": 0.0,
                    \""DeliveryGmRevenue9\"": 0.0,
                    \""DeliveryGmRevenue10\"": 0.0,
                    \""DeliveryGmRevenue11\"": 0.0,
                    \""DeliveryGmRevenue12\"": 0.0,
                    \""DeliveryHistory\"": \""<tr><td>12-06-2018&nbsp;16:33&nbsp;</td><td>JAA&nbsp;</td><td>(JAA)&nbsp;0&nbsp;</td><td colspan=\\\""2\\\"">New / General&nbsp;</td><td>No risk</td></tr> \"",
                    \""DeliveryId\"": 158,
                    \""DeliveryNextFollowUp\"": \""0001-01-01T01:00:00+01:00\"",
                    \""DeliveryNote\"": \""\"",
                    \""DeliveryNumber\"": \""4634\"",
                    \""DeliveryOpportunityCustom1\"": \""2018-06-28\"",
                    \""DeliveryOpportunityCustom2\"": \""2018-06-28\"",
                    \""DeliveryOpportunityCustom3\"": \""\"",
                    \""DeliveryOpportunityCustom6\"": \""\"",
                    \""DeliveryOpportunityMemo\"": \""\"",
                    \""DeliveryOrderDate\"": \""2018-06-12T02:00:00+02:00\"",
                    \""DeliveryOrderGmValue\"": 31650.0,
                    \""DeliveryOrderValue\"": 36850.0,
                    \""DeliveryOrganisationId\"": 145,
                    \""DeliveryPercent\"": 100,
                    \""DeliveryPersonId\"": 115,
                    \""DeliveryProduct\"": \""-- vælg --\"",
                    \""DeliveryProductId\"": 0,
                    \""DeliveryQuotationLanguageId\"":0,
                    \""DeliveryResponsible\"": 0,
                    \""DeliveryRevenue1\"": 0.0,
                    \""DeliveryRevenue2\"": 0.0,
                    \""DeliveryRevenue3\"": 0.0,
                    \""DeliveryRevenue4\"": 0.0,
                    \""DeliveryRevenue5\"": 0.0,
                    \""DeliveryRevenue6\"": 0.0,
                    \""DeliveryRevenue7\"": 0.0,
                    \""DeliveryRevenue8\"": 0.0,
                    \""DeliveryRevenue9\"": 0.0,
                    \""DeliveryRevenue10\"": 0.0,
                    \""DeliveryRevenue11\"": 0.0,
                    \""DeliveryRevenue12\"": 0.0,
                    \""DeliveryRisk\"": \""NoRisk\"",
                    \""DeliverySearch1\"": \""\"",
                    \""DeliverySearch2\"": \""\"",
                    \""DeliverySpentTime\"": \""00:00:00\"",
                    \""DeliveryStatus\"": \""New\"",
                    \""DeliveryType\"": 0,
                    \""DeliveryUpdatedAt\"": \""2018-06-28T08:32:08+02:00\"",
                    \""DeliveryUpdatedBy\"": \""Jan Aagaard\"",
                    \""DeliveryUserGroupId\"": 0,
                    \""DeliveryWinPercent\"": 0,
                    \""DeliveryWinPercent2\"": 0,
                    \""DeliveryWinYesNo\"": false,
                    \""DeliveryWonDate\"": \""0001-01-01T01:00:00+01:00\"",
                    \""DeliveryCustom1\"": \""\"",
                    \""DeliveryCustom2\"": \""-- Vælg ---\"",
                    \""DeliveryCustom3\"": \""\"",
                    \""DeliveryCustom4\"": \""\"",
                    \""DeliveryCustom5\"": \""\"",
                    \""DeliveryCustom6\"": \""\"",
                    \""DeliveryCustom7\"": \""\"",
                    \""DeliveryCustom8\"": \""\"",
                    \""DeliveryCustom9\"": \""\"",
                    \""DeliveryCustom10\"": \""\"",
                    \""DeliveryCustom11\"": \""\"",
                    \""DeliveryCustom12\"": \""\"",
                    \""DeliveryCustom13\"": \""\"",
                    \""DeliveryCustom14\"": \""\"",
                    \""DeliveryCustom15\"": \""\"",
                    \""DeliveryMemo\"": \""\""
                },
                \""WebcrmDeliveryLines\"": [
                    {
                        \""QuotationLineComment\"": \""\"",
                        \""QuotationLineCostPrice\"": 501.0,
                        \""QuotationLineCreatedAt\"": \""2018-06-28T08:32:21+02:00\"",
                        \""QuotationLineCreatedBy\"": \""Jan Aagaard\"",
                        \""QuotationLineData1\"": \""200\"",
                        \""QuotationLineData2\"": \""Te\"",
                        \""QuotationLineData3\"": \""1254549\"",
                        \""QuotationLineDiscount\"": 10.0,
                        \""QuotationLineId\"": 49,
                        \""QuotationLineListPrice\"": 451.0,
                        \""QuotationLineMemo\"": \""Created by automated test {TIMESTAMP}\"",
                        \""QuotationLineOpportunityId\"": 158,
                        \""QuotationLineOrganisationId\"": 145,
                        \""QuotationLinePersonId\"": 0,
                        \""QuotationLinePrice\"": 4051.0,
                        \""QuotationLineQuantity\"": 10.0,
                        \""QuotationLineSortOrder\"": 10,
                        \""QuotationLineStockStatus\"": \""\"",
                        \""QuotationLineUpdatedAt\"": \""2018-06-28T08:32:53+02:00\"",
                        \""QuotationLineUpdatedBy\"": \""Jan Aagaard\"",
                        \""QuotationLineVatPercentage\"": 0.0
                    },
                    {
                        \""QuotationLineComment\"": \""\"",
                        \""QuotationLineCostPrice\"": 201.0,
                        \""QuotationLineCreatedAt\"": \""2018-06-28T08:33:12+02:00\"",
                        \""QuotationLineCreatedBy\"": \""Jan Aagaard\"",
                        \""QuotationLineData1\"": \""2\"",
                        \""QuotationLineData2\"": \""Hat\"",
                        \""QuotationLineDiscount\"": 0.0,
                        \""QuotationLineId\"": 50,
                        \""QuotationLineListPrice\"": 401.0,
                        \""QuotationLineMemo\"": \""Created by automated test {TIMESTAMP}\"",
                        \""QuotationLineOpportunityId\"": 158,
                        \""QuotationLineOrganisationId\"": 145,
                        \""QuotationLinePersonId\"": 0,
                        \""QuotationLinePrice\"": 401.0,
                        \""QuotationLineQuantity\"": 1.0,
                        \""QuotationLineSortOrder\"": 20,
                        \""QuotationLineStockStatus\"": \""\"",
                        \""QuotationLineUpdatedAt\"": \""2018-06-28T08:33:27+02:00\"",
                        \""QuotationLineUpdatedBy\"": \""Jan Aagaard\"",
                        \""QuotationLineVatPercentage\"": 0.0
                    }
                ],
                \""WebcrmSystemId\"": \""27885\""
            }""
        }";

        private const string UpdatePowerofficeDeliveryQueueItem = @"{
            ""Action"": ""UpsertPowerofficeDelivery"",
            ""SerialisedPayload"": ""{
                \""WebcrmDelivery\"": {
                    \""DeliveryAssignedTo\"":5,
                    \""DeliveryAssignedTo2\"":0,
                    \""DeliveryComment\"": \""\"",
                    \""DeliveryCreatedAt\"": \""2018-06-12T16:33:42+02:00\"",
                    \""DeliveryCreatedBy\"": \""Jan Aagaard\"",
                    \""DeliveryCurrencyName\"": \""00\"",
                    \""DeliveryDescription\"": \""Delivery\"",
                    \""DeliveryDiscount\"": 0.0,
                    \""DeliveryErpId\"": \""\"",
                    \""DeliveryErpReadOnly\"": \""\"",
                    \""DeliveryErpStatus\"": \""NotReadyForSynchronization\"",
                    \""DeliveryErpSyncDateTime\"": \""0001-01-01T01:00:00+01:00\"",
                    \""DeliveryGmRevenue1\"": 0.0,
                    \""DeliveryGmRevenue2\"": 0.0,
                    \""DeliveryGmRevenue3\"": 0.0,
                    \""DeliveryGmRevenue4\"": 0.0,
                    \""DeliveryGmRevenue5\"": 0.0,
                    \""DeliveryGmRevenue6\"": 0.0,
                    \""DeliveryGmRevenue7\"": 0.0,
                    \""DeliveryGmRevenue8\"": 0.0,
                    \""DeliveryGmRevenue9\"": 0.0,
                    \""DeliveryGmRevenue10\"": 0.0,
                    \""DeliveryGmRevenue11\"": 0.0,
                    \""DeliveryGmRevenue12\"": 0.0,
                    \""DeliveryHistory\"": \""<tr><td>12-06-2018&nbsp;16:33&nbsp;</td><td>JAA&nbsp;</td><td>(JAA)&nbsp;0&nbsp;</td><td colspan=\\\""2\\\"">New / General&nbsp;</td><td>No risk</td></tr> \"",
                    \""DeliveryId\"": 158,
                    \""DeliveryNextFollowUp\"": \""0001-01-01T01:00:00+01:00\"",
                    \""DeliveryNote\"": \""\"",
                    \""DeliveryNumber\"": \""4634\"",
                    \""DeliveryOpportunityCustom1\"": \""2018-06-28\"",
                    \""DeliveryOpportunityCustom2\"": \""2018-06-28\"",
                    \""DeliveryOpportunityCustom3\"": \""\"",
                    \""DeliveryOpportunityCustom6\"": \""\"",
                    \""DeliveryOpportunityMemo\"": \""\"",
                    \""DeliveryOrderDate\"": \""2018-06-12T02:00:00+02:00\"",
                    \""DeliveryOrderGmValue\"": 31650.0,
                    \""DeliveryOrderValue\"": 36850.0,
                    \""DeliveryOrganisationId\"": 145,
                    \""DeliveryPercent\"": 100,
                    \""DeliveryPersonId\"": 58,
                    \""DeliveryProduct\"": \""-- vælg --\"",
                    \""DeliveryProductId\"": 0,
                    \""DeliveryQuotationLanguageId\"":0,
                    \""DeliveryResponsible\"": 0,
                    \""DeliveryRevenue1\"": 0.0,
                    \""DeliveryRevenue2\"": 0.0,
                    \""DeliveryRevenue3\"": 0.0,
                    \""DeliveryRevenue4\"": 0.0,
                    \""DeliveryRevenue5\"": 0.0,
                    \""DeliveryRevenue6\"": 0.0,
                    \""DeliveryRevenue7\"": 0.0,
                    \""DeliveryRevenue8\"": 0.0,
                    \""DeliveryRevenue9\"": 0.0,
                    \""DeliveryRevenue10\"": 0.0,
                    \""DeliveryRevenue11\"": 0.0,
                    \""DeliveryRevenue12\"": 0.0,
                    \""DeliveryRisk\"": \""NoRisk\"",
                    \""DeliverySearch1\"": \""\"",
                    \""DeliverySearch2\"": \""\"",
                    \""DeliverySpentTime\"": \""00:00:00\"",
                    \""DeliveryStatus\"": \""New\"",
                    \""DeliveryType\"": 0,
                    \""DeliveryUpdatedAt\"": \""2018-06-28T08:32:08+02:00\"",
                    \""DeliveryUpdatedBy\"": \""Jan Aagaard\"",
                    \""DeliveryUserGroupId\"": 0,
                    \""DeliveryWinPercent\"": 0,
                    \""DeliveryWinPercent2\"": 0,
                    \""DeliveryWinYesNo\"": false,
                    \""DeliveryWonDate\"": \""0001-01-01T01:00:00+01:00\"",
                    \""DeliveryCustom1\"": \""\"",
                    \""DeliveryCustom2\"": \""-- Vælg ---\"",
                    \""DeliveryCustom3\"": \""\"",
                    \""DeliveryCustom4\"": \""\"",
                    \""DeliveryCustom5\"": \""2d769ef1-92e2-4187-9765-ac3af442095a\"",
                    \""DeliveryCustom6\"": \""\"",
                    \""DeliveryCustom7\"": \""\"",
                    \""DeliveryCustom8\"": \""\"",
                    \""DeliveryCustom9\"": \""\"",
                    \""DeliveryCustom10\"": \""\"",
                    \""DeliveryCustom11\"": \""\"",
                    \""DeliveryCustom12\"": \""\"",
                    \""DeliveryCustom13\"": \""\"",
                    \""DeliveryCustom14\"": \""\"",
                    \""DeliveryCustom15\"": \""\"",
                    \""DeliveryMemo\"": \""\""
                },
                \""WebcrmDeliveryLines\"": [
                    {
                        \""QuotationLineComment\"": \""\"",
                        \""QuotationLineCostPrice\"": 501.0,
                        \""QuotationLineCreatedAt\"": \""2018-06-28T08:32:21+02:00\"",
                        \""QuotationLineCreatedBy\"": \""Jan Aagaard\"",
                        \""QuotationLineData1\"": \""200\"",
                        \""QuotationLineData2\"": \""Te\"",
                        \""QuotationLineData3\"": \""1254549\"",
                        \""QuotationLineDiscount\"": 10.0,
                        \""QuotationLineId\"": 49,
                        \""QuotationLineListPrice\"": 450.0,
                        \""QuotationLineMemo\"": \""Created by automated test {TIMESTAMP}\"",
                        \""QuotationLineOpportunityId\"": 158,
                        \""QuotationLineOrganisationId\"": 145,
                        \""QuotationLinePersonId\"": 0,
                        \""QuotationLinePrice\"": 4050.0,
                        \""QuotationLineQuantity\"": 10.0,
                        \""QuotationLineSortOrder\"": 10,
                        \""QuotationLineStockStatus\"": \""\"",
                        \""QuotationLineUpdatedAt\"": \""2018-06-28T08:32:53+02:00\"",
                        \""QuotationLineUpdatedBy\"": \""Jan Aagaard\"",
                        \""QuotationLineVatPercentage\"": 0.0
                    },
                    {
                        \""QuotationLineComment\"": \""\"",
                        \""QuotationLineCostPrice\"": 201.0,
                        \""QuotationLineCreatedAt\"": \""2018-06-28T08:33:12+02:00\"",
                        \""QuotationLineCreatedBy\"": \""Jan Aagaard\"",
                        \""QuotationLineData1\"": \""2\"",
                        \""QuotationLineData2\"": \""Hat\"",
                        \""QuotationLineDiscount\"": 0.0,
                        \""QuotationLineId\"": 50,
                        \""QuotationLineListPrice\"": 400.0,
                        \""QuotationLineMemo\"": \""Created by automated test {TIMESTAMP}\"",
                        \""QuotationLineOpportunityId\"": 158,
                        \""QuotationLineOrganisationId\"": 145,
                        \""QuotationLinePersonId\"": 0,
                        \""QuotationLinePrice\"": 400.0,
                        \""QuotationLineQuantity\"": 1.0,
                        \""QuotationLineSortOrder\"": 20,
                        \""QuotationLineStockStatus\"": \""\"",
                        \""QuotationLineUpdatedAt\"": \""2018-06-28T08:33:27+02:00\"",
                        \""QuotationLineUpdatedBy\"": \""Jan Aagaard\"",
                        \""QuotationLineVatPercentage\"": 0.0
                    }
                ],
                \""WebcrmSystemId\"": \""27885\""
            }""
        }";

        private const string UpdatePowerofficeOrganisationQueueItem = @"{
            ""Action"": ""UpsertPowerofficeOrganisation"",
            ""SerialisedPayload"": ""{
                \""WebcrmOrganisation\"": {
                    \""OrganisationSplittedAddress\"": [\""Typo 123\""],
                    \""OrganisationAddress\"": \""Typo 123\"",
                    \""OrganisationAlert\"": \""\"",
                    \""OrganisationApprovalStatus\"": 0,
                    \""OrganisationCity\"": \""TRONDHEIM\"",
                    \""OrganisationComment\"": \""\"",
                    \""OrganisationCompareName\"": \""AKKURATAS3\"",
                    \""OrganisationCountry\"": \""Danmark\"",
                    \""OrganisationCreatedAt\"": \""2018-05-02T10:39:47+00:00\"",
                    \""OrganisationCreatedBy\"": \""api2 ErpIntegrations project\"",
                    \""OrganisationDivisionName\"": \""\"",
                    \""OrganisationDomain\"": \""\"",
                    \""OrganisationExtraCustom1\"": \""\"",
                    \""OrganisationExtraCustom2\"": \""\"",
                    \""OrganisationExtraCustom3\"": \""\"",
                    \""OrganisationExtraCustom4\"": \""\"",
                    \""OrganisationExtraCustom5\"": \""\"",
                    \""OrganisationExtraCustom6\"": \""\"",
                    \""OrganisationExtraCustom7\"": \""\"",
                    \""OrganisationExtraCustom8\"": \""1522391\"",
                    \""OrganisationFax\"": \""\"",
                    \""OrganisationGps\"": \""\"",
                    \""OrganisationId\"": 145,
                    \""OrganisationImageFileExtension\"": \""\"",
                    \""OrganisationIndustry\"": \""-- vælg --\"",
                    \""OrganisationLastDisplayedAt\"": \""2018-06-04T08:23:55+00:00\"",
                    \""OrganisationLastItemType\"": \""Undefined\"",
                    \""OrganisationLastItemUpdatedAt\"": \""0001-01-01T00:00:00+00:00\"",
                    \""OrganisationMarketDataId\"": \""\"",
                    \""OrganisationName\"": \""Akkurat AS 3\"",
                    \""OrganisationNoAds\"": false,
                    \""OrganisationHistory\"": \""<tr><td>04-06-2018&nbsp;10:23&nbsp;</td><td>JAA&nbsp;</td><td>-- vælg --&nbsp;</td><td>&nbsp;-- vælg --</td></tr>\"",
                    \""OrganisationOutlookSync\"": 0,
                    \""OrganisationOverlayUrl\"": \""\"",
                    \""OrganisationOwner\"": 0,
                    \""OrganisationOwner2\"": 0,
                    \""OrganisationPostCode\"": \""7426\"",
                    \""OrganisationReportTemp\"": 0,
                    \""OrganisationSla\"": 0,
                    \""OrganisationState\"": \""\"",
                    \""OrganisationStatus\"": \""-- vælg --\"",
                    \""OrganisationTelephone\"": \""12974798\"",
                    \""OrganisationTelephoneSearch\"": \""-12974798-\"",
                    \""OrganisationTerritoryId\"": 1,
                    \""OrganisationType\"": \""-- vælg --\"",
                    \""OrganisationUpdatedAt\"": \""2018-06-04T08:23:55+00:00\"",
                    \""OrganisationUpdatedBy\"": \""Jan Aagaard\"",
                    \""OrganisationVatCountry\"": \""\"",
                    \""OrganisationVatGroup\"": \""\"",
                    \""OrganisationVatNumber\"": \""826257377\"",
                    \""OrganisationVatStatus\"": \""\"",
                    \""OrganisationVatVerifiedAt\"": \""0001-01-01T00:00:00+00:00\"",
                    \""OrganisationWww\"": \""www.badadvice.com\"",
                    \""OrganisationXDate1\"": \""0001-01-01T00:00:00+00:00\"",
                    \""OrganisationXDate2\"": \""0001-01-01T00:00:00+00:00\"",
                    \""OrganisationXInt1\"": 0,
                    \""OrganisationXInt2\"": 0,
                    \""OrganisationXInt3\"": 0,
                    \""OrganisationXInt4\"": 0,
                    \""OrganisationXInt5\"": 0,
                    \""OrganisationXInt6\"": 0,
                    \""OrganisationXInt7\"": 0,
                    \""OrganisationXInt8\"": 0,
                    \""OrganisationXMemo1\"": \""\"",
                    \""OrganisationXMemo2\"": \""\"",
                    \""OrganisationXText1\"": \""\"",
                    \""OrganisationXText2\"": \""\"",
                    \""OrganisationXText3\"": \""\"",
                    \""OrganisationXText4\"": \""\"",
                    \""OrganisationXText5\"": \""\"",
                    \""OrganisationXText6\"": \""\"",
                    \""OrganisationXText7\"": \""\"",
                    \""OrganisationXText8\"": \""\"",
                    \""OrganisationCustom1\"": \""\"",
                    \""OrganisationCustom2\"": \""\"",
                    \""OrganisationCustom3\"": \""\"",
                    \""OrganisationCustom4\"": \""\"",
                    \""OrganisationCustom5\"": \""No\"",
                    \""OrganisationCustom6\"": \""00: ? Item 1\"",
                    \""OrganisationCustom7\"": \""[]\"",
                    \""OrganisationCustom8\"": \""\"",
                    \""OrganisationCustom9\"": \""\"",
                    \""OrganisationCustom10\"": \""0\"",
                    \""OrganisationCustom11\"": \""\"",
                    \""OrganisationCustom12\"": \""\"",
                    \""OrganisationCustom13\"": \""\"",
                    \""OrganisationCustom14\"": \""\"",
                    \""OrganisationCustom15\"": \""\"",
                    \""OrganisationMemo\"": \""\""
                },
                \""WebcrmSystemId\"": \""27885\""
            }""
        }";

        private const string UpdateWebcrmOrganisationQueueItem = @"{
            ""Action"": ""UpsertWebcrmOrganisation"",
            ""SerialisedPayload"": ""{
                \""PowerofficeOrganisation\"": {
                    \""Id\"": 1522373,
                    \""InvoiceDeliveryType\"": 1,
                    \""LastChanged\"": \""2018-05-28T08:42:07+00:00\"",
                    \""Code\"": 10000,
                    \""ContactGroups\"": [],
                    \""ContactPersonId\"": 928350,
                    \""EmailAddress\"": \""invoice@bye.com\"",
                    \""ExternalCode\"": 10000,
                    \""InvoiceEmailAddress\"": \""invoice@bye.com\"",
                    \""IsArchived\"": false,
                    \""IsPerson\"": false,
                    \""IsVatFree\"": false,
                    \""LegalName\"": \""A-B Transport AS\"",
                    \""MailAddress\"": {
                        \""Id\"": 1255970,
                        \""Address1\"": \""Main Street 4738\"",
                        \""Address2\"": null,
                        \""Address3\"": null,
                        \""City\"": \""TINGVATN\"",
                        \""CountryCode\"": \""NO\"",
                        \""IsPrimary\"": false,
                        \""ZipCode\"": \""4595\""
                    },
                    \""Name\"": \""A-B Transport AS 10\"",
                    \""PhoneNumber\"": \""78978069\"",
                    \""Since\"": \""2018-01-11T00:00:00\"",
                    \""StreetAddresses\"": [],
                    \""VatNumber\"": \""924618094\"",
                    \""WebsiteUrl\"": \""www.bye.com\""
                },
                \""WebcrmSystemId\"": \""27885\""
            }""
        }";

        private const string UpdateWebcrmPersonQueueItem = @"{
            ""Action"": ""UpsertWebcrmPerson"",
            ""SerialisedPayload"": ""{
                \""PowerofficeOrganisationId\"": 1522373,
                \""PowerofficePerson\"": {
                    \""Id\"": 928350,
                    \""LastChanged\"": \""2018-05-28T09:32:33+00:00\"",
                    \""EmailAddress\"": \""\"",
                    \""FirstName\"": \""Bat\"",
                    \""IsActive\"": true,
                    \""LastName\"": \""Man 6\""
                },
                \""WebcrmSystemId\"": \""27885\""
            }""
        }";

        private const string UpdateWebcrmProductQueueItem1 = @"{
            ""Action"": ""UpsertWebcrmProduct"",
            ""SerialisedPayload"": ""{
                \""PowerofficeProduct\"": {
                    \""Id\"": 1254536,
                    \""CreatedFromImportJournalId\"": null,
                    \""LastChanged\"": \""2018-01-11T15:42:20+01:00\"",
                    \""Code\"": \""70\"",
                    \""CostPrice\"": 8500.0,
                    \""Description\"": null,
                    \""Gtin\"": null,
                    \""IsActive\"": true,
                    \""Name\"": \""Steam vaskemaskin\"",
                    \""ProductGroupId\"": 499055,
                    \""ProductsOnHand\"": null,
                    \""SalesAccount\"": null,
                    \""SalesPrice\"": 10000.0,
                    \""Type\"": 2,
                    \""Unit\"": \""EA\"",
                    \""VatExemptSalesAccount\"": null
                },
                \""WebcrmSystemId\"": \""27885\""
            }""
        }";

        private const string UpdateWebcrmProductQueueItem2 = @"{
            ""Action"": ""UpsertWebcrmProduct"",
            ""SerialisedPayload"": ""{
                \""PowerofficeProduct\"": {
                    \""Id\"": 1254549,
                    \""CreatedFromImportJournalId\"": null,
                    \""LastChanged\"": \""2018-01-11T15:42:20+01:00\"",
                    \""Code\"": \""200\"",
                    \""CostPrice\"": 500.0,
                    \""Description\"": null,
                    \""Gtin\"": null,
                    \""IsActive\"": true,
                    \""Name\"": \""Te\"",
                    \""ProductGroupId\"": 499057,
                    \""ProductsOnHand\"": null,
                    \""SalesAccount\"": null,
                    \""SalesPrice\"": 450.0,
                    \""Type\"": 2,
                    \""Unit\"": \""EA\"",
                    \""VatExemptSalesAccount\"": null
                },
                \""WebcrmSystemId\"": \""27885\""
            }""
        }";
    }
}