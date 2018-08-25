using System;

namespace Multivac.Utilities
{
    static class DateTimeConversion
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

        public static void VoiceRegionToTimeZone(string voiceregion, out string timezone, out int offset)
        {
            switch (voiceregion)
            {
                case "london":
                    timezone = "BST";
                    offset = 0;
                    break;
                case "eu-west":
                    timezone = "WET";
                    offset = 0;
                    break;
                case "amsterdam":
                case "eu-central":
                case "frankfurt":
                    timezone = "CET";
                    offset = 1;
                    break;
                case "southafrica":
                    timezone = "SAST";
                    offset = 2;
                    break;
                case "russia":
                    timezone = "MSK";
                    offset = 3;
                    break;
                case "singapore":
                    timezone = "SGT";
                    offset = 8;
                    break;
                case "hongkong":
                    timezone = "HKT";
                    offset = 8;
                    break;
                case "japan":
                    timezone = "JST";
                    offset = 9;
                    break;
                case "sydney":
                    timezone = "AEST";
                    offset = 10;
                    break;
                case "brazil":
                    timezone = "BRT";
                    offset = -3;
                    break;
                case "us-east":
                    timezone = "EST";
                    offset = -5;
                    break;
                case "us-central":
                case "us-south":
                    timezone = "CST";
                    offset = -6;
                    break;
                case "us-west":
                    timezone = "PST";
                    offset = -8;
                    break;
                default:
                    timezone = "UTC";
                    offset = 0;
                    break;
            }
        }
    }
}
