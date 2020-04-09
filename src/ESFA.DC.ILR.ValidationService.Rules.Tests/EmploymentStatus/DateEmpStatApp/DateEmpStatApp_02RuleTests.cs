using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.EmploymentStatus.DateEmpStatApp;
using ESFA.DC.ILR.ValidationService.Utility;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.EmploymentStatus.DateEmpStatApp
{
    public class DateEmpStatApp_02RuleTests
    {
        /// <summary>
        /// New rule with null message handler throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullMessageHandlerThrows()
        {
            // arrange / act / assert
            Assert.Throws<ArgumentNullException>(() => new DateEmpStatApp_02Rule(null));
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
            Assert.Equal("DateEmpStatApp_02", result);
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
            Assert.Equal(DateEmpStatApp_02Rule.Name, result);
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
        /// Last inviable date meets expectation.
        /// </summary>
        [Fact]
        public void LastInviableDateMeetsExpectation()
        {
            // arrange
            var sut = NewRule();

            // act
            var result = sut.LastInviableDate;

            // assert
            Assert.Equal(DateTime.Parse("1990-07-31"), result);
        }

        /// <summary>
        /// Has qualifying employment status meets expectation
        /// </summary>
        /// <param name="startDate">The start date.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData("1990-04-02", false)]
        [InlineData("1990-07-31", false)]
        [InlineData("1990-08-01", true)]
        [InlineData("2019-04-02", true)]
        public void HasQualifyingEmploymentStatusMeetsExpectation(string startDate, bool expectation)
        {
            // arrange
            var sut = NewRule();

            var mockStatus = new Mock<ILearnerEmploymentStatus>();
            mockStatus
                .SetupGet(y => y.DateEmpStatApp)
                .Returns(DateTime.Parse(startDate));

            // act
            var result = sut.HasQualifyingEmploymentStatus(mockStatus.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Invalid item raises validation message.
        /// </summary>
        /// <param name="empStart">The learn start.</param>
        [Theory]
        [InlineData("1990-04-02")]
        [InlineData("1990-07-31")]
        public void InvalidItemRaisesValidationMessage(string empStart)
        {
            // arrange
            const string LearnRefNumber = "123456789X";

            var empStartDate = DateTime.Parse(empStart);

            var mockStatus = new Mock<ILearnerEmploymentStatus>();
            mockStatus
                .SetupGet(y => y.DateEmpStatApp)
                .Returns(empStartDate);

            var statii = new List<ILearnerEmploymentStatus>();
            statii.Add(mockStatus.Object);

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            mockLearner
                .SetupGet(x => x.LearnerEmploymentStatuses)
                .Returns(statii);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            handler
                .Setup(x => x.Handle(
                    Moq.It.Is<string>(y => y == DateEmpStatApp_02Rule.Name),
                    Moq.It.Is<string>(y => y == LearnRefNumber),
                    null,
                    Moq.It.IsAny<IEnumerable<IErrorMessageParameter>>()));
            handler
                .Setup(x => x.BuildErrorMessageParameter(
                    Moq.It.Is<string>(y => y == "DateEmpStatApp"),
                    empStartDate))
                .Returns(new Mock<IErrorMessageParameter>().Object);

            var sut = new DateEmpStatApp_02Rule(handler.Object);

            // act
            sut.Validate(mockLearner.Object);

            // assert
            handler.VerifyAll();
        }

        /// <summary>
        /// Valid item does not raise validation message.
        /// </summary>
        /// <param name="empStart">The emp start.</param>
        [Theory]
        [InlineData("1990-08-01")]
        [InlineData("2018-11-18")]
        [InlineData("2019-04-02")]
        [InlineData("2019-12-11")]
        public void ValidItemDoesNotRaiseValidationMessage(string empStart)
        {
            // arrange
            const string LearnRefNumber = "123456789X";

            var empStartDate = DateTime.Parse(empStart);

            var mockStatus = new Mock<ILearnerEmploymentStatus>();
            mockStatus
                .SetupGet(y => y.DateEmpStatApp)
                .Returns(empStartDate);

            var statii = new List<ILearnerEmploymentStatus>();
            statii.Add(mockStatus.Object);

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            mockLearner
                .SetupGet(x => x.LearnerEmploymentStatuses)
                .Returns(statii);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            var sut = new DateEmpStatApp_02Rule(handler.Object);

            // act
            sut.Validate(mockLearner.Object);

            // assert
            handler.VerifyAll();
        }

        /// <summary>
        /// New rule.
        /// </summary>
        /// <returns>a constructed and mocked up validation rule</returns>
        public DateEmpStatApp_02Rule NewRule()
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            return new DateEmpStatApp_02Rule(handler.Object);
        }
    }
}
