using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using Webcrm.ErpIntegrations.ApiClients.WebcrmApiClient;

namespace Webcrm.ErpIntegrations.ApiClients.PowerofficeApiClient.Models
{
    /// <summary>`NewOutgoingInvoice` is used to create new invoices. It contains invoice lines, but no Id.</summary>
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class NewOutgoingInvoice : BaseOutgoingInvoice
    {
        public NewOutgoingInvoice()
        {
            OutgoingInvoiceLines = new List<OutgoingInvoiceLine>();
        }

        public NewOutgoingInvoice(
            DeliveryDto webcrmDelivery,
            List<QuotationLineDto> webcrmDeliveryLines,
            long powerofficeCustomerCode,
            string productCodeFieldName)
            : this()
        {
            UpdateExcludingLines(webcrmDelivery, powerofficeCustomerCode);

            OutgoingInvoiceLines = webcrmDeliveryLines
                .Select(webcrmLine => new OutgoingInvoiceLine(webcrmLine, productCodeFieldName))
                .ToList();
        }

        /// <remarks>Duplicated in OutgoingInvoice.cs.</remarks>
        public List<OutgoingInvoiceLine> OutgoingInvoiceLines { get; set; }
    }
}