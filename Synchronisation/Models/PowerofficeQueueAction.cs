namespace Webcrm.ErpIntegrations.Synchronisation.Models
{
    public enum PowerofficeQueueAction
    {
        Unknown,

        UpsertPowerofficeDelivery,
        UpsertPowerofficeOrganisation,
        UpsertPowerofficePerson,
        UpsertWebcrmDelivery,
        UpsertWebcrmOrganisation,
        UpsertWebcrmPerson,
        /// <summary>The correct webCRM terminology for `Product` is `QuotationLineLinkedDataItem`.</summary>
        UpsertWebcrmProduct
    }
}