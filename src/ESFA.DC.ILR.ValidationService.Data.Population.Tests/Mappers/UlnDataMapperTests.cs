using System.Collections.Generic;
using ESFA.DC.ILR.ValidationService.Data.Population.Mappers;
using FluentAssertions;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Data.Population.Tests.Mappers
{
    public class UlnDataMapperTests
    {
        [Fact]
        public void MapUlns()
        {
            var ulns = new List<long> { 1, 2, 3, 4, 5 };

            NewMapper().MapUlns(ulns).Should().BeEquivalentTo(ulns);
            NewMapper().MapUlns(ulns).Should().NotContain(6);
        }

        private UlnDataMapper NewMapper()
        {
            return new UlnDataMapper();
        }
    }
}
