using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnStartDate;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.LearnStartDate
{
    public class LearnStartDate_15RuleTests
    {
        [Fact]
        public void RuleName()
        {
            var sut = NewRule();

            var result = sut.RuleName;

            Assert.Equal("LearnStartDate_15", result);
        }

        [Fact]
        public void GetStartForMeetsExpectation()
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var ddRule22 = new Mock<IDerivedData_22Rule>(MockBehavior.Strict);
            ddRule22
                .Setup(x => x.GetLatestLearningStartForESFContract(null, null))
                .Returns((DateTime?)null);
            var dateTimeQS = new Mock<IDateTimeQueryService>(MockBehavior.Strict);

            var sut = new LearnStartDate_15Rule(handler.Object, ddRule22.Object, dateTimeQS.Object);

            var result = sut.GetStartFor(null, null);

            Assert.Null(result);

            handler.VerifyAll();
            ddRule22.VerifyAll();
            dateTimeQS.VerifyAll();
        }

        [Theory]
        [InlineData("2017-12-31", true)]
        [InlineData("2017-12-31", false)]
        [InlineData("2018-01-01", true)]
        [InlineData("2018-01-01", false)]
        [InlineData("2018-02-01", true)]
        [InlineData("2018-02-01", false)]
        public void HasQualifyingStartMeetsExpectation(string candidate, bool expectation)
        {
            var delivery = new Mock<ILearningDelivery>();
            var testDate = DateTime.Parse(candidate);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var ddRule22 = new Mock<IDerivedData_22Rule>(MockBehavior.Strict);
            var dateTimeQS = new Mock<IDateTimeQueryService>(MockBehavior.Strict);
            dateTimeQS
                .Setup(x => x.IsDateBetween(delivery.Object.LearnStartDate, testDate, DateTime.MaxValue, true))
                .Returns(expectation);

            var sut = new LearnStartDate_15Rule(handler.Object, ddRule22.Object, dateTimeQS.Object);

            var result = sut.HasQualifyingStart(delivery.Object, testDate);

            Assert.Equal(expectation, result);

            handler.VerifyAll();
            ddRule22.VerifyAll();
            dateTimeQS.VerifyAll();
        }

        [Theory]
        [InlineData("2017-12-31")]
        [InlineData("2018-12-30")]
        public void InvalidItemRaisesValidationMessage(string candidate)
        {
            const string LearnRefNumber = "123456789X";

            var testDate = DateTime.Parse(candidate);

            var delivery = new Mock<ILearningDelivery>();
            delivery
                .SetupGet(x => x.LearnStartDate)
                .Returns(testDate);

            var deliveries = new List<ILearningDelivery>
            {
                delivery.Object
            };

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            handler
                .Setup(x => x.Handle(RuleNameConstants.LearnStartDate_15, LearnRefNumber, 0, Moq.It.IsAny<IEnumerable<IErrorMessageParameter>>()));
            handler
                .Setup(x => x.BuildErrorMessageParameter("LearnStartDate", testDate.ToString("d", AbstractRule.RequiredCulture)))
                .Returns(new Mock<IErrorMessageParameter>().Object);

            var ddRule22 = new Mock<IDerivedData_22Rule>(MockBehavior.Strict);
            ddRule22
                .Setup(x => x.GetLatestLearningStartForESFContract(delivery.Object, deliveries))
                .Returns(testDate);

            var dateTimeQS = new Mock<IDateTimeQueryService>(MockBehavior.Strict);
            dateTimeQS
                .Setup(x => x.IsDateBetween(delivery.Object.LearnStartDate, testDate, DateTime.MaxValue, true))
                .Returns(false);

            var sut = new LearnStartDate_15Rule(handler.Object, ddRule22.Object, dateTimeQS.Object);

            sut.Validate(mockLearner.Object);

            handler.VerifyAll();
            ddRule22.VerifyAll();
            dateTimeQS.VerifyAll();
        }

        [Theory]
        [InlineData("2018-01-01")]
        [InlineData("2018-02-01")]
        [InlineData("2018-12-31")]
        public void ValidItemDoesNotRaiseValidationMessage(string candidate)
        {
            const string LearnRefNumber = "123456789X";

            var testDate = DateTime.Parse(candidate);
            var delivery = new Mock<ILearningDelivery>();
            delivery
                .SetupGet(x => x.LearnStartDate)
                .Returns(testDate);

            var deliveries = new List<ILearningDelivery>
            {
                delivery.Object
            };

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            var ddRule22 = new Mock<IDerivedData_22Rule>(MockBehavior.Strict);
            ddRule22
                .Setup(x => x.GetLatestLearningStartForESFContract(delivery.Object, deliveries))
                .Returns(testDate);

            var dateTimeQS = new Mock<IDateTimeQueryService>(MockBehavior.Strict);
            dateTimeQS
                .Setup(x => x.IsDateBetween(delivery.Object.LearnStartDate, testDate, DateTime.MaxValue, true))
                .Returns(true);

            var sut = new LearnStartDate_15Rule(handler.Object, ddRule22.Object, dateTimeQS.Object);

            sut.Validate(mockLearner.Object);

            handler.VerifyAll();
            ddRule22.VerifyAll();
            dateTimeQS.VerifyAll();
        }

        public LearnStartDate_15Rule NewRule()
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var ddRule22 = new Mock<IDerivedData_22Rule>(MockBehavior.Strict);
            var dateTimeQS = new Mock<IDateTimeQueryService>(MockBehavior.Strict);

            return new LearnStartDate_15Rule(handler.Object, ddRule22.Object, dateTimeQS.Object);
        }
    }
}
