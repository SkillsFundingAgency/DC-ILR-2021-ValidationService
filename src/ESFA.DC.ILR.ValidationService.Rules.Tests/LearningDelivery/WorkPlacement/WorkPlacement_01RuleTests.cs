using System.Collections.Generic;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.WorkPlacement;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.WorkPlacement
{
    public class WorkPlacement_01RuleTests : AbstractRuleTests<WorkPlacement_01Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("WorkPlacement_01");
        }

        [Fact]
        public void ConditionMet_True()
        {
            var learnAimRef = "ZWRK003";
            var workPlacements = new List<TestLearningDeliveryWorkPlacement>();

            NewRule().ConditionMet(learnAimRef, workPlacements).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_TrueNullWorkPlacements()
        {
            var learnAimRef = "ZWRK003";

            NewRule().ConditionMet(learnAimRef, null).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_FalseLearnAimRef()
        {
            var learnAimRef = "Test";
            var workPlacements = new List<TestLearningDeliveryWorkPlacement>();

            NewRule().ConditionMet(learnAimRef, workPlacements).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_FalseHasWorkPlacements()
        {
            var learnAimRef = "ZWRK003";
            var workPlacements = new List<TestLearningDeliveryWorkPlacement>()
            {
                new TestLearningDeliveryWorkPlacement()
                {
                    WorkPlaceEmpIdNullable = 1
                }
            };

            NewRule().ConditionMet(learnAimRef, workPlacements).Should().BeFalse();
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
                        LearnAimRef = "ZWRK003",
                        LearningDeliveryWorkPlacements = new List<TestLearningDeliveryWorkPlacement>()
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError()
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        LearnAimRef = "test",
                        LearningDeliveryWorkPlacements = new List<TestLearningDeliveryWorkPlacement>()
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
            var learnAimRef = "ZWRK003";

            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("LearnAimRef", learnAimRef)).Verifiable();

            NewRule(validationErrorHandlerMock.Object).BuildErrorMessageParameters();

            validationErrorHandlerMock.Verify();
        }

        public WorkPlacement_01Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new WorkPlacement_01Rule(validationErrorHandler);
        }
    }
}
