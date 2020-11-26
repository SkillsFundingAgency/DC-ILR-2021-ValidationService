using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Internal.AcademicYear.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.EmploymentStatus.DateEmpStatApp;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.EmploymentStatus.DateEmpStatApp
{
    public class DateEmpStatApp_01RuleTests
    {
        private static readonly DateTime TestThreshold = DateTime.Parse("2020-07-31");

        [Fact]
        public void RuleName()
        {
            var sut = NewRule();

            var result = sut.RuleName;

            Assert.Equal("DateEmpStatApp_01", result);
        }

        [Theory]
        [InlineData("2018-08-14", "2017-07-31", true)]
        [InlineData("2018-07-31", "2018-07-31", false)]
        [InlineData("2018-11-18", "2018-07-31", true)]
        [InlineData("2019-04-02", "2019-07-31", false)]
        [InlineData("2019-12-11", "2020-07-31", false)]
        public void HasDisqualifyingEmploymentStatusDateMeetsExpectation(string candidate, string yearEnd, bool expectation)
        {
            var status = new Mock<ILearnerEmploymentStatus>();
            status
                .SetupGet(y => y.DateEmpStatApp)
                .Returns(DateTime.Parse(candidate));

            var thresholdDate = DateTime.Parse(yearEnd);
            var sut = NewRule(thresholdDate);

            var result = sut.HasDisqualifyingEmploymentStatusDate(status.Object);

            Assert.Equal(expectation, result);
        }

        [Theory]
        [InlineData("2018-08-01", "2018-07-31")]
        public void InvalidItemRaisesValidationMessage(string empStart, string yearEnd)
        {
            const string LearnRefNumber = "123456789X";

            var empStartDate = DateTime.Parse(empStart);
            var yearEndDate = DateTime.Parse(yearEnd);

            var mockStatus = new Mock<ILearnerEmploymentStatus>();
            mockStatus
                .SetupGet(y => y.DateEmpStatApp)
                .Returns(empStartDate);

            var statii = new List<ILearnerEmploymentStatus>();
            statii.Add(mockStatus.Object);

            var learner = new Mock<ILearner>();
            learner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            learner
                .SetupGet(x => x.LearnerEmploymentStatuses)
                .Returns(statii);

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

            sut.Validate(learner.Object);

            handler.VerifyAll();
            yearData.VerifyAll();
        }

        [Theory]
        [InlineData("2018-11-18", "2019-07-31")]
        [InlineData("2019-04-02", "2019-07-31")]
        [InlineData("2019-12-11", "2020-07-31")]
        public void ValidItemDoesNotRaiseValidationMessage(string empStart, string yearEnd)
        {
            const string LearnRefNumber = "123456789X";

            var empStartDate = DateTime.Parse(empStart);
            var yearEndDate = DateTime.Parse(yearEnd);

            var mockStatus = new Mock<ILearnerEmploymentStatus>();
            mockStatus
                .SetupGet(y => y.DateEmpStatApp)
                .Returns(empStartDate);

            var statii = new List<ILearnerEmploymentStatus>();
            statii.Add(mockStatus.Object);

            var learner = new Mock<ILearner>();
            learner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            learner
                .SetupGet(x => x.LearnerEmploymentStatuses)
                .Returns(statii);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var yearData = new Mock<IAcademicYearDataService>(MockBehavior.Strict);
            yearData
                .Setup(x => x.End())
                .Returns(yearEndDate);

            var sut = new DateEmpStatApp_01Rule(handler.Object, yearData.Object);

            sut.Validate(learner.Object);

            handler.VerifyAll();
            yearData.VerifyAll();
        }

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
