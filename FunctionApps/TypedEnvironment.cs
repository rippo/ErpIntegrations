using System;
using Webcrm.ErpIntegrations.Configurations.Models;

namespace Webcrm.ErpIntegrations.FunctionApps
{
    /// <summary>Strongly typed accessors for the environment variables.</summary>
    public static class TypedEnvironment
    {
        public static string AppInsightsInstrumentationKey => Environment.GetEnvironmentVariable("APPINSIGHTS_INSTRUMENTATIONKEY");

        public static string AzureWebJobsStorage => Environment.GetEnvironmentVariable("AzureWebJobsStorage");

        public static DatabaseCredentials DatabaseCredentials => new DatabaseCredentials(
            Environment.GetEnvironmentVariable("DatabaseEndpoint"),
            Environment.GetEnvironmentVariable("DatabaseAccountKey"));

        public static PowerofficeApiSettings PowerofficeApiSettings => new PowerofficeApiSettings(
            Environment.GetEnvironmentVariable("PowerofficeApiBaseAddress"),
            Environment.GetEnvironmentVariable("PowerofficeAuthenticationBaseAddress"),
            Environment.GetEnvironmentVariable("PowerofficeApplicationKey"));

        public static readonly Uri WebcrmApiBaseUrl = new Uri(Environment.GetEnvironmentVariable("WebcrmApiBaseUrl")
            ?? throw new InvalidOperationException("WebcrmApiBaseUrl must be a valid URL."));
    }
}