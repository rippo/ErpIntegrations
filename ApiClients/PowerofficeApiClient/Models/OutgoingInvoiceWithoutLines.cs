using System;
using System.ComponentModel.DataAnnotations;

namespace Webcrm.ErpIntegrations.ApiClients.PowerofficeApiClient.Models
{
    /// <summary>`OutgoingInvoiceWithoutLines` is the invoice you get when fetching a list of invoices, like when getting the recently upserted invoices. The objects contain everything that a full OutgoingInvoice has, except for the invoice lines. PowerOffice calls this an `OutgoingInvoiceListItem`.</summary>
    public class OutgoingInvoiceWithoutLines : BaseOutgoingInvoice
    {
        [Key]
        public Guid Id { get; set; }

        public string Cid { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public string CustomerEmail { get; set; }
        public string DocumentNo { get; set; }
        public InvoiceDeliveryType InvoiceDeliveryType { get; set; }
        public DateTimeOffset LastChanged { get; set; }
        public decimal NetAmount { get; set; }
        public long? OrderNo { get; set; }
        public decimal TotalAmount { get; set; }
    }
}