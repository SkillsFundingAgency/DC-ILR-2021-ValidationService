using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.WorkPlaceEndDate;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.WorkPlaceEndDate
{
    public class WorkPlaceEndDate_01RuleTests : AbstractRuleTests<WorkPlaceEndDate_01Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("WorkPlaceEndDate_01");
        }

        [Fact]
        public void ValidatePasses()
        {
            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError();

            var testLearner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        LearningDeliveryWorkPlacements = new List<TestLearningDeliveryWorkPlacement>
                        {
                            new TestLearningDeliveryWorkPlacement
                            {
                                WorkPlaceStartDate = new DateTime(2018, 9, 1),
                                WorkPlaceEndDateNullable = null
                            },
                            new TestLearningDeliveryWorkPlacement
                            {
                                WorkPlaceStartDate = new DateTime(2018, 9, 1),
                                WorkPlaceEndDateNullable = new DateTime(2018, 9, 1)
                            },
                            new TestLearningDeliveryWorkPlacement
                            {
                                WorkPlaceStartDate = new DateTime(2018, 9, 1),
                                WorkPlaceEndDateNullable = new DateTime(2018, 9, 2)
                            }
                        }
                    }
                }
            };

            NewRule(validationErrorHandlerMock.Object).Validate(testLearner);
            VerifyErrorHandlerMock(validationErrorHandlerMock);
        }

        [Fact]
        public void ValidatePasses_NoLDs()
        {
            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError();

            var testLearner = new TestLearner();

            NewRule(validationErrorHandlerMock.Object).Validate(testLearner);
            VerifyErrorHandlerMock(validationErrorHandlerMock);
        }

        [Fact]
        public void ValidatePasses_NoWorkPlacements()
        {
            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError();

            var testLearner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery()
                }
            };

            NewRule(validationErrorHandlerMock.Object).Validate(testLearner);
            VerifyErrorHandlerMock(validationErrorHandlerMock);
        }

        [Fact]
        public void ValidateFails()
        {
            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError();

            var testLearner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        LearningDeliveryWorkPlacements = new List<TestLearningDeliveryWorkPlacement>
                        {
                            new TestLearningDeliveryWorkPlacement
                            {
                                WorkPlaceStartDate = new DateTime(2018, 9, 1),
                                WorkPlaceEndDateNullable = new DateTime(2018, 8, 31)
                            },
                            new TestLearningDeliveryWorkPlacement
                            {
                                WorkPlaceStartDate = new DateTime(2018, 9, 1),
                                WorkPlaceEndDateNullable = new DateTime(2018, 9, 1)
                            }
                        }
                    }
                }
            };

            NewRule(validationErrorHandlerMock.Object).Validate(testLearner);
            VerifyErrorHandlerMock(validationErrorHandlerMock, 1);
        }

        private WorkPlaceEndDate_01Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new WorkPlaceEndDate_01Rule(validationErrorHandler);
        }
    }
}
