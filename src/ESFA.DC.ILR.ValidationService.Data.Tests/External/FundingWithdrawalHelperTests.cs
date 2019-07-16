using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using Moq;
using System;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Data.Tests.External
{
    /// <summary>
    /// the funding withdrawl helper fixture
    /// </summary>
    public class FundingWithdrawalHelperTests
    {
        /// <summary>
        /// Funding withdrawal helper, is current meets expectation.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="start">The start.</param>
        /// <param name="lastNS">The last ns.</param>
        /// <param name="end">The end.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData("2017-07-31", "2013-08-01", "2017-07-31", null, true)] // last new start in range
        [InlineData("2017-07-31", "2013-08-01", "2017-07-31", "2017-08-01", true)] // last new start with end in range
        [InlineData("2013-08-01", "2013-08-01", "2017-07-31", null, true)] // start in range
        [InlineData("2017-07-31", "2013-08-01", null, "2017-07-31", true)] // end in range
        [InlineData("2017-08-01", "2013-08-01", "2017-07-31", null, false)] // last new start out of range
        [InlineData("2013-07-31", "2013-08-01", null, null, false)] // start out of range
        [InlineData("2017-08-01", "2013-08-01", null, "2017-07-31", false)] // end out of range
        public void FundingWithdrawalHelperIsCurrentMeetsExpectation(string candidate, string start, string lastNS, string end, bool expectation)
        {
            // arrange
            var candiDate = DateTime.Parse(candidate);
            var startDate = DateTime.Parse(start);
            var lastNSDate = GetNullableDate(lastNS);

            var sut = new Mock<ISupportFundingWithdrawal>();
            sut
                .SetupGet(x => x.StartDate)
                .Returns(startDate);
            sut
                .SetupGet(x => x.EndDate)
                .Returns(GetNullableDate(end));

            // act
            var result = FundingWithdrawalHelper.IsCurrent(sut.Object, candiDate, lastNSDate);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Gets the nullable date.
        /// a test helper...
        /// </summary>
        /// <param name="theDate">The date.</param>
        /// <returns>a nullable date time</returns>
        public DateTime? GetNullableDate(string theDate) =>
            string.IsNullOrWhiteSpace(theDate) ? (DateTime?)null : DateTime.Parse(theDate);
    }
}
