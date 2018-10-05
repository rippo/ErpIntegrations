using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Webcrm.ErpIntegrations.ApiClients.PowerofficeApiClient.Models;
using Webcrm.ErpIntegrations.ApiClients.PowerofficeApiClient.Models.Responses;
using Webcrm.ErpIntegrations.Configurations.Models;
using Webcrm.ErpIntegrations.GeneralUtilities;

namespace Webcrm.ErpIntegrations.ApiClients.PowerofficeApiClient
{
    public sealed class PowerofficeClient
    {
        private PowerofficeClient(AuthenticationHeaderValue authorizationHeader)
        {
            AuthorizationHeader = authorizationHeader;
        }

        internal static async Task<PowerofficeClient> Create(
            PowerofficeApiSettings apiSettings,
            Guid clientKey)
        {
            var authorizationHeader = await GetAuthorizationHeader(apiSettings, clientKey);
            var powerofficeClient = new PowerofficeClient(authorizationHeader);

            lock (SetBaseAddressLock)
            {
                if (HttpClient.BaseAddress == null)
                    HttpClient.BaseAddress = apiSettings.ApiBaseAddress;
            }

            return powerofficeClient;
        }

        private AuthenticationHeaderValue AuthorizationHeader { get; }
        private static readonly HttpClient HttpClient = new HttpClient();
        private static readonly object SetBaseAddressLock = new object();

        public async Task<ContactPerson> CreateContactPerson(long customerId, NewContactPerson newContactPerson)
        {
            var createResponse = await AuthorizedPost($"Customer/{customerId}/Contact", newContactPerson);
            var createdContactPerson = await DeserializeScalarResponse<ContactPerson>(createResponse, "contact person");
            return createdContactPerson;
        }

        public async Task<Customer> CreateCustomer(NewCustomer newCustomer)
        {
            var createResponse = await AuthorizedPost("Customer", newCustomer);
            var createdCustomer = await DeserializeScalarResponse<Customer>(createResponse, "customer");
            return createdCustomer;
        }

        public async Task<OutgoingInvoice> CreateInvoice(NewOutgoingInvoice newInvoice)
        {
            var createResponse = await AuthorizedPost("OutgoingInvoice", newInvoice);
            var createdInvoice = await DeserializeScalarResponse<OutgoingInvoice>(createResponse, "outgoing invoice");
            return createdInvoice;
        }

        public async Task DeleteContactPerson(long customerId, long contactPersonId)
        {
            var deleteResponse = await AuthorizedDelete($"Customer/{customerId}/Contact/{contactPersonId}");
            await VerifyDeleteResponse(deleteResponse, "contact person");
        }

        public async Task DeleteCustomer(long customerId)
        {
            var deleteResponse = await AuthorizedDelete($"Customer/{customerId}");
            await VerifyDeleteResponse(deleteResponse, "customer");
        }

        public async Task DeleteInvoice(Guid newInvoiceId)
        {
            var deleteResponse = await AuthorizedDelete($"OutgoingInvoice/{newInvoiceId}");
            await VerifyDeleteResponse(deleteResponse, "invoice");
        }

        public async Task<List<Customer>> GetActiveOrganisations(DateTime? upsertedAfterUtc = null)
        {
            string getCustomersPath = "Customer?$filter=IsPerson eq false and IsArchived eq false";
            if (upsertedAfterUtc != null)
            {
                string encodedFilterValue = HttpUtility.UrlEncode($"LastChanged ge datetimeoffset'{upsertedAfterUtc:O}'");
                getCustomersPath += $" and {encodedFilterValue}";
            }

            var getCustomersResponse = await AuthorizedGet(getCustomersPath);
            var customers = await DeserializeListResponse<Customer>(getCustomersResponse, "customers");

            return customers;
        }

        public async Task<ContactPerson> GetContactPerson(long customerId, long contactPersonId)
        {
            var getResponse = await AuthorizedGet($"Customer/{customerId}/Contact/{contactPersonId}");
            var contactPerson = await DeserializeScalarResponse<ContactPerson>(getResponse, "contact person");
            return contactPerson;
        }

        /// <summary>Returns `null` if the customer was not found.</summary>
        public async Task<Customer> GetCustomer(long customerId)
        {
            var getResponse = await AuthorizedGet($"Customer/{customerId}");
            var customer = await DeserializeScalarResponse<Customer>(getResponse, "customer");
            return customer;
        }

        /// <summary>Throws an `ApplicationException` if the invoice was not found.</summary>
        public async Task<OutgoingInvoice> GetInvoice(Guid invoiceId)
        {
            var getResponse = await AuthorizedGet($"OutgoingInvoice/{invoiceId}");
            var invoice = await DeserializeScalarResponse<OutgoingInvoice>(getResponse, "invoice");
            return invoice;
        }

        public async Task<ContactPerson> GetMainContactPersonIfUpserted(
            long organisationId,
            long mainContactPersonId,
            DateTime upsertedAfterUtc)
        {
            // OData is not available on Customer/{orgId}/Contact/{personId} even though the documentation states that it is. Using the endpoint for all contact persons instead.
            string lastChangedFilter = HttpUtility.UrlEncode($"Id eq {mainContactPersonId} and LastChanged ge datetimeoffset'{upsertedAfterUtc:O}'");
            var getResponse = await AuthorizedGet($"Customer/{organisationId}/Contact?$filter={lastChangedFilter}");
            // upsertedContactPersons will never contain more than one element.
            var upsertedContactPersons = await DeserializeListResponse<ContactPerson>(getResponse, "main contact persons");
            var mainContactPerson = upsertedContactPersons.FirstOrDefault();
            return mainContactPerson;
        }

        /// <summary>Thrown an exception if the product was not found.</summary>
        public async Task<Product> GetProductByCode(string productCode)
        {
            if (string.IsNullOrWhiteSpace(productCode))
                throw new ApplicationException("productCode has to be defined.");

            string filterByCode = HttpUtility.UrlEncode($"Code eq '{productCode}'");
            var getResponse = await AuthorizedGet($"Product?$filter={filterByCode}");
            var products = await DeserializeListResponse<Product>(getResponse, "product");
            return products.First();
        }

        public async Task<ProductGroup> GetProductGroup(long productGroupId)
        {
            var getResponse = await AuthorizedGet($"ProductGroup/{productGroupId}");
            var productGroup = await DeserializeScalarResponse<ProductGroup>(getResponse, "product group");
            return productGroup;
        }

        public async Task<List<OutgoingInvoiceWithoutLines>> GetUpsertedInvoices(DateTime upsertedAfterUtc)
        {
            string lastChangedFilter = HttpUtility.UrlEncode($"LastChanged ge datetimeoffset'{upsertedAfterUtc:O}'");
            var getResponse = await AuthorizedGet($"OutgoingInvoice/List?$filter={lastChangedFilter}");
            var upsertedInvoices = await DeserializeListResponse<OutgoingInvoiceWithoutLines>(getResponse, "outgoing invoices");
            return upsertedInvoices;
        }

        public async Task<List<Customer>> GetUpsertedOrganisations(DateTime upsertedAfterUtc)
        {
            return await GetActiveOrganisations(upsertedAfterUtc);
        }

        public async Task<List<Product>> GetUpsertedProducts(DateTime upsertedAfterUtc)
        {
            string lastChangedFilter = HttpUtility.UrlEncode($"LastChanged ge datetimeoffset'{upsertedAfterUtc:O}'");
            var getResponse = await AuthorizedGet($"Product?$filter={lastChangedFilter}");
            var upsertedProducts = await DeserializeListResponse<Product>(getResponse, "upserted products");
            return upsertedProducts;
        }

        public async Task<ContactPerson> UpdateContactPerson(long customerId, ContactPerson contactPerson)
        {
            var updateResponse = await AuthorizedPost($"Customer/{customerId}/Contact/", contactPerson);
            var updatedContactPerson = await DeserializeScalarResponse<ContactPerson>(updateResponse, "contact person");
            return updatedContactPerson;
        }

        public async Task<Customer> UpdateCustomer(Customer customer)
        {
            var updateResponse = await AuthorizedPost("Customer", customer);
            var updatedCustomer = await DeserializeScalarResponse<Customer>(updateResponse, "customer");
            return updatedCustomer;
        }

        public async Task<OutgoingInvoice> UpdateInvoice(OutgoingInvoice invoice)
        {
            var updateResponse = await AuthorizedPost("OutgoingInvoice", invoice);
            var updatedInvoice = await DeserializeScalarResponse<OutgoingInvoice>(updateResponse, "outgoing invoice");
            return updatedInvoice;
        }

        private async Task<HttpResponseMessage> AuthorizedDelete(string requestUri)
        {
            return await AuthorizedSend(HttpMethod.Delete, requestUri, null);
        }

        private async Task<HttpResponseMessage> AuthorizedGet(string requestUri)
        {
            return await AuthorizedSend(HttpMethod.Get, requestUri, null);
        }

        private async Task<HttpResponseMessage> AuthorizedPost(string requestUri, object content)
        {
            return await AuthorizedSend(HttpMethod.Post, requestUri, content);
        }

        private async Task<HttpResponseMessage> AuthorizedSend(HttpMethod method, string requestUri, object content)
        {
            HttpResponseMessage response;
            using (var request = new HttpRequestMessage(method, requestUri))
            {
                request.Headers.Authorization = AuthorizationHeader;
                if (content != null)
                {
                    request.Content = SerializeContent(content);
                }

                // ReSharper disable once InconsistentlySynchronizedField
                response = await HttpClient.SendAsync(request);
            }

            return response;
        }

        private static async Task<List<TData>> DeserializeListResponse<TData>(HttpResponseMessage responseMessage, string responseContentType)
        {
            return await DeserializeAndVerifyResponse<ListResponse<TData>, List<TData>>(responseMessage, responseContentType);
        }

        private static async Task<TData> DeserializeScalarResponse<TData>(HttpResponseMessage responseMessage, string responseContentType)
        {
            return await DeserializeAndVerifyResponse<ScalarResponse<TData>, TData>(responseMessage, responseContentType);
        }

        private static async Task VerifyDeleteResponse(HttpResponseMessage responseMessage, string responseContentType)
        {
            await DeserializeAndVerifyResponse<DeleteResponse, object>(responseMessage, responseContentType);
        }

        private static async Task<TData> DeserializeAndVerifyResponse<TResponse, TData>(
            HttpResponseMessage responseMessage,
            string responseContentType)
                where TResponse : Response<TData>
        {
            try
            {
                if (!responseMessage.IsSuccessStatusCode)
                {
                    throw new ApplicationException($"Http error {responseMessage.RequestMessage.Method}'ing {responseContentType}. {await GetResponseInfo(responseMessage)}");
                }

                string responseContent = await responseMessage.Content.ReadAsStringAsync();
                var responseObject = JsonConvert.DeserializeObject<TResponse>(responseContent);
                if (!responseObject.Success)
                {
                    throw new ApplicationException($"{responseMessage.RequestMessage.Method} {responseContentType} not successful. {await GetResponseInfo(responseMessage)}");
                }

                return responseObject.Data;
            }
            finally
            {
                responseMessage.Dispose();
            }
        }

        private static async Task<AuthenticationHeaderValue> GetAuthorizationHeader(
            PowerofficeApiSettings apiSettings,
            Guid clientKey)
        {
            using (var authenticationClient = new HttpClient())
            {
                authenticationClient.BaseAddress = apiSettings.AuthenticationBaseAddress;

                var tokensRequest = new HttpRequestMessage(HttpMethod.Post, $"{apiSettings.AuthenticationBaseAddress}OAuth/Token");
                tokensRequest.Content = new StringContent("grant_type=client_credentials", Encoding.UTF8, "application/x-www-form-urlencoded");
                string encodedTokens = StringUtilities.Base64Encode($"{apiSettings.ApplicationKey}:{clientKey}");
                tokensRequest.Headers.Authorization = new AuthenticationHeaderValue("Basic", encodedTokens);

                var tokensResponse = await authenticationClient.SendAsync(tokensRequest);
                if (!tokensResponse.IsSuccessStatusCode)
                {
                    string responseInfo = await GetResponseInfo(tokensResponse);
                    throw new ApplicationException($"Could not get access token. {responseInfo}");
                }

                var tokens = JsonConvert.DeserializeObject<AuthorizationTokens>(await tokensResponse.Content.ReadAsStringAsync());
                var authorizationHeader = new AuthenticationHeaderValue(tokens.TokenType, tokens.AccessToken);
                return authorizationHeader;
            }
        }

        private static async Task<string> GetResponseInfo(HttpResponseMessage response)
        {
            string content = await response.Content.ReadAsStringAsync();
            return $"Status code: {response.StatusCode}. Content: {content}.";
        }

        private static StringContent SerializeContent(object item)
        {
            return new StringContent(JsonConvert.SerializeObject(item), Encoding.UTF8, "application/json");
        }
    }
}