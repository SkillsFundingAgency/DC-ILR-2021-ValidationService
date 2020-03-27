using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Internal.AcademicYear.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
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
        private static readonly DateTime TestThreshold = DateTime.Parse("2020-07-31");

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
            Assert.Equal(RuleNameConstants.DateEmpStatApp_01, result);
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
        /// Has qualifying employment status meets expectation
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="yearEnd">The year end.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData("2018-08-14", "2017-07-31", true)]
        [InlineData("2018-07-31", "2018-07-31", false)]
        [InlineData("2018-11-18", "2018-07-31", true)]
        [InlineData("2019-04-02", "2019-07-31", false)]
        [InlineData("2019-12-11", "2020-07-31", false)]
        public void HasDisqualifyingEmploymentStatusDateMeetsExpectation(string candidate, string yearEnd, bool expectation)
        {
            // arrange
            var status = new Mock<ILearnerEmploymentStatus>();
            status
                .SetupGet(y => y.DateEmpStatApp)
                .Returns(DateTime.Parse(candidate));

            var thresholdDate = DateTime.Parse(yearEnd);
            var sut = NewRule(thresholdDate);

            // act
            var result = sut.HasDisqualifyingEmploymentStatusDate(status.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Invalid item raises validation message.
        /// </summary>
        /// <param name="empStart">The learn start.</param>
        /// <param name="yearEnd">The current year end.</param>
        [Theory]
        [InlineData("2018-08-01", "2018-07-31")]
        public void InvalidItemRaisesValidationMessage(string empStart, string yearEnd)
        {
            // arrange
            const string LearnRefNumber = "123456789X";

            var empStartDate = DateTime.Parse(empStart);
            var yearEndDate = DateTime.Parse(yearEnd);

            var mockStatus = new Mock<ILearnerEmploymentStatus>();
            mockStatus
                .SetupGet(y => y.DateEmpStatApp)
                .Returns(empStartDate);

            var statii = Collection.Empty<ILearnerEmploymentStatus>();
            statii.Add(mockStatus.Object);

            var learner = new Mock<ILearner>();
            learner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            learner
                .SetupGet(x => x.LearnerEmploymentStatuses)
                .Returns(statii.AsSafeReadOnlyList());

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            handler
                .Setup(x => x.Handle("DateEmpStatApp_01", LearnRefNumber, null, Moq.It.IsAny<IEnumerable<IErrorMessageParameter>>()));
            handler
                .Setup(x => x.BuildErrorMessageParameter("DateEmpStatApp", AbstractRule.AsRequiredCultureDate(empStartDate)))
                .Returns(new Mock<IErrorMessageParameter>().Object);

            var yearData = new Mock<IAcademicYearDataService>(MockBehavior.Strict);
            yearData
                .Setup(x => x.End())
                .Returns(yearEndDate);

            var sut = new DateEmpStatApp_01Rule(handler.Object, yearData.Object);

            // act
            sut.Validate(learner.Object);

            // assert
            handler.VerifyAll();
            yearData.VerifyAll();
        }

        /// <summary>
        /// Valid item does not raise validation message.
        /// </summary>
        /// <param name="empStart">The emp start.</param>
        /// <param name="yearEnd">The current year end.</param>
        [Theory]
        [InlineData("2018-11-18", "2019-07-31")]
        [InlineData("2019-04-02", "2019-07-31")]
        [InlineData("2019-12-11", "2020-07-31")]
        public void ValidItemDoesNotRaiseValidationMessage(string empStart, string yearEnd)
        {
            // arrange
            const string LearnRefNumber = "123456789X";

            var empStartDate = DateTime.Parse(empStart);
            var yearEndDate = DateTime.Parse(yearEnd);

            var mockStatus = new Mock<ILearnerEmploymentStatus>();
            mockStatus
                .SetupGet(y => y.DateEmpStatApp)
                .Returns(empStartDate);

            var statii = Collection.Empty<ILearnerEmploymentStatus>();
            statii.Add(mockStatus.Object);

            var learner = new Mock<ILearner>();
            learner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            learner
                .SetupGet(x => x.LearnerEmploymentStatuses)
                .Returns(statii.AsSafeReadOnlyList());

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var yearData = new Mock<IAcademicYearDataService>(MockBehavior.Strict);
            yearData
                .Setup(x => x.End())
                .Returns(yearEndDate);

            var sut = new DateEmpStatApp_01Rule(handler.Object, yearData.Object);

            // act
            sut.Validate(learner.Object);

            // assert
            handler.VerifyAll();
            yearData.VerifyAll();
        }

        /// <summary>
        /// New rule.
        /// </summary>
        /// <param name="yearEnd">The year end.</param>
        /// <returns>
        /// a constructed and mocked up validation rule
        /// </returns>
        public DateEmpStatApp_01Rule NewRule(DateTime? yearEnd = null)
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var yeardata = new Mock<IAcademicYearDataService>(MockBehavior.Strict);
            yeardata
                .Setup(x => x.End())
                .Returns(yearEnd ?? TestThreshold);

            return new DateEmpStatApp_01Rule(handler.Object, yeardata.Object);
        }
    }
}
