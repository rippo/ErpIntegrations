using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Webcrm.ErpIntegrations.Configurations;
using Webcrm.ErpIntegrations.Configurations.Models;
using Webcrm.ErpIntegrations.GeneralUtilities;

namespace Webcrm.ErpIntegrations.ApiClients.WebcrmApiClient
{
    public sealed class WebcrmClient
    {
        private WebcrmClient(
            ILogger logger,
            Uri baseApiUrl,
            HttpClient httpClient)
        {
            Logger = logger;
            SdkClient = new WebcrmSdk(httpClient);
            SdkClient.BaseUrl = baseApiUrl.AbsoluteUri;
        }

        internal static async Task<WebcrmClient> Create(
            ILogger logger,
            Uri baseApiUrl,
            string apiKey)
        {
            var webcrmClient = new WebcrmClient(logger, baseApiUrl, HttpClient);
            var tokensResponse = await webcrmClient.SdkClient.AuthApiLoginPostAsync(apiKey);
            webcrmClient.SdkClient.AccessToken = tokensResponse.Result.AccessToken;
            return webcrmClient;
        }

        private const int MaxPageSize = 1000;

        private static readonly HttpClient HttpClient = new HttpClient();
        private ILogger Logger { get; }
        private WebcrmSdk SdkClient { get; }

        public async Task<int> CreateDelivery(DeliveryDto delivery)
        {
            var response = await SdkClient.DeliveriesPostAsync(delivery);
            return response.Result;
        }

        public async Task CreateOrganisation(OrganisationDto organisation)
        {
            Logger.LogTrace("Creating organisation with name '{0}' in webCRM.", organisation.OrganisationName);
            await SdkClient.OrganisationsPostAsync(organisation);
        }

        public async Task CreatePerson(PersonDto person)
        {
            await SdkClient.PersonsPostAsync(person);
        }

        public async Task CreateProduct(QuotationLineLinkedDataItemDto product)
        {
            await SdkClient.QuotationLinesLinkedDataPostAsync(product);
        }

        public async Task CreateQuotationLine(QuotationLineDto quotationLine)
        {
            await SdkClient.QuotationLinesPostAsync(quotationLine);
        }

        public async Task DeleteQuotationLine(int quotationLineId)
        {
            await SdkClient.QuotationLinesByIdDeleteAsync(quotationLineId);
        }

        /// <summary>Get the organisation matching the specified field. Assumes that the field has been marked as unique. Returns null if the organisation was not found.</summary>
        public async Task<OrganisationDto> GetOrganisationByField(string fieldName, string fieldValue)
        {
            if (string.IsNullOrWhiteSpace(fieldName))
                throw new ArgumentNullException(nameof(fieldName));

            string selectOrganisationByCustomField = $@"
                SELECT *
                FROM Organisation
                WHERE {fieldName} = '{fieldValue}'";

            var organisationsResponse = await SdkClient.QueriesGetAsync(selectOrganisationByCustomField, 1, 1);
            if (organisationsResponse.Result.Count == 0)
            {
                return null;
            }

            var organisation = organisationsResponse.Result.First().MapToType<OrganisationDto>();
            return organisation;
        }

        public async Task<DeliveryDto> GetDeliveryByField(string fieldName, string fieldValue)
        {
            if (string.IsNullOrWhiteSpace(fieldName))
                throw new ArgumentNullException(nameof(fieldName));

            string selectDeliveryByCustomField = $@"
                SELECT *
                FROM Delivery
                WHERE {fieldName} = '{fieldValue}'";

            var deliveriesResponse = await SdkClient.QueriesGetAsync(selectDeliveryByCustomField, 1, 1);

            if (deliveriesResponse.Result.Count == 0)
                return null;

            var delivery = deliveriesResponse.Result.First().MapToType<DeliveryDto>();
            return delivery;
        }

        public async Task<List<QuotationLineDto>> GetDeliveryLines(int deliveryId)
        {
            var response = await SdkClient.DeliveriesByIdQuotationLinesGetAsync(deliveryId, 1, 1000);
            return response.Result.ToList();
        }

        public async Task<OrganisationDto> GetOrganisationById(int organisationId)
        {
            var organisationResponse = await SdkClient.OrganisationsByIdGetAsync(organisationId);
            return organisationResponse.Result;
        }

        /// <summary>Get the person matching the specified field. Assumes that the field has been marked as unique. Returns null if the person was not found.</summary>
        public async Task<PersonDto> GetPersonByField(string fieldName, string fieldValue)
        {
            if (string.IsNullOrWhiteSpace(fieldName))
                throw new ArgumentNullException(nameof(fieldName));

            string selectPersonByCustomField = $@"
                SELECT *
                FROM Person
                WHERE {fieldName} = '{fieldValue}'";

            var personResponse = await SdkClient.QueriesGetAsync(selectPersonByCustomField, 1, 1);
            if (personResponse.Result.Count == 0)
            {
                return null;
            }

            var person = personResponse.Result.First().MapToType<PersonDto>();
            return person;
        }

        public async Task<PersonDto> GetPersonById(int personId)
        {
            var personResponse = await SdkClient.PersonsByIdGetAsync(personId);
            return personResponse.Result;
        }

        private async Task<QuotationLineLinkedDataItemDto> GetProductById(int productId)
        {
            var response = await SdkClient.QuotationLinesLinkedDataByIdGetAsync(productId);
            return response.Result;
        }

        public async Task<QuotationLineLinkedDataItemDto> GetProductByField(string fieldName, string fieldValue)
        {
            if (string.IsNullOrWhiteSpace(fieldName))
                throw new ArgumentNullException(nameof(fieldName));

            string selectProductByCustomField = $@"
                SELECT *
                FROM LinkedData
                WHERE
                    LinkedDataItemEntityType = {(int)QuotationLineLinkedDataItemDtoQuotationLineLinkedDataItemEntityType.Product}
                    AND {fieldName} = '{fieldValue}'";

            var selectProductResponse = await SdkClient.QueriesGetAsync(selectProductByCustomField, 1, 1);
            if (selectProductResponse.Result.Count == 0)
            {
                return null;
            }

            var linkedItem = selectProductResponse.Result.First().MapToType<LinkedDataItemDto>();
            var product = await GetProductById(linkedItem.LinkedDataItemId);
            return product;
        }

        public async Task<List<QuotationLineDto>> GetQuotationLines(int deliveryId)
        {
            var response = await SdkClient.DeliveriesByIdQuotationLinesGetAsync(deliveryId, 1, MaxPageSize);
            return response.Result.ToList();
        }

        public async Task<List<DeliveryDto>> GetUpsertedDeliveries(DateTime upsertedAfterUtc)
        {
            string selectUpsertedDeliveries = $@"
                SELECT *
                FROM Delivery
                WHERE DeliveryUpdatedAt >= '{upsertedAfterUtc:O}'";

            var upsertedDeliveriesResponse = await SdkClient.QueriesGetAsync(selectUpsertedDeliveries, 1, MaxPageSize);

            var upsertedDeliveries = upsertedDeliveriesResponse.Result
                .Select(ObjectExtension.MapToType<DeliveryDto>)
                .ToList();

            return upsertedDeliveries;
        }

        /// <summary>Returns updated or inserted organisation where OrganisationStatus is on of the values in `acceptedStatuses` and OrganisationType is one of the values in `acceptedTypes`. Both `acceptedStatuses` and `acceptedTypes` may be `null`.</summary>
        public async Task<List<OrganisationDto>> GetUpsertedOrganisations(
            DateTime upsertedAfterUtc,
            IEnumerable<string> acceptedStatuses,
            IEnumerable<string> acceptedTypes)
        {
            string selectUpsertedOrganisations = $@"
                SELECT *
                FROM Organisation
                WHERE OrganisationUpdatedAt >= '{upsertedAfterUtc:o}'";

            if (acceptedStatuses != null && acceptedStatuses.Any())
            {
                string joinedStatuses = string.Join(" OR ", acceptedStatuses.Select(status => $"OrganisationStatus = '{status}'"));
                selectUpsertedOrganisations += $" AND ({joinedStatuses})";
            }

            if (acceptedTypes != null && acceptedTypes.Any())
            {
                string joinedTypes = string.Join(" OR ", acceptedTypes.Select(type => $"OrganisationType = '{type}'"));
                selectUpsertedOrganisations += $" AND ({joinedTypes})";
            }

            var upsertedOrganisationsResponse = await SdkClient.QueriesGetAsync(selectUpsertedOrganisations, 1, MaxPageSize);

            var upsertedOrganisations = upsertedOrganisationsResponse.Result
                .Select(ObjectExtension.MapToType<OrganisationDto>)
                .ToList();

            return upsertedOrganisations;
        }

        public async Task<List<PersonDto>> GetUpsertedPersons(
            DateTime upsertedAfterUtc,
            BaseConfiguration configuration)
        {
            string selectUpsertedPersons = $@"
                SELECT *
                FROM Person
                JOIN Organisation ON (Person.PersonOrganisationId = Organisation.OrganisationId)
                WHERE PersonUpdatedAt >= '{upsertedAfterUtc:O}'
                    AND {configuration.PrimaryContactCheckboxFieldName} = '{Constants.CustomCheckboxFieldCheckedValue}'";

            if (configuration.AcceptedOrganisationStatuses != null && configuration.AcceptedOrganisationStatuses.Any())
            {
                string joinedStatuses = string.Join(" OR ", configuration.AcceptedOrganisationStatuses.Select(status => $"OrganisationStatus = '{status}'"));
                selectUpsertedPersons += $" AND ({joinedStatuses})";
            }

            if (configuration.AcceptedOrganisationTypes != null && configuration.AcceptedOrganisationTypes.Any())
            {
                string joinedTypes = string.Join(" OR ", configuration.AcceptedOrganisationTypes.Select(type => $"OrganisationType = '{type}'"));
                selectUpsertedPersons += $" AND ({joinedTypes})";
            }

            var upsertedPersonsResponse = await SdkClient.QueriesGetAsync(selectUpsertedPersons, 1, MaxPageSize);

            var upsertedPersons = upsertedPersonsResponse.Result
                .Select(ObjectExtension.MapToType<PersonDto>)
                .ToList();

            return upsertedPersons;
        }

        public async Task UpdateDelivery(DeliveryDto delivery)
        {
            Logger.LogTrace("Updating delivery with ID {0} in webCRM.", delivery.DeliveryId);

            // Let webCRM set the delivery number. Assumes that 'Auto next number' is turned on.
            delivery.DeliveryNumber = null;
            await SdkClient.DeliveriesByIdPutAsync(delivery.DeliveryId, delivery);
        }

        public async Task UpdateOrganisation(OrganisationDto organisation)
        {
            Logger.LogTrace("Updating organisation with id {0} in webCRM.", organisation.OrganisationId);
            await SdkClient.OrganisationsByIdPutAsync(organisation.OrganisationId, organisation);
        }

        public async Task UpdatePerson(PersonDto person)
        {
            await SdkClient.PersonsByIdPutAsync(person.PersonId, person);
        }

        public async Task UpdateProduct(QuotationLineLinkedDataItemDto product)
        {
            var response = await SdkClient.QuotationLinesLinkedDataByIdPutAsync(product.QuotationLineLinkedDataItemId, product);
            if (response.StatusCode != (int)HttpStatusCode.OK)
            {
                throw new ApplicationException($"Error updating quotation lined linked data item. {response.StatusCode}.");
            }
        }
    }
}