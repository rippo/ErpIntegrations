namespace Webcrm.ErpIntegrations.ApiClients.PowerofficeApiClient.Models
{
    /// <remarks>https://api.poweroffice.net/Web/docs/index.html#Reference/Rest/Type_InvoiceDeliveryType.md</remarks>
    public enum InvoiceDeliveryType
    {
        None,
        PdfByEmail,
        Print,
        // ReSharper disable once InconsistentNaming
        EHF,
        AvtaleGiro
    }
}