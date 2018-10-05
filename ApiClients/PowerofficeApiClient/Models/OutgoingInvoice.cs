using Newtonsoft.Json;
using System.Collections.Generic;
using Webcrm.ErpIntegrations.ApiClients.WebcrmApiClient;

namespace Webcrm.ErpIntegrations.ApiClients.PowerofficeApiClient.Models
{
    /// <summary>OutgoingInvoice is the full outgoing invoice you get when fetching a specific invoice by ID. It includes read only properties like Id and TotalAmount as well as a list of the invoice lines.</summary>
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class OutgoingInvoice : OutgoingInvoiceWithoutLines
    {
        /// <remarks>Duplicated in NewOutgoingInvoice.cs.</remarks>
        public List<OutgoingInvoiceLine> OutgoingInvoiceLines { get; set; }

        /// <summary>Updates this delivery based on a webCRM delivery, including the delivery lines.</summary>
        public void UpdateIncludingLines(
            DeliveryDto webcrmDelivery,
            IEnumerable<QuotationLineDto> webcrmDeliveryLines,
            long powerofficeCustomerCode,
            string productCodeFieldName)
        {
            UpdateExcludingLines(webcrmDelivery, powerofficeCustomerCode);

            foreach (var powerofficeDeliveryLine in OutgoingInvoiceLines)
            {
                powerofficeDeliveryLine.IsDeleted = true;
            }

            foreach (var webcrmDeliveryLine in webcrmDeliveryLines)
            {
                var newLine = new OutgoingInvoiceLine(webcrmDeliveryLine, productCodeFieldName);
                OutgoingInvoiceLines.Add(newLine);
            }
        }
    }
}