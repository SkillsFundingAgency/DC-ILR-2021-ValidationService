using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Learner.PlanEEPHours;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Learner.PlanEEPHours
{
    public class PlanEEPHours_01RuleTests : AbstractRuleTests<PlanEEPHours_01Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("PlanEEPHours_01");
        }

        [Theory]
        [InlineData(25)]
        public void FundModelConditionMet_true(int fundModel)
        {
            NewRule().FundModelConditionMet(fundModel).Should().BeTrue();
        }

        [Fact]
        public void FundModelConditionMet_False()
        {
            var fundModel = 0;

            NewRule().FundModelConditionMet(fundModel).Should().BeFalse();
        }

        [Fact]
        public void PlanEEPHoursConditionMet_True()
        {
            NewRule().PlanEEPHoursConditionMet(null).Should().BeTrue();
        }

        [Fact]
        public void Excluded_TrueDD07()
        {
            var progType = 23;
            var fundModel = 99;

            var dd07Mock = new Mock<IDerivedData_07Rule>();
            dd07Mock.Setup(dm => dm.IsApprenticeship(progType)).Returns(true);

            NewRule(dd07Mock.Object).Excluded(progType, fundModel).Should().BeTrue();
        }

        [Fact]
        public void Excluded_TrueFundModel()
        {
            var fundModel = 70;

            var dd07Mock = new Mock<IDerivedData_07Rule>();
            dd07Mock.Setup(dm => dm.IsApprenticeship(null)).Returns(false);

            NewRule(dd07Mock.Object).Excluded(null, fundModel).Should().BeTrue();
        }

        [Fact]
        public void FundModelExcludeConditionMet_True()
        {
            var fundModel = 70;

            NewRule().FundModelExcludeConditionMet(fundModel).Should().BeTrue();
        }

        [Fact]
        public void FundModelExcludeConditionMet_False()
        {
            var fundModel = 0;

            NewRule().FundModelExcludeConditionMet(fundModel).Should().BeFalse();
        }

        [Theory]
        [InlineData(25, 31)]
        public void TLevelExcludeConditionMet_True(int fundModel, int? progType)
        {
            var dd07Mock = new Mock<IDerivedData_07Rule>();

            dd07Mock.Setup(dd => dd.IsApprenticeship(progType)).Returns(false);

            NewRule(dd07Mock.Object).TLevelExcludeConditionMet(fundModel, progType).Should().BeTrue();
        }

        [Fact]
        public void AllLearningAimsClosedExcludeConditionMet_True()
        {
            var learningDeliveries = new List<TestLearningDelivery>()
            {
                new TestLearningDelivery() { LearnActEndDateNullable = new DateTime(2018, 10, 01) },
                new TestLearningDelivery() { LearnActEndDateNullable = new DateTime(2018, 11, 01) },
                new TestLearningDelivery() { LearnActEndDateNullable = new DateTime(2018, 09, 01) },
            };

            NewRule().AllLearningAimsClosedExcludeConditionMet(learningDeliveries).Should().BeTrue();
        }

        [Fact]
        public void AllLearningAimsClosedExcludeConditionMet_FalseNullObject()
        {
            NewRule().AllLearningAimsClosedExcludeConditionMet(null).Should().BeFalse();
        }

        [Fact]
        public void AllLearningAimsClosedExcludeConditionMet_False()
        {
            var learningDeliveries = new List<TestLearningDelivery>()
            {
                new TestLearningDelivery() { LearnActEndDateNullable = new DateTime(2018, 10, 01) },
                new TestLearningDelivery() { LearnActEndDateNullable = new DateTime(2018, 11, 01) },
                new TestLearningDelivery() { LearnActEndDateNullable = null },
            };

            NewRule().AllLearningAimsClosedExcludeConditionMet(learningDeliveries).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_True()
        {
            var fundModel = 25;
            int? planEEPHours = null;
            int? progType = null;

            var dd07Mock = new Mock<IDerivedData_07Rule>();
            dd07Mock.Setup(dm => dm.IsApprenticeship(progType)).Returns(false);

            NewRule(dd07Mock.Object).ConditionMet(fundModel, planEEPHours, progType).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_FalseFundModel()
        {
            var fundModel = 1;
            int? planEEPHours = null;
            int? progType = null;

            var dd07Mock = new Mock<IDerivedData_07Rule>();
            dd07Mock.Setup(dm => dm.IsApprenticeship(progType)).Returns(false);

            NewRule(dd07Mock.Object).ConditionMet(fundModel, planEEPHours, progType).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_FalsePlanEEPHours()
        {
            var fundModel = 25;
            int? planEEPHours = 1;
            int? progType = null;

            var dd07Mock = new Mock<IDerivedData_07Rule>();
            dd07Mock.Setup(dm => dm.IsApprenticeship(progType)).Returns(false);

            NewRule(dd07Mock.Object).ConditionMet(fundModel, planEEPHours, progType).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_FalseExcluded()
        {
            var fundModel = 25;
            int? planEEPHours = null;
            int? progType = 2;

            var dd07Mock = new Mock<IDerivedData_07Rule>();
            dd07Mock.Setup(dm => dm.IsApprenticeship(progType)).Returns(true);

            NewRule(dd07Mock.Object).ConditionMet(fundModel, planEEPHours, progType).Should().BeFalse();
        }

        [Fact]
        public void ValidateError()
        {
            var learner = new TestLearner()
            {
                PlanEEPHoursNullable = null,
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        FundModel = 25,
                    }
                }
            };

            var dd07Mock = new Mock<IDerivedData_07Rule>();
            dd07Mock.Setup(dm => dm.IsApprenticeship(null)).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(dd07Mock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void ValidateNoError()
        {
            var learner = new TestLearner()
            {
                PlanEEPHoursNullable = null,
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        FundModel = 25,
                        LearnActEndDateNullable = new DateTime(2018, 10, 01)
                    }
                }
            };

            var dd07Mock = new Mock<IDerivedData_07Rule>();
            dd07Mock.Setup(dm => dm.IsApprenticeship(null)).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(dd07Mock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("FundModel", 1)).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(1);

            validationErrorHandlerMock.Verify();
        }

        private PlanEEPHours_01Rule NewRule(
            IDerivedData_07Rule dd07 = null,
            IValidationErrorHandler validationErrorHandler = null)
        {
            return new PlanEEPHours_01Rule(dd07, validationErrorHandler);
        }
    }
}
