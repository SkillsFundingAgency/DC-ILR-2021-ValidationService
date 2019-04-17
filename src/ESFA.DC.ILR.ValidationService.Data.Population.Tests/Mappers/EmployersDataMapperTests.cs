using System;
using System.Collections.Generic;
using ESFA.DC.ILR.ReferenceDataService.Model.Employers;
using ESFA.DC.ILR.ValidationService.Data.Population.Mappers;
using FluentAssertions;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Data.Population.Tests.Mappers
{
    public class EmployersDataMapperTests
    {
        [Fact]
        public void MapEmployers()
        {
            var expectedEmployerIds = new List<long> { 1, 2, 3, 4, 5 };

            var largeEmployerEffectiveDates = new List<LargeEmployerEffectiveDates>
            {
                new LargeEmployerEffectiveDates
                {
                    EffectiveFrom = new DateTime(2018, 8, 1)
                }
            };

            var employers = new List<Employer>
            {
                new Employer { ERN = 1, LargeEmployerEffectiveDates = largeEmployerEffectiveDates },
                new Employer { ERN = 2, LargeEmployerEffectiveDates = largeEmployerEffectiveDates },
                new Employer { ERN = 3, LargeEmployerEffectiveDates = largeEmployerEffectiveDates },
                new Employer { ERN = 4, LargeEmployerEffectiveDates = largeEmployerEffectiveDates },
                new Employer { ERN = 5, LargeEmployerEffectiveDates = largeEmployerEffectiveDates }
            };

            NewMapper().MapEmployers(employers).Should().BeEquivalentTo(expectedEmployerIds);
            NewMapper().MapEmployers(employers).Should().NotContain(6);
        }

        private EmployersDataMapper NewMapper()
        {
            return new EmployersDataMapper();
        }
    }
}
