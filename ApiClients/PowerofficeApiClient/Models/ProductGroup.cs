namespace Webcrm.ErpIntegrations.ApiClients.PowerofficeApiClient.Models
{
    /// <remarks>
    /// The product group has more properties than the ones list below, but we are only interested in the name.
    /// https://api.poweroffice.net/Web/docs/index.html#reference/rest/Type_ProductGroup.md
    /// </remarks>
    public class ProductGroup
    {
        public string Name { get; set; }
    }
}