using Microsoft.ApplicationInsights;
using System.Collections.Generic;
using Webcrm.ErpIntegrations.ApiClients.WebcrmApiClient;

namespace Webcrm.ErpIntegrations.FunctionApps
{
    internal static class SwaggerExceptionLogger
    {
        static SwaggerExceptionLogger()
        {
            TelemetryClient = new TelemetryClient();
            TelemetryClient.InstrumentationKey = TypedEnvironment.AppInsightsInstrumentationKey;
        }

        private static TelemetryClient TelemetryClient { get; }

        public static void Log(SwaggerException ex)
        {
            var additionalProperties = new Dictionary<string, string>
            {
                { "HTTP status code", ex.StatusCode.ToString() },
                { "HTTP response", ex.Response }
            };

            TelemetryClient.TrackException(ex, additionalProperties);
        }
    }
}