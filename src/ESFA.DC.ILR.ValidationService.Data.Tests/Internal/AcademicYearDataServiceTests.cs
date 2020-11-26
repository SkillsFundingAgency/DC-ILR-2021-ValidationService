using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Data.Internal.AcademicYear;
using ESFA.DC.ILR.ValidationService.Data.Internal.AcademicYear.Interface;
using FluentAssertions;
using Moq;
using System;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Data.Tests.Internal
{
    public class AcademicYearDataServiceTests
    {
        [Fact]
        public void AugustThirtyFirst()
        {
            var date = new DateTime(2018, 8, 31);

            var internalDataCacheMock = new Mock<IInternalDataCache>();

            internalDataCacheMock.SetupGet(c => c.AcademicYear.AugustThirtyFirst).Returns(date);

            NewService(internalDataCacheMock.Object).AugustThirtyFirst().Should().Be(date);
        }

        [Fact]
        public void YearEnd()
        {
            var date = new DateTime(2019, 7, 31);

            var internalDataCacheMock = new Mock<IInternalDataCache>();

            internalDataCacheMock.SetupGet(c => c.AcademicYear.End).Returns(date);

            NewService(internalDataCacheMock.Object).End().Should().Be(date);
        }

        [Fact]
        public void JanuaryFirst()
        {
            var date = new DateTime(2019, 1, 1);

            var internalDataCacheMock = new Mock<IInternalDataCache>();

            internalDataCacheMock.SetupGet(c => c.AcademicYear.JanuaryFirst).Returns(date);

            NewService(internalDataCacheMock.Object).JanuaryFirst().Should().Be(date);
        }

        [Fact]
        public void JulyThirtyFirst()
        {
            var date = new DateTime(2019, 7, 31);

            var internalDataCacheMock = new Mock<IInternalDataCache>();

            internalDataCacheMock.SetupGet(c => c.AcademicYear.JulyThirtyFirst).Returns(date);

            NewService(internalDataCacheMock.Object).JulyThirtyFirst().Should().Be(date);
        }

        [Fact]
        public void YearStart()
        {
            var date = new DateTime(2018, 8, 1);

            var internalDataCacheMock = new Mock<IInternalDataCache>();

            internalDataCacheMock.SetupGet(c => c.AcademicYear.Start).Returns(date);

            NewService(internalDataCacheMock.Object).Start().Should().Be(date);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(12)]
        public void GetReturnPeriod(int returnPeriod)
        {
            var externalDataCacheMock = new Mock<IExternalDataCache>();

            externalDataCacheMock.SetupGet(c => c.ReturnPeriod).Returns(returnPeriod);

            NewService(externalDataCache: externalDataCacheMock.Object).ReturnPeriod().Should().Be(returnPeriod);
        }

        private AcademicYearDataService NewService(IInternalDataCache internalDataCache = null, IExternalDataCache externalDataCache = null)
        {
            return new AcademicYearDataService(internalDataCache, externalDataCache);
        }
    }
}
