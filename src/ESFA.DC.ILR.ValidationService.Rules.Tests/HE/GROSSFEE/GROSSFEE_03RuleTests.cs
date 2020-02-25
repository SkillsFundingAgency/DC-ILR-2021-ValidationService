using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.HE.GROSSFEE;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.HE.GROSSFEE
{
    public class GROSSFEE_03RuleTests : AbstractRuleTests<GROSSFEE_03Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("GROSSFEE_03");
        }

        [Theory]
        [InlineData(null)]
        [InlineData(29000)]
        public void ConditionMet_False(int? grossFee)
        {
            NewRule().ConditionMet(grossFee).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_True()
        {
            NewRule().ConditionMet(31000).Should().BeTrue();
        }

        [Fact]
        public void Validate_Error()
        {
            var testLearner = new TestLearner()
            {
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        LearnAimRef = "2A2345",
                        LearningDeliveryHEEntity = new TestLearningDeliveryHE()
                        {
                            GROSSFEENullable = 31000
                        }
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandler: validationErrorHandlerMock.Object).Validate(testLearner);
            }
        }

        [Fact]
        public void Validate_NoError()
        {
            var testLearner = new TestLearner()
            {
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        LearnAimRef = "2A2345",
                        LearningDeliveryHEEntity = new TestLearningDeliveryHE()
                        {
                            GROSSFEENullable = 5
                        }
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandler: validationErrorHandlerMock.Object).Validate(testLearner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.GrossFee, 2)).Verifiable();

            NewRule(validationErrorHandlerMock.Object).BuildErrorMessageParameters(2);

            validationErrorHandlerMock.Verify();
        }

        public GROSSFEE_03Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new GROSSFEE_03Rule(validationErrorHandler);
        }
    }
}
