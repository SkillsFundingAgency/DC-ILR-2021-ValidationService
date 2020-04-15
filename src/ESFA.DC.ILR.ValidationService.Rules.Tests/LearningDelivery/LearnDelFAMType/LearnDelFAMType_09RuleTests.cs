using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnDelFAMType;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.LearnDelFAMType
{
    public class LearnDelFAMType_09RuleTests
    {
        [Fact]
        public void RuleName()
        {
            var sut = NewRule();

            var result = sut.RuleName;

            Assert.Equal("LearnDelFAMType_09", result);
        }

        [Fact]
        public void FaultyFAMCodeMeetsExpectation()
        {
            Assert.Equal("105", LearningDeliveryFAMCodeConstants.SOF_ESFA_Adult);
        }

        [Theory]
        [InlineData("LDM", "034", false)]
        [InlineData("FFI", "1", false)]
        [InlineData("FFI", "2", false)]
        [InlineData("LDM", "363", false)]
        [InlineData("LDM", "318", false)]
        [InlineData("LDM", "328", false)]
        [InlineData("LDM", "347", false)]
        [InlineData("SOF", "1", true)]
        [InlineData("SOF", "107", true)]
        [InlineData("SOF", "105", false)]
        [InlineData("SOF", "110", true)]
        [InlineData("SOF", "111", true)]
        [InlineData("SOF", "112", true)]
        [InlineData("SOF", "113", true)]
        [InlineData("SOF", "114", true)]
        [InlineData("SOF", "115", true)]
        [InlineData("SOF", "116", true)]
        public void HasDisqualifyingMonitorMeetsExpectation(string famType, string famCode, bool expectation)
        {
            var sut = NewRule();
            var fam = new Mock<ILearningDeliveryFAM>();
            fam
                .SetupGet(y => y.LearnDelFAMType)
                .Returns(famType);
            fam
                .SetupGet(y => y.LearnDelFAMCode)
                .Returns(famCode);

            var result = sut.HasDisqualifyingMonitor(fam.Object);

            Assert.Equal(expectation, result);
        }

        [Fact]
        public void HasQualifyingMonitorWithNullFAMsReturnsFalse()
        {
            var delivery = new Mock<ILearningDelivery>();

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonOps
                .Setup(x => x.CheckDeliveryFAMs(delivery.Object, It.IsAny<Func<ILearningDeliveryFAM, bool>>()))
                .Returns(false);

            var sut = new LearnDelFAMType_09Rule(handler.Object, commonOps.Object);

            var result = sut.HasDisqualifyingMonitor(delivery.Object);

            Assert.False(result);
        }

        [Fact]
        public void FirstInviableDateMeetsExpectation()
        {
            Assert.Equal(DateTime.Parse("2019-08-01"), LearnDelFAMType_09Rule.FirstInviableDate);
        }

        [Fact]
        public void ESFAAdultFundingMeetsExpectation()
        {
            Assert.Equal("SOF105", Monitoring.Delivery.ESFAAdultFunding);
        }

        [Fact]
        public void SourceOfFundingMeetsExpectation()
        {
            Assert.Equal("SOF", Monitoring.Delivery.Types.SourceOfFunding);
        }

        [Theory]
        [InlineData(10, TypeOfFunding.CommunityLearning)]
        [InlineData(35, TypeOfFunding.AdultSkills)]
        [InlineData(36, TypeOfFunding.ApprenticeshipsFrom1May2017)]
        [InlineData(70, TypeOfFunding.EuropeanSocialFund)]
        [InlineData(81, TypeOfFunding.OtherAdult)]
        public void TypeOfFundingMeetsExpectation(int expectation, int candidate)
        {
            Assert.Equal(expectation, candidate);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void HasQualifyingFundModelMeetsExpectation(bool expectation)
        {
            var mockItem = new Mock<ILearningDelivery>();

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonOps
                .Setup(x => x.HasQualifyingFunding(
                    mockItem.Object,
                    10,
                    35,
                    36,
                    70,
                    81))
                .Returns(expectation);

            var sut = new LearnDelFAMType_09Rule(handler.Object, commonOps.Object);

            var result = sut.HasQualifyingFunding(mockItem.Object);

            Assert.Equal(expectation, result);
        }

        [Theory]
        [InlineData(35)]
        [InlineData(36)]
        [InlineData(10)]
        [InlineData(70)]
        [InlineData(81)]
        public void InvalidItemRaisesValidationMessage(int candidate)
        {
            const string LearnRefNumber = "123456789X";
            const string LearnAimRef = "salddfkjeifdnase";

            var fam = new Mock<ILearningDeliveryFAM>();
            var fams = new ILearningDeliveryFAM[] { fam.Object };

            var delivery = new Mock<ILearningDelivery>();
            delivery
                .SetupGet(y => y.LearnAimRef)
                .Returns(LearnAimRef);
            delivery
                .SetupGet(y => y.FundModel)
                .Returns(candidate);
            delivery
                .SetupGet(y => y.LearnStartDate)
                .Returns(DateTime.Parse("2019-07-31"));
            delivery
                .SetupGet(y => y.LearningDeliveryFAMs)
                .Returns(fams);

            var deliveries = new ILearningDelivery[] { delivery.Object };

            var learner = new Mock<ILearner>();
            learner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            learner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            handler
                .Setup(x => x.Handle(RuleNameConstants.LearnDelFAMType_09, LearnRefNumber, 0, It.IsAny<IEnumerable<IErrorMessageParameter>>()));
            handler
                .Setup(x => x.BuildErrorMessageParameter("FundModel", candidate))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter("LearnDelFAMType", "SOF"))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter("LearnDelFAMCode", "105"))
                .Returns(new Mock<IErrorMessageParameter>().Object);

            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonOps
                .Setup(x => x.HasQualifyingFunding(
                    delivery.Object,
                    10,
                    35,
                    36,
                    70,
                    81))
                .Returns(true);
            commonOps
                .Setup(x => x.CheckDeliveryFAMs(delivery.Object, It.IsAny<Func<ILearningDeliveryFAM, bool>>()))
                .Returns(true);

            var sut = new LearnDelFAMType_09Rule(handler.Object, commonOps.Object);

            sut.Validate(learner.Object);

            handler.VerifyAll();
            commonOps.VerifyAll();
        }

        [Theory]
        [InlineData(35)]
        [InlineData(36)]
        [InlineData(10)]
        [InlineData(70)]
        [InlineData(81)]
        public void ValidItemDoesNotRaiseValidationMessage(int candidate)
        {
            const string LearnRefNumber = "123456789X";
            const string LearnAimRef = "salddfkjeifdnase";
            var fam = new Mock<ILearningDeliveryFAM>();
            var fams = new ILearningDeliveryFAM[] { fam.Object };

            var delivery = new Mock<ILearningDelivery>();
            delivery
                .SetupGet(y => y.LearnAimRef)
                .Returns(LearnAimRef);
            delivery
                .SetupGet(y => y.FundModel)
                .Returns(candidate);
            delivery
                .SetupGet(y => y.LearnStartDate)
                .Returns(DateTime.Parse("2019-07-31"));
            delivery
                .SetupGet(y => y.LearningDeliveryFAMs)
                .Returns(fams);

            var deliveries = new ILearningDelivery[] { delivery.Object };

            var learner = new Mock<ILearner>();
            learner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            learner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonOps
                .Setup(x => x.HasQualifyingFunding(
                    delivery.Object,
                    10,
                    35,
                    36,
                    70,
                    81))
                .Returns(true);

            commonOps
                .Setup(x => x.CheckDeliveryFAMs(delivery.Object, It.IsAny<Func<ILearningDeliveryFAM, bool>>()))
                .Returns(false);

            var sut = new LearnDelFAMType_09Rule(handler.Object, commonOps.Object);

            sut.Validate(learner.Object);

            handler.VerifyAll();
            commonOps.VerifyAll();
        }

        public LearnDelFAMType_09Rule NewRule()
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);

            return new LearnDelFAMType_09Rule(handler.Object, commonOps.Object);
        }
    }
}
