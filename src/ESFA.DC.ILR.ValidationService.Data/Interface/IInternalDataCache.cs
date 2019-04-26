using System.Collections.Generic;
using ESFA.DC.ILR.ValidationService.Data.Internal.AcademicYear.Interface;
using ESFA.DC.ILR.ValidationService.Data.Internal.Model;

namespace ESFA.DC.ILR.ValidationService.Data.Interface
{
    public interface IInternalDataCache
    {
        IAcademicYear AcademicYear { get; }

        /// <summary>
        /// Gets the simple lookups.
        /// </summary>
        IDictionary<TypeOfIntegerCodedLookup, IReadOnlyCollection<int>> IntegerLookups { get; }

        /// <summary>
        /// Gets the coded lookups.
        /// </summary>
        IDictionary<TypeOfStringCodedLookup, IReadOnlyCollection<string>> StringLookups { get; }

        /// <summary>
        /// Gets the time restricted lookups.
        /// </summary>
        IDictionary<TypeOfLimitedLifeLookup, IReadOnlyDictionary<string, ValidityPeriods>> LimitedLifeLookups { get; }

        /// <summary>
        /// Gets the list item lookups.
        /// </summary>
        IDictionary<TypeOfListItemLookup, IReadOnlyDictionary<string, IReadOnlyCollection<string>>> ListItemLookups { get; }
    }
}
