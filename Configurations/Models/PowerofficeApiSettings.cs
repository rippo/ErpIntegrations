using System;

namespace Webcrm.ErpIntegrations.Configurations.Models
{
    public class PowerofficeApiSettings
    {
        public PowerofficeApiSettings(
            string apiBaseAddress,
            string authenticationBaseAddress,
            string applicationKey)
        {
            ApiBaseAddress = new Uri(apiBaseAddress);
            AuthenticationBaseAddress = new Uri(authenticationBaseAddress);
            ApplicationKey = new Guid(applicationKey);
        }

        public Uri ApiBaseAddress { get; }
        public Uri AuthenticationBaseAddress { get; }
        public Guid ApplicationKey { get; }
    }
}