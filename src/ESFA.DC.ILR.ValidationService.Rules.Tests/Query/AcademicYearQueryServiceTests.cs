using System;
using ESFA.DC.ILR.ValidationService.Data.Internal.AcademicYear.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Query;
using FluentAssertions;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Query
{
    public class AcademicYearQueryServiceTests
    {
        [Theory]
        [InlineData(1, 2017, "2017-01-27")]
        [InlineData(2, 2017, "2017-02-24")]
        [InlineData(3, 2017, "2017-03-31")]
        [InlineData(4, 2017, "2017-04-28")]
        [InlineData(5, 2017, "2017-05-26")]
        [InlineData(12, 2016, "2016-12-30")]
        [InlineData(11, 2016, "2016-11-25")]
        [InlineData(6, 2016, "2016-06-24")]
        public void LastFridayInMonth(int month, int year, string expectedDate)
        {
            var date = new DateTime(year, month, 1);
            var expectedValue = DateTime.Parse(expectedDate);

            var academicYearQueryService = NewService();

            while (date.Month == month)
            {
                academicYearQueryService.LastFridayInMonth(date).Should().Be(expectedValue);
                date = date.AddDays(1);
            }
        }

        [Theory]
        [InlineData("2017-1-1", "2017-6-30")]
        [InlineData("2017-8-31", "2017-6-30")]
        [InlineData("2017-9-1", "2018-6-29")]
        public void LastFridayInJuneForDateInAcademicYear(string inputDate, string expectedDate)
        {
            var inputDateTime = DateTime.Parse(inputDate);
            var expectedDateTime = DateTime.Parse(expectedDate);

            NewService().LastFridayInJuneForDateInAcademicYear(inputDateTime).Should().Be(expectedDateTime);
        }

        [Fact]
        public void DateIsInPrevAcademicYear_True()
        {
            var date = new DateTime(2018, 7, 1);
            var academicYearStart = new DateTime(2018, 8, 1);

            NewService().DateIsInPrevAcademicYear(date, academicYearStart).Should().BeTrue();
        }

        [Fact]
        public void DateIsInPrevAcademicYear_False()
        {
            var date = new DateTime(2018, 9, 1);
            var academicYearStart = new DateTime(2018, 8, 1);

            NewService().DateIsInPrevAcademicYear(date, academicYearStart).Should().BeFalse();
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

        [Theory]
        [InlineData("0001-01-02", AcademicYearDates.Commencement, "0001-08-1")]
        [InlineData("0001-01-02", AcademicYearDates.PreviousYearEnd, "0001-07-31")]
        [InlineData("0001-01-02", AcademicYearDates.August31, "0001-08-31")]
        [InlineData("0001-01-02", AcademicYearDates.CurrentYearEnd, "0001-07-31")]
        [InlineData("0001-01-02", AcademicYearDates.NextYearCommencement, "0001-08-1")]
        public void GetAcademicYearFor_WrongYear(string candidate, AcademicYearDates forThisDate, string expectation)
        {
            var testDate = DateTime.Parse(candidate);
            var expectedResult = DateTime.Parse(expectation);

            var sut = NewService();
            var result = sut.GetAcademicYearOfLearningDate(testDate, forThisDate);

            result.Should().Be(expectedResult);
        }

        private AcademicYearQueryService NewService()
        {
            return new AcademicYearQueryService();
        }
    }
}
