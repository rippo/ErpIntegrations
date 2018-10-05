using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Webcrm.ErpIntegrations.ApiClients.PowerofficeApiClient;
using Webcrm.ErpIntegrations.ApiClients.PowerofficeApiClient.Models;
using Webcrm.ErpIntegrations.ApiClients.WebcrmApiClient;
using Webcrm.ErpIntegrations.Configurations.Models;

namespace Webcrm.ErpIntegrations.Synchronisation
{
    /// <summary>Can copy organisations and persons between PowerOffice and webCRM. Items that don't have changes to any of the properties are not copied.</summary>
    internal sealed class PowerofficeDataCopier
    {
        /// <remarks>All the copy methods log a single entry containing the word "copying" and "PowerOffice" so that it is possible to filter them out.</remarks>
        private PowerofficeDataCopier(
            ILogger logger,
            PowerofficeConfiguration configuration,
            PowerofficeClient powerofficeClient,
            WebcrmClient webcrmClient)
        {
            Logger = logger;
            Configuration = configuration;
            PowerofficeClient = powerofficeClient;
            WebcrmClient = webcrmClient;
        }

        public static async Task<PowerofficeDataCopier> Create(
            ILogger logger,
            WebcrmClientFactory webcrmClientFactory,
            PowerofficeClientFactory powerofficeClientFactory,
            PowerofficeConfiguration configuration)
        {
            var powerofficeClient = await powerofficeClientFactory.Create(configuration.PowerofficeClientKey);
            var webcrmClient = await webcrmClientFactory.Create(configuration.WebcrmApiKey);
            return new PowerofficeDataCopier(logger, configuration, powerofficeClient, webcrmClient);
        }

        private ILogger Logger { get; }
        private PowerofficeConfiguration Configuration { get; }
        private PowerofficeClient PowerofficeClient { get; }
        private WebcrmClient WebcrmClient { get; }

        public async Task CopyDeliveryFromPoweroffice(OutgoingInvoice powerofficeDelivery)
        {
            var matchingWebcrmDelivery = await WebcrmClient.GetDeliveryByField(Configuration.DeliveryIdFieldName, powerofficeDelivery.Id.ToString());

            // Creating or updating a delivery in webCRM requires multiple calls to the API. If one of the call fails, the queue message will be retried, and this should make the incorrect state very temporary.
            int webcrmDeliveryId;
            int webcrmOrganisationId;
            if (matchingWebcrmDelivery == null)
            {
                if (!powerofficeDelivery.CustomerCode.HasValue)
                {
                    Logger.LogInformation("The PowerOffice delivery is not associated with a customer, so we cannot create it in webCRM.");
                    return;
                }

                // PowerOffice deliveries are associated with the Code of the organisation and not the Id.
                var webcrmOrganisation = await WebcrmClient.GetOrganisationByField(Configuration.OrganisationNumberFieldName, powerofficeDelivery.CustomerCode.ToString());

                if (webcrmOrganisation == null)
                    throw new ApplicationException($"Not copying the delivery to webCRM because no organisation found with the PowerOffice code {powerofficeDelivery.CustomerCode}.");

                Logger.LogTrace("Copying delivery from PowerOffice, creating a new one.");

                webcrmOrganisationId = webcrmOrganisation.OrganisationId;

                var newWebcrmDelivery = new DeliveryDto(powerofficeDelivery, webcrmOrganisation.OrganisationId, Configuration);
                webcrmDeliveryId = await WebcrmClient.CreateDelivery(newWebcrmDelivery);
            }
            else
            {
                if (!matchingWebcrmDelivery.DeliveryOrganisationId.HasValue)
                    throw new ApplicationException("Not copying the delivery to webCRM because the matching webCRM delivery does not have a DeliveryOrganisationId.");

                // Not comparing the two deliveries, since we're only synchronising deliveries one way.

                Logger.LogTrace("Copying delivery from PowerOffice, updating an existing one.");

                webcrmOrganisationId = matchingWebcrmDelivery.DeliveryOrganisationId.Value;

                // We cannot delete a delivery through the API, so we delete the associated delivery lines, but modify the delivery.
                var lines = await WebcrmClient.GetQuotationLines(matchingWebcrmDelivery.DeliveryId);
                Logger.LogTrace($"Deleting {lines.Count} associated quotation lines.");
                foreach (var line in lines)
                    await WebcrmClient.DeleteQuotationLine(line.QuotationLineId);

                matchingWebcrmDelivery.Update(powerofficeDelivery, matchingWebcrmDelivery.DeliveryOrganisationId.Value, Configuration);
                await WebcrmClient.UpdateDelivery(matchingWebcrmDelivery);
                webcrmDeliveryId = matchingWebcrmDelivery.DeliveryId;
            }

            Logger.LogTrace($"Copying {powerofficeDelivery.OutgoingInvoiceLines.Count} quotation lines from PowerOffice.");
            const int millisecondsDelayBetweenCalls = 20;
            foreach (var powerofficeLine in powerofficeDelivery.OutgoingInvoiceLines)
            {
                Logger.LogTrace("Copying quotation lines from PowerOffice.");
                var powerofficeProduct = await PowerofficeClient.GetProductByCode(powerofficeLine.ProductCode);
                var webcrmLine = new QuotationLineDto(
                    powerofficeLine,
                    powerofficeProduct,
                    webcrmDeliveryId,
                    webcrmOrganisationId,
                    Configuration);

                await WebcrmClient.CreateQuotationLine(webcrmLine);

                await Task.Delay(millisecondsDelayBetweenCalls);
            }
        }

        public async Task CopyDeliveryToPoweroffice(
            DeliveryDto webcrmDelivery,
            List<QuotationLineDto> webcrmDeliveryLines)
        {
            Guid? powerofficeDeliveryId = webcrmDelivery.GetPowerofficeDeliveryId(Configuration.DeliveryIdFieldName);
            if (powerofficeDeliveryId == null)
            {
                long? powerofficeCustomerCode = await GetPowerofficeCustomerCode(webcrmDelivery);
                if (!powerofficeCustomerCode.HasValue)
                {
                    Logger.LogInformation($"Not copying webCRM delivery with ID {webcrmDelivery.DeliveryId} to PowerOffice because the corresponding PowerOffice organisation could not be found.");
                    return;
                }

                Logger.LogTrace("Copying delivery to PowerOffice, creating a new one.");
                var newPowerofficeDelivery = new NewOutgoingInvoice(webcrmDelivery, webcrmDeliveryLines, powerofficeCustomerCode.Value, Configuration.ProductIdFieldName);
                var createdPowerofficeDelivery = await PowerofficeClient.CreateInvoice(newPowerofficeDelivery);
                webcrmDelivery.SetPowerofficeDeliveryId(Configuration.DeliveryIdFieldName, createdPowerofficeDelivery.Id);
                await WebcrmClient.UpdateDelivery(webcrmDelivery);

                if (createdPowerofficeDelivery.OutgoingInvoiceLines.Count != webcrmDeliveryLines.Count)
                    throw new ApplicationException($"Sanity check failed: Expected the same number of lines in newly created PowerOffice delivery. Lines in webCRM delivery: {webcrmDeliveryLines.Count}. Lines in PowerOffice delivery: {createdPowerofficeDelivery.OutgoingInvoiceLines.Count}.");
            }
            else
            {
                var matchingPowerofficeDelivery = await PowerofficeClient.GetInvoice(powerofficeDeliveryId.Value);
                if (matchingPowerofficeDelivery == null)
                {
                    Logger.LogWarning($"Could not find PowerOffice delivery (invoice) with id {powerofficeDeliveryId.Value}. Not copying the delivery to PowerOffice.");
                    return;
                }

                // Not comparing the two deliveries, since we're only synchronising deliveries one way.

                long? powerofficeCustomerCode = await GetPowerofficeCustomerCode(webcrmDelivery);
                if (!powerofficeCustomerCode.HasValue)
                {
                    Logger.LogInformation($"Not copying webCRM delivery with ID {webcrmDelivery.DeliveryId} to PowerOffice because the corresponding PowerOffice organisation could not be found.");
                    return;
                }

                Logger.LogTrace("Copying delivery to PowerOffice, updating an existing one.");
                matchingPowerofficeDelivery.UpdateIncludingLines(webcrmDelivery, webcrmDeliveryLines, powerofficeCustomerCode.Value, Configuration.ProductCodeFieldName);
                await PowerofficeClient.UpdateInvoice(matchingPowerofficeDelivery);
            }
        }

        public async Task CopyOrganisationFromPoweroffice(
            Customer powerofficeOrganisation)
        {
            var matchingWebcrmOrganisation = await WebcrmClient.GetOrganisationByField(Configuration.OrganisationIdFieldName, powerofficeOrganisation.Id.ToString());

            if (matchingWebcrmOrganisation == null)
            {
                Logger.LogTrace("Copying organisation to PowerOffice, creating a new one.");
                var newWebcrmOrganisation = new OrganisationDto(powerofficeOrganisation, Configuration);
                await WebcrmClient.CreateOrganisation(newWebcrmOrganisation);
            }
            else
            {
                if (!matchingWebcrmOrganisation.ShouldSynchronise(Configuration))
                {
                    Logger.LogTrace("Not copying organisation from PowerOffice because the matching webCRM organisation should not be synchronised.");
                    return;
                }

                if (!powerofficeOrganisation.HasChangesRelevantToWebcrm(matchingWebcrmOrganisation, Configuration))
                {
                    Logger.LogTrace("Not copying organisation from PowerOffice because it does not have any relevant changes.");
                    return;
                }

                Logger.LogTrace("Copying organisation from PowerOffice, updating an existing one.");
                matchingWebcrmOrganisation.Update(powerofficeOrganisation, Configuration);
                await WebcrmClient.UpdateOrganisation(matchingWebcrmOrganisation);
            }
        }

        public async Task CopyOrganisationToPoweroffice(
            OrganisationDto webcrmOrganisation)
        {
            long? powerofficeOrganisationId = webcrmOrganisation.GetPowerofficeOrganisationId(Configuration.OrganisationIdFieldName);
            if (powerofficeOrganisationId == null)
            {
                Logger.LogTrace("Copying organisation to PowerOffice, creating a new one.");
                var newPowerofficeOrganisation = new NewCustomer(webcrmOrganisation);
                var createdPowerofficeOrganisation = await PowerofficeClient.CreateCustomer(newPowerofficeOrganisation);
                webcrmOrganisation.SetPowerofficeOrganisationId(Configuration.OrganisationIdFieldName, createdPowerofficeOrganisation.Id);
                await WebcrmClient.UpdateOrganisation(webcrmOrganisation);
            }
            else
            {
                var matchingPowerofficeOrganisation = await PowerofficeClient.GetCustomer(powerofficeOrganisationId.Value);
                if (matchingPowerofficeOrganisation == null)
                {
                    Logger.LogWarning($"Could not find PowerOffice organisation (customer) with id {powerofficeOrganisationId.Value}.");
                    return;
                }

                if (!webcrmOrganisation.HasChangesRelevantToPoweroffice(matchingPowerofficeOrganisation, Configuration))
                {
                    Logger.LogTrace("Not copying organisation to PowerOffice because it does not have any relevant changes.");
                    return;
                }

                Logger.LogTrace("Copying organisation to PowerOffice, updating an existing one.");
                matchingPowerofficeOrganisation.Update(webcrmOrganisation);
                await PowerofficeClient.UpdateCustomer(matchingPowerofficeOrganisation);
            }
        }

        public async Task CopyPersonFromPoweroffice(
            ContactPerson powerofficeContactPerson,
            long powerofficeOrganisationId)
        {
            var webcrmOrganisation = await WebcrmClient.GetOrganisationByField(Configuration.OrganisationIdFieldName, powerofficeOrganisationId.ToString());

            if (webcrmOrganisation == null)
            {
                // We don't know if the missing organisation is not being synchronized or simply hasn't been synchronized yet. Throwing an exception to put this person message back on the queue. This might result in false positives on the poison queue, but will make the system handle the out-of-order execution that might happen when multiple messages are dequeued simultaneously.
                throw new ApplicationException($"Could not find matching organisation in webCRM where field name '{Configuration.OrganisationIdFieldName}' has the value '{powerofficeOrganisationId}'. Cannot update upserted person with PowerOffice id {powerofficeOrganisationId}/{powerofficeContactPerson.Id}.");
            }

            if (!webcrmOrganisation.ShouldSynchronise(Configuration))
            {
                Logger.LogTrace("Not copying person from PowerOffice, because the associated organisation should not be synchronised.");
                return;
            }

            var personKey = new PowerofficePersonKey(powerofficeOrganisationId, powerofficeContactPerson.Id);
            var matchingWebcrmPerson = await WebcrmClient.GetPersonByField(Configuration.PersonIdFieldName, personKey.ToString());

            if (matchingWebcrmPerson == null)
            {
                Logger.LogTrace("Copying person from PowerOffice, creating a new one.");
                var newWebcrmPerson = new PersonDto();
                newWebcrmPerson.Update(Logger, powerofficeContactPerson, powerofficeOrganisationId, webcrmOrganisation.OrganisationId, Configuration);
                await WebcrmClient.CreatePerson(newWebcrmPerson);
            }
            else
            {
                if (!powerofficeContactPerson.HasRelevantChanges(matchingWebcrmPerson))
                {
                    Logger.LogTrace("Not copying person from PowerOffice because it does not have any relevant changes.");
                    return;
                }

                Logger.LogTrace("Copying person from PowerOffice, updating an existing one.");
                matchingWebcrmPerson.Update(Logger, powerofficeContactPerson, powerofficeOrganisationId, webcrmOrganisation.OrganisationId, Configuration);
                await WebcrmClient.UpdatePerson(matchingWebcrmPerson);
            }
        }

        public async Task CopyPersonToPoweroffice(
            PersonDto webcrmPerson)
        {
            var personKey = webcrmPerson.GetPowerofficePersonKey(Logger, Configuration.PersonIdFieldName);
            if (personKey == null)
            {
                long? powerofficeOrganisationId = await GetPowerofficeOrganisationId(webcrmPerson);
                if (powerofficeOrganisationId == null)
                    throw new ApplicationException("Cannot create person in PowerOffice without an associated organisation.");

                Logger.LogTrace("Copying person to PowerOffice, creating a new one.");
                var newPowerofficePerson = new NewContactPerson();
                newPowerofficePerson.Update(webcrmPerson);
                var createdPowerofficePerson = await PowerofficeClient.CreateContactPerson(powerofficeOrganisationId.Value, newPowerofficePerson);

                personKey = new PowerofficePersonKey(powerofficeOrganisationId.Value, createdPowerofficePerson.Id);
                webcrmPerson.SetPowerofficePersonKey(Configuration.PersonIdFieldName, personKey);
                await WebcrmClient.UpdatePerson(webcrmPerson);
            }
            else
            {
                var matchingPowerofficePerson = await PowerofficeClient.GetContactPerson(personKey.PowerofficeOrganisationId, personKey.PowerofficePersonId);

                if (matchingPowerofficePerson == null)
                {
                    Logger.LogWarning($"Could not find PowerOffice person (contact person) with key {personKey}.");
                    return;
                }

                if (!matchingPowerofficePerson.HasRelevantChanges(webcrmPerson))
                {
                    Logger.LogTrace("Not copying person to PowerOffice because it does not have any relevant changes.");
                    return;
                }

                Logger.LogTrace("Copying person to PowerOffice, updating an existing one.");
                matchingPowerofficePerson.Update(webcrmPerson);
                await PowerofficeClient.UpdateContactPerson(personKey.PowerofficeOrganisationId, matchingPowerofficePerson);
            }
        }

        public async Task CopyProductFromPoweroffice(
            Product powerofficeProduct)
        {
            var matchingWebcrmProduct = await WebcrmClient.GetProductByField(Configuration.ProductIdFieldName, powerofficeProduct.Id.ToString());
            string powerofficeProductGroupName = await GetProductGroupName(powerofficeProduct.ProductGroupId);

            if (matchingWebcrmProduct == null)
            {
                Logger.LogTrace("Copying product from PowerOffice, creating a new one.");
                var newProduct = new QuotationLineLinkedDataItemDto();
                newProduct.Update(powerofficeProduct, powerofficeProductGroupName, Configuration);
                await WebcrmClient.CreateProduct(newProduct);
            }
            else
            {
                // No need to verify if there are relevant changes, since we are only synchronising products in one direction.

                Logger.LogTrace("Copying product from PowerOffice, updating an existing one.");
                matchingWebcrmProduct.Update(powerofficeProduct, powerofficeProductGroupName, Configuration);
                await WebcrmClient.UpdateProduct(matchingWebcrmProduct);
            }
        }

        private async Task<string> GetProductGroupName(long? powerofficeProductGroupId)
        {
            if (!powerofficeProductGroupId.HasValue)
                return string.Empty;

            var powerofficeProductGroup = await PowerofficeClient.GetProductGroup(powerofficeProductGroupId.Value);

            if (powerofficeProductGroup == null)
                return string.Empty;

            return powerofficeProductGroup.Name;
        }

        // The internal methods are only used by the tests.

        internal async Task CopyOrganisationFromPoweroffice(
            long powerofficeOrganisationId)
        {
            var powerofficeOrganisation = await PowerofficeClient.GetCustomer(powerofficeOrganisationId);
            await CopyOrganisationFromPoweroffice(powerofficeOrganisation);
        }

        public async Task CopyOrganisationToPoweroffice(
            int webcrmOrganisationId)
        {
            var webcrmOrganisation = await WebcrmClient.GetOrganisationById(webcrmOrganisationId);
            await CopyOrganisationToPoweroffice(webcrmOrganisation);
        }

        internal async Task CopyPersonFromPoweroffice(
            long powerofficeOrganisationId,
            long powerofficePersonId)
        {
            var contactPerson = await PowerofficeClient.GetContactPerson(powerofficeOrganisationId, powerofficePersonId);
            await CopyPersonFromPoweroffice(contactPerson, powerofficeOrganisationId);
        }

        internal async Task CopyPersonToPoweroffice(
            int webcrmPersonId)
        {
            var webcrmPerson = await WebcrmClient.GetPersonById(webcrmPersonId);
            await CopyPersonToPoweroffice(webcrmPerson);
        }

        /// <summary>Returns the PowerOffice customer code of the organisation that this delivery is associated with. Returns `null` if the code couldn't be found. This can happen if the organisation doesn't exist in PowerOffice.</summary>
        private async Task<long?> GetPowerofficeCustomerCode(
            DeliveryDto webcrmDelivery)
        {
            if (!webcrmDelivery.DeliveryOrganisationId.HasValue)
                throw new ApplicationException("Cannot get PowerOffice organisation ID since DeliveryOrganisationId is null.");

            var webcrmOrganisation = await WebcrmClient.GetOrganisationById(webcrmDelivery.DeliveryOrganisationId.Value);
            long? powerofficeOrganisationId = webcrmOrganisation.GetPowerofficeOrganisationId(Configuration.OrganisationIdFieldName);

            if (!powerofficeOrganisationId.HasValue)
                return null;

            var powerofficeOrganisation = await PowerofficeClient.GetCustomer(powerofficeOrganisationId.Value);

            if (powerofficeOrganisation == null)
                throw new ApplicationException($"Could not find a PowerOffice organisation with ID {powerofficeOrganisationId.Value}.");

            if (!powerofficeOrganisation.Code.HasValue)
                throw new ApplicationException("PowerOffice organisation code is null.");

            return powerofficeOrganisation.Code.Value;
        }

        private async Task<long?> GetPowerofficeOrganisationId(
            PersonDto person)
        {
            // It's safe to use .Value because all webCRM persons are associated with an organisation.
            // ReSharper disable once PossibleInvalidOperationException
            var webcrmOrganisation = await WebcrmClient.GetOrganisationById(person.PersonOrganisationId.Value);
            long? powerofficeOrganisationId = webcrmOrganisation.GetPowerofficeOrganisationId(Configuration.OrganisationIdFieldName);
            return powerofficeOrganisationId;
        }
    }
}