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

        /// <summary>
        /// Get academic year of learning date meets expectation.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="forThisDate">For this date.</param>
        /// <param name="expectation">The expectation.</param>
        [Theory]
        [InlineData("2017-08-26", AcademicYearDates.PreviousYearEnd, "2016-07-31")]
        [InlineData("2017-08-26", AcademicYearDates.Commencement, "2016-08-01")]
        [InlineData("2017-08-26", AcademicYearDates.August31, "2017-08-31")]
        [InlineData("2017-08-26", AcademicYearDates.CurrentYearEnd, "2018-07-31")]
        [InlineData("2017-08-26", AcademicYearDates.NextYearCommencement, "2018-08-01")]
        [InlineData("2017-08-31", AcademicYearDates.PreviousYearEnd, "2016-07-31")]
        [InlineData("2017-08-31", AcademicYearDates.Commencement, "2016-08-01")]
        [InlineData("2017-08-31", AcademicYearDates.August31, "2017-08-31")]
        [InlineData("2017-08-31", AcademicYearDates.CurrentYearEnd, "2018-07-31")]
        [InlineData("2017-08-31", AcademicYearDates.NextYearCommencement, "2018-08-01")]
        [InlineData("2017-09-01", AcademicYearDates.PreviousYearEnd, "2017-07-31")]
        [InlineData("2017-09-01", AcademicYearDates.Commencement, "2017-08-01")]
        [InlineData("2017-09-01", AcademicYearDates.August31, "2017-08-31")]
        [InlineData("2017-09-01", AcademicYearDates.CurrentYearEnd, "2018-07-31")]
        [InlineData("2017-09-01", AcademicYearDates.NextYearCommencement, "2018-08-01")]
        [InlineData("2018-02-06", AcademicYearDates.PreviousYearEnd, "2017-07-31")]
        [InlineData("2018-02-06", AcademicYearDates.Commencement, "2017-08-01")]
        [InlineData("2018-02-06", AcademicYearDates.August31, "2017-08-31")]
        [InlineData("2018-02-06", AcademicYearDates.CurrentYearEnd, "2018-07-31")]
        [InlineData("2018-02-06", AcademicYearDates.NextYearCommencement, "2018-08-01")]
        [InlineData("2018-07-31", AcademicYearDates.PreviousYearEnd, "2017-07-31")]
        [InlineData("2018-07-31", AcademicYearDates.Commencement, "2017-08-01")]
        [InlineData("2018-07-31", AcademicYearDates.August31, "2017-08-31")]
        [InlineData("2018-07-31", AcademicYearDates.CurrentYearEnd, "2018-07-31")]
        [InlineData("2018-07-31", AcademicYearDates.NextYearCommencement, "2018-08-01")]
        [InlineData("2018-08-01", AcademicYearDates.August31, "2018-08-31")]
        public void GetAcademicYearOfLearningDateMeetsExpectation(string candidate, AcademicYearDates forThisDate, string expectation)
        {
            // arrange
            var sut = NewService();

            var testDate = DateTime.Parse(candidate);

            // act
            var result = sut.GetAcademicYearOfLearningDate(testDate, forThisDate);

            // assert
            result.Should().Be(DateTime.Parse(expectation));
        }

        private AcademicYearDataService NewService(IInternalDataCache internalDataCache = null)
        {
            return new AcademicYearDataService(internalDataCache);
        }
    }
}
