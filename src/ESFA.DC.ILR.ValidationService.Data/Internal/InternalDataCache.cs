using System.Collections.Generic;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Data.Internal.AcademicYear.Interface;
using ESFA.DC.ILR.ValidationService.Data.Internal.Model;

namespace ESFA.DC.ILR.ValidationService.Data.Internal
{
    public class InternalDataCache : IInternalDataCache
    {
        public IAcademicYear AcademicYear { get; set; }

        public IDictionary<TypeOfIntegerCodedLookup, IReadOnlyCollection<int>> IntegerLookups { get; set; }

        public IDictionary<TypeOfStringCodedLookup, IReadOnlyCollection<string>> StringLookups { get; set; }

        public IDictionary<TypeOfListItemLookup, IReadOnlyDictionary<string, IReadOnlyCollection<string>>> ListItemLookups { get; set; }

        public IDictionary<TypeOfLimitedLifeLookup, IReadOnlyDictionary<string, ValidityPeriods>> LimitedLifeLookups { get; set; }
    }
}
