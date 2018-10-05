namespace Webcrm.ErpIntegrations.ApiClients.PowerofficeApiClient.Models
{
    public enum VoucherLineType
    {
        /// <summary>A normal line, usually with a product.</summary>
        Normal,

        /// <summary>A text line.</summary>
        Text,

        /// <summary>A summary (subtotal) line. See also the specialized value flags below.</summary>
        Summary,

        /// <summary>A line containing an invoice fee.</summary>
        InvoiceFee,

        Total
    }
}