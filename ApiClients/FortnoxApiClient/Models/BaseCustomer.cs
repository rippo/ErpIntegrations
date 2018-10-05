using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using Webcrm.ErpIntegrations.GeneralUtilities;

namespace Webcrm.ErpIntegrations.ApiClients.FortnoxApiClient.Models
{
    public abstract class BaseCustomer
    {
        [Key]
        public string CustomerNumber { get; set; }

        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }

        [JsonIgnore]
        public string ConcatenatedAddress => StringUtilities.JoinDefined(StringUtilities.WindowsNewline, Address1, Address2);
    }
}