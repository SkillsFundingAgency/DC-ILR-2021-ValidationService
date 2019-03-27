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
    /// from version 1.1 validation spread sheet
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
        /// Has esfa adult funding meets expectation
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
        public void HasESFAAdultFundingMeetsExpectation(string famType, string famCode, bool expectation)
        {
            // arrange
            var sut = NewRule();
            var mockItem = new Mock<ILearningDeliveryFAM>();
            mockItem
                .SetupGet(y => y.LearnDelFAMType)
                .Returns(famType);
            mockItem
                .SetupGet(y => y.LearnDelFAMCode)
                .Returns(famCode);

            // act
            var result = sut.HasESFAAdultFunding(mockItem.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        [Fact]
        public void HasESFAAdultFundingMeetsExpectation_NullCheck()
        {
            // arrange
            var sut = NewRule();
            ILearningDeliveryFAM learningDeliverFAM = null;

            // act
            var result = sut.HasESFAAdultFunding(learningDeliverFAM);

            // assert
            Assert.False(result);
        }

        /// <summary>
        /// Has esfa adult funding with null fams returns false
        /// </summary>
        [Fact]
        public void HasESFAAdultFundingWithNullFAMsReturnsFalse()
        {
            // arrange
            var mockItem = new Mock<ILearningDelivery>();

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonOps
                .Setup(x => x.CheckDeliveryFAMs(mockItem.Object, Moq.It.IsAny<Func<ILearningDeliveryFAM, bool>>()))
                .Returns(false);

            var sut = new LearnDelFAMType_09Rule(handler.Object, commonOps.Object);

            // act
            var result = sut.HasESFAAdultFunding(mockItem.Object);

            // assert
            Assert.False(result);
        }

        /// <summary>
        /// Has qualifying fund model meets expectation
        /// </summary>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void IsQualifyingFundModelMeetsExpectation(bool expectation)
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
            const string learnAimRef = "salddfkjeifdnase";

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.LearnAimRef)
                .Returns(learnAimRef);
            mockDelivery
                .SetupGet(y => y.FundModel)
                .Returns(candidate);
            mockDelivery
                .SetupGet(y => y.AimSeqNumber)
                .Returns(0);

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
                    mockDelivery.Object,
                    10, // TypeOfFunding.CommunityLearning,
                    35, // TypeOfFunding.AdultSkills
                    36, // TypeOfFunding.ApprenticeshipsFrom1May2017,
                    70, // TypeOfFunding.EuropeanSocialFund,
                    81)) // TypeOfFunding.OtherAdult
                .Returns(true);
            commonOps
                .Setup(x => x.CheckDeliveryFAMs(mockDelivery.Object, It.IsAny<Func<ILearningDeliveryFAM, bool>>()))
                .Returns(true);

            var sut = new LearnDelFAMType_09Rule(handler.Object, commonOps.Object);

            // act
            sut.Validate(mockLearner.Object);

            // assert
            handler.VerifyAll();
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
            const string learnAimRef = "salddfkjeifdnase";

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.LearnAimRef)
                .Returns(learnAimRef);
            mockDelivery
                .SetupGet(y => y.FundModel)
                .Returns(candidate);

            var deliveries = new ILearningDelivery[] { mockDelivery.Object };

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            // these two operations control the path
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonOps
                .Setup(x => x.HasQualifyingFunding(
                    mockDelivery.Object,
                    10, // TypeOfFunding.CommunityLearning,
                    35, // TypeOfFunding.AdultSkills
                    36, // TypeOfFunding.ApprenticeshipsFrom1May2017,
                    70, // TypeOfFunding.EuropeanSocialFund,
                    81)) // TypeOfFunding.OtherAdult
                .Returns(true);

            commonOps
                .Setup(x => x.CheckDeliveryFAMs(mockDelivery.Object, It.IsAny<Func<ILearningDeliveryFAM, bool>>()))
                .Returns(false);

            var sut = new LearnDelFAMType_09Rule(handler.Object, commonOps.Object);

            // act
            sut.Validate(mockLearner.Object);

            // assert
            handler.VerifyAll();
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
