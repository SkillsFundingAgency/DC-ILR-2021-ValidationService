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

        public static bool CaseInsensitiveStartsWith(this string source, string thisCandidate)
        {
            if (string.IsNullOrWhiteSpace(source))
            {
                return false;
            }

            return source.StartsWith(thisCandidate, StringComparison.OrdinalIgnoreCase);
        }
    }
}