using System.Text.RegularExpressions;

namespace Webcrm.ErpIntegrations.Synchronisation.Models
{
    internal class WebcrmResourceUrl
    {
        public WebcrmResourceUrl(
            string itemResourceUrl)
        {
            ItemId = GetItemId(itemResourceUrl);
            ItemType = GetItemType(itemResourceUrl);
        }

        public int ItemId { get; }
        public WebcrmItemType ItemType { get; }

        private static int GetItemId(string itemResourceUrl)
        {
            var matchDigitsInEnd = new Regex(@"\d+$", RegexOptions.RightToLeft);
            string digits = matchDigitsInEnd.Match(itemResourceUrl).Value;
            int id = int.Parse(digits);
            return id;
        }

        private static WebcrmItemType GetItemType(string itemResourceUrl)
        {
            if (itemResourceUrl.Contains("/Deliveries/"))
                return WebcrmItemType.Delivery;

            if (itemResourceUrl.Contains("/Organisations/"))
                return WebcrmItemType.Organisation;

            if (itemResourceUrl.Contains("/Persons/"))
                return WebcrmItemType.Person;

            return WebcrmItemType.Unknown;
        }
    }
}