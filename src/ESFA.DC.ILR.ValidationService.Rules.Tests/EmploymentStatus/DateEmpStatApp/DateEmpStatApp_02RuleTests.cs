using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.EmploymentStatus.DateEmpStatApp;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.EmploymentStatus.DateEmpStatApp
{
    public class DateEmpStatApp_02RuleTests
    {
        [Fact]
        public void RuleName()
        {
            var sut = NewRule();

            var result = sut.RuleName;

            Assert.Equal("DateEmpStatApp_02", result);
        }

        [Fact]
        public void LastInviableDateMeetsExpectation()
        {
            var sut = NewRule();

            var result = sut.LastInviableDate;

            Assert.Equal(DateTime.Parse("1990-07-31"), result);
        }

        [Theory]
        [InlineData("1990-04-02", false)]
        [InlineData("1990-07-31", false)]
        [InlineData("1990-08-01", true)]
        [InlineData("2019-04-02", true)]
        public void HasQualifyingEmploymentStatusMeetsExpectation(string startDate, bool expectation)
        {
            var sut = NewRule();

            var mockStatus = new Mock<ILearnerEmploymentStatus>();
            mockStatus
                .SetupGet(y => y.DateEmpStatApp)
                .Returns(DateTime.Parse(startDate));

            var result = sut.HasQualifyingEmploymentStatus(mockStatus.Object);

            Assert.Equal(expectation, result);
        }

        [Theory]
        [InlineData("1990-04-02")]
        [InlineData("1990-07-31")]
        public void InvalidItemRaisesValidationMessage(string empStart)
        {
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

            sut.Validate(mockLearner.Object);

            handler.VerifyAll();
        }

        [Theory]
        [InlineData("1990-08-01")]
        [InlineData("2018-11-18")]
        [InlineData("2019-04-02")]
        [InlineData("2019-12-11")]
        public void ValidItemDoesNotRaiseValidationMessage(string empStart)
        {
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

            sut.Validate(mockLearner.Object);

            handler.VerifyAll();
        }

        public DateEmpStatApp_02Rule NewRule()
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            return new DateEmpStatApp_02Rule(handler.Object);
        }
    }
}
