using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnAimRef;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.LearnAimRef
{
    public class LearnAimRef_85RuleTests
    {
        [Fact]
        public void RuleName()
        {
            var sut = NewRule();

            var result = sut.RuleName;

            Assert.Equal("LearnAimRef_85", result);
        }

        [Fact]
        public void FirstViableDateMeetsExpectation()
        {
            var result = LearnAimRef_85Rule.FirstViableDate;

            Assert.Equal(DateTime.Parse("2017-08-01"), result);
        }

        [Theory]
        [InlineData(LARSConstants.NotionalNVQLevelV2Strings.EntryLevel, false)]
        [InlineData(LARSConstants.NotionalNVQLevelV2Strings.HigherLevel, false)]
        [InlineData(LARSConstants.NotionalNVQLevelV2Strings.Level1, false)]
        [InlineData(LARSConstants.NotionalNVQLevelV2Strings.Level1_2, false)]
        [InlineData(LARSConstants.NotionalNVQLevelV2Strings.Level2, false)]
        [InlineData(LARSConstants.NotionalNVQLevelV2Strings.Level3, true)]
        [InlineData(LARSConstants.NotionalNVQLevelV2Strings.Level4, false)]
        [InlineData(LARSConstants.NotionalNVQLevelV2Strings.Level5, false)]
        [InlineData(LARSConstants.NotionalNVQLevelV2Strings.Level6, false)]
        [InlineData(LARSConstants.NotionalNVQLevelV2Strings.Level7, false)]
        [InlineData(LARSConstants.NotionalNVQLevelV2Strings.Level8, false)]
        [InlineData(LARSConstants.NotionalNVQLevelV2Strings.MixedLevel, false)]
        [InlineData(LARSConstants.NotionalNVQLevelV2Strings.NotKnown, false)]
        public void IsDisqualifyingNotionalNVQMeetsExpectation(string candidate, bool expectation)
        {
            var sut = NewRule();
            var mockDelivery = new Mock<ILARSLearningDelivery>();
            mockDelivery
                .SetupGet(y => y.NotionalNVQLevelv2)
                .Returns(candidate);

            var result = sut.IsDisqualifyingNotionalNVQ(mockDelivery.Object);

            Assert.Equal(expectation, result);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("testAim1")]
        [InlineData("testAim2")]
        public void HasQualifyingNotionalNVQMeetsExpectation(string candidate)
        {
            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(x => x.LearnAimRef)
                .Returns(candidate);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            service
                .Setup(x => x.GetDeliveryFor(candidate))
                .Returns((ILARSLearningDelivery)null);

            var learningDeliveryFAMQS = new Mock<ILearningDeliveryFAMQueryService>(MockBehavior.Strict);
            var dd07 = new Mock<IDerivedData_07Rule>(MockBehavior.Strict);
            var dateTimeQS = new Mock<IDateTimeQueryService>(MockBehavior.Strict);

            var sut = new LearnAimRef_85Rule(handler.Object, service.Object, learningDeliveryFAMQS.Object, dd07.Object, dateTimeQS.Object);

            var result = sut.HasDisqualifyingNotionalNVQ(mockDelivery.Object);

            Assert.False(result);

            handler.VerifyAll();
            service.VerifyAll();
            learningDeliveryFAMQS.VerifyAll();
            dd07.VerifyAll();
        }

        [Theory]
        [InlineData(PriorAttainments.FullLevel3)]
        [InlineData(PriorAttainments.Level4)]
        [InlineData(PriorAttainments.Level4Expired20130731)]
        [InlineData(PriorAttainments.Level5)]
        [InlineData(PriorAttainments.Level5AndAboveExpired20130731)]
        [InlineData(PriorAttainments.Level6)]
        [InlineData(PriorAttainments.Level7AndAbove)]
        [InlineData(PriorAttainments.OtherLevelNotKnown)]
        [InlineData(PriorAttainments.NotKnown)]
        public void InvalidItemRaisesValidationMessage(int candidate)
        {
            const string learnRefNumber = "123456789X";
            const string learnAimRef = "salddfkjeifdnase";

            var testDate = DateTime.Parse("2016-08-01");

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.LearnAimRef)
                .Returns(learnAimRef);
            mockDelivery
                .SetupGet(y => y.LearnStartDate)
                .Returns(testDate);
            mockDelivery
                .SetupGet(y => y.FundModel)
                .Returns(FundModels.AdultSkills);

            var deliveries = new List<ILearningDelivery>();
            deliveries.Add(mockDelivery.Object);

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(learnRefNumber);
            mockLearner
                .SetupGet(x => x.PriorAttainNullable)
                .Returns(candidate);
            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            handler
                .Setup(x => x.Handle("LearnAimRef_85", learnRefNumber, 0, Moq.It.IsAny<IEnumerable<IErrorMessageParameter>>()));
            handler
                .Setup(x => x.BuildErrorMessageParameter("PriorAttain", candidate))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter("LearnAimRef", learnAimRef))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter("LearnStartDate", testDate))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter("FundModel", FundModels.AdultSkills))
                .Returns(new Mock<IErrorMessageParameter>().Object);

            var mockLars = new Mock<ILARSLearningDelivery>();
            mockLars
                .SetupGet(x => x.NotionalNVQLevelv2)
                .Returns(LARSConstants.NotionalNVQLevelV2Strings.Level3);

            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            service
                .Setup(x => x.GetDeliveryFor(learnAimRef))
                .Returns(mockLars.Object);

            var learningDeliveryFAMQS = new Mock<ILearningDeliveryFAMQueryService>(MockBehavior.Strict);
            learningDeliveryFAMQS
                .Setup(x => x.HasLearningDeliveryFAMType(
                    mockDelivery.Object.LearningDeliveryFAMs,
                    "RES"))
                .Returns(false);

            var dd07 = new Mock<IDerivedData_07Rule>(MockBehavior.Strict);
            dd07
                .Setup(dd => dd.IsApprenticeship(mockDelivery.Object.ProgTypeNullable)).Returns(false);

            var dateTimeQS = new Mock<IDateTimeQueryService>(MockBehavior.Strict);
            dateTimeQS
                .Setup(x => x.IsDateBetween(mockDelivery.Object.LearnStartDate, LearnAimRef_85Rule.FirstViableDate, DateTime.MaxValue, true))
                .Returns(true);

            var sut = new LearnAimRef_85Rule(handler.Object, service.Object, learningDeliveryFAMQS.Object, dd07.Object, dateTimeQS.Object);

            sut.Validate(mockLearner.Object);

            handler.VerifyAll();
            service.VerifyAll();
            learningDeliveryFAMQS.VerifyAll();
            dd07.VerifyAll();
        }

        [Theory]
        [InlineData(PriorAttainments.FullLevel3)]
        [InlineData(PriorAttainments.Level4)]
        [InlineData(PriorAttainments.Level4Expired20130731)]
        [InlineData(PriorAttainments.Level5)]
        [InlineData(PriorAttainments.Level5AndAboveExpired20130731)]
        [InlineData(PriorAttainments.Level6)]
        [InlineData(PriorAttainments.Level7AndAbove)]
        [InlineData(PriorAttainments.OtherLevelNotKnown)]
        [InlineData(PriorAttainments.NotKnown)]
        public void ValidItemDoesNotRaiseValidationMessage(int candidate)
        {
            const string learnRefNumber = "123456789X";
            const string learnAimRef = "salddfkjeifdnase";

            var testDate = DateTime.Parse("2016-08-01");

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.LearnAimRef)
                .Returns(learnAimRef);
            mockDelivery
                .SetupGet(y => y.LearnStartDate)
                .Returns(testDate);
            mockDelivery
                .SetupGet(y => y.FundModel)
                .Returns(FundModels.AdultSkills);

            var deliveries = new List<ILearningDelivery>();
            deliveries.Add(mockDelivery.Object);

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(learnRefNumber);
            mockLearner
                .SetupGet(x => x.PriorAttainNullable)
                .Returns(candidate);
            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            var mockLars = new Mock<ILARSLearningDelivery>();
            mockLars
                .SetupGet(x => x.NotionalNVQLevelv2)
                .Returns(LARSConstants.NotionalNVQLevelV2Strings.Level2);

            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            service
                .Setup(x => x.GetDeliveryFor(learnAimRef))
                .Returns(mockLars.Object);

            var learningDeliveryFAMQS = new Mock<ILearningDeliveryFAMQueryService>(MockBehavior.Strict);
            learningDeliveryFAMQS
                .Setup(x => x.HasLearningDeliveryFAMType(
                    mockDelivery.Object.LearningDeliveryFAMs,
                    "RES"))
                .Returns(false);

            var dd07 = new Mock<IDerivedData_07Rule>(MockBehavior.Strict);
            dd07
                .Setup(dd => dd.IsApprenticeship(mockDelivery.Object.ProgTypeNullable)).Returns(false);

            var dateTimeQS = new Mock<IDateTimeQueryService>(MockBehavior.Strict);
            dateTimeQS
                .Setup(x => x.IsDateBetween(mockDelivery.Object.LearnStartDate, LearnAimRef_85Rule.FirstViableDate, DateTime.MaxValue, true))
                .Returns(true);

            var sut = new LearnAimRef_85Rule(handler.Object, service.Object, learningDeliveryFAMQS.Object, dd07.Object, dateTimeQS.Object);

            sut.Validate(mockLearner.Object);

            handler.VerifyAll();
            service.VerifyAll();
            learningDeliveryFAMQS.VerifyAll();
            dd07.VerifyAll();
        }

        public LearnAimRef_85Rule NewRule()
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            var learningDeliveryFAMQS = new Mock<ILearningDeliveryFAMQueryService>(MockBehavior.Strict);
            var dd07 = new Mock<IDerivedData_07Rule>(MockBehavior.Strict);
            var dateTimeQS = new Mock<IDateTimeQueryService>(MockBehavior.Strict);

            return new LearnAimRef_85Rule(handler.Object, service.Object, learningDeliveryFAMQS.Object, dd07.Object, dateTimeQS.Object);
        }
    }
}
