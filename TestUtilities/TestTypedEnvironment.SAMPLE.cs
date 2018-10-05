using System;
using Webcrm.ErpIntegrations.Configurations.Models;

namespace Webcrm.ErpIntegrations.Test
{
    /// <summary>Duplicate of local.settings.json to make it easy to access the values in the test code. Only use this class in test projects.</summary>
    public static class TestTypedEnvironment
    {
        public const string AzureWebJobsStorage = "XXX";

        public static readonly DatabaseCredentials DatabaseCredentials = new DatabaseCredentials(
            endpoint: "XXX",
            accountKey: "XXX");

        public static readonly PowerofficeApiSettings PowerofficeApiSettings = new PowerofficeApiSettings(
            apiBaseAddress: "https://api-demo.poweroffice.net/",
            authenticationBaseAddress: "https://godemo.poweroffice.net/",
            applicationKey: "XXX");

        public static readonly Uri WebcrmApiBaseUrl = new Uri("https://api.webcrm.com/");
    }
}