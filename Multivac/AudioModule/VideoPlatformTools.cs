using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Multivac.AudioModule
{
    public static class VideoPlatformTools
    {
        public static void PlatformSearchSelector(string input, out string siteSpecifier, out string searchTerm)
        {
            if (new Regex("(?i)-(yt|youtube)").IsMatch(input))
            {
                Console.WriteLine("youtube");

                siteSpecifier = "ytsearch:";
                searchTerm = new Regex("(?i)-(yt|youtube)").Replace(input, "").Trim();
            }
            else if (new Regex("(?i)-(sc|soundcloud)").IsMatch(input))
            {
                Console.WriteLine("soundcloud");

                siteSpecifier = "scsearch:";
                searchTerm = new Regex("(?i)-(sc|soundcloud)").Replace(input, "").Trim();
            }
            else if (input.Contains("-raw"))
            {
                Console.WriteLine("raw");

                siteSpecifier = "";
                searchTerm = input.Replace("-raw", "").Trim();
                Console.WriteLine($"searching for: {searchTerm}");
            }
            else if (new Regex("(?i)https?://").IsMatch(input))
            {
                Console.WriteLine("url");

                siteSpecifier = "";
                searchTerm = input;
            }
            else
            {
                siteSpecifier = "ytsearch:";
                searchTerm = input;
            }
        }

        public static void ThumbnailGrabber()
        {

        }
    }
}
