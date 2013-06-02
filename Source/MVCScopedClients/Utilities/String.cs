using System;

namespace MVCScopedClients.Utilities
{
    public static class String
    {
        public static string Substring(this string str, string startString, string endString)
        {
            if (str.Contains(startString))
            {
                var iStart = str.IndexOf(startString, StringComparison.Ordinal) + startString.Length;
                var iEnd = str.IndexOf(endString, iStart, StringComparison.Ordinal);
                return str.Substring(iStart, (iEnd - iStart));
            }
            return null;
        }
    }
}