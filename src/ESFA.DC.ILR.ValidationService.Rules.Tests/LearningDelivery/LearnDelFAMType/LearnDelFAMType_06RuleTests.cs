using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnDelFAMType;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.LearnDelFAMType
{
    public class LearnDelFAMType_06RuleTests
    {
        [Fact]
        public void RuleName()
        {
            var sut = NewRule();

            var result = sut.RuleName;

            Assert.Equal("LearnDelFAMType_06", result);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void IsQualifyingDeliveryMeetsExpectation(bool expectation)
        {
            var delivery = new Mock<ILearningDelivery>();

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var lookups = new Mock<IProvideLookupDetails>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonOps
                .Setup(x => x.IsRestart(delivery.Object))
                .Returns(!expectation);

            var sut = new LearnDelFAMType_06Rule(handler.Object, lookups.Object, commonOps.Object);

            var result = sut.IsQualifyingDelivery(delivery.Object);

            Assert.Equal(expectation, result);

            handler.VerifyAll();
            lookups.VerifyAll();
        }

        [Fact]
        public void CheckDeliveryFAMsMeetsExpectation()
        {
            var delivery = new Mock<ILearningDelivery>();

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var lookups = new Mock<IProvideLookupDetails>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            var sut = new LearnDelFAMType_06Rule(handler.Object, lookups.Object, commonOps.Object);

            sut.CheckDeliveryFAMs(delivery.Object, x => { });

            handler.VerifyAll();
            lookups.VerifyAll();
        }

        [Theory]
        [InlineData("FTP1", "2016-09-04", true)]
        [InlineData("FTP2", "2016-09-05", false)]
        [InlineData("LDM358", "2099-12-31", true)]
        [InlineData("LDM358", "2018-03-31", false)]
        public void IsNotCurrentMeetsExpectation(string candidate, string testDate, bool expectation)
        {
            var monitor = new Mock<ILearningDeliveryFAM>();
            monitor
                .SetupGet(x => x.LearnDelFAMType)
                .Returns(candidate.Substring(0, 3));
            monitor
                .SetupGet(x => x.LearnDelFAMCode)
                .Returns(candidate.Substring(3));

            var referenceDate = DateTime.Parse(testDate);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var lookups = new Mock<IProvideLookupDetails>(MockBehavior.Strict);
            lookups
                .Setup(x => x.IsVaguelyCurrent(TypeOfLimitedLifeLookup.LearnDelFAMType, candidate, referenceDate))
                .Returns(!expectation);

            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);

            var sut = new LearnDelFAMType_06Rule(handler.Object, lookups.Object, commonOps.Object);

            var result = sut.IsNotCurrent(monitor.Object, referenceDate);

            Assert.Equal(expectation, result);
        }

        [Theory]
        [InlineData("FTP1", "2016-09-04")]
        [InlineData("FTP2", "2016-09-05")]
        public void InvalidItemRaisesValidationMessage(string candidate, string testDate)
        {
            var famType = candidate.Substring(0, 3);
            var famCode = candidate.Substring(3);

            var monitor = new Mock<ILearningDeliveryFAM>();
            monitor
                .SetupGet(x => x.LearnDelFAMType)
                .Returns(famType);
            monitor
                .SetupGet(x => x.LearnDelFAMCode)
                .Returns(famCode);

            var fams = new ILearningDeliveryFAM[] { monitor.Object };

            var referenceDate = DateTime.Parse(testDate);

            const string LearnRefNumber = "123456789X";
            const string learnAimRef = "salddfkjeifdnase";

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.LearnAimRef)
                .Returns(learnAimRef);
            mockDelivery
                .SetupGet(y => y.LearnStartDate)
                .Returns(referenceDate);
            mockDelivery
                .SetupGet(y => y.LearningDeliveryFAMs)
                .Returns(fams);

            var deliveries = new ILearningDelivery[] { mockDelivery.Object };

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            handler
                .Setup(x => x.Handle(RuleNameConstants.LearnDelFAMType_06, LearnRefNumber, 0, It.IsAny<IEnumerable<IErrorMessageParameter>>()));
            handler
                .Setup(x => x.BuildErrorMessageParameter("LearnStartDate", AbstractRule.AsRequiredCultureDate(referenceDate)))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter("LearnDelFAMType", famType))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter("LearnDelFAMCode", famCode))
                .Returns(new Mock<IErrorMessageParameter>().Object);

            var lookups = new Mock<IProvideLookupDetails>(MockBehavior.Strict);
            lookups
                .Setup(x => x.IsVaguelyCurrent(TypeOfLimitedLifeLookup.LearnDelFAMType, candidate, referenceDate))
                .Returns(false);

            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonOps
                .Setup(x => x.IsRestart(mockDelivery.Object))
                .Returns(false);

            var sut = new LearnDelFAMType_06Rule(handler.Object, lookups.Object, commonOps.Object);

            sut.Validate(mockLearner.Object);

            handler.VerifyAll();
            lookups.VerifyAll();
        }

        [Theory]
        [InlineData("FTP1", "2016-09-04")]
        [InlineData("FTP2", "2016-09-05")]
        public void ValidItemDoesNotRaiseValidationMessage(string candidate, string testDate)
        {
            var famType = candidate.Substring(0, 3);
            var famCode = candidate.Substring(3);

            var monitor = new Mock<ILearningDeliveryFAM>();
            monitor
                .SetupGet(x => x.LearnDelFAMType)
                .Returns(famType);
            monitor
                .SetupGet(x => x.LearnDelFAMCode)
                .Returns(famCode);

            var fams = new ILearningDeliveryFAM[] { monitor.Object };

            var referenceDate = DateTime.Parse(testDate);

            const string LearnRefNumber = "123456789X";
            const string learnAimRef = "salddfkjeifdnase";

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.LearnAimRef)
                .Returns(learnAimRef);
            mockDelivery
                .SetupGet(y => y.LearnStartDate)
                .Returns(referenceDate);
            mockDelivery
                .SetupGet(y => y.LearningDeliveryFAMs)
                .Returns(fams);

            var deliveries = new ILearningDelivery[] { mockDelivery.Object };

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var lookups = new Mock<IProvideLookupDetails>(MockBehavior.Strict);
            lookups
                .Setup(x => x.IsVaguelyCurrent(TypeOfLimitedLifeLookup.LearnDelFAMType, candidate, referenceDate))
                .Returns(true);

            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonOps
                .Setup(x => x.IsRestart(mockDelivery.Object))
                .Returns(false);

            var sut = new LearnDelFAMType_06Rule(handler.Object, lookups.Object, commonOps.Object);

            sut.Validate(mockLearner.Object);

            handler.VerifyAll();
            lookups.VerifyAll();
        }

        public LearnDelFAMType_06Rule NewRule()
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var lookups = new Mock<IProvideLookupDetails>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);

            return new LearnDelFAMType_06Rule(handler.Object, lookups.Object, commonOps.Object);
        }
    }
}
