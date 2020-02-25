namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.LearnPlanEndDate
{
    using System;
    using System.Collections.Generic;
    using ESFA.DC.ILR.Model.Interface;
    using ESFA.DC.ILR.Tests.Model;
    using ESFA.DC.ILR.ValidationService.Interface;
    using ESFA.DC.ILR.ValidationService.Rules.Constants;
    using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnPlanEndDate;
    using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
    using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
    using FluentAssertions;
    using Moq;
    using Xunit;

    public class LearnPlanEndDate_03RuleTests : AbstractRuleTests<LearnPlanEndDate_03Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("LearnPlanEndDate_03");
        }

        [Theory]
        [InlineData("2017-08-04", "2007-08-04", true)]
        [InlineData("2018-06-01", "2007-06-01", true)]
        public void ConditionMet_True(string learnPlanEndDate, string learnStartDate, bool expectation)
        {
            // arrange
            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(DateTime.Parse(learnStartDate), DateTime.Parse(learnPlanEndDate))).Returns(10);

            var sut = NewRule(dateTimeQueryServiceMock.Object);
            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.LearnPlanEndDate)
                .Returns(DateTime.Parse(learnPlanEndDate));
            mockDelivery
                .SetupGet(y => y.LearnStartDate)
                .Returns(DateTime.Parse(learnStartDate));

            // act
            var result = sut.ConditionMet(mockDelivery.Object.LearnPlanEndDate, mockDelivery.Object.LearnStartDate);

            // assert
            Assert.Equal(expectation, result);
        }

        [Theory]
        [InlineData("2018-01-01", "2017-12-01", false)]
        [InlineData("2017-09-29", "2017-08-29", false)]
        public void ConditionMet_False(string learnPlanEndDate, string learnStartDate, bool expectation)
        {
            // arrange
            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(DateTime.Parse(learnStartDate), DateTime.Parse(learnPlanEndDate))).Returns(0);

            var sut = NewRule(dateTimeQueryServiceMock.Object);
            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.LearnPlanEndDate)
                .Returns(DateTime.Parse(learnPlanEndDate));
            mockDelivery
                .SetupGet(y => y.LearnStartDate)
                .Returns(DateTime.Parse(learnStartDate));

            // act
            var result = sut.ConditionMet(mockDelivery.Object.LearnPlanEndDate, mockDelivery.Object.LearnStartDate);

            // assert
            Assert.Equal(expectation, result);
        }

        [Fact]
        public void Validate_Error()
        {
            var learnStartDate = new DateTime(2007, 08, 04);
            var learnPlanEndDate = new DateTime(2017, 08, 04);

            var learner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        LearnPlanEndDate = learnPlanEndDate,
                        LearnStartDate = learnStartDate
                    }
                }
            };

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(learnStartDate, learnPlanEndDate)).Returns(10);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(dateTimeQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError()
        {
            var learnStartDate = new DateTime(2012, 09, 29);
            var learnPlanEndDate = new DateTime(2017, 09, 29);

            var learner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        LearnPlanEndDate = learnPlanEndDate,
                        LearnStartDate = learnStartDate
                    }
                }
            };

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(learnStartDate, learnPlanEndDate)).Returns(5);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(dateTimeQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
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

        private LearnPlanEndDate_03Rule NewRule(IDateTimeQueryService dateTimeQueryService = null, IValidationErrorHandler validationErrorHandler = null)
        {
            return new LearnPlanEndDate_03Rule(dateTimeQueryService, validationErrorHandler);
        }
    }
}
