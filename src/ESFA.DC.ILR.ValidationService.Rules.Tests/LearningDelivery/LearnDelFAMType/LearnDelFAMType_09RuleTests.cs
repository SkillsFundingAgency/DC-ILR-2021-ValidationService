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
    /// <summary>
    /// learning delivery funding and monitoring rule 09 tests
    /// </summary>
    public class LearnDelFAMType_09RuleTests
    {
        /// <summary>
        /// New rule with null message handler throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullMessageHandlerThrows()
        {
            // arrange
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new LearnDelFAMType_09Rule(null, commonOps.Object));
        }

        /// <summary>
        /// New rule with null common operations throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullCommonOperationsThrows()
        {
            // arrange
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new LearnDelFAMType_09Rule(handler.Object, null));
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
            Assert.Equal("LearnDelFAMType_09", result);
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
            Assert.Equal(RuleNameConstants.LearnDelFAMType_09, result);
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
        /// Faulty fam code meets expectation.
        /// </summary>
        [Fact]
        public void FaultyFAMCodeMeetsExpectation()
        {
            // arrange / act / assert
            Assert.Equal("105", LearningDeliveryFAMCodeConstants.SOF_ESFA_Adult);
        }

        /// <summary>
        /// Has disqualifying monitor meets expectation
        /// </summary>
        /// <param name="famType">The Learning Delivery FAM Type.</param>
        /// <param name="famCode">The Learning Delivery FAM Code.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData("LDM", "034", false)] // Monitoring.Delivery.OLASSOffendersInCustody
        [InlineData("FFI", "1", false)] // Monitoring.Delivery.FullyFundedLearningAim
        [InlineData("FFI", "2", false)] // Monitoring.Delivery.CoFundedLearningAim
        [InlineData("LDM", "363", false)] // Monitoring.Delivery.InReceiptOfLowWages
        [InlineData("LDM", "318", false)] // Monitoring.Delivery.MandationToSkillsTraining
        [InlineData("LDM", "328", false)] // Monitoring.Delivery.ReleasedOnTemporaryLicence
        [InlineData("LDM", "347", false)] // Monitoring.Delivery.SteelIndustriesRedundancyTraining
        [InlineData("SOF", "1", true)] // Monitoring.Delivery.HigherEducationFundingCouncilEngland
        [InlineData("SOF", "107", true)] // Monitoring.Delivery.ESFA16To19Funding
        [InlineData("SOF", "105", false)] // Monitoring.Delivery.ESFAAdultFunding
        [InlineData("SOF", "110", true)] // Monitoring.Delivery.GreaterManchesterCombinedAuthority
        [InlineData("SOF", "111", true)] // Monitoring.Delivery.LiverpoolCityRegionCombinedAuthority
        [InlineData("SOF", "112", true)] // Monitoring.Delivery.WestMidlandsCombinedAuthority
        [InlineData("SOF", "113", true)] // Monitoring.Delivery.WestOfEnglandCombinedAuthority
        [InlineData("SOF", "114", true)] // Monitoring.Delivery.TeesValleyCombinedAuthority
        [InlineData("SOF", "115", true)] // Monitoring.Delivery.CambridgeshireAndPeterboroughCombinedAuthority
        [InlineData("SOF", "116", true)] // Monitoring.Delivery.GreaterLondonAuthority
        public void HasDisqualifyingMonitorMeetsExpectation(string famType, string famCode, bool expectation)
        {
            // arrange
            var sut = NewRule();
            var fam = new Mock<ILearningDeliveryFAM>();
            fam
                .SetupGet(y => y.LearnDelFAMType)
                .Returns(famType);
            fam
                .SetupGet(y => y.LearnDelFAMCode)
                .Returns(famCode);

            // act
            var result = sut.HasDisqualifyingMonitor(fam.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Has qualifying monitor with null fams returns false
        /// </summary>
        [Fact]
        public void HasQualifyingMonitorWithNullFAMsReturnsFalse()
        {
            // arrange
            var delivery = new Mock<ILearningDelivery>();

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonOps
                .Setup(x => x.CheckDeliveryFAMs(delivery.Object, It.IsAny<Func<ILearningDeliveryFAM, bool>>()))
                .Returns(false);

            var sut = new LearnDelFAMType_09Rule(handler.Object, commonOps.Object);

            // act
            var result = sut.HasDisqualifyingMonitor(delivery.Object);

            // assert
            Assert.False(result);
        }

        /// <summary>
        /// First inviable date meets expectation.
        /// </summary>
        [Fact]
        public void FirstInviableDateMeetsExpectation()
        {
            // arrange / act / assert
            Assert.Equal(DateTime.Parse("2019-08-01"), LearnDelFAMType_09Rule.FirstInviableDate);
        }

        /// <summary>
        /// Esfa adult funding meets expectation.
        /// </summary>
        [Fact]
        public void ESFAAdultFundingMeetsExpectation()
        {
            // arrange / act / assert
            Assert.Equal("SOF105", Monitoring.Delivery.ESFAAdultFunding);
        }

        /// <summary>
        /// Source of funding meets expectation.
        /// </summary>
        [Fact]
        public void SourceOfFundingMeetsExpectation()
        {
            // arrange / act / assert
            Assert.Equal("SOF", Monitoring.Delivery.Types.SourceOfFunding);
        }

        /// <summary>
        /// Type of funding meets expectation.
        /// </summary>
        /// <param name="expectation">The expectation.</param>
        /// <param name="candidate">The candidate.</param>
        [Theory]
        [InlineData(10, TypeOfFunding.CommunityLearning)]
        [InlineData(35, TypeOfFunding.AdultSkills)]
        [InlineData(36, TypeOfFunding.ApprenticeshipsFrom1May2017)]
        [InlineData(70, TypeOfFunding.EuropeanSocialFund)]
        [InlineData(81, TypeOfFunding.OtherAdult)]
        public void TypeOfFundingMeetsExpectation(int expectation, int candidate)
        {
            // arrange / act / assert
            Assert.Equal(expectation, candidate);
        }

        /// <summary>
        /// Has qualifying fund model meets expectation
        /// </summary>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void HasQualifyingFundModelMeetsExpectation(bool expectation)
        {
            // arrange
            var mockItem = new Mock<ILearningDelivery>();

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonOps
                .Setup(x => x.HasQualifyingFunding(
                    mockItem.Object,
                    10, // TypeOfFunding.CommunityLearning,
                    35, // TypeOfFunding.AdultSkills
                    36, // TypeOfFunding.ApprenticeshipsFrom1May2017,
                    70, // TypeOfFunding.EuropeanSocialFund,
                    81)) // TypeOfFunding.OtherAdult
                .Returns(expectation);

            var sut = new LearnDelFAMType_09Rule(handler.Object, commonOps.Object);

            // act
            var result = sut.HasQualifyingFunding(mockItem.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Invalid item raises validation message.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        [Theory]
        [InlineData(35)] // TypeOfFunding.AdultSkills
        [InlineData(36)] // TypeOfFunding.ApprenticeshipsFrom1May2017
        [InlineData(10)] // TypeOfFunding.CommunityLearning
        [InlineData(70)] // TypeOfFunding.EuropeanSocialFund
        [InlineData(81)] // TypeOfFunding.OtherAdult
        public void InvalidItemRaisesValidationMessage(int candidate)
        {
            // arrange
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
                .Setup(x => x.BuildErrorMessageParameter("LearnDelFAMType", "SOF")) // Monitoring.Delivery.Types.SourceOfFunding
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter("LearnDelFAMCode", "105"))
                .Returns(new Mock<IErrorMessageParameter>().Object);

            // these two operations control the path
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonOps
                .Setup(x => x.HasQualifyingFunding(
                    delivery.Object,
                    10, // TypeOfFunding.CommunityLearning,
                    35, // TypeOfFunding.AdultSkills
                    36, // TypeOfFunding.ApprenticeshipsFrom1May2017,
                    70, // TypeOfFunding.EuropeanSocialFund,
                    81)) // TypeOfFunding.OtherAdult
                .Returns(true);
            commonOps
                .Setup(x => x.CheckDeliveryFAMs(delivery.Object, It.IsAny<Func<ILearningDeliveryFAM, bool>>()))
                .Returns(true);

            var sut = new LearnDelFAMType_09Rule(handler.Object, commonOps.Object);

            // act
            sut.Validate(learner.Object);

            // assert
            handler.VerifyAll();
            commonOps.VerifyAll();
        }

        /// <summary>
        /// Valid item does not raise validation message.
        /// the conditions here will get you to the final check which will return false for 'IsEarlyStageNVQ'
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        [Theory]
        [InlineData(35)] // TypeOfFunding.AdultSkills
        [InlineData(36)] // TypeOfFunding.ApprenticeshipsFrom1May2017
        [InlineData(10)] // TypeOfFunding.CommunityLearning
        [InlineData(70)] // TypeOfFunding.EuropeanSocialFund
        [InlineData(81)] // TypeOfFunding.OtherAdult
        public void ValidItemDoesNotRaiseValidationMessage(int candidate)
        {
            // arrange
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

            // these two operations control the path
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonOps
                .Setup(x => x.HasQualifyingFunding(
                    delivery.Object,
                    10, // TypeOfFunding.CommunityLearning,
                    35, // TypeOfFunding.AdultSkills
                    36, // TypeOfFunding.ApprenticeshipsFrom1May2017,
                    70, // TypeOfFunding.EuropeanSocialFund,
                    81)) // TypeOfFunding.OtherAdult
                .Returns(true);

            commonOps
                .Setup(x => x.CheckDeliveryFAMs(delivery.Object, It.IsAny<Func<ILearningDeliveryFAM, bool>>()))
                .Returns(false);

            var sut = new LearnDelFAMType_09Rule(handler.Object, commonOps.Object);

            // act
            sut.Validate(learner.Object);

            // assert
            handler.VerifyAll();
            commonOps.VerifyAll();
        }

        /// <summary>
        /// New rule.
        /// </summary>
        /// <returns>a constructed and mocked up validation rule</returns>
        public LearnDelFAMType_09Rule NewRule()
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);

            return new LearnDelFAMType_09Rule(handler.Object, commonOps.Object);
        }
    }
}
