using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnAimRef;
using Moq;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.LearnAimRef
{
    public class LearnAimRef_73RuleTests
    {
        [Fact]
        public void RuleName1()
        {
            // arrange
            var sut = NewRule();

            // act
            var result = sut.RuleName;

            // assert
            Assert.Equal("LearnAimRef_73", result);
        }

        [Fact]
        public void RuleName2()
        {
            // arrange
            var sut = NewRule();

            // act
            var result = sut.RuleName;

            // assert
            Assert.Equal(RuleNameConstants.LearnAimRef_73, result);
        }

        [Fact]
        public void RuleName3()
        {
            // arrange
            var sut = NewRule();

            // act
            var result = sut.RuleName;

            // assert
            Assert.NotEqual("SomeOtherRuleName_07", result);
        }

        [Fact]
        public void HasDisqualifyingSubjectSectorWithNullLARSDeliveryReturnsFalse()
        {
            // arrange
            var sut = NewRule();

            var mockAreaLevel = new Mock<IEsfEligibilityRuleSectorSubjectAreaLevel>();
            mockAreaLevel
                .SetupGet(x => x.SectorSubjectAreaCode)
                .Returns((decimal?)6.10);
            mockAreaLevel
                .SetupGet(x => x.MinLevelCode)
                .Returns("min");
            mockAreaLevel
                .SetupGet(x => x.MaxLevelCode)
                .Returns("max");

            // act
            var result = sut.HasDisqualifyingSubjectSector(null, new List<IEsfEligibilityRuleSectorSubjectAreaLevel>() { mockAreaLevel.Object });

            // assert
            Assert.True(result);
        }

        [Fact]
        public void IsUsableSubjectAreaWithNullSubjectAreaReturnsFalse()
        {
            // arrange
            var sut = NewRule();

            // act
            var result = sut.IsUsableSubjectArea(null);

            // assert
            Assert.False(result);
        }

        [Theory]
        [InlineData(null, null, null, false)]
        [InlineData(null, "blahMin", "blahMax", false)]
        [InlineData(1.0, null, null, false)]
        [InlineData(1.0, "blahMin", null, true)]
        [InlineData(1.0, null, "blahMax", true)]
        [InlineData(2.0, null, null, false)]
        [InlineData(2.0, "blahMin", null, true)]
        [InlineData(2.0, null, "blahMax", true)]
        public void IsUsableSubjectAreaMeetsExpectation(double? areaCode, string minLevel, string maxLevel, bool expectation)
        {
            // arrange
            var sut = NewRule();
            var mockItem = new Mock<IEsfEligibilityRuleSectorSubjectAreaLevel>();
            mockItem
                .SetupGet(x => x.SectorSubjectAreaCode)
                .Returns((decimal?)areaCode);
            mockItem
                .SetupGet(x => x.MinLevelCode)
                .Returns(minLevel);
            mockItem
                .SetupGet(x => x.MaxLevelCode)
                .Returns(maxLevel);

            // act
            var result = sut.IsUsableSubjectArea(mockItem.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        [Theory]
        [InlineData(null, LARSConstants.NotionalNVQLevelV2Doubles.OutOfScope)] // int.minvalue
        [InlineData("A", LARSConstants.NotionalNVQLevelV2Doubles.OutOfScope)] // int.minvalue
        [InlineData("E", LARSConstants.NotionalNVQLevelV2Doubles.EntryLevel)]
        [InlineData("1", LARSConstants.NotionalNVQLevelV2Doubles.Level1)]
        [InlineData("2", LARSConstants.NotionalNVQLevelV2Doubles.Level2)]
        [InlineData("3", LARSConstants.NotionalNVQLevelV2Doubles.Level3)]
        [InlineData("H", LARSConstants.NotionalNVQLevelV2Doubles.HigherLevel)]
        [InlineData("1.5", LARSConstants.NotionalNVQLevelV2Doubles.Level1_2)]
        [InlineData("4", LARSConstants.NotionalNVQLevelV2Doubles.Level4)]
        [InlineData("5", LARSConstants.NotionalNVQLevelV2Doubles.Level5)]
        [InlineData("6", LARSConstants.NotionalNVQLevelV2Doubles.Level6)]
        [InlineData("7", LARSConstants.NotionalNVQLevelV2Doubles.Level7)]
        [InlineData("8", LARSConstants.NotionalNVQLevelV2Doubles.Level8)]
        [InlineData("M", LARSConstants.NotionalNVQLevelV2Doubles.MixedLevel)]
        [InlineData("X", LARSConstants.NotionalNVQLevelV2Doubles.NotKnown)]
        public void GetNotionalNVQLevelV2MeetsExpectation(string candidate, double expectation)
        {
            // arrange
            var test = candidate;
            var sut = NewRule();
            var mockItem = new Mock<ILARSLearningDelivery>();
            mockItem
                .SetupGet(x => x.NotionalNVQLevelv2)
                .Returns(candidate);

            // act
            var result = sut.GetNotionalNVQLevelV2(mockItem.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        [Fact]
        public void FundModelConditionMet()
        {
            Assert.True(NewRule().FundModelConditionMet(70));
        }

        [Fact]
        public void FundModelConditionMet_False()
        {
            Assert.False(NewRule().FundModelConditionMet(99));
        }

        [Theory]
        [InlineData("1", "2", "3", 1.0, 1.0, 1.0)] // fails @ min level
        [InlineData("H", "1", "3", 1.0, 1.0, 1.0)] // fails @ max level
        [InlineData("2", "2", "3", 1.1, 1.0, 1.0)] // fails @ tier1 level
        [InlineData("2", "2", "3", 1.2, 1.1, 1.0)] // fails @ tier2 level
        public void InvalidItemRaisesValidationMessage(string notional, string min, string max, decimal area, decimal tier1, decimal tier2)
        {
            // arrange
            const string LearnRefNumber = "123456789X";
            const string AimRefNumber = "shonkyAimRef";
            const string ContractRefNumber = "shonkyRefNumber";

            var delivery = new Mock<ILearningDelivery>();
            delivery
                .SetupGet(y => y.FundModel)
                .Returns(70); // TypeOfFunding.EuropeanSocialFund
            delivery
                .SetupGet(y => y.LearnAimRef)
                .Returns(AimRefNumber);
            delivery
                .SetupGet(y => y.ConRefNumber)
                .Returns(ContractRefNumber);

            var deliveries = new List<ILearningDelivery> { delivery.Object };

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            handler
                .Setup(x => x.Handle(RuleNameConstants.LearnAimRef_73, LearnRefNumber, 0, It.IsAny<IEnumerable<IErrorMessageParameter>>()));
            handler
                .Setup(x => x.BuildErrorMessageParameter("FundModel", 70)) // TypeOfFunding.EuropeanSocialFund
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter("ConRefNumber", ContractRefNumber))
                .Returns(new Mock<IErrorMessageParameter>().Object);

            var eligibilityItem = new Mock<IEsfEligibilityRuleSectorSubjectAreaLevel>();
            eligibilityItem
                .SetupGet(x => x.SectorSubjectAreaCode)
                .Returns(area);
            eligibilityItem
                .SetupGet(x => x.MinLevelCode)
                .Returns(min);
            eligibilityItem
                .SetupGet(x => x.MaxLevelCode)
                .Returns(max);
            var fcsData = new Mock<IFCSDataService>(MockBehavior.Strict);
            fcsData
                .Setup(x => x.GetEligibilityRuleSectorSubjectAreaLevelsFor(ContractRefNumber))
                .Returns(new IEsfEligibilityRuleSectorSubjectAreaLevel[] { eligibilityItem.Object });

            var larsItem = new Mock<ILARSLearningDelivery>();
            larsItem
                .SetupGet(x => x.NotionalNVQLevelv2)
                .Returns(notional);
            larsItem
                .SetupGet(x => x.SectorSubjectAreaTier1)
                .Returns(tier1);
            larsItem
                .SetupGet(x => x.SectorSubjectAreaTier2)
                .Returns(tier2);
            var larsData = new Mock<ILARSDataService>(MockBehavior.Strict);
            larsData
                .Setup(x => x.GetDeliveryFor(AimRefNumber))
                .Returns(larsItem.Object);

            var sut = new LearnAimRef_73Rule(handler.Object, fcsData.Object, larsData.Object);

            // act
            sut.Validate(mockLearner.Object);

            // assert
            handler.VerifyAll();
            fcsData.VerifyAll();
            larsData.VerifyAll();
        }

        [Theory]
        [InlineData("2", "2", "3", 1.0, 1.0, 2.0)]
        [InlineData("2", "2", "3", 2.0, 1.0, 2.0)]
        [InlineData("3", "2", "3", 1.0, 1.0, 2.0)]
        [InlineData("3", "2", "4", 4.10, 4.0, 4.1)]
        public void ValidItemDoesNotRaiseValidationMessage(string notional, string min, string max, decimal area, decimal tier1, decimal tier2)
        {
            // arrange
            const string LearnRefNumber = "123456789X";
            const string AimRefNumber = "shonkyAimRef";
            const string ContractRefNumber = "shonkyRefNumber";

            var delivery = new Mock<ILearningDelivery>();
            delivery
                .SetupGet(y => y.FundModel)
                .Returns(70); // TypeOfFunding.EuropeanSocialFund
            delivery
                .SetupGet(y => y.LearnAimRef)
                .Returns(AimRefNumber);
            delivery
                .SetupGet(y => y.ConRefNumber)
                .Returns(ContractRefNumber);

            var deliveries = new List<ILearningDelivery> { delivery.Object };

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            var eligibilityItem1 = new Mock<IEsfEligibilityRuleSectorSubjectAreaLevel>();
            eligibilityItem1
                .SetupGet(x => x.SectorSubjectAreaCode)
                .Returns(area);
            eligibilityItem1
                .SetupGet(x => x.MinLevelCode)
                .Returns(min);
            eligibilityItem1
                .SetupGet(x => x.MaxLevelCode)
                .Returns(max);

            var eligibilityItem2 = new Mock<IEsfEligibilityRuleSectorSubjectAreaLevel>();
            eligibilityItem2
                .SetupGet(x => x.SectorSubjectAreaCode)
                .Returns(5m);
            eligibilityItem2
                .SetupGet(x => x.MinLevelCode)
                .Returns("1");
            eligibilityItem2
                .SetupGet(x => x.MaxLevelCode)
                .Returns("3");
            var fcsData = new Mock<IFCSDataService>(MockBehavior.Strict);
            fcsData
                .Setup(x => x.GetEligibilityRuleSectorSubjectAreaLevelsFor(ContractRefNumber))
                .Returns(new IEsfEligibilityRuleSectorSubjectAreaLevel[] { eligibilityItem1.Object, eligibilityItem2.Object });

            var larsItem = new Mock<ILARSLearningDelivery>();
            larsItem
                .SetupGet(x => x.NotionalNVQLevelv2)
                .Returns(notional);
            larsItem
                .SetupGet(x => x.SectorSubjectAreaTier1)
                .Returns(tier1);
            larsItem
                .SetupGet(x => x.SectorSubjectAreaTier2)
                .Returns(tier2);
            var larsData = new Mock<ILARSDataService>(MockBehavior.Strict);
            larsData
                .Setup(x => x.GetDeliveryFor(AimRefNumber))
                .Returns(larsItem.Object);

            var sut = new LearnAimRef_73Rule(handler.Object, fcsData.Object, larsData.Object);

            // act
            sut.Validate(mockLearner.Object);

            // assert
            handler.VerifyAll();
            fcsData.VerifyAll();
            larsData.VerifyAll();
        }

        [Fact]
        public void ValidItemDoesNotRaiseValidationMessage_NoEligibility()
        {
            // arrange
            const string LearnRefNumber = "123456789X";
            const string AimRefNumber = "ZESF0001";
            const string ContractRefNumber = "shonkyRefNumber";

            var delivery = new Mock<ILearningDelivery>();
            delivery
                .SetupGet(y => y.FundModel)
                .Returns(70); // TypeOfFunding.EuropeanSocialFund
            delivery
                .SetupGet(y => y.LearnAimRef)
                .Returns(AimRefNumber);
            delivery
                .SetupGet(y => y.ConRefNumber)
                .Returns(ContractRefNumber);

            var deliveries = new List<ILearningDelivery> { delivery.Object };

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries);

            var handler = new Mock<IValidationErrorHandler>();

            var fcsData = new Mock<IFCSDataService>();
            fcsData
                .Setup(x => x.GetEligibilityRuleSectorSubjectAreaLevelsFor(ContractRefNumber))
                .Returns(new IEsfEligibilityRuleSectorSubjectAreaLevel[] { });

            var larsItem = new Mock<ILARSLearningDelivery>();
            larsItem
                .SetupGet(x => x.NotionalNVQLevelv2)
                .Returns("2");
            larsItem
                .SetupGet(x => x.SectorSubjectAreaTier1)
                .Returns(1m);
            larsItem
                .SetupGet(x => x.SectorSubjectAreaTier2)
                .Returns(2m);
            var larsData = new Mock<ILARSDataService>();
            larsData
                .Setup(x => x.GetDeliveryFor(deliveries[0].LearnAimRef))
                .Returns(larsItem.Object);

            var sut = new LearnAimRef_73Rule(handler.Object, fcsData.Object, larsData.Object);

            // act
            sut.Validate(mockLearner.Object);

            // assert
            handler.VerifyAll();
            fcsData.VerifyAll();
            larsData.VerifyAll();
        }

        public LearnAimRef_73Rule NewRule()
        {
            var handler = new Mock<IValidationErrorHandler>();
            var fcsData = new Mock<IFCSDataService>();
            var larsData = new Mock<ILARSDataService>();

            return new LearnAimRef_73Rule(handler.Object, fcsData.Object, larsData.Object);
        }
    }
}
