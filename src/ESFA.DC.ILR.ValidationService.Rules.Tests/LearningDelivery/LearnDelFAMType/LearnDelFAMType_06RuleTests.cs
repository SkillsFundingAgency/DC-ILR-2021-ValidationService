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
        /// <summary>
        /// New rule with null meessage handler throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullMessageHandlerThrows()
        {
            // arrange
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var lookups = new Mock<IProvideLookupDetails>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new LearnDelFAMType_06Rule(null, lookups.Object, commonOps.Object));
        }

        /// <summary>
        /// New rule with null lookups provider throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullLookupsProviderThrows()
        {
            // arrange
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var lookups = new Mock<IProvideLookupDetails>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new LearnDelFAMType_06Rule(handler.Object, null, commonOps.Object));
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
            Assert.Equal("LearnDelFAMType_06", result);
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
            Assert.Equal(RuleNameConstants.LearnDelFAMType_06, result);
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
            Assert.NotEqual("SomeRandomRuleName_01", result);
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
        /// Is qualifying delivery meets expectation
        /// </summary>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void IsQualifyingDeliveryMeetsExpectation(bool expectation)
        {
            // arrange
            var delivery = new Mock<ILearningDelivery>();

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var lookups = new Mock<IProvideLookupDetails>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonOps
                .Setup(x => x.IsRestart(delivery.Object))
                .Returns(!expectation);

            var sut = new LearnDelFAMType_06Rule(handler.Object, lookups.Object, commonOps.Object);

            // act
            var result = sut.IsQualifyingDelivery(delivery.Object);

            // assert
            Assert.Equal(expectation, result);

            handler.VerifyAll();
            lookups.VerifyAll();
        }

        /// <summary>
        /// Checks the delivery fams meets expectation.
        /// does not throw and does not execute any lookup comparisons
        /// </summary>
        [Fact]
        public void CheckDeliveryFAMsMeetsExpectation()
        {
            // arrange
            var delivery = new Mock<ILearningDelivery>();

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var lookups = new Mock<IProvideLookupDetails>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            var sut = new LearnDelFAMType_06Rule(handler.Object, lookups.Object, commonOps.Object);

            // act
            sut.CheckDeliveryFAMs(delivery.Object, x => { }); // <= the action cannot be null

            // assert
            handler.VerifyAll();
            lookups.VerifyAll();
        }

        /// <summary>
        /// Is not current meets expectation
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="testDate">The test date.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData("FTP1", "2016-09-04", true)]
        [InlineData("FTP2", "2016-09-05", false)]
        [InlineData("LDM358", "2099-12-31", true)]
        [InlineData("LDM358", "2018-03-31", false)]
        public void IsNotCurrentMeetsExpectation(string candidate, string testDate, bool expectation)
        {
            // arrange
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
                .Setup(x => x.IsVaguelyCurrent(TypeOfLimitedLifeLookup.LearningDeliveryFAM, candidate, referenceDate))
                .Returns(!expectation);

            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);

            var sut = new LearnDelFAMType_06Rule(handler.Object, lookups.Object, commonOps.Object);

            // act
            var result = sut.IsNotCurrent(monitor.Object, referenceDate);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Invalid item raises validation message.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="testDate">The test date.</param>
        [Theory]
        [InlineData("FTP1", "2016-09-04")]
        [InlineData("FTP2", "2016-09-05")]
        public void InvalidItemRaisesValidationMessage(string candidate, string testDate)
        {
            // arrange
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
                .Setup(x => x.IsVaguelyCurrent(TypeOfLimitedLifeLookup.LearningDeliveryFAM, candidate, referenceDate))
                .Returns(false);

            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonOps
                .Setup(x => x.IsRestart(mockDelivery.Object))
                .Returns(false);

            var sut = new LearnDelFAMType_06Rule(handler.Object, lookups.Object, commonOps.Object);

            // act
            sut.Validate(mockLearner.Object);

            // assert
            handler.VerifyAll();
            lookups.VerifyAll();
        }

        /// <summary>
        /// Valid item does not raise validation message.
        /// the conditions here will get you to the final check which will return false for 'IsEarlyStageNVQ'
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="testDate">The test date.</param>
        [Theory]
        [InlineData("FTP1", "2016-09-04")]
        [InlineData("FTP2", "2016-09-05")]
        public void ValidItemDoesNotRaiseValidationMessage(string candidate, string testDate)
        {
            // arrange
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
                .Setup(x => x.IsVaguelyCurrent(TypeOfLimitedLifeLookup.LearningDeliveryFAM, candidate, referenceDate))
                .Returns(true);

            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonOps
                .Setup(x => x.IsRestart(mockDelivery.Object))
                .Returns(false);

            var sut = new LearnDelFAMType_06Rule(handler.Object, lookups.Object, commonOps.Object);

            // act
            sut.Validate(mockLearner.Object);

            // assert
            handler.VerifyAll();
            lookups.VerifyAll();
        }

        /// <summary>
        /// New rule.
        /// </summary>
        /// <returns>a constructed and mocked up validation rule</returns>
        public LearnDelFAMType_06Rule NewRule()
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var lookups = new Mock<IProvideLookupDetails>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);

            return new LearnDelFAMType_06Rule(handler.Object, lookups.Object, commonOps.Object);
        }
    }
}
