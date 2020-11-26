using System.Collections.Generic;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.CrossEntity;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.CrossEntity
{
    public class R125RuleTests : AbstractRuleTests<R125Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("R125");
        }

        [Theory]
        [InlineData(30)]
        [InlineData(31)]
        public void ConditionMet_True(int tLevelProgType)
        {
            var learningDeliveries = new List<TestLearningDelivery>()
            {
                new TestLearningDelivery()
                {
                    AimType = 1,
                    ProgTypeNullable = 30,
                    FundModel = 2
                },
                new TestLearningDelivery()
                {
                    AimType = 1,
                    ProgTypeNullable = 31,
                    FundModel = 3
                },
            };

            var tLevelDelivery = new TestLearningDelivery()
            {
                AimType = 3,
                ProgTypeNullable = tLevelProgType,
                FundModel = 1
            };

            NewRule().ConditionMet(learningDeliveries, tLevelDelivery).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_FalseHasMatch()
        {
            var learningDeliveries = new List<TestLearningDelivery>()
            {
                new TestLearningDelivery()
                {
                    AimType = 1,
                    ProgTypeNullable = 30,
                    FundModel = 1
                },
                new TestLearningDelivery()
                {
                    AimType = 1,
                    ProgTypeNullable = 31,
                    FundModel = 3
                },
            };

            var tLevelDelivery = new TestLearningDelivery()
            {
                AimType = 3,
                ProgTypeNullable = 30,
                FundModel = 1
            };

            NewRule().ConditionMet(learningDeliveries, tLevelDelivery).Should().BeFalse();
        }

        [Fact]
        public void Validate_Error()
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        AimType = 1,
                        ProgTypeNullable = 30,
                        FundModel = 2
                    },
                    new TestLearningDelivery()
                    {
                        AimType = 1,
                        ProgTypeNullable = 31,
                        FundModel = 3
                    },
                    new TestLearningDelivery()
                    {
                        AimType = 3,
                        ProgTypeNullable = 30,
                        FundModel = 1
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError_NullDeliveries()
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = null
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError_NoTLevelDelivery()
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        AimType = 1,
                        ProgTypeNullable = 30,
                        FundModel = 2
                    },
                    new TestLearningDelivery()
                    {
                        AimType = 1,
                        ProgTypeNullable = 31,
                        FundModel = 3
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError_NullProgTypeTLevelDelivery()
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        AimType = 1,
                        ProgTypeNullable = 30,
                        FundModel = 2
                    },
                    new TestLearningDelivery()
                    {
                        AimType = 1,
                        ProgTypeNullable = 31,
                        FundModel = 3
                    },
                    new TestLearningDelivery()
                    {
                        AimType = 3,
                        ProgTypeNullable = null,
                        FundModel = 1
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

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("AimType", 1)).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("ProgType", 2)).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("FundModel", 3)).Verifiable();

            NewRule(validationErrorHandlerMock.Object).BuildErrorMessageParameters(1, 2, 3);

            validationErrorHandlerMock.Verify();
        }

        private R125Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new R125Rule(validationErrorHandler);
        }
    }
}
