using Newtonsoft.Json;
using Webcrm.ErpIntegrations.ApiClients.WebcrmApiClient;
using Webcrm.ErpIntegrations.GeneralUtilities;

namespace Webcrm.ErpIntegrations.ApiClients.PowerofficeApiClient.Models
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class NewContactPerson
    {
        public string EmailAddress { get; set; }
        public string FirstName { get; set; }
        public bool? IsActive { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }

        [JsonIgnore]
        public string ConcatenatedName => StringUtilities.JoinDefined(" ", FirstName, LastName);

        public bool HasRelevantChanges(
            PersonDto webcrmPerson)
        {
            if (!StringUtilities.AreEquivalent(EmailAddress, webcrmPerson.PersonEmail))
                return true;

            if (!StringUtilities.AreEquivalent(FirstName, webcrmPerson.PersonFirstName))
                return true;

            if (!StringUtilities.AreEquivalent(LastName, webcrmPerson.PersonAdjustedLastName))
                return true;

            if (!StringUtilities.AreEquivalent(PhoneNumber, webcrmPerson.PersonDirectPhone))
                return true;

            return false;
        }

        public void Update(PersonDto sourceWebcrmPerson)
        {
            EmailAddress = sourceWebcrmPerson.PersonEmail;
            FirstName = sourceWebcrmPerson.PersonFirstName;
            LastName = sourceWebcrmPerson.PersonAdjustedLastName;
            PhoneNumber = sourceWebcrmPerson.PersonDirectPhone;
        }
    }
}