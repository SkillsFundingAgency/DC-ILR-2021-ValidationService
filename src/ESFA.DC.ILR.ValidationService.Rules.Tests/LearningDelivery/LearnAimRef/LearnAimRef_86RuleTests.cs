using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnAimRef;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using Moq;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.LearnAimRef
{
    public class LearnAimRef_86RuleTests
    {
        [Fact]
        public void RuleName()
        {
            var sut = NewRule();

            var result = sut.RuleName;

            Assert.Equal("LearnAimRef_86", result);
        }

        [Theory]
        [InlineData(AimTypes.References.ESFLearnerStartandAssessment, false)]
        [InlineData(AimTypes.References.IndustryPlacement, false)]
        [InlineData(AimTypes.References.SupportedInternship16To19, false)]
        [InlineData(AimTypes.References.WorkExperience, true)]
        [InlineData(AimTypes.References.WorkPlacement0To49Hours, false)]
        [InlineData(AimTypes.References.WorkPlacement100To199Hours, false)]
        [InlineData(AimTypes.References.WorkPlacement200To499Hours, false)]
        [InlineData(AimTypes.References.WorkPlacement500PlusHours, false)]
        [InlineData(AimTypes.References.WorkPlacement50To99Hours, false)]
        public void IsWorkExperienceMeetsExpectation(string candidate, bool expectation)
        {
            var sut = NewRule();
            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.LearnAimRef)
                .Returns(candidate);

            var result = sut.IsWorkExperience(mockDelivery.Object);

            Assert.Equal(expectation, result);
        }

        [Theory]
        [InlineData(ProgTypes.AdvancedLevelApprenticeship)]
        [InlineData(ProgTypes.ApprenticeshipStandard)]
        [InlineData(ProgTypes.HigherApprenticeshipLevel4)]
        [InlineData(ProgTypes.HigherApprenticeshipLevel5)]
        public void InvalidItemRaisesValidationMessage(int candidate)
        {
            const string learnRefNumber = "123456789X";

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.FundModel)
                .Returns(FundModels.AdultSkills);
            mockDelivery
                .SetupGet(y => y.ProgTypeNullable)
                .Returns(candidate);
            mockDelivery
                .SetupGet(y => y.LearnAimRef)
                .Returns(AimTypes.References.WorkExperience);

            var deliveries = new List<ILearningDelivery>();
            deliveries.Add(mockDelivery.Object);

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(learnRefNumber);
            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            handler
                .Setup(x => x.Handle("LearnAimRef_86", learnRefNumber, 0, Moq.It.IsAny<IEnumerable<IErrorMessageParameter>>()));
            handler
                .Setup(x => x.BuildErrorMessageParameter("ProgType", candidate))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter("LearnAimRef", AimTypes.References.WorkExperience))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter("FundModel", FundModels.AdultSkills))
                .Returns(new Mock<IErrorMessageParameter>().Object);

            var learningDeliveryFAMQS = new Mock<ILearningDeliveryFAMQueryService>(MockBehavior.Strict);
            learningDeliveryFAMQS
                .Setup(x => x.HasLearningDeliveryFAMCodeForType(
                    mockDelivery.Object.LearningDeliveryFAMs,
                    "LDM",
                    "347"))
                .Returns(false);

            var sut = new LearnAimRef_86Rule(handler.Object, learningDeliveryFAMQS.Object);

            sut.Validate(mockLearner.Object);

            handler.VerifyAll();
            learningDeliveryFAMQS.VerifyAll();
        }

        [Fact]
        public void ValidItemDoesNotRaiseValidationMessage()
        {
            const string learnRefNumber = "123456789X";

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
               .SetupGet(y => y.ProgTypeNullable)
               .Returns(24);
            mockDelivery
               .SetupGet(y => y.FundModel)
               .Returns(35);

            var deliveries = new List<ILearningDelivery>();
            deliveries.Add(mockDelivery.Object);

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(learnRefNumber);
            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            var learningDeliveryFAMQS = new Mock<ILearningDeliveryFAMQueryService>(MockBehavior.Strict);
            learningDeliveryFAMQS
                .Setup(x => x.HasLearningDeliveryFAMCodeForType(
                    mockDelivery.Object.LearningDeliveryFAMs,
                    "LDM",
                    "347"))
                .Returns(false);

            var sut = new LearnAimRef_86Rule(handler.Object, learningDeliveryFAMQS.Object);

            sut.Validate(mockLearner.Object);

            handler.VerifyAll();
            learningDeliveryFAMQS.VerifyAll();
        }

        public LearnAimRef_86Rule NewRule()
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var learningDeliveryFAMQS = new Mock<ILearningDeliveryFAMQueryService>(MockBehavior.Strict);

            return new LearnAimRef_86Rule(handler.Object, learningDeliveryFAMQS.Object);
        }
    }
}
