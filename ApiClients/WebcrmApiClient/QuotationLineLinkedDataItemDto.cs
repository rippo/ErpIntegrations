using System;
using Webcrm.ErpIntegrations.ApiClients.PowerofficeApiClient.Models;
using Webcrm.ErpIntegrations.Configurations.Models;
using Webcrm.ErpIntegrations.GeneralUtilities;

namespace Webcrm.ErpIntegrations.ApiClients.WebcrmApiClient
{
    public partial class QuotationLineLinkedDataItemDto
    {
        public QuotationLineLinkedDataItemDto()
        {
            // These defaults are required by webCRM.
            QuotationLineLinkedDataItemCostPrice = null;
            QuotationLineLinkedDataItemData1 = DataItemDefault;
            QuotationLineLinkedDataItemData2 = DataItemDefault;
            QuotationLineLinkedDataItemData3 = DataItemDefault;
            QuotationLineLinkedDataItemData4 = DataItemDefault;
            QuotationLineLinkedDataItemData5 = DataItemDefault;
            QuotationLineLinkedDataItemData6 = DataItemDefault;
            QuotationLineLinkedDataItemData7 = DataItemDefault;
            QuotationLineLinkedDataItemData8 = DataItemDefault;
            QuotationLineLinkedDataItemData9 = DataItemDefault;
            QuotationLineLinkedDataItemDataMemo = string.Empty;
            QuotationLineLinkedDataItemEntityType = QuotationLineLinkedDataItemDtoQuotationLineLinkedDataItemEntityType.Product;
            QuotationLineLinkedDataItemId = 0;
            QuotationLineLinkedDataItemItemGroup = string.Empty;
            QuotationLineLinkedDataItemPrice = null;
            QuotationLineLinkedDataItemVatCode = string.Empty;
        }

        /// <summary>webCRM does not allow empty values in the data files. Defaulting to a dash as a workaround.</summary>
        public const string DataItemDefault = "-";

        public void Update(
            Product sourcePowerofficeProduct,
            string sourcePowerofficeProductGroupName,
            PowerofficeConfiguration configuration)
        {
            QuotationLineLinkedDataItemCostPrice = sourcePowerofficeProduct.CostPrice.HasValue
                ? Convert.ToDouble(sourcePowerofficeProduct.CostPrice)
                : (double?)null;

            QuotationLineLinkedDataItemDataMemo = sourcePowerofficeProduct.Description ?? string.Empty;

            QuotationLineLinkedDataItemItemGroup = sourcePowerofficeProductGroupName ?? string.Empty;

            QuotationLineLinkedDataItemPrice = sourcePowerofficeProduct.SalesPrice.HasValue
                ? Convert.ToDouble(sourcePowerofficeProduct.SalesPrice)
                : (double?)null;

            this.SetPropertyValue(configuration.ProductCodeFieldName, UseDashIfNullOrWhiteSpace(sourcePowerofficeProduct.Code));

            this.SetPropertyValue(configuration.ProductIdFieldName, sourcePowerofficeProduct.Id.ToString());

            this.SetPropertyValue(configuration.ProductNameFieldName, UseDashIfNullOrWhiteSpace(sourcePowerofficeProduct.Name));

            this.SetPropertyValue(configuration.ProductSalesAccountFieldName, UseDashIfNullOrWhiteSpace(sourcePowerofficeProduct.SalesAccount?.ToString()));

            this.SetPropertyValue(configuration.ProductUnitFieldName, UseDashIfNullOrWhiteSpace(sourcePowerofficeProduct.Unit));
        }

        private static string UseDashIfNullOrWhiteSpace(string sourceValue)
        {
            if (string.IsNullOrWhiteSpace(sourceValue))
                return DataItemDefault;

            return sourceValue;
        }
    }
}