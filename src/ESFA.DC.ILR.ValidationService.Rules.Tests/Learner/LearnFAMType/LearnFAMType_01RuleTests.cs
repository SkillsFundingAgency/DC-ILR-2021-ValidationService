using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Learner.LearnFAMType;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Learner.LearnFAMType
{
    public class LearnFAMType_01RuleTests : AbstractRuleTests<LearnFAMType_01Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("LearnFAMType_01");
        }

        [Theory]
        [InlineData(LearnerFAMTypeConstants.HNS, 1, false, true)]
        [InlineData(LearnerFAMTypeConstants.HNS, 3, true, false)]
        [InlineData(null, 0, false, false)]
        public void ConditionMetMeetsExpectation(string learnFamType, int learnFamCode, bool lookUpResult, bool expectation)
        {
            var lookupsMock = new Mock<IProvideLookupDetails>();

            lookupsMock
                .Setup(l => l.Contains(TypeOfLimitedLifeLookup.LearnFAMType, $"{learnFamType}{learnFamCode}"))
                .Returns(lookUpResult);

            var testLearnerFam = new TestLearnerFAM { LearnFAMType = learnFamType, LearnFAMCode = learnFamCode };

            NewRule(lookupsMock.Object).ConditionMet(testLearnerFam).Should().Be(expectation);
        }

        [Fact]
        public void Validate_Error()
        {
            var learner = new TestLearner
            {
                LearnerFAMs = new List<TestLearnerFAM>()
                {
                    new TestLearnerFAM()
                    {
                        LearnFAMType = LearnerFAMTypeConstants.DLA,
                        LearnFAMCode = 2
                    }
                }
            };

            var lookupsMock = new Mock<IProvideLookupDetails>();

            lookupsMock
                .Setup(l => l.Contains(TypeOfLimitedLifeLookup.LearnFAMType, It.IsAny<string>()))
                .Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(lookupsMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError()
        {
            var learner = new TestLearner
            {
                LearnerFAMs = new List<TestLearnerFAM>()
                {
                    new TestLearnerFAM()
                    {
                        LearnFAMType = LearnerFAMTypeConstants.DLA,
                        LearnFAMCode = 2
                    }
                }
            };

            var lookupsMock = new Mock<IProvideLookupDetails>();

            lookupsMock
                .Setup(l => l.Contains(TypeOfLimitedLifeLookup.LearnFAMType, It.IsAny<string>()))
                .Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(lookupsMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_WithNoLearnerFAMS_Returns_NoError()
        {
            var learner = new TestLearner();

            var lookupsMock = new Mock<IProvideLookupDetails>();

            lookupsMock
                .Setup(l => l.Contains(TypeOfLimitedLifeLookup.LearnFAMType, It.IsAny<string>()))
                .Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(lookupsMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuilderErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();
            validationErrorHandlerMock.Setup(dd => dd.BuildErrorMessageParameter(PropertyNameConstants.LearnFAMType, LearnerFAMTypeConstants.ECF)).Verifiable();
            validationErrorHandlerMock.Setup(dd => dd.BuildErrorMessageParameter(PropertyNameConstants.LearnFAMCode, 1)).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(LearnerFAMTypeConstants.ECF, 1);

            validationErrorHandlerMock.Verify();
        }

        private LearnFAMType_01Rule NewRule(
            IProvideLookupDetails providerLookupDetails = null,
            IValidationErrorHandler validationErrorHandler = null)
        {
            return new LearnFAMType_01Rule(providerLookupDetails, validationErrorHandler);
        }
    }
}
