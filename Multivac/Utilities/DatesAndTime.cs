using System;

namespace Multivac.Utilities
{
    static class DatesAndTime
    {
        public static string YearsSince(DateTimeOffset input)
        {
            var years = (DateTime.Now - input).Days / 365;
            if (years == 1) return $"{years} year";
            else return $"{years} years";
        }

        public static string DaysSince(DateTimeOffset input, bool dayOfYear = true)
        {
            int days;
            if (dayOfYear) days = (DateTime.Now - input).Days % 365;
            else days = (DateTime.Now - input).Days;

            if (days == 1) return $"{days} day";
            else return $"{days} days";
        }
    }
}
