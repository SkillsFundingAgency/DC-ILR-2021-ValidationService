using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnAimRef;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Utility;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.LearnAimRef
{
    public class LearnAimRef_85RuleTests
    {
        /// <summary>
        /// New rule with null message handler throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullMessageHandlerThrows()
        {
            // arrange
            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            var commonChecks = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new LearnAimRef_85Rule(null, service.Object, commonChecks.Object));
        }

        /// <summary>
        /// New rule with null lars service throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullLARSServiceThrows()
        {
            // arrange
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var commonChecks = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new LearnAimRef_85Rule(handler.Object, null, commonChecks.Object));
        }

        /// <summary>
        /// New rule with null common checks throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullCommonchecksThrows()
        {
            // arrange
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var service = new Mock<ILARSDataService>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new LearnAimRef_85Rule(handler.Object, service.Object, null));
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
            Assert.Equal("LearnAimRef_85", result);
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
            Assert.Equal(LearnAimRef_85Rule.Name, result);
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
        /// First viable date meets expectation.
        /// </summary>
        [Fact]
        public void FirstViableDateMeetsExpectation()
        {
            // arrange / act
            var result = LearnAimRef_85Rule.FirstViableDate;

            // assert
            Assert.Equal(DateTime.Parse("2017-08-01"), result);
        }

        /// <summary>
        /// Is disqualifying notional NVQ meets expectation
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(LARSNotionalNVQLevelV2.EntryLevel, false)]
        [InlineData(LARSNotionalNVQLevelV2.HigherLevel, false)]
        [InlineData(LARSNotionalNVQLevelV2.Level1, false)]
        [InlineData(LARSNotionalNVQLevelV2.Level1_2, false)]
        [InlineData(LARSNotionalNVQLevelV2.Level2, false)]
        [InlineData(LARSNotionalNVQLevelV2.Level3, true)]
        [InlineData(LARSNotionalNVQLevelV2.Level4, false)]
        [InlineData(LARSNotionalNVQLevelV2.Level5, false)]
        [InlineData(LARSNotionalNVQLevelV2.Level6, false)]
        [InlineData(LARSNotionalNVQLevelV2.Level7, false)]
        [InlineData(LARSNotionalNVQLevelV2.Level8, false)]
        [InlineData(LARSNotionalNVQLevelV2.MixedLevel, false)]
        [InlineData(LARSNotionalNVQLevelV2.NotKnown, false)]
        public void IsDisqualifyingNotionalNVQMeetsExpectation(string candidate, bool expectation)
        {
            // arrange
            var sut = NewRule();
            var mockDelivery = new Mock<ILARSLearningDelivery>();
            mockDelivery
                .SetupGet(y => y.NotionalNVQLevelv2)
                .Returns(candidate);

            // act
            var result = sut.IsDisqualifyingNotionalNVQ(mockDelivery.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Has qualifying notional NVQ meets expectation
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        [Theory]
        [InlineData(null)]
        [InlineData("testAim1")]
        [InlineData("testAim2")]
        public void HasQualifyingNotionalNVQMeetsExpectation(string candidate)
        {
            // arrange
            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(x => x.LearnAimRef)
                .Returns(candidate);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            service
                .Setup(x => x.GetDeliveryFor(candidate))
                .Returns((ILARSLearningDelivery)null);

            var commonChecks = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);

            var sut = new LearnAimRef_85Rule(handler.Object, service.Object, commonChecks.Object);

            // act
            var result = sut.HasDisqualifyingNotionalNVQ(mockDelivery.Object);

            // assert
            handler.VerifyAll();
            service.VerifyAll();
            commonChecks.VerifyAll();

            Assert.False(result);
        }

        /// <summary>
        /// Invalid item raises validation message.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        [Theory]
        [InlineData(TypeOfPriorAttainment.FullLevel3)]
        [InlineData(TypeOfPriorAttainment.Level4)]
        [InlineData(TypeOfPriorAttainment.Level4Expired20130731)]
        [InlineData(TypeOfPriorAttainment.Level5)]
        [InlineData(TypeOfPriorAttainment.Level5AndAboveExpired20130731)]
        [InlineData(TypeOfPriorAttainment.Level6)]
        [InlineData(TypeOfPriorAttainment.Level7AndAbove)]
        [InlineData(TypeOfPriorAttainment.OtherLevelNotKnown)]
        [InlineData(TypeOfPriorAttainment.NotKnown)]
        public void InvalidItemRaisesValidationMessage(int candidate)
        {
            // arrange
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
                .Returns(TypeOfFunding.AdultSkills);

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
                .Setup(x => x.BuildErrorMessageParameter("FundModel", TypeOfFunding.AdultSkills))
                .Returns(new Mock<IErrorMessageParameter>().Object);

            var mockLars = new Mock<ILARSLearningDelivery>();
            mockLars
                .SetupGet(x => x.NotionalNVQLevelv2)
                .Returns(LARSNotionalNVQLevelV2.Level3);

            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            service
                .Setup(x => x.GetDeliveryFor(learnAimRef))
                .Returns(mockLars.Object);

            var commonChecks = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonChecks
                .Setup(x => x.IsRestart(mockDelivery.Object))
                .Returns(false);
            commonChecks
                .Setup(x => x.InApprenticeship(mockDelivery.Object))
                .Returns(false);
            commonChecks
                .Setup(x => x.HasQualifyingFunding(mockDelivery.Object, TypeOfFunding.AdultSkills))
                .Returns(true);
            commonChecks
                .Setup(x => x.HasQualifyingStart(mockDelivery.Object, LearnAimRef_85Rule.FirstViableDate, null))
                .Returns(true);

            var sut = new LearnAimRef_85Rule(handler.Object, service.Object, commonChecks.Object);

            // act
            sut.Validate(mockLearner.Object);

            // assert
            handler.VerifyAll();
            service.VerifyAll();
            commonChecks.VerifyAll();
        }

        /// <summary>
        /// Valid item does not raise validation message.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        [Theory]
        [InlineData(TypeOfPriorAttainment.FullLevel3)]
        [InlineData(TypeOfPriorAttainment.Level4)]
        [InlineData(TypeOfPriorAttainment.Level4Expired20130731)]
        [InlineData(TypeOfPriorAttainment.Level5)]
        [InlineData(TypeOfPriorAttainment.Level5AndAboveExpired20130731)]
        [InlineData(TypeOfPriorAttainment.Level6)]
        [InlineData(TypeOfPriorAttainment.Level7AndAbove)]
        [InlineData(TypeOfPriorAttainment.OtherLevelNotKnown)]
        [InlineData(TypeOfPriorAttainment.NotKnown)]
        public void ValidItemDoesNotRaiseValidationMessage(int candidate)
        {
            // arrange
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
                .Returns(TypeOfFunding.AdultSkills);

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
                .Returns(LARSNotionalNVQLevelV2.Level2);

            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            service
                .Setup(x => x.GetDeliveryFor(learnAimRef))
                .Returns(mockLars.Object);

            var commonChecks = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonChecks
                .Setup(x => x.IsRestart(mockDelivery.Object))
                .Returns(false);
            commonChecks
                .Setup(x => x.InApprenticeship(mockDelivery.Object))
                .Returns(false);
            commonChecks
                .Setup(x => x.HasQualifyingFunding(mockDelivery.Object, TypeOfFunding.AdultSkills))
                .Returns(true);
            commonChecks
                .Setup(x => x.HasQualifyingStart(mockDelivery.Object, LearnAimRef_85Rule.FirstViableDate, null))
                .Returns(true);

            var sut = new LearnAimRef_85Rule(handler.Object, service.Object, commonChecks.Object);

            // act
            sut.Validate(mockLearner.Object);

            // assert
            handler.VerifyAll();
            service.VerifyAll();
            commonChecks.VerifyAll();
        }

        /// <summary>
        /// New rule.
        /// </summary>
        /// <returns>a constructed and mocked up validation rule</returns>
        public LearnAimRef_85Rule NewRule()
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            var commonChecks = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);

            return new LearnAimRef_85Rule(handler.Object, service.Object, commonChecks.Object);
        }
    }
}
