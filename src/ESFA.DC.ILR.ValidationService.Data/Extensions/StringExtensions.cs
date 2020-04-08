using System;

namespace ESFA.DC.ILR.ValidationService.Data.Extensions
{
    public static class StringExtensions
    {
        public static bool CaseInsensitiveEquals(this string source, string data)
        {
            if (source == null && data == null)
            {
                return true;
            }

            return source?.Equals(data, StringComparison.OrdinalIgnoreCase) ?? false;
        }

        public static bool CommencesWith(this string item, params string[] anyCandidate)
        {
           if (anyCandidate == null)
           {
               return false;
           }

            foreach (var candidate in anyCandidate)
            {
                var result = CommencesWith(item, candidate);
                if (result)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool CommencesWith(this string item, string thisCandidate)
        {
            return item.IndexOf(thisCandidate, StringComparison.OrdinalIgnoreCase) == 0;
        }
    }
}