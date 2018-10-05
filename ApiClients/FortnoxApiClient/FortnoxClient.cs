using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Webcrm.ErpIntegrations.ApiClients.FortnoxApiClient.Models;
using Webcrm.ErpIntegrations.GeneralUtilities;

namespace Webcrm.ErpIntegrations.ApiClients.FortnoxApiClient
{
    public class FortnoxClient : IDisposable
    {
        public FortnoxClient(FortnoxApiKeys fortnoxApiKeys)
        {
            Client = new HttpClient
            {
                DefaultRequestHeaders =
                {
                    { "Access-Token", fortnoxApiKeys.AccessToken },
                    { "Client-Secret", fortnoxApiKeys.ClientSecret }
                }
            };
        }

        public void Dispose()
        {
            Client?.Dispose();
        }

        private const string BaseApiUrl = "https://api.fortnox.se/3/";
        private HttpClient Client { get; }
        private const int PageSize = 100;

        public async Task<Customer> CreateCustomer(Customer newFortnoxCustomer)
        {
            var customerContent = SerialiseCustomerContent(newFortnoxCustomer);
            var request = new HttpRequestMessage
            {
                Content = customerContent,
                Method = HttpMethod.Post,
                RequestUri = new Uri($"{BaseApiUrl}customers")
            };
            var response = await Client.SendAsync(request);

            var customerResponse = await Deserializer<CustomerResponse>.DeserializeAndVerify(response, "Create Customer");

            return customerResponse.Customer;
        }

        public async Task<FortnoxInvoice> CreateInvoice(NewFortnoxInvoice newFortnoxInvoice)
        {
            var invoiceContent = SerialiseInvoiceContent(newFortnoxInvoice);
            var request = new HttpRequestMessage
            {
                Content = invoiceContent,
                Method = HttpMethod.Post,
                RequestUri = new Uri($"{BaseApiUrl}invoices")
            };
            var response = await Client.SendAsync(request);

            var invoiceResponse = await Deserializer<InvoiceResponse>.DeserializeAndVerify(response, "Create Invoice");

            return invoiceResponse.Invoice;
        }

        public async Task<List<FilteredCustomer>> GetActiveCustomers()
        {
            int currentPageNumber = 1;
            int numberOfPages;
            var filterCustomerList = new List<FilteredCustomer>();

            do
            {
                var response = await GetActiveCustomers(currentPageNumber);
                filterCustomerList.AddRange(response.Customers);

                numberOfPages = response.MetaInformation.TotalPages;
                currentPageNumber++;
            } while (currentPageNumber <= numberOfPages);

            return filterCustomerList;
        }

        public async Task<Customer> GetCustomer(string customerNumber)
        {
            string requestUri = $"{BaseApiUrl}customers/{customerNumber}";
            var response = await Client.GetAsync(requestUri);
            var customerResponse = await Deserializer<CustomerResponse>.DeserializeAndVerify(response, $"Get Customer {customerNumber}");
            return customerResponse.Customer;
        }

        public async Task<List<FilteredCustomer>> GetRecentlyUpsertedCustomers(DateTime dateSinceUpserted)
        {
            int currentPageNumber = 1;
            var filteredCustomerList = new List<FilteredCustomer>();
            int numberOfPages;

            do
            {
                var response = await GetRecentlyUpsertedCustomers(dateSinceUpserted, currentPageNumber);
                filteredCustomerList.AddRange(response.Customers);

                numberOfPages = response.MetaInformation.TotalPages;
                currentPageNumber++;
            } while (currentPageNumber <= numberOfPages);

            return filteredCustomerList;
        }

        public async Task<List<FilteredInvoice>> GetRecentlyUpsertedInvoices(DateTime dateSinceUpserted)
        {
            int currentPageNumber = 1;
            int numberOfPages;
            var filteredInvoiceList = new List<FilteredInvoice>();

            do
            {
                var response = await GetRecentlyUpsertedInvoices(dateSinceUpserted, currentPageNumber);
                filteredInvoiceList.AddRange(response.Invoices);

                numberOfPages = response.MetaInformation.TotalPages;
                currentPageNumber++;
            } while (currentPageNumber <= numberOfPages);

            return filteredInvoiceList;
        }

        public async Task UpdateCustomer(Customer customer)
        {
            var payload = new { Customer = customer };
            var payloadJson = JsonConvert.SerializeObject(payload);
            var content = new StringContent(payloadJson, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage
            {
                Content = content,
                Method = HttpMethod.Put,
                RequestUri = new Uri($"{BaseApiUrl}customers/{customer.CustomerNumber}")
            };
            var response = await Client.SendAsync(request);

            await Deserializer<CustomerResponse>.DeserializeAndVerify(response, "Update Customer");
        }

        private async Task<FilteredCustomersResponse> GetActiveCustomers(int page)
        {
            var customersRequestMessage = GetCustomersRequestMessage(page);
            var response = await Client.SendAsync(customersRequestMessage);
            var customers = await Deserializer<FilteredCustomersResponse>.DeserializeAndVerify(response, "Active Customers");
            return customers;
        }

        private HttpRequestMessage GetCustomersRequestMessage(int page)
        {
            var requestUri = GetCustomersRequestUri(page);
            var message = new HttpRequestMessage
            {
                RequestUri = requestUri,
                Method = HttpMethod.Get
            };

            return message;
        }

        private HttpRequestMessage GetCustomersRequestMessage(DateTime dateSinceUpserted, int page)
        {
            var requestUri = GetCustomersRequestUri(page);
            requestUri.AddQueryParameter("lastmodified", $"{dateSinceUpserted:yyyy-MM-dd HH:mm}");

            var message = new HttpRequestMessage
            {
                RequestUri = requestUri,
                Method = HttpMethod.Get
            };

            return message;
        }

        private Uri GetCustomersRequestUri(int page)
        {
            var request = new Uri($"{BaseApiUrl}customers");
            request.AddQueryParameter("filter", "active");
            request.AddQueryParameter("sortby", "customernumber");
            request.AddQueryParameter("sortorder", "ascending");
            request.AddQueryParameter("limit", PageSize.ToString());
            request.AddQueryParameter("page", page.ToString());
            return request;
        }

        private HttpRequestMessage GetInvoicesRequest(DateTime dateSinceUpserted, int page)
        {
            var requestUri = new Uri($"{BaseApiUrl}invoices");
            requestUri.AddQueryParameter("sortby", "documentnumber");
            requestUri.AddQueryParameter("sortorder", "ascending");
            requestUri.AddQueryParameter("limit", PageSize.ToString());
            requestUri.AddQueryParameter("page", page.ToString());
            requestUri.AddQueryParameter("lastmodified", $"{dateSinceUpserted:yyyy-MM-dd HH:mm}");

            var message = new HttpRequestMessage
            {
                RequestUri = requestUri,
                Method = HttpMethod.Get
            };

            return message;
        }

        private async Task<FilteredCustomersResponse> GetRecentlyUpsertedCustomers(DateTime dateSinceUpserted, int page)
        {
            var customersRequestMessage = GetCustomersRequestMessage(dateSinceUpserted, page);
            var response = await Client.SendAsync(customersRequestMessage);
            var customers = await Deserializer<FilteredCustomersResponse>.DeserializeAndVerify(response, "Recently Upserted Customers");
            return customers;
        }

        private async Task<FilteredInvoicesResponse> GetRecentlyUpsertedInvoices(DateTime dateSinceUpserted, int page)
        {
            var invoicesRequestMessage = GetInvoicesRequest(dateSinceUpserted, page);
            var response = await Client.SendAsync(invoicesRequestMessage);
            var invoices = await Deserializer<FilteredInvoicesResponse>.DeserializeAndVerify(response, "Recently Upserted Invoices");
            return invoices;
        }

        private StringContent SerialiseCustomerContent(Customer newFortnoxCustomer)
        {
            var payload = new { Customer = newFortnoxCustomer };
            string payloadJson = JsonConvert.SerializeObject(payload);
            return new StringContent(payloadJson, Encoding.UTF8, "application/json");
        }

        private StringContent SerialiseInvoiceContent(NewFortnoxInvoice newFortnoxInvoice)
        {
            var payload = new { Invoice = newFortnoxInvoice };
            string payloadJson = JsonConvert.SerializeObject(payload);
            return new StringContent(payloadJson, Encoding.UTF8, "application/json");
        }
    }
}