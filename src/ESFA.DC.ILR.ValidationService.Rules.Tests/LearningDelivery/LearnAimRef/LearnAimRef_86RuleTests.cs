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
        [InlineData(TypeOfAim.References.ESFLearnerStartandAssessment, false)]
        [InlineData(TypeOfAim.References.IndustryPlacement, false)]
        [InlineData(TypeOfAim.References.SupportedInternship16To19, false)]
        [InlineData(TypeOfAim.References.WorkExperience, true)]
        [InlineData(TypeOfAim.References.WorkPlacement0To49Hours, false)]
        [InlineData(TypeOfAim.References.WorkPlacement100To199Hours, false)]
        [InlineData(TypeOfAim.References.WorkPlacement200To499Hours, false)]
        [InlineData(TypeOfAim.References.WorkPlacement500PlusHours, false)]
        [InlineData(TypeOfAim.References.WorkPlacement50To99Hours, false)]
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
        [InlineData(TypeOfLearningProgramme.AdvancedLevelApprenticeship)]
        [InlineData(TypeOfLearningProgramme.ApprenticeshipStandard)]
        [InlineData(TypeOfLearningProgramme.HigherApprenticeshipLevel4)]
        [InlineData(TypeOfLearningProgramme.HigherApprenticeshipLevel5)]
        public void InvalidItemRaisesValidationMessage(int candidate)
        {
            const string learnRefNumber = "123456789X";

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.FundModel)
                .Returns(TypeOfFunding.AdultSkills);
            mockDelivery
                .SetupGet(y => y.ProgTypeNullable)
                .Returns(candidate);
            mockDelivery
                .SetupGet(y => y.LearnAimRef)
                .Returns(TypeOfAim.References.WorkExperience);

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
                .Setup(x => x.BuildErrorMessageParameter("LearnAimRef", TypeOfAim.References.WorkExperience))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter("FundModel", TypeOfFunding.AdultSkills))
                .Returns(new Mock<IErrorMessageParameter>().Object);

            var commonChecks = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonChecks
                .Setup(x => x.IsSteelWorkerRedundancyTraining(mockDelivery.Object))
                .Returns(false);
            commonChecks
                .Setup(x => x.HasQualifyingFunding(mockDelivery.Object, TypeOfFunding.AdultSkills))
                .Returns(true);

            var sut = new LearnAimRef_86Rule(handler.Object, commonChecks.Object);

            sut.Validate(mockLearner.Object);

            handler.VerifyAll();
            commonChecks.VerifyAll();
        }

        [Fact]
        public void ValidItemDoesNotRaiseValidationMessage()
        {
            const string learnRefNumber = "123456789X";

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
               .SetupGet(y => y.ProgTypeNullable)
               .Returns(24);

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

            var commonChecks = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonChecks
                .Setup(x => x.IsSteelWorkerRedundancyTraining(mockDelivery.Object))
                .Returns(false);
            commonChecks
                .Setup(x => x.HasQualifyingFunding(mockDelivery.Object, TypeOfFunding.AdultSkills))
                .Returns(true);

            var sut = new LearnAimRef_86Rule(handler.Object, commonChecks.Object);

            sut.Validate(mockLearner.Object);

            handler.VerifyAll();
            commonChecks.VerifyAll();
        }

        public LearnAimRef_86Rule NewRule()
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var commonChecks = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);

            return new LearnAimRef_86Rule(handler.Object, commonChecks.Object);
        }
    }
}
