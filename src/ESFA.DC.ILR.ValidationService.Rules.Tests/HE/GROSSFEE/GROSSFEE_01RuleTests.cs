using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    public class GROSSFEE_01RuleTests : AbstractRuleTests<GROSSFEE_01Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("GROSSFEE_01");
        }

        [Theory]
        [InlineData(null, 2)]
        [InlineData(2, 3)]
        public void ConditionMet_False(int? netFee, int? grossFee)
        {
            NewRule().ConditionMet(netFee, grossFee).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_True()
        {
            NewRule().ConditionMet(2, null).Should().BeTrue();
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
                            NETFEENullable = 3,
                            GROSSFEENullable = null
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
                            NETFEENullable = 3,
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
        public void Validate_NoError_LearningDeliveryHENull()
        {
            var testLearner = new TestLearner()
            {
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        LearnAimRef = "2A2345",
                        LearningDeliveryHEEntity = null
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

        public GROSSFEE_01Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new GROSSFEE_01Rule(validationErrorHandler);
        }
    }
}
