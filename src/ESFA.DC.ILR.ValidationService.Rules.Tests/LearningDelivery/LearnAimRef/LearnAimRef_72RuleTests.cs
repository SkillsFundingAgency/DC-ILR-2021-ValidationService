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
    public class LearnAimRef_72RuleTests
    {
        [Fact]
        public void RuleName()
        {
            // arrange
            var sut = NewRule();

            // act
            var result = sut.RuleName;

            // assert
            Assert.Equal("LearnAimRef_72", result);
        }

        [Theory]
        [InlineData("testAim_1")]
        [InlineData("testAim_2")]
        [InlineData("testAim_3")]
        public void GetLARSLearningDeliveryForMeetsExepctation(string candidate)
        {
            // arrange
            var mockItem = new Mock<ILearningDelivery>();
            mockItem
                .SetupGet(x => x.LearnAimRef)
                .Returns(candidate);

            var mockReturn = new Mock<ILARSLearningDelivery>();

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var fcsData = new Mock<IFCSDataService>(MockBehavior.Strict);
            var larsData = new Mock<ILARSDataService>(MockBehavior.Strict);
            larsData
                .Setup(x => x.GetDeliveryFor(candidate))
                .Returns(mockReturn.Object);

            var sut = new LearnAimRef_72Rule(handler.Object, fcsData.Object, larsData.Object);

            // act
            var result = sut.GetLARSLearningDeliveryFor(mockItem.Object);

            // assert
            Assert.Equal(mockReturn.Object, result);

            handler.VerifyAll();
            fcsData.VerifyAll();
            larsData.VerifyAll();
        }

        [Theory]
        [InlineData("testAim_1")]
        [InlineData("testAim_2")]
        [InlineData("testAim_3")]
        public void GetSubjectAreaLevelsForMeetsExpectation(string candidate)
        {
            // arrange
            var mockItem = new Mock<ILearningDelivery>();
            mockItem
                .SetupGet(x => x.ConRefNumber)
                .Returns(candidate);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            var fcsData = new Mock<IFCSDataService>(MockBehavior.Strict);
            fcsData
                .Setup(x => x.GetEligibilityRuleSectorSubjectAreaLevelsFor(candidate))
                .Returns(new IEsfEligibilityRuleSectorSubjectAreaLevel[] { });

            var larsData = new Mock<ILARSDataService>(MockBehavior.Strict);

            var sut = new LearnAimRef_72Rule(handler.Object, fcsData.Object, larsData.Object);

            // act
            var result = sut.GetSubjectAreaLevelsFor(mockItem.Object);

            // assert
            Assert.Empty(result);

            handler.VerifyAll();
            fcsData.VerifyAll();
            larsData.VerifyAll();
        }

        [Fact]
        public void HasDisqualifyingSubjectSectorWithNullLARSDeliveryReturnsFalse()
        {
            // arrange
            var sut = NewRule();

            // act
            var result = sut.HasDisqualifyingSubjectSector(null, new IEsfEligibilityRuleSectorSubjectAreaLevel[] { });

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
        [InlineData(null, "blahMin", "blahMax", true)]
        [InlineData(1.0, "blahMin", "blahMax", false)]
        [InlineData(null, null, null, false)]
        [InlineData(1.1, null, null, false)]
        [InlineData(null, "blahMin", null, true)]
        [InlineData(1.2, "blahMin", null, false)]
        [InlineData(null, null, "blahMax", true)]
        [InlineData(1.3, null, "blahMax", false)]
        public void IsUsableSubjectAreaMeetsExpectation(double? area, string minLevel, string maxLevel, bool expectation)
        {
            // arrange
            var sut = NewRule();
            var mockItem = new Mock<IEsfEligibilityRuleSectorSubjectAreaLevel>();
            mockItem
                .SetupGet(x => x.SectorSubjectAreaCode)
                .Returns((decimal?)area);
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
        [InlineData(null, TypeOfNotionalNVQLevelV2.OutOfScope)]
        [InlineData("A", TypeOfNotionalNVQLevelV2.OutOfScope)]
        [InlineData("E", TypeOfNotionalNVQLevelV2.EntryLevel)]
        [InlineData("1", TypeOfNotionalNVQLevelV2.Level1)]
        [InlineData("2", TypeOfNotionalNVQLevelV2.Level2)]
        [InlineData("3", TypeOfNotionalNVQLevelV2.Level3)]
        [InlineData("H", TypeOfNotionalNVQLevelV2.HigherLevel)]
        [InlineData("1.5", TypeOfNotionalNVQLevelV2.Level1_2)]
        [InlineData("4", TypeOfNotionalNVQLevelV2.Level4)]
        [InlineData("5", TypeOfNotionalNVQLevelV2.Level5)]
        [InlineData("6", TypeOfNotionalNVQLevelV2.Level6)]
        [InlineData("7", TypeOfNotionalNVQLevelV2.Level7)]
        [InlineData("8", TypeOfNotionalNVQLevelV2.Level8)]
        [InlineData("M", TypeOfNotionalNVQLevelV2.MixedLevel)]
        [InlineData("X", TypeOfNotionalNVQLevelV2.NotKnown)]
        public void GetNotionalNVQLevelV2MeetsExpectation(string candidate, double expectation)
        {
            // arrange
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

        [Theory]
        [InlineData("E", TypeOfNotionalNVQLevelV2.EntryLevel, false)]
        [InlineData("A", TypeOfNotionalNVQLevelV2.EntryLevel, true)]
        [InlineData("1", TypeOfNotionalNVQLevelV2.EntryLevel, true)]
        [InlineData("2", TypeOfNotionalNVQLevelV2.EntryLevel, true)]
        [InlineData("1", TypeOfNotionalNVQLevelV2.Level2, false)]
        [InlineData("2", TypeOfNotionalNVQLevelV2.Level2, false)]
        [InlineData("1234568", TypeOfNotionalNVQLevelV2.EntryLevel, true)]
        public void HasDisqualifyingMinimumLevelMeetsExpectation(string candidate, double notionalLevel, bool expectation)
        {
            // arrange
            var sut = NewRule();
            var mockItem = new Mock<IEsfEligibilityRuleSectorSubjectAreaLevel>();
            mockItem
                .SetupGet(x => x.MinLevelCode)
                .Returns(candidate);

            // act
            var result = sut.HasDisqualifyingMinimumLevel(mockItem.Object, notionalLevel);

            // assert
            Assert.Equal(expectation, result);
        }

        [Theory]
        [InlineData("E", TypeOfNotionalNVQLevelV2.EntryLevel, false)]
        [InlineData("A", TypeOfNotionalNVQLevelV2.Level3, false)]
        [InlineData("1", TypeOfNotionalNVQLevelV2.EntryLevel, false)]
        [InlineData("2", TypeOfNotionalNVQLevelV2.EntryLevel, false)]
        [InlineData("1", TypeOfNotionalNVQLevelV2.Level2, true)]
        [InlineData("2", TypeOfNotionalNVQLevelV2.Level2, false)]
        [InlineData("1234568", TypeOfNotionalNVQLevelV2.OutOfScope, false)]
        public void HasDisqualifyingMaximumLevelMeetsExpectation(string candidate, double notionalLevel, bool expectation)
        {
            // arrange
            var sut = NewRule();
            var mockItem = new Mock<IEsfEligibilityRuleSectorSubjectAreaLevel>();
            mockItem
                .SetupGet(x => x.MaxLevelCode)
                .Returns(candidate);

            // act
            var result = sut.HasDisqualifyingMaximumLevel(mockItem.Object, notionalLevel);

            // assert
            Assert.Equal(expectation, result);
        }

        [Theory]
        [InlineData("1", "2", "3")] // fails @ min level
        [InlineData("H", "2", "3")] // fails @ max level
        public void InvalidItemRaisesValidationMessage(string notional, string min, string max)
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
                .Setup(x => x.Handle(RuleNameConstants.LearnAimRef_72, LearnRefNumber, 0, It.IsAny<IEnumerable<IErrorMessageParameter>>()));
            handler
                .Setup(x => x.BuildErrorMessageParameter("FundModel", 70)) // TypeOfFunding.EuropeanSocialFund
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter("ConRefNumber", ContractRefNumber))
                .Returns(new Mock<IErrorMessageParameter>().Object);

            var eligibilityItem = new Mock<IEsfEligibilityRuleSectorSubjectAreaLevel>();
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

            var larsData = new Mock<ILARSDataService>(MockBehavior.Strict);
            larsData
                .Setup(x => x.GetDeliveryFor(AimRefNumber))
                .Returns(larsItem.Object);

            var sut = new LearnAimRef_72Rule(handler.Object, fcsData.Object, larsData.Object);

            // act
            sut.Validate(mockLearner.Object);

            // assert
            handler.VerifyAll();
            fcsData.VerifyAll();
            larsData.VerifyAll();
        }

        [Theory]
        [InlineData("2", "2", "3", 1.0)]
        [InlineData("2", "2", "3", null)]
        [InlineData("3", "2", "3", 1.0)]
        [InlineData("3", "2", "3", null)]
        public void ValidItemDoesNotRaiseValidationMessage(string notional, string min, string max, double? area)
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

            var eligibilityItem = new Mock<IEsfEligibilityRuleSectorSubjectAreaLevel>();
            eligibilityItem
                .SetupGet(x => x.SectorSubjectAreaCode)
                .Returns((decimal?)area);
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
            var larsData = new Mock<ILARSDataService>(MockBehavior.Strict);
            larsData
                .Setup(x => x.GetDeliveryFor(AimRefNumber))
                .Returns(larsItem.Object);

            var sut = new LearnAimRef_72Rule(handler.Object, fcsData.Object, larsData.Object);

            // act
            sut.Validate(mockLearner.Object);

            // assert
            handler.VerifyAll();
            fcsData.VerifyAll();
            larsData.VerifyAll();
        }

        public LearnAimRef_72Rule NewRule()
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var fcsData = new Mock<IFCSDataService>(MockBehavior.Strict);
            var larsData = new Mock<ILARSDataService>(MockBehavior.Strict);

            return new LearnAimRef_72Rule(handler.Object, fcsData.Object, larsData.Object);
        }
    }
}
