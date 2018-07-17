namespace Multivac.Utilities
{
    public static class TextManipulation
    {
        public static string Pluralize(string singular, string plural, int count)
        {
            if (count == 1) return $"{count} {singular}";
            else return $"{count} {plural}";
        }
    }
}
