using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Learner.PlanLearnHours;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Learner.PlanLearnHours
{
    public class PlanLearnHours_01RuleTests : AbstractRuleTests<PlanLearnHours_01Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("PlanLearnHours_01");
        }

        [Fact]
        public void PlanLearnHoursConditionMet_True()
        {
            NewRule().PlanLearnHoursConditionMet(null).Should().BeTrue();
        }

        [Fact]
        public void PlanLearnHoursConditionMet_False()
        {
            NewRule().PlanLearnHoursConditionMet(1).Should().BeFalse();
        }

        [Fact]
        public void LearnActEndDateConditionMet_True()
        {
            var learningDeliveries = new List<TestLearningDelivery>
            {
                new TestLearningDelivery
                {
                    LearnActEndDateNullable = new DateTime(2017, 07, 20)
                },
                new TestLearningDelivery
                {
                }
            };

            NewRule().LearnActEndDateConditionMet(learningDeliveries).Should().BeTrue();
        }

        [Fact]
        public void LearnActEndDateConditionMet_False()
        {
            var learningDeliveries = new List<TestLearningDelivery>
            {
                new TestLearningDelivery
                {
                    LearnActEndDateNullable = new DateTime(2017, 07, 20)
                },
                new TestLearningDelivery
                {
                    LearnActEndDateNullable = new DateTime(2017, 07, 20)
                }
            };

            NewRule().LearnActEndDateConditionMet(learningDeliveries).Should().BeFalse();
        }

        [Fact]
        public void LearnerConditionMet_True()
        {
            var learningDeliveries = new List<TestLearningDelivery>
            {
                new TestLearningDelivery
                {
                    LearnActEndDateNullable = new DateTime(2017, 07, 20)
                },
                new TestLearningDelivery
                {
                }
            };

            NewRule().LearnerConditionMet(null, learningDeliveries).Should().BeTrue();
        }

        [Fact]
        public void LearnerConditionMet_False_LearnActEndDate()
        {
            var learningDeliveries = new List<TestLearningDelivery>
            {
                new TestLearningDelivery
                {
                    LearnActEndDateNullable = new DateTime(2017, 07, 20)
                },
                new TestLearningDelivery
                {
                    LearnActEndDateNullable = new DateTime(2017, 07, 20)
                }
            };

            NewRule().LearnerConditionMet(null, learningDeliveries).Should().BeFalse();
        }

        [Fact]
        public void LearnerConditionMet_False_LearnPlanHours()
        {
            var learningDeliveries = new List<TestLearningDelivery>
            {
                new TestLearningDelivery
                {
                    LearnActEndDateNullable = new DateTime(2017, 07, 20)
                },
                new TestLearningDelivery
                {
                }
            };

            NewRule().LearnerConditionMet(1, learningDeliveries).Should().BeFalse();
        }

        [Fact]
        public void FundModelConditionMet_True()
        {
            NewRule().FundModelConditionMet(1).Should().BeTrue();
        }

        [Fact]
        public void FundModelConditionMet_False()
        {
            NewRule().FundModelConditionMet(70).Should().BeFalse();
        }

        [Fact]
        public void DD07ConditionMet_True()
        {
            var progType = 101;
            var dd07Mock = new Mock<IDerivedData_07Rule>();

            dd07Mock.Setup(dd => dd.IsApprenticeship(progType)).Returns(false);

            NewRule(dd07Mock.Object).DD07ConditionMet(progType).Should().BeTrue();
        }

        [Fact]
        public void DD07ConditionMet_True_Null()
        {
            var dd07Mock = new Mock<IDerivedData_07Rule>();

            dd07Mock.Setup(dd => dd.IsApprenticeship(null)).Returns(false);

            NewRule(dd07Mock.Object).DD07ConditionMet(null).Should().BeTrue();
        }

        [Fact]
        public void DD07ConditionMet_False()
        {
            var progType = 24;
            var dd07Mock = new Mock<IDerivedData_07Rule>();

            dd07Mock.Setup(dd => dd.IsApprenticeship(progType)).Returns(true);

            NewRule(dd07Mock.Object).DD07ConditionMet(progType).Should().BeFalse();
        }

        [Fact]
        public void LearningDeliveryConditionMet_True()
        {
            var progType = 101;
            var fundModel = 1;
            var dd07Mock = new Mock<IDerivedData_07Rule>();

            dd07Mock.Setup(dd => dd.IsApprenticeship(progType)).Returns(false);

            NewRule(dd07Mock.Object).LearningDeliveryConditionMet(fundModel, progType).Should().BeTrue();
        }

        [Fact]
        public void LearningDeliveryConditionMet_False_ProgType()
        {
            var progType = 24;
            var fundModel = 1;
            var dd07Mock = new Mock<IDerivedData_07Rule>();

            dd07Mock.Setup(dd => dd.IsApprenticeship(progType)).Returns(true);

            NewRule(dd07Mock.Object).LearningDeliveryConditionMet(fundModel, progType).Should().BeFalse();
        }

        [Fact]
        public void LearningDeliveryConditionMet_False_FundModel()
        {
            var progType = 101;
            var fundModel = 70;
            var dd07Mock = new Mock<IDerivedData_07Rule>();

            dd07Mock.Setup(dd => dd.IsApprenticeship(progType)).Returns(false);

            NewRule(dd07Mock.Object).LearningDeliveryConditionMet(fundModel, progType).Should().BeFalse();
        }

        [Theory]
        [InlineData(25, 30)]
        [InlineData(25, 31)]
        public void LearningDeliveryConditionMet_False_Excluded(int fundModel, int? progType)
        {
            var dd07Mock = new Mock<IDerivedData_07Rule>();

            dd07Mock.Setup(dd => dd.IsApprenticeship(progType)).Returns(false);

            NewRule(dd07Mock.Object).LearningDeliveryConditionMet(fundModel, progType).Should().BeFalse();
        }

        [Fact]
        public void Validate_Error()
        {
            var progType = 101;
            var fundModel = 1;

            var learner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery()
                    {
                        ProgTypeNullable = progType,
                        FundModel = fundModel,
                    },
                    new TestLearningDelivery()
                    {
                        ProgTypeNullable = progType,
                        FundModel = fundModel,
                    }
                }
            };

            var dd07Mock = new Mock<IDerivedData_07Rule>();

            dd07Mock.Setup(dd => dd.IsApprenticeship(progType)).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(dd07Mock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError()
        {
            var progType = 101;
            var fundModel = 1;

            var learner = new TestLearner
            {
                PlanLearnHoursNullable = 1,
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery()
                    {
                        ProgTypeNullable = progType,
                        FundModel = fundModel,
                    },
                    new TestLearningDelivery()
                    {
                        ProgTypeNullable = progType,
                        FundModel = fundModel,
                    }
                }
            };

            var dd07Mock = new Mock<IDerivedData_07Rule>();

            dd07Mock.Setup(dd => dd.IsApprenticeship(progType)).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(dd07Mock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            int? planLearnHours = 0;

            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("PlanLearnHours", planLearnHours)).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("FundModel", 10)).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(planLearnHours, 10);

            validationErrorHandlerMock.Verify();
        }

        private PlanLearnHours_01Rule NewRule(IDerivedData_07Rule dd07 = null, IValidationErrorHandler validationErrorHandler = null)
        {
            return new PlanLearnHours_01Rule(dd07, validationErrorHandler);
        }
    }
}
