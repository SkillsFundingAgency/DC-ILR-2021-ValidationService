using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Internal.AcademicYear.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.EmploymentStatus.DateEmpStatApp;
using ESFA.DC.ILR.ValidationService.Utility;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.EmploymentStatus.DateEmpStatApp
{
    public class DateEmpStatApp_01RuleTests
    {
        /// <summary>
        /// New rule with null message handler throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullMessageHandlerThrows()
        {
            // arrange
            var yeardata = new Mock<IAcademicYearDataService>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new DateEmpStatApp_01Rule(null, yeardata.Object));
        }

        /// <summary>
        /// New rule with null derived data rule 07 throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullDerivedDataRule07Throws()
        {
            // arrange
            var mockHandler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new DateEmpStatApp_01Rule(mockHandler.Object, null));
        }

        /// <summary>
        /// Rule name 1, matches a literal.
        /// </summary>
        [Fact]
        public void RuleName1()
        {
            // arrange
            var sut = NewRule();

            // act
            var result = sut.RuleName;

            // assert
            Assert.Equal("DateEmpStatApp_01", result);
        }

        /// <summary>
        /// Rule name 2, matches the constant.
        /// </summary>
        [Fact]
        public void RuleName2()
        {
            // arrange
            var sut = NewRule();

            // act
            var result = sut.RuleName;

            // assert
            Assert.Equal(DateEmpStatApp_01Rule.Name, result);
        }

        /// <summary>
        /// Rule name 3 test, account for potential false positives.
        /// </summary>
        [Fact]
        public void RuleName3()
        {
            // arrange
            var sut = NewRule();

            // act
            var result = sut.RuleName;

            // assert
            Assert.NotEqual("SomeOtherRuleName_07", result);
        }

        /// <summary>
        /// Validate with null learner throws.
        /// </summary>
        [Fact]
        public void ValidateWithNullLearnerThrows()
        {
            // arrange
            var sut = NewRule();

            // act/assert
            Assert.Throws<ArgumentNullException>(() => sut.Validate(null));
        }

        /// <summary>
        /// Get year of learning commencement date meets expectation.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        [Theory]
        [InlineData("2017-08-26")]
        [InlineData("2017-08-31")]
        [InlineData("2017-09-01")]
        public void GetYearOfLearningCommencementDateMeetsExpectation(string candidate)
        {
            // arrange
            var testDate = DateTime.Parse(candidate);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var yearData = new Mock<IAcademicYearDataService>(MockBehavior.Strict);
            yearData
                .Setup(x => x.GetAcademicYearOfLearningDate(testDate, AcademicYearDates.NextYearCommencement))
                .Returns(testDate);

            var sut = new DateEmpStatApp_01Rule(handler.Object, yearData.Object);

            // act
            var result = sut.GetNextAcademicYearDate(testDate);

            // assert
            Assert.Equal(testDate, result);

            handler.VerifyAll();
            yearData.VerifyAll();
        }

        /// <summary>
        /// Has qualifying employment status meets expectation
        /// </summary>
        /// <param name="startDate">The start date.</param>
        /// <param name="candidate">The candidate.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData("2018-08-14", "2017-08-01", false)]
        [InlineData("2018-11-18", "2018-08-01", true)]
        [InlineData("2019-04-02", "2019-08-01", true)]
        [InlineData("2019-12-11", "2020-08-01", true)]
        public void HasQualifyingEmploymentStatusMeetsExpectation(string startDate, string candidate, bool expectation)
        {
            // arrange
            var sut = NewRule();

            var thresholdDate = DateTime.Parse(candidate);
            var mockStatus = new Mock<ILearnerEmploymentStatus>();
            mockStatus
                .SetupGet(y => y.DateEmpStatApp)
                .Returns(DateTime.Parse(startDate));

            // act
            var result = sut.HasQualifyingEmploymentStatus(mockStatus.Object, thresholdDate);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Invalid item raises validation message.
        /// </summary>
        /// <param name="empStart">The learn start.</param>
        /// <param name="nextYearStart">The previous year end.</param>
        [Theory]
        [InlineData("2018-08-14", "2017-08-01")]
        public void InvalidItemRaisesValidationMessage(string empStart, string nextYearStart)
        {
            // arrange
            const string LearnRefNumber = "123456789X";

            var todayDate = DateTime.Parse("2018-09-28");
            var empStartDate = DateTime.Parse(empStart);
            var nextYearStartDate = DateTime.Parse(nextYearStart);

            var mockStatus = new Mock<ILearnerEmploymentStatus>();
            mockStatus
                .SetupGet(y => y.DateEmpStatApp)
                .Returns(empStartDate);

            var statii = Collection.Empty<ILearnerEmploymentStatus>();
            statii.Add(mockStatus.Object);

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            mockLearner
                .SetupGet(x => x.LearnerEmploymentStatuses)
                .Returns(statii.AsSafeReadOnlyList());

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            handler
                .Setup(x => x.Handle(
                    Moq.It.Is<string>(y => y == DateEmpStatApp_01Rule.Name),
                    Moq.It.Is<string>(y => y == LearnRefNumber),
                    null,
                    Moq.It.IsAny<IEnumerable<IErrorMessageParameter>>()));
            handler
                .Setup(x => x.BuildErrorMessageParameter(
                    Moq.It.Is<string>(y => y == "DateEmpStatApp"),
                    empStartDate))
                .Returns(new Mock<IErrorMessageParameter>().Object);

            var yearData = new Mock<IAcademicYearDataService>(MockBehavior.Strict);
            yearData
                .Setup(x => x.End())
                .Returns(todayDate);
            yearData
                .Setup(x => x.GetAcademicYearOfLearningDate(todayDate, AcademicYearDates.NextYearCommencement))
                .Returns(nextYearStartDate);

            var sut = new DateEmpStatApp_01Rule(handler.Object, yearData.Object);

            // act
            sut.Validate(mockLearner.Object);

            // assert
            handler.VerifyAll();
            yearData.VerifyAll();
        }

        /// <summary>
        /// Valid item does not raise validation message.
        /// </summary>
        /// <param name="empStart">The emp start.</param>
        /// <param name="nextYearStart">The next year start.</param>
        [Theory]
        [InlineData("2018-11-18", "2018-08-01")]
        [InlineData("2019-04-02", "2019-08-01")]
        [InlineData("2019-12-11", "2020-08-01")]
        public void ValidItemDoesNotRaiseValidationMessage(string empStart, string nextYearStart)
        {
            // arrange
            const string LearnRefNumber = "123456789X";

            var todayDate = DateTime.Parse("2018-09-28");
            var empStartDate = DateTime.Parse(empStart);
            var nextYearStartDate = DateTime.Parse(nextYearStart);

            var mockStatus = new Mock<ILearnerEmploymentStatus>();
            mockStatus
                .SetupGet(y => y.DateEmpStatApp)
                .Returns(empStartDate);

            var statii = Collection.Empty<ILearnerEmploymentStatus>();
            statii.Add(mockStatus.Object);

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            mockLearner
                .SetupGet(x => x.LearnerEmploymentStatuses)
                .Returns(statii.AsSafeReadOnlyList());

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var yearData = new Mock<IAcademicYearDataService>(MockBehavior.Strict);
            yearData
                .Setup(x => x.End())
                .Returns(todayDate);
            yearData
                .Setup(x => x.GetAcademicYearOfLearningDate(todayDate, AcademicYearDates.NextYearCommencement))
                .Returns(nextYearStartDate);

            var sut = new DateEmpStatApp_01Rule(handler.Object, yearData.Object);

            // act
            sut.Validate(mockLearner.Object);

            // assert
            handler.VerifyAll();
            yearData.VerifyAll();
        }

        /// <summary>
        /// New rule.
        /// </summary>
        /// <returns>a constructed and mocked up validation rule</returns>
        public DateEmpStatApp_01Rule NewRule()
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var yeardata = new Mock<IAcademicYearDataService>(MockBehavior.Strict);

            return new DateEmpStatApp_01Rule(handler.Object, yeardata.Object);
        }
    }
}
