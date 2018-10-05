using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using Webcrm.ErpIntegrations.GeneralUtilities;

namespace Webcrm.ErpIntegrations.ApiClients.PowerofficeApiClient.Models
{
    /// <summary>https://api.poweroffice.net/Web/docs/index.html#Reference/Rest/Type_Address.md</summary>
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class Address
    {
        [Key]
        public long Id { get; set; }

        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string City { get; set; }
        public string CountryCode { get; set; }
        public bool IsPrimary { get; set; }
        public string ZipCode { get; set; }

        [JsonIgnore]
        public string ConcatenatedAddress => StringUtilities.JoinDefined(StringUtilities.WindowsNewline, Address1, Address2, Address3);
    }
}