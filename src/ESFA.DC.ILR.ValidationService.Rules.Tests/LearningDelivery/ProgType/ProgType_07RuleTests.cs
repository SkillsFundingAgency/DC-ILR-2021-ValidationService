using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.ProgType;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.ProgType
{
    public class ProgType_07RuleTests
    {
        [Fact]
        public void RuleName()
        {
            var sut = NewRule();

            var result = sut.RuleName;

            Assert.Equal("ProgType_07", result);
        }

        [Fact]
        public void ConditionMetWithNullLearningDeliveryReturnsTrue()
        {
            var sut = NewRule();

            var result = sut.ConditionMet(null);

            Assert.True(result);
        }

        [Fact]
        public void ConditionMetWithLearningDeliveryContainingNullFundModelReturnsFalse()
        {
            var sut = NewRule();
            var mockDelivery = new Mock<ILearningDelivery>();

            var result = sut.ConditionMet(mockDelivery.Object);

            Assert.False(result);
        }

        [Theory]
        [InlineData("2017-08-01", true)]
        [InlineData("2016-09-01", true)]
        [InlineData("2017-01-01", true)]
        [InlineData("2017-04-01", true)]
        [InlineData("2015-04-01", false)]
        [InlineData("2015-07-31", false)]
        [InlineData("2015-08-01", true)]
        public void IsViableMeetsExpectation(string candidate, bool expectation)
        {
            var sut = NewRule();
            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.LearnStartDate)
                .Returns(DateTime.Parse(candidate));

            var result = sut.IsViable(mockDelivery.Object);

            Assert.Equal(expectation, result);
        }

        [Theory]
        [InlineData(null, false)]
        [InlineData(TypeOfLearningProgramme.Traineeship, true)]
        [InlineData(TypeOfLearningProgramme.AdvancedLevelApprenticeship, false)]
        [InlineData(TypeOfLearningProgramme.ApprenticeshipStandard, false)]
        [InlineData(TypeOfLearningProgramme.HigherApprenticeshipLevel4, false)]
        [InlineData(TypeOfLearningProgramme.HigherApprenticeshipLevel5, false)]
        [InlineData(TypeOfLearningProgramme.HigherApprenticeshipLevel6, false)]
        [InlineData(TypeOfLearningProgramme.HigherApprenticeshipLevel7Plus, false)]
        [InlineData(TypeOfLearningProgramme.IntermediateLevelApprenticeship, false)]
        public void IsTraineeMeetsExpectation(int? candidate, bool expectation)
        {
            var sut = NewRule();
            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.ProgTypeNullable)
                .Returns(candidate);

            var result = sut.IsTrainee(mockDelivery.Object);

            Assert.Equal(expectation, result);
        }

        [Theory]
        [InlineData(AimTypes.ProgrammeAim, true)]
        [InlineData(AimTypes.AimNotPartOfAProgramme, false)]
        [InlineData(AimTypes.ComponentAimInAProgramme, false)]
        [InlineData(AimTypes.CoreAim16To19ExcludingApprenticeships, false)]
        public void IsInAProgrammeMeetsExpectation(int candidate, bool expectation)
        {
            var sut = NewRule();
            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.AimType)
                .Returns(candidate);

            var result = sut.IsInAProgramme(mockDelivery.Object);

            Assert.Equal(expectation, result);
        }

        [Theory]
        [InlineData("2017-08-01", "2017-09-30", true)]
        [InlineData("2016-09-01", "2017-09-30", false)]
        [InlineData("2017-01-01", "2017-06-30", true)]
        [InlineData("2017-02-01", "2017-07-31", true)]
        [InlineData("2017-02-26", "2017-09-30", false)]
        [InlineData("2017-03-14", "2017-09-30", false)]
        [InlineData("2017-03-31", "2017-10-01", false)]
        [InlineData("2017-03-31", "2017-09-30", true)]
        [InlineData("2017-04-01", "2017-09-30", true)]
        [InlineData("2017-04-01", "2017-10-02", false)]
        [InlineData("2015-04-01", "2017-10-02", false)]
        [InlineData("2015-07-31", "2015-10-01", true)]
        [InlineData("2015-08-01", "2016-02-01", true)]
        [InlineData("2016-02-01", "2016-08-01", true)]
        [InlineData("2016-02-01", "2016-08-02", false)]
        [InlineData("2015-08-01", "2016-01-30", true)]
        public void ConditionMetWithLearningDeliveriesContainingFundModelsMeetsExpectation(string startDate, string endDate, bool expectation)
        {
            var sut = NewRule();
            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.LearnStartDate)
                .Returns(DateTime.Parse(startDate));

            mockDelivery
                .SetupGet(y => y.LearnPlanEndDate)
                .Returns(DateTime.Parse(endDate));

            var result = sut.ConditionMet(mockDelivery.Object);

            Assert.Equal(expectation, result);
        }

        [Theory]
        [InlineData("2016-08-01", "2017-09-30")]
        [InlineData("2016-01-01", "2017-06-30")]
        [InlineData("2016-02-01", "2017-07-31")]
        [InlineData("2015-08-01", "2017-07-31")]
        public void InvalidItemRaisesValidationMessage(string startDate, string endDate)
        {
            const string LearnRefNumber = "123456789X";

            var testStartDate = DateTime.Parse(startDate);
            var testEndDate = DateTime.Parse(endDate);

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.LearnStartDate)
                .Returns(testStartDate);
            mockDelivery
                .SetupGet(y => y.LearnPlanEndDate)
                .Returns(testEndDate);
            mockDelivery
                .SetupGet(y => y.ProgTypeNullable)
                .Returns(TypeOfLearningProgramme.Traineeship);
            mockDelivery
                .SetupGet(y => y.AimType)
                .Returns(AimTypes.ProgrammeAim);

            var deliveries = new List<ILearningDelivery>();
            deliveries.Add(mockDelivery.Object);

            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries);

            var mockHandler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            mockHandler.Setup(x => x.Handle(
                Moq.It.Is<string>(y => y == ProgType_07Rule.Name),
                Moq.It.Is<string>(y => y == LearnRefNumber),
                0,
                Moq.It.IsAny<IEnumerable<IErrorMessageParameter>>()));
            mockHandler
                .Setup(x => x.BuildErrorMessageParameter(
                    Moq.It.Is<string>(y => y == "LearnPlanEndDate"),
                    Moq.It.Is<DateTime>(y => y == testEndDate)))
                .Returns(new Mock<IErrorMessageParameter>().Object);

            mockHandler
                .Setup(x => x.BuildErrorMessageParameter(
                    Moq.It.Is<string>(y => y == "LearnStartDate"),
                    Moq.It.Is<DateTime>(y => y == testStartDate)))
                .Returns(new Mock<IErrorMessageParameter>().Object);

            var sut = new ProgType_07Rule(mockHandler.Object);

            sut.Validate(mockLearner.Object);

            mockHandler.VerifyAll();
        }

        [Theory]
        [InlineData("2017-08-01", "2017-09-30")]
        [InlineData("2017-01-01", "2017-06-30")]
        [InlineData("2017-02-01", "2017-07-31")]
        [InlineData("2015-02-01", "2017-07-31")]
        [InlineData("2015-07-31", "2017-07-31")]
        public void ValidItemDoesNotRaiseAValidationMessage(string startDate, string endDate)
        {
            const string LearnRefNumber = "123456789X";

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.LearnStartDate)
                .Returns(DateTime.Parse(startDate));
            mockDelivery
                .SetupGet(y => y.LearnPlanEndDate)
                .Returns(DateTime.Parse(endDate));
            mockDelivery
                .SetupGet(y => y.ProgTypeNullable)
                .Returns(TypeOfLearningProgramme.Traineeship);
            mockDelivery
                .SetupGet(y => y.AimType)
                .Returns(AimTypes.ProgrammeAim);

            var deliveries = new List<ILearningDelivery>();
            deliveries.Add(mockDelivery.Object);

            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries);

            var mockHandler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            var sut = new ProgType_07Rule(mockHandler.Object);

            sut.Validate(mockLearner.Object);

            mockHandler.VerifyAll();
        }

        public ProgType_07Rule NewRule()
        {
            var mock = new Mock<IValidationErrorHandler>();

            return new ProgType_07Rule(mock.Object);
        }
    }
}
