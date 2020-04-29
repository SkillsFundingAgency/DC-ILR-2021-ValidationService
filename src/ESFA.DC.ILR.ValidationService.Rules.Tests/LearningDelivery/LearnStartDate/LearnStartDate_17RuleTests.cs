using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnStartDate;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.LearnStartDate
{
    public class LearnStartDate_17RuleTests
    {
        [Fact]
        public void RuleName()
        {
            var sut = NewRule();

            var result = sut.RuleName;

            Assert.Equal("LearnStartDate_17", result);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        public void GetStandardPeriodsOfValidityForMeetsExpectation(int candidate)
        {
            var delivery = new Mock<ILearningDelivery>();
            delivery
                .SetupGet(x => x.StdCodeNullable)
                .Returns(candidate);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var larsData = new Mock<ILARSDataService>(MockBehavior.Strict);
            larsData
                .Setup(x => x.GetStandardValiditiesFor(candidate))
                .Returns(new List<ILARSStandardValidity>());

            var learningDeliveryFAMQS = new Mock<ILearningDeliveryFAMQueryService>(MockBehavior.Strict);
            var dateTimeQS = new Mock<IDateTimeQueryService>(MockBehavior.Strict);

            var sut = new LearnStartDate_17Rule(handler.Object, larsData.Object, learningDeliveryFAMQS.Object, dateTimeQS.Object);

            var result = sut.GetStandardPeriodsOfValidityFor(delivery.Object);

            Assert.Empty(result);

            handler.VerifyAll();
            larsData.VerifyAll();
            learningDeliveryFAMQS.VerifyAll();
        }

        [Theory]
        [InlineData("Z32cty", "2017-12-31", true)]
        [InlineData("Btr4567", "2017-12-31", false)]
        [InlineData("Byfru", "2018-01-01", true)]
        [InlineData("MdR4es23", "2018-01-01", false)]
        [InlineData("Pfke^5b", "2018-02-01", true)]
        [InlineData("Ax3gBu6", "2018-02-01", false)]
        public void HasQualifyingStartMeetsExpectation(string contractRef, string candidate, bool expectation)
        {
            var delivery = new Mock<ILearningDelivery>();
            delivery
                .SetupGet(x => x.ConRefNumber)
                .Returns(contractRef);

            var testDate = DateTime.Parse(candidate);

            var validity = new Mock<ILARSStandardValidity>();
            validity
                .SetupGet(x => x.StartDate)
                .Returns(testDate);

            var validities = new List<ILARSStandardValidity>();
            validities.Add(validity.Object);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var larsData = new Mock<ILARSDataService>(MockBehavior.Strict);
            var learningDeliveryFAMQS = new Mock<ILearningDeliveryFAMQueryService>(MockBehavior.Strict);
            var dateTimeQS = new Mock<IDateTimeQueryService>(MockBehavior.Strict);
            dateTimeQS
                .Setup(x => x.IsDateBetween(delivery.Object.LearnStartDate, testDate, DateTime.MaxValue, true))
                .Returns(expectation);

            var sut = new LearnStartDate_17Rule(handler.Object, larsData.Object, learningDeliveryFAMQS.Object, dateTimeQS.Object);

            var result = sut.HasQualifyingStart(delivery.Object, validities);

            Assert.Equal(expectation, result);

            handler.VerifyAll();
            larsData.VerifyAll();
            learningDeliveryFAMQS.VerifyAll();
        }

        [Fact]
        public void HasQualifyingStartWithNullAllocationsMeetsExpectation()
        {
            var delivery = new Mock<ILearningDelivery>();

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var larsData = new Mock<ILARSDataService>(MockBehavior.Strict);
            var learningDeliveryFAMQS = new Mock<ILearningDeliveryFAMQueryService>(MockBehavior.Strict);
            var dateTimeQS = new Mock<IDateTimeQueryService>(MockBehavior.Strict);

            var sut = new LearnStartDate_17Rule(handler.Object, larsData.Object, learningDeliveryFAMQS.Object, dateTimeQS.Object);

            var result = sut.HasQualifyingStart(delivery.Object, null);

            Assert.False(result);

            handler.VerifyAll();
            larsData.VerifyAll();
            learningDeliveryFAMQS.VerifyAll();
        }

        [Theory]
        [InlineData(1, "2017-12-31")]
        [InlineData(2, "2017-12-31")]
        [InlineData(3, "2018-01-01")]
        [InlineData(4, "2018-01-01")]
        public void InvalidItemRaisesValidationMessage(int stdCode, string candidate)
        {
            const string learnRefNumber = "123456789X";
            const int aimType = 1;
            const int progType = 25;

            var testDate = DateTime.Parse(candidate);

            var delivery = new Mock<ILearningDelivery>();
            delivery
                .SetupGet(x => x.StdCodeNullable)
                .Returns(stdCode);
            delivery
                .SetupGet(x => x.LearnStartDate)
                .Returns(testDate);
            delivery
                .SetupGet(x => x.AimType)
                .Returns(aimType);
            delivery
                .SetupGet(x => x.ProgTypeNullable)
                .Returns(progType);

            var validity = new Mock<ILARSStandardValidity>();
            validity
                .SetupGet(x => x.StartDate)
                .Returns(testDate);

            var validities = new List<ILARSStandardValidity>();
            validities.Add(validity.Object);

            var deliveries = new List<ILearningDelivery>();
            deliveries.Add(delivery.Object);

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(learnRefNumber);
            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            handler
                .Setup(x => x.Handle(RuleNameConstants.LearnStartDate_17, learnRefNumber, 0, Moq.It.IsAny<IEnumerable<IErrorMessageParameter>>()));
            handler
                .Setup(x => x.BuildErrorMessageParameter("AimType", aimType))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter("LearnStartDate", testDate.ToString("d", AbstractRule.RequiredCulture)))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter("ProgType", progType))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter("StdCode", stdCode))
                .Returns(new Mock<IErrorMessageParameter>().Object);

            var larsData = new Mock<ILARSDataService>(MockBehavior.Strict);
            larsData
                .Setup(x => x.GetStandardValiditiesFor(stdCode))
                .Returns(validities);

            var learningDeliveryFAMQS = new Mock<ILearningDeliveryFAMQueryService>(MockBehavior.Strict);
            learningDeliveryFAMQS
                .Setup(x => x.HasLearningDeliveryFAMType(
                    delivery.Object.LearningDeliveryFAMs,
                    "RES"))
                .Returns(false);

            var dateTimeQS = new Mock<IDateTimeQueryService>(MockBehavior.Strict);
            dateTimeQS
                .Setup(x => x.IsDateBetween(delivery.Object.LearnStartDate, testDate, DateTime.MaxValue, true))
                .Returns(false);

            var sut = new LearnStartDate_17Rule(handler.Object, larsData.Object, learningDeliveryFAMQS.Object, dateTimeQS.Object);

            sut.Validate(mockLearner.Object);

            handler.VerifyAll();
            larsData.VerifyAll();
            learningDeliveryFAMQS.VerifyAll();
        }

        [Theory]
        [InlineData(1, "2017-12-31")]
        [InlineData(2, "2017-12-31")]
        [InlineData(3, "2018-01-01")]
        [InlineData(4, "2018-01-01")]
        public void ValidItemDoesNotRaiseValidationMessage(int stdCode, string candidate)
        {
            const string learnRefNumber = "123456789X";
            const int aimType = 1;
            const int progType = 25;

            var testDate = DateTime.Parse(candidate);

            var delivery = new Mock<ILearningDelivery>();
            delivery
                .SetupGet(x => x.StdCodeNullable)
                .Returns(stdCode);
            delivery
                .SetupGet(x => x.LearnStartDate)
                .Returns(testDate);
            delivery
                .SetupGet(x => x.AimType)
                .Returns(aimType);
            delivery
                .SetupGet(x => x.ProgTypeNullable)
                .Returns(progType);

            var validity = new Mock<ILARSStandardValidity>();
            validity
                .SetupGet(x => x.StartDate)
                .Returns(testDate);

            var validities = new List<ILARSStandardValidity>();
            validities.Add(validity.Object);

            var deliveries = new List<ILearningDelivery>();
            deliveries.Add(delivery.Object);

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(learnRefNumber);
            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var larsData = new Mock<ILARSDataService>(MockBehavior.Strict);
            larsData
                .Setup(x => x.GetStandardValiditiesFor(stdCode))
                .Returns(validities);

            var learningDeliveryFAMQS = new Mock<ILearningDeliveryFAMQueryService>(MockBehavior.Strict);
            learningDeliveryFAMQS
                .Setup(x => x.HasLearningDeliveryFAMType(
                    delivery.Object.LearningDeliveryFAMs,
                    "RES"))
                .Returns(false);

            var dateTimeQS = new Mock<IDateTimeQueryService>(MockBehavior.Strict);
            dateTimeQS
                .Setup(x => x.IsDateBetween(delivery.Object.LearnStartDate, testDate, DateTime.MaxValue, true))
                .Returns(true);

            var sut = new LearnStartDate_17Rule(handler.Object, larsData.Object, learningDeliveryFAMQS.Object, dateTimeQS.Object);

            sut.Validate(mockLearner.Object);

            handler.VerifyAll();
            larsData.VerifyAll();
            learningDeliveryFAMQS.VerifyAll();
        }

        public LearnStartDate_17Rule NewRule()
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var larsData = new Mock<ILARSDataService>(MockBehavior.Strict);
            var learningDeliveryFAMQS = new Mock<ILearningDeliveryFAMQueryService>(MockBehavior.Strict);
            var dateTimeQS = new Mock<IDateTimeQueryService>(MockBehavior.Strict);

            return new LearnStartDate_17Rule(handler.Object, larsData.Object, learningDeliveryFAMQS.Object, dateTimeQS.Object);
        }
    }
}
