using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using Webcrm.ErpIntegrations.ApiClients.WebcrmApiClient;

namespace Webcrm.ErpIntegrations.ApiClients.PowerofficeApiClient.Models
{
    /// <summary>OutgoingInvoiceLine is a line on an invoice. The lines are not available on OutgoingInvoiceWithoutLines.</summary>
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class OutgoingInvoiceLine
    {
        public OutgoingInvoiceLine()
        { }

        public OutgoingInvoiceLine(
            QuotationLineDto webcrmDeliveryLine,
            string productCodeFieldName)
        {
            Description = webcrmDeliveryLine.QuotationLineMemo;

            ProductCode = webcrmDeliveryLine.GetPowerofficeProductCode(productCodeFieldName);

            if (webcrmDeliveryLine.QuotationLineQuantity != null)
                Quantity = Convert.ToDecimal(webcrmDeliveryLine.QuotationLineQuantity.Value);

            if (webcrmDeliveryLine.QuotationLineSortOrder != null)
                SortOrder = webcrmDeliveryLine.QuotationLineSortOrder.Value;

            if (webcrmDeliveryLine.QuotationLinePrice != null)
                UnitPrice = Convert.ToDecimal(webcrmDeliveryLine.QuotationLinePrice.Value);
        }

        // Read-only values that are set by PowerOffice. Set to null when creating an invoice line.
        [Key]
        public long? Id { get; set; }

        public decimal? NetAmount { get; set; }
        public decimal? TotalAmount { get; set; }

        // Read-write values.
        public string DepartmentCode { get; set; }
        public string Description { get; set; }
        public decimal? DiscountPercent { get; set; }
        public bool? ExemptVat { get; set; }
        public bool? IsDeleted { get; set; }
        public VoucherLineType LineType { get; set; }
        public string ProductCode { get; set; }
        public decimal? Quantity { get; set; }
        public long? SalesPersonEmployeeCode { get; set; }
        public int SortOrder { get; set; }
        public string UnitOfMeasure { get; set; }
        public decimal? UnitPrice { get; set; }
    }
}