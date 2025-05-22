using System.Text.RegularExpressions;

namespace Riffle.Utilities
{
    public static class NormString
    {

        private static readonly Regex whitespace = new("\\s+", RegexOptions.Compiled);

        /// <summary>
        /// Normalizes the input string by trimming whitespace, replacing any/onsecutive whitespace with a single space,
        /// and converting it to uppercase.
        /// </summary>
        /// <param name="s">The input string to normalize. Cannot be null.</param>
        /// <returns>A normalized version of the input string with trimmed and standardized whitespace, in uppercase.</returns>
        public static string NormalizeString(string s)
        {
            string ret = s.ToUpper();
            s = whitespace.Replace(s, " ");
            s = s.Trim();
            return s;
        }
    }
}
