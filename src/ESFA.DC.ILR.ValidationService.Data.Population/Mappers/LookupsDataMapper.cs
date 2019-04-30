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
        public IReadOnlyDictionary<string, IReadOnlyDictionary<string, IlrLookup>> BuildLookups(IReadOnlyCollection<Lookup> lookups)
        {
           return lookups
                .GroupBy(n => n.Name)
                .ToDictionary(
                k => k.Key,
                v => v.Select(l => new IlrLookup
                {
                    Name = l.Name,
                    Code = l.Code,
                    ValidityPeriods = new ValidityPeriods(l.EffectiveFrom, l.EffectiveTo),
                    SubLookup = l.SubCategories?.Select(sc => new IlrSubLookup
                    {
                        Code = sc.Code,
                        ValidityPeriods = new ValidityPeriods(sc.EffectiveFrom ?? l.EffectiveFrom, sc.EffectiveTo ?? l.EffectiveFrom)
                    }).ToList()
                })
                .ToDictionary(
                    k => k.Code,
                    s => s,
                    StringComparer.OrdinalIgnoreCase) as IReadOnlyDictionary<string, IlrLookup>, StringComparer.OrdinalIgnoreCase);
        }

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

        public IDictionary<TypeOfIntegerCodedLookup, IReadOnlyCollection<int>> MapIntegerLookups(IReadOnlyDictionary<string, IReadOnlyDictionary<string, IlrLookup>> lookups)
        {
            var integerDictionary = new Dictionary<TypeOfIntegerCodedLookup, IReadOnlyCollection<int>>();

            var type = typeof(TypeOfIntegerCodedLookup);

            foreach (var integerLookup in Enum.GetNames(type))
            {
                lookups.TryGetValue(integerLookup, out var lookupValue);

                if (lookupValue != null)
                {
                    integerDictionary.Add((TypeOfIntegerCodedLookup)Enum.Parse(type, integerLookup), lookupValue.Keys.Select(x => int.Parse(x)).ToList());
                }
            }

            return integerDictionary;
        }

        public IDictionary<TypeOfStringCodedLookup, IReadOnlyCollection<string>> MapStringLookups(IReadOnlyDictionary<string, IReadOnlyDictionary<string, IlrLookup>> lookups)
        {
            var stringDictionary = new Dictionary<TypeOfStringCodedLookup, IReadOnlyCollection<string>>();

            var type = typeof(TypeOfStringCodedLookup);

            foreach (var stringLookup in Enum.GetNames(type))
            {
                lookups.TryGetValue(stringLookup, out var lookupValue);

                if (lookupValue != null)
                {
                    stringDictionary.Add((TypeOfStringCodedLookup)Enum.Parse(type, stringLookup), lookupValue.Keys.ToList());
                }
            }

            return stringDictionary;
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
