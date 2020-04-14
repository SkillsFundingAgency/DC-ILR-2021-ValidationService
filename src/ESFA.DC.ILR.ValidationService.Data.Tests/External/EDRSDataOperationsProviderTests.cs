using ESFA.DC.ILR.ValidationService.Data.External.EDRS;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Utility;
using Moq;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Data.Tests.External
{
    public class EDRSDataOperationsProviderTests
    {
        [Theory]
        [InlineData(99999999, false)]
        [InlineData(999999998, false)]
        [InlineData(1000000000, true)]
        [InlineData(1234567891, false)]
        [InlineData(2112345678, true)]
        [InlineData(2123456788, false)]
        [InlineData(2134567891, false)]
        [InlineData(2145678901, true)]
        [InlineData(2146789012, true)]
        [InlineData(2147456788, false)]
        public void IsValidExpectation(int? candidate, bool expectation)
        {
            // arrange
            var cache = LoadCandidates();

            var sut = NewService(cache);

            // act
            var result = sut.IsValid(candidate);

            // assert
            Assert.Equal(expectation, result);
        }

        public IExternalDataCache LoadCandidates()
        {
            var cache = new Mock<IExternalDataCache>();
            var collection = new List<int>
            {
                1000000000,
                1234567890,
                2112345678,
                2123456789,
                2134567890,
                2145678901,
                2146789012,
                2147456789,
            };

            cache
                .SetupGet(x => x.ERNs)
                .Returns(collection);

            return cache.Object;
        }

        private EmployersDataService NewService(IExternalDataCache externalDataCache)
        {
            return new EmployersDataService(externalDataCache);
        }
    }
}