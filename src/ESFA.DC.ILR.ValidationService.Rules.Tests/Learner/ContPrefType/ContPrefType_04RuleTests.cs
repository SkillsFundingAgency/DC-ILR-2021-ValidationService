using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Learner.ContPrefType;
using Moq;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Learner.ContPrefType
{
    public class ContPrefType_04RuleTests
    {
        [Fact]
        public void RuleName()
        {
            var sut = NewRule();

            var result = sut.RuleName;

            Assert.Equal("ContPrefType_04", result);
        }

        [Theory]
        [InlineData(1, 6)]
        [InlineData(1, 7)]
        [InlineData(2, 6)]
        [InlineData(2, 7)]
        public void InvalidItemRaisesValidationMessage(int preGDPR, int postGDPR)
        {
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

            sut.Validate(mockLearner.Object);

            handler.VerifyAll();
        }

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

            sut.Validate(mockLearner.Object);

            handler.VerifyAll();
        }

        private ContPrefType_04Rule NewRule()
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            return new ContPrefType_04Rule(handler.Object);
        }
    }
}