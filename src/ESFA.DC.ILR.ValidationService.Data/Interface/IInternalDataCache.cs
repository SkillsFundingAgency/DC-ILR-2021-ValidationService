using System.Collections.Generic;
using ESFA.DC.ILR.ValidationService.Data.Internal.AcademicYear.Interface;
using ESFA.DC.ILR.ValidationService.Data.Internal.Model;

namespace ESFA.DC.ILR.ValidationService.Data.Interface
{
    public interface IInternalDataCache
    {
        IAcademicYear AcademicYear { get; }

        IDictionary<TypeOfIntegerCodedLookup, IReadOnlyCollection<int>> IntegerLookups { get; }

        IDictionary<TypeOfStringCodedLookup, IReadOnlyCollection<string>> StringLookups { get; }

        IDictionary<TypeOfLimitedLifeLookup, IReadOnlyDictionary<string, ValidityPeriods>> LimitedLifeLookups { get; }

        IDictionary<TypeOfListItemLookup, IReadOnlyDictionary<string, IReadOnlyCollection<string>>> ListItemLookups { get; }
    }
}
