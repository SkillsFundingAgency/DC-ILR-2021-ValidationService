using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.ProgType;
using Moq;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.ProgType
{
    public class ProgType_06RuleTests
    {
        [Fact]
        public void RuleName()
        {
            var sut = NewRule();

            var result = sut.RuleName;

            Assert.Equal("ProgType_06", result);
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
        [InlineData(FundModels.EuropeanSocialFund, false)]
        [InlineData(FundModels.CommunityLearning, false)]
        [InlineData(FundModels.Age16To19ExcludingApprenticeships, false)]
        [InlineData(FundModels.AdultSkills, false)]
        [InlineData(FundModels.Other16To19, false)]
        [InlineData(FundModels.NotFundedByESFA, true)]
        [InlineData(FundModels.OtherAdult, true)]
        [InlineData(FundModels.ApprenticeshipsFrom1May2017, true)]
        public void ConditionMetWithLearningDeliveriesContainingFundModelsMeetsExpectation(int fundModel, bool expectation)
        {
            var sut = NewRule();
            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.FundModel)
                .Returns(fundModel);

            var result = sut.ConditionMet(mockDelivery.Object);

            Assert.Equal(expectation, result);
        }

        [Theory]
        [InlineData(FundModels.EuropeanSocialFund)]
        [InlineData(FundModels.CommunityLearning)]
        [InlineData(FundModels.Age16To19ExcludingApprenticeships)]
        [InlineData(FundModels.AdultSkills)]
        [InlineData(FundModels.Other16To19)]
        public void InvalidItemRaisesValidationMessage(int fundModel)
        {
            const string LearnRefNumber = "123456789X";

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.FundModel)
                .Returns(fundModel);
            mockDelivery
                .SetupGet(y => y.ProgTypeNullable)
                .Returns(TypeOfLearningProgramme.ApprenticeshipStandard);

            var deliveries = new List<ILearningDelivery>();
            deliveries.Add(mockDelivery.Object);

            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries);

            var mockHandler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            mockHandler.Setup(x => x.Handle(
                Moq.It.Is<string>(y => y == ProgType_06Rule.Name),
                Moq.It.Is<string>(y => y == LearnRefNumber),
                0,
                Moq.It.IsAny<IEnumerable<IErrorMessageParameter>>()));

            mockHandler
                .Setup(x => x.BuildErrorMessageParameter(
                    Moq.It.Is<string>(y => y == "FundModel"),
                    Moq.It.Is<int>(y => y == fundModel)))
                .Returns(new Mock<IErrorMessageParameter>().Object);

            var sut = new ProgType_06Rule(mockHandler.Object);

            sut.Validate(mockLearner.Object);

            mockHandler.VerifyAll();
        }

        [Theory]
        [InlineData(FundModels.NotFundedByESFA, 26)]
        [InlineData(FundModels.OtherAdult, 27)]
        [InlineData(FundModels.ApprenticeshipsFrom1May2017, 28)]
        [InlineData(FundModels.NotFundedByESFA, TypeOfLearningProgramme.ApprenticeshipStandard)]
        [InlineData(FundModels.OtherAdult, TypeOfLearningProgramme.ApprenticeshipStandard)]
        [InlineData(FundModels.ApprenticeshipsFrom1May2017, TypeOfLearningProgramme.ApprenticeshipStandard)]
        [InlineData(FundModels.EuropeanSocialFund, null)]
        [InlineData(FundModels.CommunityLearning, null)]
        [InlineData(FundModels.Age16To19ExcludingApprenticeships, null)]
        [InlineData(FundModels.AdultSkills, null)]
        [InlineData(FundModels.Other16To19, null)]
        public void ValidItemDoesNotRaiseAValidationMessage(int fundModel, int? programmeType)
        {
            const string LearnRefNumber = "123456789X";

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.FundModel)
                .Returns(fundModel);
            mockDelivery
                .SetupGet(y => y.ProgTypeNullable)
                .Returns(programmeType);

            var deliveries = new List<ILearningDelivery>();
            deliveries.Add(mockDelivery.Object);

            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries);

            var mockHandler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            var sut = new ProgType_06Rule(mockHandler.Object);

            sut.Validate(mockLearner.Object);

            mockHandler.VerifyAll();
        }

        public ProgType_06Rule NewRule()
        {
            var mock = new Mock<IValidationErrorHandler>();

            return new ProgType_06Rule(mock.Object);
        }
    }
}
