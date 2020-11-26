using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Learner.EngGrade;
using Moq;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Learner.EngGrade
{
    public class EngGrade_03RuleTests
    {
        [Fact]
        public void RuleName()
        {
            var sut = NewRule();

            var result = sut.RuleName;

            Assert.Equal("EngGrade_03", result);
        }

        [Theory]
        [InlineData("A**", false)]
        [InlineData("A*", false)]
        [InlineData("A", false)]
        [InlineData("AB", false)]
        [InlineData("B", false)]
        [InlineData("BC", false)]
        [InlineData("C", false)]
        [InlineData("CD", false)]
        [InlineData("D", true)]
        [InlineData("DD", true)]
        [InlineData("DE", true)]
        [InlineData("E", true)]
        [InlineData("EE", true)]
        [InlineData("EF", true)]
        [InlineData("F", true)]
        [InlineData("FF", true)]
        [InlineData("FG", true)]
        [InlineData("G", true)]
        [InlineData("GG", true)]
        [InlineData("N", true)]
        [InlineData("U", true)]
        [InlineData("3", true)]
        [InlineData("2", true)]
        [InlineData("1", true)]
        [InlineData("", false)]
        [InlineData(null, false)]
        public void IsEligibleForFundingMeetsExpectation(string candidate, bool expectation)
        {
            var sut = NewRule();
            var mockItem = new Mock<ILearner>();
            mockItem
                .SetupGet(y => y.EngGrade)
                .Returns(candidate);

            var result = sut.IsEligibleForFunding(mockItem.Object);

            Assert.Equal(expectation, result);
        }

        [Theory]
        [InlineData(Monitoring.Learner.NotAchievedLevel2EnglishGCSEByYear11, true)]
        [InlineData(Monitoring.Learner.NotAchievedLevel2MathsGCSEByYear11, false)]
        public void HasEligibleFundingMeetsExpectation(string candidate, bool expectation)
        {
            var sut = NewRule();
            var mockItem = new Mock<ILearnerFAM>();
            mockItem
                .SetupGet(y => y.LearnFAMType)
                .Returns(candidate.Substring(0, 3));
            mockItem
                .SetupGet(y => y.LearnFAMCode)
                .Returns(int.Parse(candidate.Substring(3)));

            var result = sut.HasEligibleFunding(mockItem.Object);

            Assert.Equal(expectation, result);
        }

        [Theory]
        [InlineData("D")]
        [InlineData("DD")]
        [InlineData("DE")]
        [InlineData("E")]
        [InlineData("EE")]
        [InlineData("EF")]
        [InlineData("F")]
        [InlineData("FF")]
        [InlineData("FG")]
        [InlineData("G")]
        [InlineData("GG")]
        [InlineData("N")]
        [InlineData("U")]
        [InlineData("3")]
        [InlineData("2")]
        [InlineData("1")]
        public void InvalidItemRaisesValidationMessage(string candidate)
        {
            const string LearnRefNumber = "123456789X";

            var fams = new List<ILearnerFAM>();

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            mockLearner
                .SetupGet(x => x.EngGrade)
                .Returns(candidate);
            mockLearner
                .SetupGet(x => x.LearnerFAMs)
                .Returns(fams);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            handler
                .Setup(x => x.Handle(
                    Moq.It.Is<string>(y => y == EngGrade_03Rule.Name),
                    Moq.It.Is<string>(y => y == LearnRefNumber),
                    null,
                    null));

            var sut = new EngGrade_03Rule(handler.Object);

            sut.Validate(mockLearner.Object);

            handler.VerifyAll();
        }

        [Theory]
        [InlineData("A**")]
        [InlineData("A*")]
        [InlineData("A")]
        [InlineData("AB")]
        [InlineData("B")]
        [InlineData("BC")]
        [InlineData("C")]
        [InlineData("CD")]
        [InlineData("D")]
        [InlineData("DD")]
        [InlineData("DE")]
        [InlineData("E")]
        [InlineData("EE")]
        [InlineData("EF")]
        [InlineData("F")]
        [InlineData("FF")]
        [InlineData("FG")]
        [InlineData("G")]
        [InlineData("GG")]
        [InlineData("N")]
        [InlineData("U")]
        [InlineData("3")]
        [InlineData("2")]
        [InlineData("1")]
        [InlineData("")]
        [InlineData(null)]
        public void ValidItemDoesNotRaiseValidationMessage(string candidate)
        {
            const string LearnRefNumber = "123456789X";

            var mockFAM = new Mock<ILearnerFAM>();
            mockFAM
                .SetupGet(x => x.LearnFAMType)
                .Returns(Monitoring.Learner.Types.EligibilityFor16To19DisadvantageFunding);
            mockFAM
                .SetupGet(x => x.LearnFAMCode)
                .Returns(2);

            var fams = new List<ILearnerFAM>();
            fams.Add(mockFAM.Object);

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            mockLearner
                .SetupGet(x => x.EngGrade)
                .Returns(candidate);
            mockLearner
                .SetupGet(x => x.LearnerFAMs)
                .Returns(fams);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            var sut = new EngGrade_03Rule(handler.Object);

            sut.Validate(mockLearner.Object);

            handler.VerifyAll();
        }

        public EngGrade_03Rule NewRule()
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            return new EngGrade_03Rule(handler.Object);
        }
    }
}
