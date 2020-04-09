using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Learner.ContPrefType;
using ESFA.DC.ILR.ValidationService.Utility;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Learner.ContPrefType
{
    public class ContPrefType_03RuleTests
    {
        /// <summary>
        /// New rule with null message handler throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullMessageHandlerThrows()
        {
            // arrange
            var ddRule06 = new Mock<IDerivedData_06Rule>(MockBehavior.Strict);
            var provider = new Mock<IProvideLookupDetails>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new ContPrefType_03Rule(null, ddRule06.Object, provider.Object));
        }

        [Fact]
        public void NewRuleWithNullDerivedDataThrows()
        {
            // arrange
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var provider = new Mock<IProvideLookupDetails>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new ContPrefType_03Rule(handler.Object, null, provider.Object));
        }

        [Fact]
        public void NewRuleWithNullProviderThrows()
        {
            // arrange
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var ddRule06 = new Mock<IDerivedData_06Rule>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new ContPrefType_03Rule(handler.Object, ddRule06.Object, null));
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
            Assert.Equal("ContPrefType_03", result);
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
            Assert.Equal(RuleNameConstants.ContPrefType_03, result);
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
        /// Invalid item raises validation message.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        [Theory]
        [InlineData("bla1")]
        [InlineData("bla2")]
        [InlineData("bla3")]
        [InlineData("bla4")]
        [InlineData("bla5")]
        [InlineData("bla6")]
        public void InvalidItemRaisesValidationMessage(string candidate)
        {
            // arrange
            const string learnRefNumber = "123456789X";

            var preferences = new List<IContactPreference>();

            var prefType = candidate.Substring(0, 3);
            var prefCode = int.Parse(candidate.Substring(3));

            var preference = new Mock<IContactPreference>();
            preference
                .SetupGet(y => y.ContPrefType)
                .Returns(prefType);
            preference
                .SetupGet(y => y.ContPrefCode)
                .Returns(prefCode);
            preferences.Add(preference.Object);

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(learnRefNumber);
            mockLearner
                .SetupGet(x => x.ContactPreferences)
                .Returns(preferences);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            handler
                .Setup(x => x.Handle("ContPrefType_03", learnRefNumber, null, Moq.It.IsAny<IEnumerable<IErrorMessageParameter>>()));
            handler
                .Setup(y => y.BuildErrorMessageParameter("DD06", DateTime.Today.ToString("d", AbstractRule.RequiredCulture)))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(y => y.BuildErrorMessageParameter("ContPrefType", prefType))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(y => y.BuildErrorMessageParameter("ContPrefCode", prefCode))
                .Returns(new Mock<IErrorMessageParameter>().Object);

            var ddRule06 = new Mock<IDerivedData_06Rule>(MockBehavior.Strict);
            ddRule06
                .Setup(x => x.Derive(Moq.It.IsAny<IReadOnlyCollection<ILearningDelivery>>()))
                .Returns(DateTime.Today);

            // pass or fail is determined by this call
            var provider = new Mock<IProvideLookupDetails>(MockBehavior.Strict);
            provider
                .Setup(x => x.IsCurrent(TypeOfLimitedLifeLookup.ContPrefType, candidate, DateTime.Today))
                .Returns(false);

            var sut = new ContPrefType_03Rule(handler.Object, ddRule06.Object, provider.Object);

            // act
            sut.Validate(mockLearner.Object);

            // assert
            handler.VerifyAll();
        }

        /// <summary>
        /// Valid item does not raise validation message.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        [Theory]
        [InlineData("bla1")]
        [InlineData("bla2")]
        [InlineData("bla3")]
        [InlineData("bla4")]
        [InlineData("bla5")]
        [InlineData("bla6")]
        public void ValidItemDoesNotRaiseValidationMessage(string candidate)
        {
            // arrange
            const string learnRefNumber = "123456789X";

            var preferences = new List<IContactPreference>();

            var prefType = candidate.Substring(0, 3);
            var prefCode = int.Parse(candidate.Substring(3));

            var preference = new Mock<IContactPreference>();
            preference
                .SetupGet(y => y.ContPrefType)
                .Returns(prefType);
            preference
                .SetupGet(y => y.ContPrefCode)
                .Returns(prefCode);
            preferences.Add(preference.Object);

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(learnRefNumber);
            mockLearner
                .SetupGet(x => x.ContactPreferences)
                .Returns(preferences);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            var ddRule06 = new Mock<IDerivedData_06Rule>(MockBehavior.Strict);
            ddRule06
                .Setup(x => x.Derive(Moq.It.IsAny<IReadOnlyCollection<ILearningDelivery>>()))
                .Returns(DateTime.Today);

            // pass or fail is determined by this call
            var provider = new Mock<IProvideLookupDetails>(MockBehavior.Strict);
            provider
                .Setup(x => x.IsCurrent(TypeOfLimitedLifeLookup.ContPrefType, candidate, DateTime.Today))
                .Returns(true);

            var sut = new ContPrefType_03Rule(handler.Object, ddRule06.Object, provider.Object);

            // act
            sut.Validate(mockLearner.Object);

            // assert
            handler.VerifyAll();
        }

        /// <summary>
        /// New rule.
        /// </summary>
        /// <returns>a newly contructed rule</returns>
        private ContPrefType_03Rule NewRule()
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var ddRule06 = new Mock<IDerivedData_06Rule>(MockBehavior.Strict);
            var provider = new Mock<IProvideLookupDetails>(MockBehavior.Strict);

            return new ContPrefType_03Rule(handler.Object, ddRule06.Object, provider.Object);
        }
    }
}