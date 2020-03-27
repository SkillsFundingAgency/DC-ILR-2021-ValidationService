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
    public class WorkPlaceStartDate_04RuleTests : AbstractRuleTests<WorkPlaceStartDate_04Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("WorkPlaceStartDate_04");
        }

       [Theory]
       [InlineData("ZWRKX002", 35, true)]
       [InlineData("ZWRKX123", 35, false)]
       [InlineData("ZWRKX002", 25, false)]
       [InlineData("ZWRKX123", 25, false)]
        public void LearnAimRefAndFundModelMeetsExpectation(string learnAimRef, int fundModel, bool expectation)
        {
            // arrange
            var sut = NewRule();

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.LearnAimRef)
                .Returns(learnAimRef);
            mockDelivery
                .SetupGet(y => y.FundModel)
                .Returns(fundModel);

            // act
            var result = sut.ConditionMet(mockDelivery.Object.LearnAimRef, mockDelivery.Object.FundModel);

            // assert
            Assert.Equal(expectation, result);
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
        public void Validate_Error()
        {
            var learner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        LearnAimRef = "ZWRKX002",
                        FundModel = 35
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
            var learner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        LearnAimRef = "ZWRKX002",
                        FundModel = 25
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
                        FundModel = 25
                    },
                    new TestLearningDelivery()
                    {
                        LearnAimRef = "ZWRKX002",
                        FundModel = 35
                    },
                    new TestLearningDelivery()
                    {
                        LearnAimRef = "ZWRKX001",
                        FundModel = 25
                    },
                    new TestLearningDelivery()
                    {
                        LearnAimRef = "ZWRKX001",
                        FundModel = 35
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
                        FundModel = 25
                    },
                    new TestLearningDelivery()
                    {
                        LearnAimRef = "ZWRKX001",
                        FundModel = 35
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

            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.FundModel, 25)).Verifiable();
            NewRule(validationErrorHandlerMock.Object).BuildErrorMessageParameters(25);

            validationErrorHandlerMock.Verify();
        }

        public WorkPlaceStartDate_04Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new WorkPlaceStartDate_04Rule(validationErrorHandler);
        }
    }
}
