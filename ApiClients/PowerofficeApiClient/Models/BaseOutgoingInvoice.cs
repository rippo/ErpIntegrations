using Newtonsoft.Json;
using System;
using Webcrm.ErpIntegrations.ApiClients.WebcrmApiClient;

namespace Webcrm.ErpIntegrations.ApiClients.PowerofficeApiClient.Models
{
    /// <summary>
    /// Class hierarchy
    /// 
    ///             BaseOutgoingInvoice
    ///               /            \
    /// NewOutgoingInvoice     OutgoingInvoiceWithoutLines
    ///                                   |
    ///                            OutgoingInvoice
    /// </summary>
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public abstract class BaseOutgoingInvoice
    {
        public string BrandingThemeCode { get; set; }
        public string ContractNo { get; set; }
        public string CurrencyCode { get; set; }
        public long? CustomerCode { get; set; }
        public string CustomerReference { get; set; }
        public long? DeliveryAddressId { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public string DepartmentCode { get; set; }
        public long? ImportedOrderNo { get; set; }
        public DateTime? OrderDate { get; set; }
        public long? OurReferenceEmployeeCode { get; set; }
        public string PurchaseOrderNo { get; set; }
        public string ProjectCode { get; set; }
        public int? PaymentTerms { get; set; }
        public OutgoingInvoiceStatus Status { get; set; }

        /// <summary>Updates this delivery based on a webCRM delivery, excluding the delivery lines.</summary>
        protected void UpdateExcludingLines(
            DeliveryDto webcrmDelivery,
            long powerofficeCustomerCode)
        {
            CustomerCode = powerofficeCustomerCode;
            OrderDate = webcrmDelivery.DeliveryOrderDate;
            Status = OutgoingInvoiceStatus.Approved;
        }
    }
}