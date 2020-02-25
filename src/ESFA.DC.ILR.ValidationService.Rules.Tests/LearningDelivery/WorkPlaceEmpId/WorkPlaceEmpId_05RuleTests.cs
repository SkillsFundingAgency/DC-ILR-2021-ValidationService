using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.WorkPlaceEmpId;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.WorkPlaceEmpId
{
    public class WorkPlaceEmpId_05RuleTests : AbstractRuleTests<WorkPlaceEmpId_05Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("WorkPlaceEmpId_05");
        }

        [Fact]
        public void ProgTypeConditionMet_True()
        {
            var progType = 24;

            NewRule().ProgTypeConditionMet(progType).Should().BeTrue();
        }

        [Theory]
        [InlineData(null)]
        [InlineData(0)]
        [InlineData(1)]
        public void ProgTypeConditionMet_False(int? progType)
        {
            NewRule().ProgTypeConditionMet(progType).Should().BeFalse();
        }

        [Fact]
        public void WorkPlacementConditionMet_True()
        {
            var workPlacements = new List<TestLearningDeliveryWorkPlacement>
            {
                new TestLearningDeliveryWorkPlacement()
                {
                    WorkPlaceStartDate = new DateTime(2018, 10, 01)
                }
            };

            var learningDeliverWorkPlacementQueryServiceMock = new Mock<ILearningDeliveryWorkPlacementQueryService>();
            learningDeliverWorkPlacementQueryServiceMock
                .Setup(qs => qs.HasAnyEmpIdNullAndStartDateNotNull(workPlacements))
                .Returns(true);

            NewRule(learningDeliverWorkPlacementQueryServiceMock.Object).WorkPlacementConditionMet(workPlacements).Should().BeTrue();
        }

        [Fact]
        public void WorkPlacementsCondtitionMet_False()
        {
            var workPlacements = new List<TestLearningDeliveryWorkPlacement>
            {
                new TestLearningDeliveryWorkPlacement()
                {
                    WorkPlaceEmpIdNullable = 123456789,
                    WorkPlaceStartDate = new DateTime(2018, 10, 01)
                }
            };

            var learningDeliveryWorkPlacementQueryServiceMock = new Mock<ILearningDeliveryWorkPlacementQueryService>();
            learningDeliveryWorkPlacementQueryServiceMock
                .Setup(qs => qs.HasAnyEmpIdNullAndStartDateNotNull(workPlacements))
                .Returns(false);

            NewRule(learningDeliveryWorkPlacementQueryServiceMock.Object).WorkPlacementConditionMet(workPlacements).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_True()
        {
            var progType = 24;

            var workPlacements = new List<TestLearningDeliveryWorkPlacement>
            {
                new TestLearningDeliveryWorkPlacement()
                {
                    WorkPlaceStartDate = new DateTime(2018, 10, 01)
                }
            };

            var learningDeliveryWorkPlacementQueryServiceMock = new Mock<ILearningDeliveryWorkPlacementQueryService>();
            learningDeliveryWorkPlacementQueryServiceMock
                .Setup(qs => qs.HasAnyEmpIdNullAndStartDateNotNull(workPlacements))
                .Returns(true);

            NewRule(learningDeliveryWorkPlacementQueryServiceMock.Object).ConditionMet(progType, workPlacements).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_FalseProgType()
        {
            var progType = 20;

            var workPlacements = new List<TestLearningDeliveryWorkPlacement>
            {
                new TestLearningDeliveryWorkPlacement()
                {
                    WorkPlaceStartDate = new DateTime(2018, 10, 01)
                }
            };

            var learningDeliveryWorkPlacementQueryServiceMock = new Mock<ILearningDeliveryWorkPlacementQueryService>();
            learningDeliveryWorkPlacementQueryServiceMock
                .Setup(qs => qs.HasAnyEmpIdNullAndStartDateNotNull(workPlacements))
                .Returns(true);

            NewRule(learningDeliveryWorkPlacementQueryServiceMock.Object).ConditionMet(progType, workPlacements).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_FalseWorkPlacement()
        {
            var progType = 24;

            var workPlacements = new List<TestLearningDeliveryWorkPlacement>
            {
                new TestLearningDeliveryWorkPlacement()
                {
                    WorkPlaceStartDate = new DateTime(2018, 10, 01),
                    WorkPlaceEmpIdNullable = 123456789
                }
            };

            var learningDeliveryWorkPlacementQueryServiceMock = new Mock<ILearningDeliveryWorkPlacementQueryService>();
            learningDeliveryWorkPlacementQueryServiceMock
                .Setup(qs => qs.HasAnyEmpIdNullAndStartDateNotNull(workPlacements))
                .Returns(false);

            NewRule(learningDeliveryWorkPlacementQueryServiceMock.Object).ConditionMet(progType, workPlacements).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_FalseWorkPlacementNull()
        {
            var progType = 24;

            var learningDeliveryWorkPlacementQueryServiceMock = new Mock<ILearningDeliveryWorkPlacementQueryService>();
            learningDeliveryWorkPlacementQueryServiceMock
                .Setup(qs => qs.HasAnyEmpIdNullAndStartDateNotNull(null))
                .Returns(false);

            NewRule(learningDeliveryWorkPlacementQueryServiceMock.Object).ConditionMet(progType, null).Should().BeFalse();
        }

        [Fact]
        public void ValidateError()
        {
            var progType = 24;

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        ProgTypeNullable = progType,
                        LearningDeliveryWorkPlacements = new List<TestLearningDeliveryWorkPlacement>
                        {
                            new TestLearningDeliveryWorkPlacement()
                            {
                                WorkPlaceStartDate = new DateTime(2018, 10, 01)
                            }
                        }
                    }
                }
            };

            var workPlacements = learner.LearningDeliveries.SelectMany(ld => ld.LearningDeliveryWorkPlacements);

            var learningDeliveryWorkPlacementQueryServiceMock = new Mock<ILearningDeliveryWorkPlacementQueryService>();
            learningDeliveryWorkPlacementQueryServiceMock
                .Setup(qs => qs.HasAnyEmpIdNullAndStartDateNotNull(workPlacements))
                .Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(learningDeliveryWorkPlacementQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void ValidateNoError()
        {
            var progType = 24;

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        ProgTypeNullable = progType,
                        LearningDeliveryWorkPlacements = new List<TestLearningDeliveryWorkPlacement>
                        {
                            new TestLearningDeliveryWorkPlacement()
                            {
                                WorkPlaceStartDate = new DateTime(2018, 10, 01),
                                WorkPlaceEmpIdNullable = 123456789
                            }
                        }
                    }
                }
            };

            var workPlacements = learner.LearningDeliveries.SelectMany(ld => ld.LearningDeliveryWorkPlacements);

            var learningDeliveryWorkPlacementQueryServiceMock = new Mock<ILearningDeliveryWorkPlacementQueryService>();
            learningDeliveryWorkPlacementQueryServiceMock
                .Setup(qs => qs.HasAnyEmpIdNullAndStartDateNotNull(workPlacements))
                .Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(learningDeliveryWorkPlacementQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var progType = 24;

            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("ProgType", progType)).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(progType);

            validationErrorHandlerMock.Verify();
        }

        private WorkPlaceEmpId_05Rule NewRule(
            ILearningDeliveryWorkPlacementQueryService learningDeliveryWorkPlacementQueryService = null,
            IValidationErrorHandler validationErrorHandler = null)
        {
            return new WorkPlaceEmpId_05Rule(learningDeliveryWorkPlacementQueryService, validationErrorHandler);
        }
    }
}
