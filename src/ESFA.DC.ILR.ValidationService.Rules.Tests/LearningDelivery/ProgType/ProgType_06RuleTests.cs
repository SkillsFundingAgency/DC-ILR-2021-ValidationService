﻿using ESFA.DC.ILR.Model.Interface;
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
        [InlineData(TypeOfFunding.EuropeanSocialFund, false)]
        [InlineData(TypeOfFunding.CommunityLearning, false)]
        [InlineData(TypeOfFunding.Age16To19ExcludingApprenticeships, false)]
        [InlineData(TypeOfFunding.AdultSkills, false)]
        [InlineData(TypeOfFunding.Other16To19, false)]
        [InlineData(TypeOfFunding.NotFundedByESFA, true)]
        [InlineData(TypeOfFunding.OtherAdult, true)]
        [InlineData(TypeOfFunding.ApprenticeshipsFrom1May2017, true)]
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
        [InlineData(TypeOfFunding.EuropeanSocialFund)]
        [InlineData(TypeOfFunding.CommunityLearning)]
        [InlineData(TypeOfFunding.Age16To19ExcludingApprenticeships)]
        [InlineData(TypeOfFunding.AdultSkills)]
        [InlineData(TypeOfFunding.Other16To19)]
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
        [InlineData(TypeOfFunding.NotFundedByESFA, 26)]
        [InlineData(TypeOfFunding.OtherAdult, 27)]
        [InlineData(TypeOfFunding.ApprenticeshipsFrom1May2017, 28)]
        [InlineData(TypeOfFunding.NotFundedByESFA, TypeOfLearningProgramme.ApprenticeshipStandard)]
        [InlineData(TypeOfFunding.OtherAdult, TypeOfLearningProgramme.ApprenticeshipStandard)]
        [InlineData(TypeOfFunding.ApprenticeshipsFrom1May2017, TypeOfLearningProgramme.ApprenticeshipStandard)]
        [InlineData(TypeOfFunding.EuropeanSocialFund, null)]
        [InlineData(TypeOfFunding.CommunityLearning, null)]
        [InlineData(TypeOfFunding.Age16To19ExcludingApprenticeships, null)]
        [InlineData(TypeOfFunding.AdultSkills, null)]
        [InlineData(TypeOfFunding.Other16To19, null)]
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
