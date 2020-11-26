using System.Collections.Generic;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.ProgType;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.ProgType
{
    public class ProgType_18RuleTests : AbstractRuleTests<ProgType_18Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("ProgType_18");
        }

        [Theory]
        [InlineData("xxxx")]
        public void Validate_Error_Incorrect_LearnAimRef(string learnAimRef)
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        ProgTypeNullable = 30,
                    },
                    new TestLearningDelivery()
                    {
                        LearnAimRef = learnAimRef,
                        AimType = 5
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Theory]
        [InlineData("ZTPRxxxx")]
        public void Validate_Error_Incorrect_AimType(string learnAimRef)
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        ProgTypeNullable = 30,
                    },
                    new TestLearningDelivery()
                    {
                        LearnAimRef = learnAimRef,
                        AimType = 6
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Theory]
        [InlineData("xxxx")]
        public void Validate_Error_Incorrect(string learnAimRef)
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        ProgTypeNullable = 30,
                    },
                    new TestLearningDelivery()
                    {
                        LearnAimRef = learnAimRef,
                        AimType = 6
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Theory]
        [InlineData("ZTPRxxxx")]
        public void Validate_NoError(string learnAimRef)
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        ProgTypeNullable = 30,
                    },
                    new TestLearningDelivery()
                    {
                        LearnAimRef = learnAimRef,
                        AimType = 5
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var learnAimRef = "ZTPRxxxx";
            var progType = 30;

            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("LearnAimRef", learnAimRef)).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("ProgType", progType)).Verifiable();

            NewRule(validationErrorHandlerMock.Object).BuildErrorMessageParameters(learnAimRef, progType);

            validationErrorHandlerMock.Verify();
        }

        private ProgType_18Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new ProgType_18Rule(validationErrorHandler);
        }
    }
}
