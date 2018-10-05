using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Webcrm.ErpIntegrations.ApiClients.FortnoxApiClient
{
    public static class Deserializer<TData>
    {
        public static async Task<TData> DeserializeAndVerify(HttpResponseMessage responseMessage, string responseContentType)
        {
            if (!responseMessage.IsSuccessStatusCode)
            {
                throw new ApplicationException($"Http error {responseMessage.RequestMessage.Method}'ing {responseContentType}. {await GetResponseInfo(responseMessage)}");
            }

            string responseContent = await responseMessage.Content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject<TData>(responseContent);

            return responseObject;
        }

        private static async Task<string> GetResponseInfo(HttpResponseMessage response)
        {
            string content = await response.Content.ReadAsStringAsync();
            return $"Status code: {response.StatusCode}. Content: {content}.";
        }
    }
}