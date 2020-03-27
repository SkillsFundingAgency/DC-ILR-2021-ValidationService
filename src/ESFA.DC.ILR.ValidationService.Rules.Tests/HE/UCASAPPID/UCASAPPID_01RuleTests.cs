using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.HE.UCASAPPID;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.HE.UCASAPPID
{
    public class UCASAPPID_01RuleTests : AbstractRuleTests<UCASAPPID_01Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("UCASAPPID_01");
        }

        [Fact]
        public void ConditionMet_True()
        {
            var learnStartDate = new DateTime(2018, 01, 01);
            string ucasAppId = null;

            NewRule().ConditionMet(learnStartDate, ucasAppId).Should().BeTrue();
        }

        [Theory]
        [InlineData("2010-01-01", null)]
        [InlineData("2018-01-01", "ABC")]
        public void ConditionMet_False(string learnStartDate, string ucasAppId)
        {
            var learnStartDateParam = DateTime.Parse(learnStartDate);

            NewRule().ConditionMet(learnStartDateParam, ucasAppId).Should().BeFalse();
        }

        [Fact]
        public void ValidateError()
        {
            var learner = new TestLearner()
            {
                LearnerHEEntity = new TestLearnerHE()
                {
                    UCASPERID = "ABC"
                },
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        LearnStartDate = new DateTime(2018, 01, 01),
                        LearningDeliveryHEEntity = new TestLearningDeliveryHE()
                        {
                            UCASAPPID = null
                        }
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void ValidateNoError()
        {
            var learner = new TestLearner()
            {
                LearnerHEEntity = new TestLearnerHE()
                {
                    UCASPERID = "ABC"
                },
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        LearnStartDate = new DateTime(2018, 01, 01),
                        LearningDeliveryHEEntity = new TestLearningDeliveryHE()
                        {
                            UCASAPPID = "DEF"
                        }
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void ValidateNoError_NullUcasPerId()
        {
            var learner = new TestLearner()
            {
                LearnerHEEntity = new TestLearnerHE(),
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        LearnStartDate = new DateTime(2018, 01, 01),
                        LearningDeliveryHEEntity = new TestLearningDeliveryHE()
                        {
                            UCASAPPID = null
                        }
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void ValidateNoError_NullLearnerHeEntity()
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        LearnStartDate = new DateTime(2018, 01, 01),
                        LearningDeliveryHEEntity = new TestLearningDeliveryHE()
                        {
                            UCASAPPID = null
                        }
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

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter(PropertyNameConstants.UCASPERID, "ABC")).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, "01/05/2017")).Verifiable();

            NewRule(validationErrorHandlerMock.Object).BuildErrorMessageParameters("ABC", new DateTime(2017, 05, 01));

            validationErrorHandlerMock.Verify();
        }

        private UCASAPPID_01Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new UCASAPPID_01Rule(validationErrorHandler);
        }
    }
}
