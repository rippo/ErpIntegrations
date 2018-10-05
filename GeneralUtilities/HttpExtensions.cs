using System;
using System.Web;

namespace Webcrm.ErpIntegrations.GeneralUtilities
{
    public static class HttpExtensions
    {
        public static Uri AddQueryParameter(this Uri uri, string key, string value)
        {
            var httpValueCollection = HttpUtility.ParseQueryString(uri.Query);

            httpValueCollection.Remove(key);
            httpValueCollection.Add(key, value);

            var ub = new UriBuilder(uri)
            {
                Query = httpValueCollection.ToString()
            };

            return ub.Uri;
        }
    }
}