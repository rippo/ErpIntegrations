using System;
using System.Linq;
using System.Text;

namespace Webcrm.ErpIntegrations.GeneralUtilities
{
    public static class StringUtilities
    {
        public const string WindowsNewline = "\r\n";

        /// <summary>Compares two strings considering null and empty strings as equal.</summary>
        public static bool AreEquivalent(string stringA, string stringB)
        {
            if (stringA == null)
                stringA = string.Empty;

            if (stringB == null)
                stringB = string.Empty;

            return string.Equals(stringA, stringB, StringComparison.InvariantCulture);
        }

        public static string Base64Encode(string plainText)
        {
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            string encodedText = Convert.ToBase64String(plainTextBytes);
            return encodedText;
        }

        /// <summary>Joins strings with the separator as string.Join, except null or whitespace strings are ignored.</summary>
        public static string JoinDefined(string separator, params string[] strings)
        {
            var definedStrings = strings.Where(s => !string.IsNullOrWhiteSpace(s));
            return string.Join(separator, definedStrings);
        }

        public static string Truncate(this string value, int maximumLength, bool addEllipsis = false)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            if (value.Length <= maximumLength)
                return value;

            string truncated = value.Substring(0, maximumLength);

            if (addEllipsis)
            {
                truncated += "...";
            }

            return truncated;
        }
    }
}