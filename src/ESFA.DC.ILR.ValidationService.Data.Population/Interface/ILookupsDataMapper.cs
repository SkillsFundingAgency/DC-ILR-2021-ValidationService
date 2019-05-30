using System.Collections.Generic;
using ESFA.DC.ILR.ReferenceDataService.Model.MetaData;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Data.Internal.AcademicYear.Interface;
using ESFA.DC.ILR.ValidationService.Data.Internal.AcademicYear.Model;
using ESFA.DC.ILR.ValidationService.Data.Internal.Model;

namespace ESFA.DC.ILR.ValidationService.Data.Population.Interface
{
    public interface ILookupsDataMapper : IMapper
    {
        IReadOnlyDictionary<string, IReadOnlyDictionary<string, IlrLookup>> BuildLookups(IReadOnlyCollection<Lookup> lookups);

        IAcademicYear MapAcademicYear(AcademicYear academicYear);

        IDictionary<TypeOfIntegerCodedLookup, IReadOnlyCollection<int>> MapIntegerLookups(IReadOnlyDictionary<string, IReadOnlyDictionary<string, IlrLookup>> lookups);

        IDictionary<TypeOfStringCodedLookup, IReadOnlyCollection<string>> MapStringLookups(IReadOnlyDictionary<string, IReadOnlyDictionary<string, IlrLookup>> lookups);

        IDictionary<TypeOfLimitedLifeLookup, IReadOnlyDictionary<string, ValidityPeriods>> MapLimitedLifeLookups(IReadOnlyDictionary<string, IReadOnlyDictionary<string, IlrLookup>> lookups);

        IDictionary<TypeOfListItemLookup, IReadOnlyDictionary<string, IReadOnlyCollection<string>>> MapListItemLookups(IReadOnlyDictionary<string, IReadOnlyDictionary<string, IlrLookup>> lookups);
    }
}
