using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Utility;

namespace ESFA.DC.ILR.ValidationService.Data
{
    public sealed class LookupDetailsProvider : IProvideLookupDetails
    {
        private IInternalDataCache _internalCache;

        public LookupDetailsProvider(IInternalDataCache internalCache)
        {
            _internalCache = internalCache;
        }

        /// <summary>
        /// Determines whether the specified from date is between.
        /// </summary>
        /// <param name="fromDate">From date.</param>
        /// <param name="toDate">To date.</param>
        /// <param name="candidate">The candidate.</param>
        /// <returns>
        ///   <c>true</c> if the specified from date is between; otherwise, <c>false</c>.
        /// </returns>
        public bool IsBetween(DateTime fromDate, DateTime toDate, DateTime candidate) => (candidate >= fromDate) && (candidate <= toDate);

        public IReadOnlyCollection<int> Get(TypeOfIntegerCodedLookup lookupKey) =>
            _internalCache.IntegerLookups[lookupKey];

        public IReadOnlyCollection<string> Get(TypeOfStringCodedLookup lookupKey) =>
            _internalCache.StringLookups[lookupKey];

        /// <summary>
        /// Determines whether [the specified lookup key] [contains] the value.
        /// </summary>
        /// <param name="lookupKey">The lookup key.</param>
        /// <param name="candidate">The candidate.</param>
        /// <returns>
        /// <c>true</c> if [the specified lookup] [contains]; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(TypeOfIntegerCodedLookup lookupKey, int candidate)
        {
            return _internalCache.IntegerLookups[lookupKey].Contains(candidate);
        }

        /// <summary>
        /// Determines whether [the specified lookup key] [contains] the value.
        /// </summary>
        /// <param name="lookupKey">The lookup key.</param>
        /// <param name="candidate">The candidate.</param>
        /// <returns>
        /// <c>true</c> if [the specified lookup] [contains]; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(TypeOfStringCodedLookup lookupKey, string candidate)
        {
            return It.Has(candidate)
                && _internalCache.StringLookups[lookupKey].Contains(candidate);
        }

        /// <summary>
        /// Determines whether [the specified lookup key] [contains] the value.
        /// </summary>
        /// <param name="lookupKey">The lookup key.</param>
        /// <param name="candidate">The candidate.</param>
        /// <returns>
        /// <c>true</c> if [the specified lookup] [contains]; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(TypeOfLimitedLifeLookup lookupKey, int candidate)
        {
            return Contains(lookupKey, $"{candidate}");
        }

        /// <summary>
        /// Determines whether [contains] [the specified lookup key].
        /// </summary>
        /// <param name="lookupKey">The lookup key.</param>
        /// <param name="keyCandidate">The candidate.</param>
        /// <returns>
        /// <c>true</c> if [contains] [the specified lookup key]; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(TypeOfLimitedLifeLookup lookupKey, string keyCandidate)
        {
            return It.Has(keyCandidate)
                && _internalCache.LimitedLifeLookups[lookupKey].ContainsKey(keyCandidate);
        }

        /// <summary>
        /// Determines whether [contains] [for the specified lookup] [the key and value].
        /// </summary>
        /// <param name="lookupKey">The lookup key.</param>
        /// <param name="keyCandidate">The key candidate.</param>
        /// <param name="valueCandidate">The value candidate.</param>
        /// <returns>
        /// <c>true</c> if [contains] [the specified lookup key]; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(TypeOfListItemLookup lookupKey, string keyCandidate, string valueCandidate)
        {
            return !string.IsNullOrEmpty(keyCandidate)
                && _internalCache.ListItemLookups[lookupKey].TryGetValue(keyCandidate, out var value)
                && value.Contains(valueCandidate);
        }

        /// <summary>
        /// Determines whether [the specified lookup key] is current on the given date
        /// </summary>
        /// <param name="lookupKey">The lookup key.</param>
        /// <param name="candidate">The candidate.</param>
        /// <param name="referenceDate">The reference date.</param>
        /// <returns>
        /// <c>true</c> if [the specified lookup] [is current] for the given date; otherwise, <c>false</c>.
        /// </returns>
        public bool IsCurrent(TypeOfLimitedLifeLookup lookupKey, int candidate, DateTime referenceDate)
        {
            return IsCurrent(lookupKey, $"{candidate}", referenceDate);
        }

        /// <summary>
        /// Determines whether the specified lookup key is current.
        /// </summary>
        /// <param name="lookupKey">The lookup key.</param>
        /// <param name="candidate">The candidate.</param>
        /// <param name="referenceDate">The reference date.</param>
        /// <returns>
        /// <c>true</c> if the specified lookup key is current; otherwise, <c>false</c>.
        /// </returns>
        public bool IsCurrent(TypeOfLimitedLifeLookup lookupKey, string candidate, DateTime referenceDate)
        {
            return Contains(lookupKey, candidate)
                && _internalCache.LimitedLifeLookups[lookupKey].TryGetValue(candidate, out var value)
                && IsBetween(value.ValidFrom, value.ValidTo, referenceDate);
        }

        /// <summary>
        /// Determines whether [is vaguely cuurrent] [the specified lookup key].
        /// a loose check for things with a bottomless beginning
        /// </summary>
        /// <param name="lookupKey">The lookup key.</param>
        /// <param name="candidate">The candidate.</param>
        /// <param name="referenceDate">The reference date.</param>
        /// <returns>
        /// <c>true</c> if [is vaguely current] [the specified lookup key]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsVaguelyCurrent(TypeOfLimitedLifeLookup lookupKey, string candidate, DateTime referenceDate)
        {
            return Contains(lookupKey, candidate)
                && _internalCache.LimitedLifeLookups[lookupKey].TryGetValue(candidate, out var value)
                && IsBetween(DateTime.MinValue, value.ValidTo, referenceDate);
        }
    }
}
