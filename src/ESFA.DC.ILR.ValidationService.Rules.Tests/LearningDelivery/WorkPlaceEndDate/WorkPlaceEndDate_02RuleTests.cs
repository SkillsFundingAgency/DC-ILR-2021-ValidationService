using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.WorkPlaceEndDate;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.WorkPlaceEndDate
{
    public class WorkPlaceEndDate_02RuleTests : AbstractRuleTests<WorkPlaceEndDate_02Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("WorkPlaceEndDate_02");
        }

        [Fact]
        public void LearnActEndDateConditionMet_True()
        {
            var learnActEndDate = new DateTime(2018, 10, 1);

            NewRule().LearnActEndDateConditionMet(learnActEndDate).Should().BeTrue();
        }

        [Fact]
        public void LearnActEndDateConditionMet_False()
        {
            DateTime? learnActEndDate = null;

            NewRule().LearnActEndDateConditionMet(learnActEndDate).Should().BeFalse();
        }

        [Fact]
        public void WorkPlaceEndDateConditionMet_True()
        {
            var learnActEndDate = new DateTime(2018, 10, 1);

            var learningDeliveryWorkPlacements = new TestLearningDeliveryWorkPlacement[]
            {
                new TestLearningDeliveryWorkPlacement() { WorkPlaceEndDateNullable = new DateTime(2018, 12, 1) },
            };

            var learningDeliveryWorkPlacementQueryServiceMock = new Mock<ILearningDeliveryWorkPlacementQueryService>();
            learningDeliveryWorkPlacementQueryServiceMock
                .Setup(x => x.HasAnyWorkPlaceEndDatesGreaterThanLearnActEndDate(learningDeliveryWorkPlacements, learnActEndDate))
                .Returns(true);

            NewRule(learningDeliveryWorkPlacementQueryServiceMock.Object)
                .WorkPlaceEndDateConditionMet(learnActEndDate, learningDeliveryWorkPlacements).Should().BeTrue();
        }

        [Fact]
        public void WorkPlaceEndDateConditionMet_False()
        {
            var learnActEndDate = new DateTime(2018, 10, 1);

            var learningDeliveryWorkPlacements = new TestLearningDeliveryWorkPlacement[]
            {
                new TestLearningDeliveryWorkPlacement() { WorkPlaceEndDateNullable = new DateTime(2018, 09, 1) },
            };

            var learningDeliveryWorkPlacementQueryServiceMock = new Mock<ILearningDeliveryWorkPlacementQueryService>();
            learningDeliveryWorkPlacementQueryServiceMock
                .Setup(x => x.HasAnyWorkPlaceEndDatesGreaterThanLearnActEndDate(learningDeliveryWorkPlacements, learnActEndDate))
                .Returns(false);

            NewRule(learningDeliveryWorkPlacementQueryServiceMock.Object)
                .WorkPlaceEndDateConditionMet(learnActEndDate, learningDeliveryWorkPlacements).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_True()
        {
            var learnActEndDate = new DateTime(2018, 10, 1);

            var learningDeliveryWorkPlacements = new TestLearningDeliveryWorkPlacement[]
            {
                new TestLearningDeliveryWorkPlacement() { WorkPlaceEndDateNullable = new DateTime(2018, 12, 1) },
            };

            var learningDeliveryWorkPlacementQueryServiceMock = new Mock<ILearningDeliveryWorkPlacementQueryService>();
            learningDeliveryWorkPlacementQueryServiceMock
                .Setup(x => x.HasAnyWorkPlaceEndDatesGreaterThanLearnActEndDate(learningDeliveryWorkPlacements, learnActEndDate))
                .Returns(true);

            NewRule(learningDeliveryWorkPlacementQueryServiceMock.Object)
                .ConditionMet(learnActEndDate, learningDeliveryWorkPlacements).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False()
        {
            var learnActEndDate = new DateTime(2018, 10, 1);

            var learningDeliveryWorkPlacements = new TestLearningDeliveryWorkPlacement[]
            {
                new TestLearningDeliveryWorkPlacement() { WorkPlaceEndDateNullable = new DateTime(2018, 09, 1) },
            };

            var learningDeliveryWorkPlacementQueryServiceMock = new Mock<ILearningDeliveryWorkPlacementQueryService>();
            learningDeliveryWorkPlacementQueryServiceMock
                .Setup(x => x.HasAnyWorkPlaceEndDatesGreaterThanLearnActEndDate(learningDeliveryWorkPlacements, learnActEndDate))
                .Returns(false);

            NewRule(learningDeliveryWorkPlacementQueryServiceMock.Object)
                .ConditionMet(learnActEndDate, learningDeliveryWorkPlacements).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_FalseNull()
        {
            DateTime? learnActEndDate = null;

            var learningDeliveryWorkPlacements = new TestLearningDeliveryWorkPlacement[]
            {
                new TestLearningDeliveryWorkPlacement() { WorkPlaceEndDateNullable = null },
            };

            var learningDeliveryWorkPlacementQueryServiceMock = new Mock<ILearningDeliveryWorkPlacementQueryService>();
            learningDeliveryWorkPlacementQueryServiceMock
                .Setup(x => x.HasAnyWorkPlaceEndDatesGreaterThanLearnActEndDate(learningDeliveryWorkPlacements, learnActEndDate))
                .Returns(false);

            NewRule(learningDeliveryWorkPlacementQueryServiceMock.Object)
                .ConditionMet(learnActEndDate, learningDeliveryWorkPlacements).Should().BeFalse();
        }

        [Fact]
        public void Validate_Error()
        {
            var learnActEndDate = new DateTime(2018, 10, 1);
            var workPlaceEndDate = new DateTime(2018, 11, 1);

            var learner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery
                    {
                        LearnActEndDateNullable = learnActEndDate,
                        LearningDeliveryWorkPlacements = new List<TestLearningDeliveryWorkPlacement>()
                        {
                            new TestLearningDeliveryWorkPlacement
                            {
                                WorkPlaceEndDateNullable = workPlaceEndDate
                            },
                        }
                    }
                }
            };

            var learningDeliveryWorkPlacementQueryServiceMock = new Mock<ILearningDeliveryWorkPlacementQueryService>();
            learningDeliveryWorkPlacementQueryServiceMock
                .Setup(x => x.HasAnyWorkPlaceEndDatesGreaterThanLearnActEndDate(It.IsAny<IEnumerable<ILearningDeliveryWorkPlacement>>(), learnActEndDate))
                .Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(learningDeliveryWorkPlacementQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError()
        {
            var learnActEndDate = new DateTime(2018, 10, 1);
            var workPlaceEndDate = new DateTime(2018, 09, 1);

            var learner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery
                    {
                        LearnActEndDateNullable = learnActEndDate,
                        LearningDeliveryWorkPlacements = new List<TestLearningDeliveryWorkPlacement>()
                        {
                            new TestLearningDeliveryWorkPlacement
                            {
                                WorkPlaceEndDateNullable = workPlaceEndDate
                            },
                        }
                    }
                }
            };

            var learningDeliveryWorkPlacementQueryServiceMock = new Mock<ILearningDeliveryWorkPlacementQueryService>();
            learningDeliveryWorkPlacementQueryServiceMock
                .Setup(x => x.HasAnyWorkPlaceEndDatesGreaterThanLearnActEndDate(It.IsAny<IEnumerable<ILearningDeliveryWorkPlacement>>(), learnActEndDate))
                .Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(learningDeliveryWorkPlacementQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoErrorNull()
        {
            DateTime? learnActEndDate = new DateTime(2018, 10, 1);
            DateTime? workPlaceEndDate = null;

            var learner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery
                    {
                        LearnActEndDateNullable = learnActEndDate,
                        LearningDeliveryWorkPlacements = new List<TestLearningDeliveryWorkPlacement>()
                        {
                            new TestLearningDeliveryWorkPlacement
                            {
                                WorkPlaceEndDateNullable = workPlaceEndDate
                            },
                        }
                    },
                }
            };

            var learningDeliveryWorkPlacementQueryServiceMock = new Mock<ILearningDeliveryWorkPlacementQueryService>();
            learningDeliveryWorkPlacementQueryServiceMock
                .Setup(x => x.HasAnyWorkPlaceEndDatesGreaterThanLearnActEndDate(It.IsAny<IEnumerable<ILearningDeliveryWorkPlacement>>(), learnActEndDate))
                .Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(learningDeliveryWorkPlacementQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("LearnActEndDate", "01/10/2018")).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(new DateTime(2018, 10, 01));

            validationErrorHandlerMock.Verify();
        }

        private WorkPlaceEndDate_02Rule NewRule(
            ILearningDeliveryWorkPlacementQueryService learningDeliveryWorkPlacementQueryService = null,
            IValidationErrorHandler validationErrorHandler = null)
        {
            return new WorkPlaceEndDate_02Rule(learningDeliveryWorkPlacementQueryService, validationErrorHandler);
        }
    }
}
