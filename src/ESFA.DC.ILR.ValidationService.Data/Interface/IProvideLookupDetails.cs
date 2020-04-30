using System;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Data.Interface
{
    public interface IProvideLookupDetails
    {
        IReadOnlyCollection<int> Get(TypeOfIntegerCodedLookup lookupKey);

        IReadOnlyCollection<string> Get(TypeOfStringCodedLookup lookupKey);

        bool Contains(TypeOfIntegerCodedLookup lookupKey, int candidate);

        bool Contains(TypeOfStringCodedLookup lookupKey, string candidate);

        bool Contains(TypeOfLimitedLifeLookup lookupKey, int candidate);

        bool Contains(TypeOfLimitedLifeLookup lookupKey, string candidate);

        bool Contains(TypeOfListItemLookup lookupKey, string keyCandidate, string valueCandidate);

        bool IsCurrent(TypeOfLimitedLifeLookup lookupKey, int candidate, DateTime referenceDate);

        bool IsCurrent(TypeOfLimitedLifeLookup lookupKey, string candidate, DateTime referenceDate);

        bool IsExpired(TypeOfLimitedLifeLookup lookupKey, string candidate, DateTime referenceDate);

        bool IsVaguelyCurrent(TypeOfLimitedLifeLookup lookupKey, string candidate, DateTime referenceDate);
    }
}
