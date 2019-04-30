using System.Collections.Generic;
using ESFA.DC.ILR.ReferenceDataService.Model.MetaData;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Data.Internal.AcademicYear.Interface;
using ESFA.DC.ILR.ValidationService.Data.Internal.AcademicYear.Model;
using ESFA.DC.ILR.ValidationService.Data.Internal.Model;
using ESFA.DC.ILR.ValidationService.Utility;

namespace ESFA.DC.ILR.ValidationService.Data.Population.Interface
{
    public interface ILookupsDataMapper
    {
        IReadOnlyDictionary<string, IReadOnlyDictionary<string, IlrLookup>> BuildLookups(IReadOnlyCollection<Lookup> lookups);

        IAcademicYear MapAcademicYear(AcademicYear academicYear);

        IDictionary<TypeOfIntegerCodedLookup, IReadOnlyCollection<int>> MapIntegerLookups(IReadOnlyDictionary<string, IReadOnlyDictionary<string, IlrLookup>> lookups);

        IDictionary<TypeOfLimitedLifeLookup, IReadOnlyDictionary<string, ValidityPeriods>> MapLimitedLifeLookups(IReadOnlyCollection<Lookup> lookups);

        IDictionary<TypeOfListItemLookup, IReadOnlyDictionary<string, IReadOnlyCollection<string>>> MapListItemLookups(IReadOnlyCollection<Lookup> lookups);

        IDictionary<TypeOfStringCodedLookup, IReadOnlyCollection<string>> MapStringLookups(IReadOnlyDictionary<string, IReadOnlyDictionary<string, IlrLookup>> lookups);
    }
}
