using System;

namespace Webcrm.ErpIntegrations.Configurations.Models
{
    public class DatabaseCredentials
    {
        public DatabaseCredentials(
            string endpoint,
            string accountKey)
        {
            Endpoint = new Uri(endpoint);
            AccountKey = accountKey;
        }

        public Uri Endpoint { get; }
        public string AccountKey { get; }
    }
}