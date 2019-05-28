using System;
using System.Collections.Generic;
using System.Linq;

namespace ESFA.DC.ILR.ValidationService.Utility
{
    /// <summary>
    /// Class Format, performs an invariant culture string formatting
    /// </summary>
    public static class Format
    {
        /// <summary>
        /// Compares (the item) with any candidate.
        /// this is a case and culture insensitive comparison
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="anyCandidate">Any candidate.</param>
        /// <returns>true if the item matches any of the candididates</returns>
        /// <exception cref="ArgumentNullException">if there aren't any Candidate(s)</exception>
        public static bool ComparesWith(this string item, params string[] anyCandidate)
        {
            It.IsNull(anyCandidate)
                .AsGuard<ArgumentNullException>(nameof(anyCandidate));

            foreach (var candidate in anyCandidate)
            {
                var result = ComparesWith(item, candidate);
                if (result)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Compares (the item) with any candidate.
        /// this is a case and culture insensitive comparison
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="anyCandidate">Any candidate.</param>
        /// <returns>true if the item matches any of the candididates</returns>
        /// <exception cref="ArgumentNullException">if there aren't any Candidate(s)</exception>
        public static bool ComparesWith(this string item, IEnumerable<string> anyCandidate)
        {
            It.IsNull(anyCandidate)
                .AsGuard<ArgumentNullException>(nameof(anyCandidate));

            return ComparesWith(item, anyCandidate.ToArray());
        }

        /// <summary>
        /// Compares the item with this candidate.
        /// this is a case and culture insensitive comparison
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="thisCandidate">this candidate.</param>
        /// <returns>
        /// true if the item matches the candididate
        /// </returns>
        public static bool ComparesWith(this string item, string thisCandidate)
        {
            return string.Compare(item, thisCandidate, StringComparison.OrdinalIgnoreCase) == 0;
        }

        /// <summary>
        /// Commences with.
        /// this is a case and culture insensitive comparison
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="anyCandidate">Any candidate.</param>
        /// <returns>
        /// true if the item commences with the any of the candididates
        /// </returns>
        /// <exception cref="ArgumentNullException">if there aren't any Candidate(s)</exception>
        public static bool CommencesWith(this string item, params string[] anyCandidate)
        {
            It.IsNull(anyCandidate)
                .AsGuard<ArgumentNullException>(nameof(anyCandidate));

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

        /// <summary>
        /// Commences with.
        /// this is a case and culture insensitive comparison
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="anyCandidate">Any candidate.</param>
        /// <returns>
        /// true if the item commences with the any of the candididates
        /// </returns>
        /// <exception cref="ArgumentNullException">if there aren't any Candidate(s)</exception>
        public static bool CommencesWith(this string item, IEnumerable<string> anyCandidate)
        {
            It.IsNull(anyCandidate)
                .AsGuard<ArgumentNullException>(nameof(anyCandidate));

            return CommencesWith(item, anyCandidate.ToArray());
        }

        /// <summary>
        /// Commences with.
        /// this is a case and culture insensitive comparison
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="thisCandidate">this candidate.</param>
        /// <returns>
        /// true if the item commences with this candididate
        /// </returns>
        public static bool CommencesWith(this string item, string thisCandidate)
        {
            return item.IndexOf(thisCandidate, StringComparison.OrdinalIgnoreCase) == 0;
        }
    }
}
