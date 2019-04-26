using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.ReferenceDataService.Model.MetaData;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Data.Internal.AcademicYear.Interface;
using ESFA.DC.ILR.ValidationService.Data.Internal.AcademicYear.Model;
using ESFA.DC.ILR.ValidationService.Data.Internal.Model;
using ESFA.DC.ILR.ValidationService.Data.Population.Interface;

namespace ESFA.DC.ILR.ValidationService.Data.Population.Mappers
{
    public class LookupsDataMapper : ILookupsDataMapper
    {
        public IAcademicYear MapAcademicYear(AcademicYear academicYear)
        {
            return new AcademicYear
            {
                Start = academicYear.Start,
                AugustThirtyFirst = academicYear.AugustThirtyFirst,
                JanuaryFirst = academicYear.JanuaryFirst,
                JulyThirtyFirst = academicYear.JulyThirtyFirst,
                End = academicYear.End
            };
        }

        public IDictionary<TypeOfIntegerCodedLookup, IReadOnlyCollection<int>> MapIntegerLookups(IReadOnlyCollection<Lookup> lookups)
        {
            var type = typeof(TypeOfIntegerCodedLookup);

            return lookups
                .Where(l => Enum.IsDefined(type, l.Name))
                .GroupBy(n => n.Name)
                .ToDictionary(
                l => (TypeOfIntegerCodedLookup)Enum.Parse(type, l.Key),
                l => l.Select(v => int.Parse(v.Code)).ToList() as IReadOnlyCollection<int>);
        }

        public IDictionary<TypeOfStringCodedLookup, IReadOnlyCollection<string>> MapStringLookups(IReadOnlyCollection<Lookup> lookups)
        {
            var type = typeof(TypeOfStringCodedLookup);

            return lookups
                .Where(l => Enum.IsDefined(type, l.Name))
                .GroupBy(n => n.Name)
                .ToDictionary(
                l => (TypeOfStringCodedLookup)Enum.Parse(type, l.Key),
                l => l.Select(v => v.Code).ToList() as IReadOnlyCollection<string>);
        }

        public IDictionary<TypeOfLimitedLifeLookup, IReadOnlyDictionary<string, ValidityPeriods>> MapLimitedLifeLookups(IReadOnlyCollection<Lookup> lookups)
        {
            throw new NotImplementedException();
        }

        public IDictionary<TypeOfListItemLookup, IReadOnlyDictionary<string, IReadOnlyCollection<string>>> MapListItemLookups(IReadOnlyCollection<Lookup> lookups)
        {
            throw new NotImplementedException();
        }
    }
}
