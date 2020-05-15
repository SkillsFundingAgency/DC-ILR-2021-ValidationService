using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.ProgType;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.ProgType
{
    public class ProgType_14RuleTests : AbstractRuleTests<ProgType_14Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("ProgType_14");
        }

        [Theory]
        [InlineData("ZWRKX002", 24)]
        [InlineData("ZWRKX003", 24)]
        public void ConditionMet_True(string learnAimRef, int? progType)
        {
            NewRule().ConditionMet(learnAimRef, progType).Should().BeTrue();
        }

        [Theory]
        [InlineData("ZWRKX002", 35)]
        [InlineData("ZWRKX003", 35)]
        [InlineData("ZWRKX003", null)]
        [InlineData("ZWRKX002", null)]
        [InlineData("ZWRKX123", 24)]
        public void ConditionMet_False(string learnAimRef, int? progType)
        {
            NewRule().ConditionMet(learnAimRef, progType).Should().BeFalse();
        }

        [Fact]
        public void Validate_Null_LearningDelivery_NoError()
        {
            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(new TestLearner());
            }
        }

        [Theory]
        [InlineData("ZWRKX002")]
        [InlineData("ZWRKX003")]
        public void Validate_Error(string learnAimRef)
        {
            var learner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        LearnAimRef = learnAimRef,
                        ProgTypeNullable = 24
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Theory]
        [InlineData("ZWRKX002", 35)]
        [InlineData("ZWRKX003", 35)]
        [InlineData("ZWRKX003", null)]
        [InlineData("ZWRKX002", null)]
        [InlineData("ZWRKX123", 24)]
        public void Validate_NoError(string learnAimRef, int? progType)
        {
            var learner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        LearnAimRef = learnAimRef,
                        ProgTypeNullable = progType
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_Error_InMultipleDeliveries()
        {
            var learner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        LearnAimRef = "ZWRKX002",
                        ProgTypeNullable = 24
                    },
                    new TestLearningDelivery()
                    {
                        LearnAimRef = "ZWRKX002",
                        ProgTypeNullable = 35
                    },
                    new TestLearningDelivery()
                    {
                        LearnAimRef = "ZWRKX003",
                        ProgTypeNullable = 24
                    },
                    new TestLearningDelivery()
                    {
                        LearnAimRef = "ZWRKX001",
                        ProgTypeNullable = 24
                    },
                    new TestLearningDelivery()
                    {
                        LearnAimRef = "ZWRKX001",
                        ProgTypeNullable = 35
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError_InMultipleDeliveries()
        {
            var learner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        LearnAimRef = "ZWRKX001",
                        ProgTypeNullable = 24
                    },
                    new TestLearningDelivery()
                    {
                        LearnAimRef = "ZWRKX001",
                        ProgTypeNullable = 35
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
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.LearnAimRef, "LearnAimRef")).Verifiable();
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.ProgType, 24)).Verifiable();

            NewRule(validationErrorHandlerMock.Object).BuildErrorMessageParameters("LearnAimRef", 24);

            validationErrorHandlerMock.Verify();
        }

        public ProgType_14Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new ProgType_14Rule(validationErrorHandler);
        }
    }
}
