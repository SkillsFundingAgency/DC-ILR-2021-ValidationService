using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnAimRef;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.LearnAimRef
{
    public class LearnAimRef_72RuleTests
    {
        /// <summary>
        /// New rule with null message handler throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullMessageHandlerThrows()
        {
            // arrange
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            var fcsData = new Mock<IFCSDataService>(MockBehavior.Strict);
            var larsData = new Mock<ILARSDataService>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new LearnAimRef_72Rule(null, commonOps.Object, fcsData.Object, larsData.Object));
        }

        /// <summary>
        /// New rule with null common operations throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullCommonOperationsThrows()
        {
            // arrange
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var fcsData = new Mock<IFCSDataService>(MockBehavior.Strict);
            var larsData = new Mock<ILARSDataService>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new LearnAimRef_72Rule(handler.Object, null, fcsData.Object, larsData.Object));
        }

        /// <summary>
        /// New rule with null FCS data throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullFCSDataThrows()
        {
            // arrange
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            var larsData = new Mock<ILARSDataService>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new LearnAimRef_72Rule(handler.Object, commonOps.Object, null, larsData.Object));
        }

        /// <summary>
        /// New rule with null lars data throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullLARSDataThrows()
        {
            // arrange
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            var fcsData = new Mock<IFCSDataService>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new LearnAimRef_72Rule(handler.Object, commonOps.Object, fcsData.Object, null));
        }

        /// <summary>
        /// Rule name 1, matches a literal.
        /// </summary>
        [Fact]
        public void RuleName1()
        {
            // arrange
            var sut = NewRule();

            // act
            var result = sut.RuleName;

            // assert
            Assert.Equal("LearnAimRef_72", result);
        }

        /// <summary>
        /// Rule name 2, matches the constant.
        /// </summary>
        [Fact]
        public void RuleName2()
        {
            // arrange
            var sut = NewRule();

            // act
            var result = sut.RuleName;

            // assert
            Assert.Equal(RuleNameConstants.LearnAimRef_72, result);
        }

        /// <summary>
        /// Rule name 3 test, account for potential false positives.
        /// </summary>
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

        /// <summary>
        /// Validate with null learner throws.
        /// </summary>
        [Fact]
        public void ValidateWithNullLearnerThrows()
        {
            // arrange
            var sut = NewRule();

            // act/assert
            Assert.Throws<ArgumentNullException>(() => sut.Validate(null));
        }

        /// <summary>
        /// Get lars learning delivery for, meets exepctation.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
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
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            var fcsData = new Mock<IFCSDataService>(MockBehavior.Strict);
            var larsData = new Mock<ILARSDataService>(MockBehavior.Strict);
            larsData
                .Setup(x => x.GetDeliveryFor(candidate))
                .Returns(mockReturn.Object);

            var sut = new LearnAimRef_72Rule(handler.Object, commonOps.Object, fcsData.Object, larsData.Object);

            // act
            var result = sut.GetLARSLearningDeliveryFor(mockItem.Object);

            // assert
            Assert.Equal(mockReturn.Object, result);

            handler.VerifyAll();
            commonOps.VerifyAll();
            fcsData.VerifyAll();
            larsData.VerifyAll();
        }

        /// <summary>
        /// Get subject area levels for, meets expectation.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
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
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            var fcsData = new Mock<IFCSDataService>(MockBehavior.Strict);
            fcsData
                .Setup(x => x.GetEligibilityRuleSectorSubjectAreaLevelsFor(candidate))
                .Returns(new IEsfEligibilityRuleSectorSubjectAreaLevel[] { });

            var larsData = new Mock<ILARSDataService>(MockBehavior.Strict);

            var sut = new LearnAimRef_72Rule(handler.Object, commonOps.Object, fcsData.Object, larsData.Object);

            // act
            var result = sut.GetSubjectAreaLevelsFor(mockItem.Object);

            // assert
            Assert.Empty(result);

            handler.VerifyAll();
            commonOps.VerifyAll();
            fcsData.VerifyAll();
            larsData.VerifyAll();
        }

        /// <summary>
        /// Has disqualifying subject sector with null lars delivery returns false
        /// </summary>
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

        /// <summary>
        /// Is usable subject area with null subject area returns false
        /// </summary>
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

        /// <summary>
        /// Is usable subject area meets expectation
        /// </summary>
        /// <param name="area">The area.</param>
        /// <param name="minLevel">The minimum level.</param>
        /// <param name="maxLevel">The maximum level.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
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

        /// <summary>
        /// Get notional NVQ level v2 meets expectation.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="expectation">The expectation.</param>
        [Theory]
        [InlineData(null, TypeOfNotionalNVQLevelV2.OutOfScope)]
        [InlineData("A", TypeOfNotionalNVQLevelV2.OutOfScope)]
        [InlineData("E", TypeOfNotionalNVQLevelV2.EntryLevel)]
        [InlineData("1", TypeOfNotionalNVQLevelV2.Level1)]
        [InlineData("2", TypeOfNotionalNVQLevelV2.Level2)]
        [InlineData("3", TypeOfNotionalNVQLevelV2.Level3)]
        [InlineData("H", TypeOfNotionalNVQLevelV2.HigherLevel)]
        [InlineData("1.5", TypeOfNotionalNVQLevelV2.OutOfScope)]
        [InlineData("4", TypeOfNotionalNVQLevelV2.OutOfScope)]
        [InlineData("5", TypeOfNotionalNVQLevelV2.OutOfScope)]
        [InlineData("6", TypeOfNotionalNVQLevelV2.OutOfScope)]
        [InlineData("7", TypeOfNotionalNVQLevelV2.OutOfScope)]
        [InlineData("8", TypeOfNotionalNVQLevelV2.OutOfScope)]
        [InlineData("M", TypeOfNotionalNVQLevelV2.OutOfScope)]
        [InlineData("X", TypeOfNotionalNVQLevelV2.OutOfScope)]
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

        /// <summary>
        /// Has disqualifying notional level meets expectation
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(TypeOfNotionalNVQLevelV2.OutOfScope, true)]
        [InlineData(TypeOfNotionalNVQLevelV2.EntryLevel, false)]
        [InlineData(TypeOfNotionalNVQLevelV2.Level1, false)]
        [InlineData(TypeOfNotionalNVQLevelV2.Level2, false)]
        [InlineData(TypeOfNotionalNVQLevelV2.Level3, false)]
        [InlineData(TypeOfNotionalNVQLevelV2.HigherLevel, false)]
        public void IsOutOfScopeMeetsExpectation(double candidate, bool expectation)
        {
            // arrange
            var sut = NewRule();

            // act
            var result = sut.IsOutOfScope(candidate);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Has disqualifying minimum level meets expectation
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="notionalLevel">The notional level.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData("E", TypeOfNotionalNVQLevelV2.EntryLevel, false)]
        [InlineData("A", TypeOfNotionalNVQLevelV2.EntryLevel, false)]
        [InlineData("1", TypeOfNotionalNVQLevelV2.EntryLevel, true)]
        [InlineData("2", TypeOfNotionalNVQLevelV2.EntryLevel, true)]
        [InlineData("1", TypeOfNotionalNVQLevelV2.Level2, false)]
        [InlineData("2", TypeOfNotionalNVQLevelV2.Level2, false)]
        [InlineData("1234568", TypeOfNotionalNVQLevelV2.EntryLevel, false)]
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

        /// <summary>
        /// Has qualifying maximum level meets expectation
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="notionalLevel">The notional level.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData("E", TypeOfNotionalNVQLevelV2.EntryLevel, false)]
        [InlineData("A", TypeOfNotionalNVQLevelV2.EntryLevel, true)]
        [InlineData("1", TypeOfNotionalNVQLevelV2.EntryLevel, false)]
        [InlineData("2", TypeOfNotionalNVQLevelV2.EntryLevel, false)]
        [InlineData("1", TypeOfNotionalNVQLevelV2.Level2, true)]
        [InlineData("2", TypeOfNotionalNVQLevelV2.Level2, false)]
        [InlineData("1234568", TypeOfNotionalNVQLevelV2.EntryLevel, true)]
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

        /// <summary>
        /// Invalid item raises validation message.
        /// </summary>
        /// <param name="notional">The notional.</param>
        /// <param name="min">The minimum.</param>
        /// <param name="max">The maximum.</param>
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

            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonOps
                .Setup(x => x.HasQualifyingFunding(delivery.Object, 70)) // TypeOfFunding.EuropeanSocialFund
                .Returns(true);

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

            var sut = new LearnAimRef_72Rule(handler.Object, commonOps.Object, fcsData.Object, larsData.Object);

            // act
            sut.Validate(mockLearner.Object);

            // assert
            handler.VerifyAll();
            commonOps.VerifyAll();
            fcsData.VerifyAll();
            larsData.VerifyAll();
        }

        /// <summary>
        /// Valid item does not raise validation message.
        /// </summary>
        /// <param name="notional">The notional.</param>
        /// <param name="min">The minimum.</param>
        /// <param name="max">The maximum.</param>
        /// <param name="area">The area.</param>
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
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonOps
                .Setup(x => x.HasQualifyingFunding(delivery.Object, 70)) // TypeOfFunding.EuropeanSocialFund
                .Returns(true);

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

            var sut = new LearnAimRef_72Rule(handler.Object, commonOps.Object, fcsData.Object, larsData.Object);

            // act
            sut.Validate(mockLearner.Object);

            // assert
            handler.VerifyAll();
            commonOps.VerifyAll();
            fcsData.VerifyAll();
            larsData.VerifyAll();
        }

        /// <summary>
        /// New rule.
        /// </summary>
        /// <returns>a constructed and mocked up validation rule</returns>
        public LearnAimRef_72Rule NewRule()
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            var fcsData = new Mock<IFCSDataService>(MockBehavior.Strict);
            var larsData = new Mock<ILARSDataService>(MockBehavior.Strict);

            return new LearnAimRef_72Rule(handler.Object, commonOps.Object, fcsData.Object, larsData.Object);
        }
    }
}
