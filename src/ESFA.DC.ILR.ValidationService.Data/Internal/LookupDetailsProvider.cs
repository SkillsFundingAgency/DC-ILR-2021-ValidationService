using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.ValidationService.Data.Interface;


namespace ESFA.DC.ILR.ValidationService.Data
{
    public sealed class LookupDetailsProvider : IProvideLookupDetails
    {
        private IInternalDataCache _internalCache;

        public LookupDetailsProvider(IInternalDataCache internalCache)
        {
            _internalCache = internalCache;
        }

        public bool IsBetween(DateTime? fromDate, DateTime? toDate, DateTime candidate) => (candidate >= fromDate) && (candidate <= toDate);

        public IReadOnlyCollection<int> Get(TypeOfIntegerCodedLookup lookupKey) =>
            _internalCache.IntegerLookups[lookupKey];

        public IReadOnlyCollection<string> Get(TypeOfStringCodedLookup lookupKey) =>
            _internalCache.StringLookups[lookupKey];

        public bool Contains(TypeOfIntegerCodedLookup lookupKey, int candidate)
        {
            return _internalCache.IntegerLookups[lookupKey].Contains(candidate);
        }

        public bool Contains(TypeOfStringCodedLookup lookupKey, string candidate)
        {
            return !string.IsNullOrWhiteSpace(candidate)
                && _internalCache.StringLookups[lookupKey].Contains(candidate);
        }

        public bool Contains(TypeOfLimitedLifeLookup lookupKey, int candidate)
        {
            return Contains(lookupKey, $"{candidate}");
        }

        public bool Contains(TypeOfLimitedLifeLookup lookupKey, string keyCandidate)
        {
            return !string.IsNullOrWhiteSpace(keyCandidate)
                && _internalCache.LimitedLifeLookups[lookupKey].ContainsKey(keyCandidate);
        }

        public bool Contains(TypeOfListItemLookup lookupKey, string keyCandidate, string valueCandidate)
        {
            return !string.IsNullOrEmpty(keyCandidate)
                && _internalCache.ListItemLookups[lookupKey].TryGetValue(keyCandidate, out var value)
                && value.Contains(valueCandidate);
        }

        public bool IsCurrent(TypeOfLimitedLifeLookup lookupKey, int candidate, DateTime referenceDate)
        {
            return IsCurrent(lookupKey, $"{candidate}", referenceDate);
        }

        public bool IsCurrent(TypeOfLimitedLifeLookup lookupKey, string candidate, DateTime referenceDate)
        {
            return Contains(lookupKey, candidate)
                && _internalCache.LimitedLifeLookups[lookupKey].TryGetValue(candidate, out var value)
                && IsBetween(value.ValidFrom, value.ValidTo, referenceDate);
        }

        public bool IsExpired(TypeOfLimitedLifeLookup lookupKey, string candidate, DateTime referenceDate)
        {
            return Contains(lookupKey, candidate)
                && _internalCache.LimitedLifeLookups[lookupKey].TryGetValue(candidate, out var value)
                && referenceDate > value.ValidTo;
        }

        public bool IsVaguelyCurrent(TypeOfLimitedLifeLookup lookupKey, string candidate, DateTime referenceDate)
        {
            return Contains(lookupKey, candidate)
                && _internalCache.LimitedLifeLookups[lookupKey].TryGetValue(candidate, out var value)
                && IsBetween(DateTime.MinValue, value.ValidTo, referenceDate);
        }
    }
}
