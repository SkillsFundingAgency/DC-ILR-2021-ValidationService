using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.WorkPlaceStartDate;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using System;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.WorkPlaceStartDate
{
    public class WorkPlaceStartDate_03RuleTests : AbstractRuleTests<WorkPlaceStartDate_03Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("WorkPlaceStartDate_03");
        }

        [Theory]
        [InlineData(TypeOfAim.References.WorkPlacement0To49Hours)]
        [InlineData(TypeOfAim.References.WorkPlacement50To99Hours)]
        [InlineData(TypeOfAim.References.WorkPlacement100To199Hours)]
        [InlineData(TypeOfAim.References.WorkPlacement200To499Hours)]
        [InlineData(TypeOfAim.References.WorkPlacement500PlusHours)]
        [InlineData(TypeOfAim.References.SupportedInternship16To19)]
        [InlineData(TypeOfAim.References.WorkExperience)]
        [InlineData("zwrkx002")]
        public void LearnAimRefConditionMet_False(string learnAimRef)
        {
            NewRule().LearnAimRefConditionMet(learnAimRef).Should().BeFalse();
        }

        [Theory]
        [InlineData(TypeOfAim.References.ESFLearnerStartandAssessment)]
        [InlineData("zesf0001")]
        public void LearnAimRefConditionMet_True(string learnAimRef)
        {
            NewRule().LearnAimRefConditionMet(learnAimRef).Should().BeTrue();
        }

        [Fact]
        public void WorkPlacementsConditionMet_False()
        {
            var learningDeliveryWorkPlacements = new TestLearningDeliveryWorkPlacement[]
                {
                };

            NewRule().WorkPlacementsConditionMet(learningDeliveryWorkPlacements).Should().BeFalse();
        }

        [Fact]
        public void WorkPlacementsConditionMet_True()
        {
            var learningDeliveryWorkPlacements = new TestLearningDeliveryWorkPlacement[]
                {
                    new TestLearningDeliveryWorkPlacement()
                    {
                        WorkPlaceEmpIdNullable = 123,
                        WorkPlaceStartDate = new DateTime(2018, 10, 11)
                    }
                };

            NewRule().WorkPlacementsConditionMet(learningDeliveryWorkPlacements).Should().BeTrue();
        }

        [Theory]
        [InlineData(TypeOfAim.References.WorkPlacement0To49Hours, true)]
        [InlineData(TypeOfAim.References.WorkPlacement0To49Hours, false)]
        [InlineData(TypeOfAim.References.ESFLearnerStartandAssessment, false)]
        public void ConditionMet_False(string learnRefNumber, bool includeWorkPlacement)
        {
            var learningDeliveryWorkPlacements = new TestLearningDeliveryWorkPlacement[]
               {
               };
            var learningDeliveryWorkPlacementsWithRecord = new TestLearningDeliveryWorkPlacement[]
               {
                   new TestLearningDeliveryWorkPlacement()
                   {
                       WorkPlaceEmpIdNullable = 123,
                       WorkPlaceStartDate = new DateTime(2018, 4, 01)
                   }
               };

            NewRule().ConditionMet(learnRefNumber, includeWorkPlacement ? learningDeliveryWorkPlacementsWithRecord : learningDeliveryWorkPlacements).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_True()
        {
            var learningDeliveryWorkPlacements = new TestLearningDeliveryWorkPlacement[]
                {
                    new TestLearningDeliveryWorkPlacement()
                    {
                        WorkPlaceEmpIdNullable = 123,
                        WorkPlaceStartDate = new DateTime(2018, 10, 11)
                    }
                };

            NewRule().ConditionMet(TypeOfAim.References.ESFLearnerStartandAssessment, learningDeliveryWorkPlacements).Should().BeTrue();
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
                        LearnAimRef = TypeOfAim.References.ESFLearnerStartandAssessment,
                        LearningDeliveryWorkPlacements = new TestLearningDeliveryWorkPlacement[]
                        {
                            new TestLearningDeliveryWorkPlacement()
                            {
                                WorkPlaceEmpIdNullable = 123,
                                WorkPlaceStartDate = new DateTime(2018, 10, 11)
                            }
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
                        LearnAimRef = TypeOfAim.References.WorkPlacement0To49Hours,
                        LearningDeliveryWorkPlacements = new TestLearningDeliveryWorkPlacement[]
                        {
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

            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.LearnAimRef, "Z0007835")).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters("Z0007835");

            validationErrorHandlerMock.Verify();
        }

        public WorkPlaceStartDate_03Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new WorkPlaceStartDate_03Rule(validationErrorHandler: validationErrorHandler);
        }
    }
}
