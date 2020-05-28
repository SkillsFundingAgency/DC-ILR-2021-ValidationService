using ESFA.DC.ILR.ValidationService.Data.External.Postcodes;
using ESFA.DC.ILR.ValidationService.Data.External.Postcodes.Interface;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using Moq;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Data.Tests.External
{
    public class PostcodesDataServiceTests
    {
        [Theory]
        [InlineData(null, false)]
        [InlineData(" ", false)]
        [InlineData("jkl", false)]
        [InlineData("def", true)]
        [InlineData("ghi", true)]
        public void PostcodeExistsMeetsExpectation(string candidate, bool expectation)
        {
            // arrange
            var externalDC = new Mock<IExternalDataCache>();
            externalDC
                .SetupGet(rdc => rdc.Postcodes)
                .Returns(new HashSet<string>() { "abc", "def", "ghi" });

            var sut = new PostcodesDataService(externalDC.Object);

            // act
            var result = sut.PostcodeExists(candidate);

            // assert
            Assert.Equal(expectation, result);
        }

        [Theory]
        [InlineData(null, false)]
        [InlineData(" ", false)]
        [InlineData("jkl", false)]
        [InlineData("def", true)]
        [InlineData("ghi", true)]
        public void ONSPostcodeExistsMeetsExpectation(string candidate, bool expectation)
        {
            var onsPostcodes = new List<IONSPostcode>()
            {
                new ONSPostcode
                {
                    Postcode = "abc"
                },
                new ONSPostcode
                {
                    Postcode = "def"
                },
                new ONSPostcode
                {
                    Postcode = "ghi"
                }
            };

            // arrange
            var externalDC = new Mock<IExternalDataCache>();
            externalDC
                .SetupGet(rdc => rdc.ONSPostcodes)
                .Returns(onsPostcodes);

            var sut = new PostcodesDataService(externalDC.Object);

            // act
            var result = sut.ONSPostcodeExists(candidate);

            // assert
            Assert.Equal(expectation, result);
        }
    }
}
