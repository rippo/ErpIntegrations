using Microsoft.Azure.Documents;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Webcrm.ErpIntegrations.Configurations.Models
{
    public abstract class BaseConfiguration : Resource
    {
        protected BaseConfiguration()
        {
            AcceptedOrganisationStatuses = new string[0];
            AcceptedOrganisationTypes = new string[0];
            Disabled = false;
            LastSuccessfulCopyFromErpHeartbeat = DateTime.UtcNow;
            LastSuccessfulCopyToErpHeartbeat = DateTime.UtcNow;
            SynchroniseDeliveries = SynchroniseDeliveries.Off;
            SynchroniseProducts = false;
        }

        public IEnumerable<string> AcceptedOrganisationStatuses { get; set; }

        public IEnumerable<string> AcceptedOrganisationTypes { get; set; }

        /// <summary>The name of the field in webCRM that is used to store the ID of the corresponding delivery in the ERP system, e.g. "DeliveryCustom5".</summary>
        public string DeliveryIdFieldName { get; set; }

        /// <summary>Disabled looking for changes in this system. Any current messages on the queue will load.</summary>
        public bool Disabled { get; set; }

        [JsonIgnore]
        public string FirstAcceptedOrganisationStatus => AcceptedOrganisationStatuses?.FirstOrDefault();

        [JsonIgnore]
        public string FirstAcceptedOrganisationType => AcceptedOrganisationTypes?.FirstOrDefault();

        public DateTime LastSuccessfulCopyFromErpHeartbeat { get; set; }

        public DateTime LastSuccessfulCopyToErpHeartbeat { get; set; }

        /// <summary>The name of the field in webCRM that is used to store the ID (the internal system ID) of the corresponding organisation in the ERP system, e.g. "OrganisationExtraCustom8".</summary>
        public string OrganisationIdFieldName { get; set; }

        /// <summary>The name of the field in webCRM that is used to determine if a contact is the primary contact. Only primary contacts are synchronised with PowerOffice. The type of the field has to be set to Checkbox. Example: "PersonCustom4".</summary>
        public string PrimaryContactCheckboxFieldName { get; set; }

        /// <summary>Turn on synchronising deliveries in this system.</summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public SynchroniseDeliveries SynchroniseDeliveries { get; set; }

        /// <summary>Turn on synchronising products in this system.</summary>
        public bool SynchroniseProducts { get; set; }

        public string WebcrmApiKey { get; set; }

        [JsonProperty(PropertyName = "id")]
        public string WebcrmSystemId { get; set; }

        /// <summary>Returns a list of errors messages. Returns an empty list if there aren't any issues.</summary>
        protected List<string> GetBaseValidationErrorMessages()
        {
            var errorMessages = new List<string>();

            if (AcceptedOrganisationStatuses == null)
            {
                errorMessages.Add("AcceptedOrganisationStatuses isn't defined.");
            }
            else
            {
                errorMessages.AddRange(AcceptedOrganisationStatuses
                    .Where(string.IsNullOrWhiteSpace)
                    .Select(status => $"'{status}' is not a valid value in AcceptedOrganisationStatuses."));
            }

            if (AcceptedOrganisationTypes == null)
            {
                errorMessages.Add("AcceptedOrganisationTypes isn't defined.");
            }
            else
            {
                errorMessages.AddRange(AcceptedOrganisationTypes
                    .Where(string.IsNullOrWhiteSpace)
                    .Select(type => $"'{type}' is not a valid value in AcceptedOrganisationTypes."));
            }

            if (string.IsNullOrWhiteSpace(FirstAcceptedOrganisationStatus) && string.IsNullOrWhiteSpace(FirstAcceptedOrganisationType))
            {
                errorMessages.Add("Either AcceptedOrganisationStatuses or AcceptedOrganisationTypes has to contain at least one value.");
            }

            if (string.IsNullOrWhiteSpace(DeliveryIdFieldName))
                errorMessages.Add("DeliveryIdFieldName isn't defined.");

            if (string.IsNullOrWhiteSpace(OrganisationIdFieldName))
                errorMessages.Add("OrganisationIdFieldName isn't defined.");

            if (string.IsNullOrWhiteSpace(PrimaryContactCheckboxFieldName))
                errorMessages.Add("PrimaryContactCheckboxFieldName isn't defined.");

            if (string.IsNullOrWhiteSpace(WebcrmApiKey))
                errorMessages.Add("OrganisationIdFieldName isn't defined.");

            if (string.IsNullOrWhiteSpace(WebcrmSystemId))
                errorMessages.Add("WebcrmSystemId isn't defined.");

            return errorMessages;
        }
    }
}