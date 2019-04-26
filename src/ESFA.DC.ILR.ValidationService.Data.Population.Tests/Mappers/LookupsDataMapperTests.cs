using System;
using System.Collections.Generic;
using ESFA.DC.ILR.ReferenceDataService.Model.Employers;
using ESFA.DC.ILR.ReferenceDataService.Model.MetaData;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Data.Internal.AcademicYear.Model;
using ESFA.DC.ILR.ValidationService.Data.Population.Mappers;
using FluentAssertions;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Data.Population.Tests.Mappers
{
    public class LookupsDataMapperTests
    {
        [Fact]
        public void MapAcademicYear()
        {
            var academicYearLookup = new AcademicYear
            {
                AugustThirtyFirst = new DateTime(2018, 8, 31),
                End = new DateTime(2019, 7, 31),
                JanuaryFirst = new DateTime(2019, 1, 1),
                JulyThirtyFirst = new DateTime(2019, 7, 31),
                Start = new DateTime(2018, 8, 1)
            };

            var expectedAademicYearLookup = new AcademicYear
            {
                AugustThirtyFirst = new DateTime(2018, 8, 31),
                End = new DateTime(2019, 7, 31),
                JanuaryFirst = new DateTime(2019, 1, 1),
                JulyThirtyFirst = new DateTime(2019, 7, 31),
                Start = new DateTime(2018, 8, 1)
            };

            expectedAademicYearLookup.Should().BeEquivalentTo(NewMapper().MapAcademicYear(academicYearLookup));
        }

        [Fact]
        public void MapIntegerLookups()
        {
            var lookup = new List<Lookup>
            {
                new Lookup { Name = "AimType", Code = "1" },
                new Lookup { Name = "AimType", Code = "3" },
                new Lookup { Name = "CompStatus", Code = "1" },
                new Lookup { Name = "CompStatus", Code = "2" },
            };

            var expectedLookups = new Dictionary<TypeOfIntegerCodedLookup, List<int>>
            {
                {
                    TypeOfIntegerCodedLookup.AimType, new List<int> { 1, 3 }
                },
                {
                    TypeOfIntegerCodedLookup.CompStatus, new List<int> { 1, 2 }
                },
            };

            expectedLookups.Should().BeEquivalentTo(NewMapper().MapIntegerLookups(lookup));
        }

        [Fact]
        public void MapStringLookups()
        {
            var lookup = new List<Lookup>
            {
                new Lookup { Name = "AppFinType", Code = "PMR" },
                new Lookup { Name = "AppFinType", Code = "TNP" },
                new Lookup { Name = "ContPrefType", Code = "PMC" },
                new Lookup { Name = "ContPrefType", Code = "RUI" },
            };

            var expectedLookups = new Dictionary<TypeOfStringCodedLookup, List<string>>
            {
                {
                    TypeOfStringCodedLookup.AppFinType, new List<string> { "PMR", "TNP" }
                },
                {
                    TypeOfStringCodedLookup.ContPrefType, new List<string> { "PMC", "RUI" }
                },
            };

            expectedLookups.Should().BeEquivalentTo(NewMapper().MapStringLookups(lookup));
        }

        private LookupsDataMapper NewMapper()
        {
            return new LookupsDataMapper();
        }
    }
}
