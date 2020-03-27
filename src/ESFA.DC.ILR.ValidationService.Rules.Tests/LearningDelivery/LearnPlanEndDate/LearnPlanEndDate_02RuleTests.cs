namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.LearnPlanEndDate
{
    using System;
    using System.Collections.Generic;
    using ESFA.DC.ILR.Model.Interface;
    using ESFA.DC.ILR.Tests.Model;
    using ESFA.DC.ILR.ValidationService.Interface;
    using ESFA.DC.ILR.ValidationService.Rules.Constants;
    using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnPlanEndDate;
    using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
    using FluentAssertions;
    using Moq;
    using Xunit;

    public class LearnPlanEndDate_02RuleTests : AbstractRuleTests<LearnPlanEndDate_02Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("LearnPlanEndDate_02");
        }

        [Theory]
        [InlineData("2017-10-29", "2017-09-29", false)]
        [InlineData("2018-04-01", "2018-04-18", true)]
        [InlineData("2018-06-01", "2018-07-31", true)]
        [InlineData("2017-08-07", "2017-08-04", false)]
        public void LearnPlanEndDateMeetsExpectation(string learnPlanEndDate, string startDate, bool expectation)
        {
            // arrange
            var sut = NewRule();

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.LearnPlanEndDate)
                .Returns(DateTime.Parse(learnPlanEndDate));
            mockDelivery
                .SetupGet(y => y.LearnStartDate)
                .Returns(DateTime.Parse(startDate));

            // act
            var result = sut.ConditionMet(mockDelivery.Object.LearnPlanEndDate, mockDelivery.Object.LearnStartDate);

            // assert
            Assert.Equal(expectation, result);
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
                        LearnPlanEndDate = new DateTime(2018, 01, 01),
                        LearnStartDate = new DateTime(2018, 01, 04)
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
                        LearnPlanEndDate = new DateTime(2018, 01, 04),
                        LearnStartDate = new DateTime(2018, 01, 01)
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

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter(PropertyNameConstants.LearnPlanEndDate, "01/01/2000")).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, "01/10/2018")).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(new DateTime(2000, 01, 01), new DateTime(2018, 10, 01));

            validationErrorHandlerMock.Verify();
        }

        private LearnPlanEndDate_02Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new LearnPlanEndDate_02Rule(validationErrorHandler);
        }
    }
}
