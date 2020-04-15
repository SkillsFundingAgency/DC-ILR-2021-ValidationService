using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Learner.ContPrefType;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Learner.ContPrefType
{
    public class ContPrefType_01RuleTests
    {
        [Fact]
        public void RuleName()
        {
            var sut = NewRule();

            var result = sut.RuleName;

            Assert.Equal("ContPrefType_01", result);
        }

        [Theory]
        [InlineData("bla1")]
        [InlineData("bla2")]
        [InlineData("bla3")]
        [InlineData("bla4")]
        [InlineData("bla5")]
        [InlineData("bla6")]
        public void InvalidItemRaisesValidationMessage(string candidate)
        {
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
                .Setup(x => x.Handle("ContPrefType_01", learnRefNumber, null, Moq.It.IsAny<IEnumerable<IErrorMessageParameter>>()));
            handler
                .Setup(y => y.BuildErrorMessageParameter("ContPrefType", prefType))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(y => y.BuildErrorMessageParameter("ContPrefCode", prefCode))
                .Returns(new Mock<IErrorMessageParameter>().Object);

            var provider = new Mock<IProvideLookupDetails>(MockBehavior.Strict);
            provider
                .Setup(x => x.Contains(TypeOfLimitedLifeLookup.ContPrefType, candidate))
                .Returns(false);

            var sut = new ContPrefType_01Rule(handler.Object, provider.Object);

            sut.Validate(mockLearner.Object);

            handler.VerifyAll();
        }

        [Theory]
        [InlineData("bla1")]
        [InlineData("bla2")]
        [InlineData("bla3")]
        [InlineData("bla4")]
        [InlineData("bla5")]
        [InlineData("bla6")]
        public void ValidItemDoesNotRaiseValidationMessage(string candidate)
        {
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

            var provider = new Mock<IProvideLookupDetails>(MockBehavior.Strict);
            provider
                .Setup(x => x.Contains(TypeOfLimitedLifeLookup.ContPrefType, candidate))
                .Returns(true);

            var sut = new ContPrefType_01Rule(handler.Object, provider.Object);

            sut.Validate(mockLearner.Object);

            handler.VerifyAll();
        }

        private ContPrefType_01Rule NewRule()
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var provider = new Mock<IProvideLookupDetails>(MockBehavior.Strict);

            return new ContPrefType_01Rule(handler.Object, provider.Object);
        }
    }
}