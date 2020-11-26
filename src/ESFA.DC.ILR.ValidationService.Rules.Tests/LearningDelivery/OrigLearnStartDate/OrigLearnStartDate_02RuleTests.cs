using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.OrigLearnStartDate;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.OrigLearnStartDate
{
    public class OrigLearnStartDate_02RuleTests
    {
        [Fact]
        public void RuleName()
        {
            var sut = NewRule();

            var result = sut.RuleName;

            Assert.Equal("OrigLearnStartDate_02", result);
        }

        [Theory]
        [InlineData("2018-04-20", "2018-04-18", true)]
        [InlineData("2018-04-19", "2018-04-18", true)]
        [InlineData("2018-04-18", "2018-04-18", false)]
        [InlineData("2018-04-17", "2018-04-18", false)]
        public void HasQualifyingDatesMeetsExpectation(string startDate, string originalDate, bool expectation)
        {
            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.LearnStartDate)
                .Returns(DateTime.Parse(startDate));
            mockDelivery
                .SetupGet(y => y.OrigLearnStartDateNullable)
                .Returns(DateTime.Parse(originalDate));

            var dateTimeQueryService = new Mock<IDateTimeQueryService>(MockBehavior.Strict);
            dateTimeQueryService
                .Setup(x => x.IsDateBetween(
                    mockDelivery.Object.OrigLearnStartDateNullable.Value,
                    Moq.It.IsAny<DateTime>(),
                    mockDelivery.Object.LearnStartDate,
                    false))
                .Returns(expectation);

            var result = NewRule(dateTimeQueryService.Object).HasQualifyingDates(mockDelivery.Object);

            Assert.Equal(expectation, result);
        }

        [Fact]
        public void InvalidItemRaisesValidationMessage()
        {
            const string LearnRefNumber = "123456789X";
            var referenceDate = DateTime.Parse("2019-04-19");

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.LearnStartDate)
                .Returns(referenceDate.AddDays(-1));
            mockDelivery
                .SetupGet(y => y.OrigLearnStartDateNullable)
                .Returns(referenceDate);

            mockDelivery
                .SetupGet(y => y.FundModel)
                .Returns(35);

            var deliveries = new List<ILearningDelivery>();
            deliveries.Add(mockDelivery.Object);

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            handler
                .Setup(x => x.Handle(
                    Moq.It.Is<string>(y => y == OrigLearnStartDate_02Rule.Name),
                    Moq.It.Is<string>(y => y == LearnRefNumber),
                    0,
                    Moq.It.IsAny<IEnumerable<IErrorMessageParameter>>()));
            handler
                .Setup(x => x.BuildErrorMessageParameter(
                    Moq.It.Is<string>(y => y == PropertyNameConstants.LearnStartDate),
                    referenceDate.AddDays(-1)))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter(
                    Moq.It.Is<string>(y => y == OrigLearnStartDate_02Rule.MessagePropertyName),
                    referenceDate))
                .Returns(new Mock<IErrorMessageParameter>().Object);

            var dateTimeQueryService = new Mock<IDateTimeQueryService>(MockBehavior.Strict);
            dateTimeQueryService
                .Setup(x => x.IsDateBetween(
                    mockDelivery.Object.OrigLearnStartDateNullable.Value,
                    Moq.It.IsAny<DateTime>(),
                    mockDelivery.Object.LearnStartDate,
                    false))
                    .Returns(false);

            var sut = NewRule(dateTimeQueryService.Object, handler.Object);

            sut.Validate(mockLearner.Object);

            handler.VerifyAll();
        }

        [Fact]
        public void ValidItemDoesNotRaiseValidationMessage()
        {
            const string LearnRefNumber = "123456789X";

            var referenceDate = DateTime.Parse("2019-04-19");

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.LearnStartDate)
                .Returns(referenceDate);

            mockDelivery
                .SetupGet(y => y.OrigLearnStartDateNullable)
                .Returns(referenceDate.AddDays(-1));

            mockDelivery
                .SetupGet(y => y.FundModel)
                .Returns(FundModels.AdultSkills);

            var deliveries = new List<ILearningDelivery>();
            deliveries.Add(mockDelivery.Object);

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            var dateTimeQueryService = new Mock<IDateTimeQueryService>(MockBehavior.Strict);
            dateTimeQueryService
                .Setup(x => x.IsDateBetween(
                    mockDelivery.Object.OrigLearnStartDateNullable.Value,
                    Moq.It.IsAny<DateTime>(),
                    mockDelivery.Object.LearnStartDate,
                    false))
                    .Returns(true);

            var sut = NewRule(dateTimeQueryService.Object, handler.Object);

            sut.Validate(mockLearner.Object);

            handler.VerifyAll();
        }

        [Theory]
        [InlineData(35)]
        [InlineData(36)]
        [InlineData(81)]
        [InlineData(99)]
        public void FundModelConditionMet_True(int fundModel)
        {
            var learningDelivery = new TestLearningDelivery
            {
                FundModel = fundModel
            };

            NewRule().HasValidFundModel(learningDelivery).Should().BeTrue();
        }

        [Theory]
        [InlineData(25)]
        [InlineData(88)]
        [InlineData(10)]
        [InlineData(100)]
        public void FundModelConditionMet_False(int fundModel)
        {
            var learningDelivery = new TestLearningDelivery
            {
                FundModel = fundModel
            };

            NewRule().HasValidFundModel(learningDelivery).Should().BeFalse();
        }

        public OrigLearnStartDate_02Rule NewRule(
            IDateTimeQueryService dateTimeQueryService = null,
            IValidationErrorHandler handler = null)
        {
            return new OrigLearnStartDate_02Rule(dateTimeQueryService, handler);
        }
    }
}
