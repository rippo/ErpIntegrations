using Newtonsoft.Json;
using System;
using System.Reflection;

namespace Webcrm.ErpIntegrations.GeneralUtilities
{
    public static class ObjectExtension
    {
        public static string GetPropertyValue(
            this object someObject,
            string propertyName)
        {
            var property = GetPropertyInfo(someObject, propertyName);
            object value = property.GetValue(someObject);
            if (value == null)
            {
                return string.Empty;
            }

            return value.ToString();
        }

        /// <remarks>It is unfortunately not possible to get the raw response string from `WebcrmSdk.QueriesGetAsync`, so the response string is being deserialized to `object` in `WebcrmSdk` and then deserialized back right after in this method.
        ///
        /// It might be more efficient to use reflection instead, but it was not possible to use AutoMapper out of the box. I guess it's because the source type is a plain object and not a specific class.</remarks>
        public static TTargetType MapToType<TTargetType>(this object source)
        {
            string serialized = JsonConvert.SerializeObject(source);
            var mapped = JsonConvert.DeserializeObject<TTargetType>(serialized);
            return mapped;
        }

        public static void SetPropertyValue(
            this object someObject,
            string propertyName,
            string value)
        {
            var property = GetPropertyInfo(someObject, propertyName);
            property.SetValue(someObject, value);
        }

        private static PropertyInfo GetPropertyInfo(object someObject, string propertyName)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
                throw new ArgumentNullException(nameof(propertyName));

            var property = someObject.GetType().GetProperty(propertyName);
            if (property == null)
            {
                throw new ApplicationException($"Could not find property '{propertyName}' on {someObject.GetType().Name} class.");
            }

            return property;
        }
    }
}