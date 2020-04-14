using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Learner.ContPrefType;
using ESFA.DC.ILR.ValidationService.Utility;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Learner.ContPrefType
{
    public class ContPrefType_04RuleTests
    {
        /// <summary>
        /// New rule with null message handler throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullMessageHandlerThrows()
        {
            // arrange / act / assert
            Assert.Throws<ArgumentNullException>(() => new ContPrefType_04Rule(null));
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
            Assert.Equal("ContPrefType_04", result);
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
            Assert.Equal(RuleNameConstants.ContPrefType_04, result);
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
        /// <param name="preGDPR">pre GDPR.</param>
        /// <param name="postGDPR">post GDPR.</param>
        [Theory]
        [InlineData(1, 6)]
        [InlineData(1, 7)]
        [InlineData(2, 6)]
        [InlineData(2, 7)]
        public void InvalidItemRaisesValidationMessage(int preGDPR, int postGDPR)
        {
            // arrange
            const string learnRefNumber = "123456789X";

            var preferences = new List<IContactPreference>();

            var preference = new Mock<IContactPreference>();
            preference
                .SetupGet(y => y.ContPrefType)
                .Returns("RUI");
            preference
                .SetupGet(y => y.ContPrefCode)
                .Returns(preGDPR);

            preferences.Add(preference.Object);

            preference = new Mock<IContactPreference>();
            preference
                .SetupGet(y => y.ContPrefType)
                .Returns("RUI");
            preference
                .SetupGet(y => y.ContPrefCode)
                .Returns(postGDPR);

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
                .Setup(x => x.Handle("ContPrefType_04", learnRefNumber, null, Moq.It.IsAny<IEnumerable<IErrorMessageParameter>>()));
            handler
                .Setup(y => y.BuildErrorMessageParameter("ContPrefType", "RUI"))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(y => y.BuildErrorMessageParameter("ContPrefCode", "(incompatible combination)"))
                .Returns(new Mock<IErrorMessageParameter>().Object);

            var sut = new ContPrefType_04Rule(handler.Object);

            // act
            sut.Validate(mockLearner.Object);

            // assert
            handler.VerifyAll();
        }

        /// <summary>
        /// Valid item does not raise validation message.
        /// </summary>
        /// <param name="preGDPR">pre GDPR.</param>
        /// <param name="postGDPR">post GDPR.</param>
        [Theory]
        [InlineData(1, 2)]
        [InlineData(1, 3)]
        [InlineData(1, 4)]
        [InlineData(1, 5)]
        [InlineData(2, 1)]
        [InlineData(2, 3)]
        [InlineData(2, 4)]
        [InlineData(2, 5)]
        public void ValidItemDoesNotRaiseValidationMessage(int preGDPR, int postGDPR)
        {
            // arrange
            const string learnRefNumber = "123456789X";

            var preferences = new List<IContactPreference>();

            var preference = new Mock<IContactPreference>();
            preference
                .SetupGet(y => y.ContPrefType)
                .Returns("RUI");
            preference
                .SetupGet(y => y.ContPrefCode)
                .Returns(preGDPR);

            preferences.Add(preference.Object);

            preference = new Mock<IContactPreference>();
            preference
                .SetupGet(y => y.ContPrefType)
                .Returns("RUI");
            preference
                .SetupGet(y => y.ContPrefCode)
                .Returns(postGDPR);

            preferences.Add(preference.Object);

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(learnRefNumber);
            mockLearner
                .SetupGet(x => x.ContactPreferences)
                .Returns(preferences);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            var sut = new ContPrefType_04Rule(handler.Object);

            // act
            sut.Validate(mockLearner.Object);

            // assert
            handler.VerifyAll();
        }

        /// <summary>
        /// New rule.
        /// </summary>
        /// <returns>a newly contructed rule</returns>
        private ContPrefType_04Rule NewRule()
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            return new ContPrefType_04Rule(handler.Object);
        }
    }
}