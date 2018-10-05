namespace Webcrm.ErpIntegrations.ApiClients.PowerofficeApiClient.Models
{
    // https://api.poweroffice.net/Web/docs/index.html#reference/rest/Type_OutgoingInvoiceStatus.md
    public enum OutgoingInvoiceStatus
    {
        Draft = 0,
        Approved = 1,
        Sent = 2,
        Paid = 3
    }
}