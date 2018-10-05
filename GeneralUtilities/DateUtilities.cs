using System;

namespace Webcrm.ErpIntegrations.GeneralUtilities
{
    public static class DateUtilities
    {
        public static DateTime FromUtcToSwedish(this DateTime dateTime)
        {
            var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
            if (timeZoneInfo.IsDaylightSavingTime(dateTime))
                return dateTime.AddHours(2);

            return dateTime.AddHours(1);
        }
    }
}