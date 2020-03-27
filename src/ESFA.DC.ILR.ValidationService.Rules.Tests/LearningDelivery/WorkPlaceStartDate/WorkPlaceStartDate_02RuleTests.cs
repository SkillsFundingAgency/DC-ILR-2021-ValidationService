using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.WorkPlaceStartDate;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.WorkPlaceStartDate
{
    public class WorkPlaceStartDate_02RuleTests : AbstractRuleTests<WorkPlaceStartDate_02Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("WorkPlaceStartDate_02");
        }

       [Theory]
       [InlineData("2018-10-10", "2018-09-10")]
       [InlineData("2018-10-10", "2018-10-09")]
       [InlineData("2018-10-10", "2017-10-10")]
        public void ConditionMet_True(string learnStarDate, string workplaceStartDate)
        {
           NewRule().ConditionMet(DateTime.Parse(learnStarDate), DateTime.Parse(workplaceStartDate)).Should().BeTrue();
        }

        [Theory]
        [InlineData("2018-10-10", "2018-10-10")]
        [InlineData("2018-10-10", "2018-10-11")]
        [InlineData("2018-10-10", "2018-11-10")]
        public void ConditionMet_False(string learnStarDate, string workplaceStartDate)
        {
            NewRule().ConditionMet(DateTime.Parse(learnStarDate), DateTime.Parse(workplaceStartDate)).Should().BeFalse();
        }

        [Fact]
        public void Validate_Null_LearningDelivery_NoError()
        {
            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(new TestLearner());
            }
        }

        [Fact]
        public void Validate_Null_Workplacements_NoError()
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = new ILearningDelivery[]
                {
                    new TestLearningDelivery()
                }
            };
            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
            }
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
                        LearnStartDate = new DateTime(2018, 10, 09),
                        LearningDeliveryWorkPlacements = new TestLearningDeliveryWorkPlacement[]
                        {
                            new TestLearningDeliveryWorkPlacement()
                            {
                                WorkPlaceStartDate = new DateTime(2018, 10, 08)
                            },
                            new TestLearningDeliveryWorkPlacement()
                            {
                                WorkPlaceStartDate = new DateTime(2018, 09, 10)
                            },
                            new TestLearningDeliveryWorkPlacement()
                            {
                                WorkPlaceStartDate = new DateTime(2018, 10, 09)
                            }
                        }
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandler: validationErrorHandlerMock.Object).Validate(testLearner);
                validationErrorHandlerMock.Verify(
                    x =>
                            x.Handle(
                                RuleNameConstants.WorkPlaceStartDate_02,
                                It.IsAny<string>(),
                                It.IsAny<int>(),
                                It.IsAny<IEnumerable<IErrorMessageParameter>>()), Times.Exactly(2));
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
                        LearnStartDate = new DateTime(2018, 10, 09),
                        LearningDeliveryWorkPlacements = new TestLearningDeliveryWorkPlacement[]
                        {
                            new TestLearningDeliveryWorkPlacement()
                            {
                                WorkPlaceStartDate = new DateTime(2018, 10, 09)
                            },
                            new TestLearningDeliveryWorkPlacement()
                            {
                                WorkPlaceStartDate = new DateTime(2018, 10, 10)
                            }
                        }
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(testLearner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, "01/01/2018")).Verifiable();
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.WorkPlaceStartDate, "01/01/2017")).Verifiable();
            NewRule(validationErrorHandlerMock.Object).BuildErrorMessageParameters(new DateTime(2018, 1, 1), new DateTime(2017, 1, 1));

            validationErrorHandlerMock.Verify();
        }

        public WorkPlaceStartDate_02Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new WorkPlaceStartDate_02Rule(validationErrorHandler);
        }
    }
}
