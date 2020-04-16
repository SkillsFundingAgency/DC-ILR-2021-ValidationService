using ESFA.DC.ILR.Model.Interface;
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
    public class LearnAimRef_87RuleTests
    {
        [Fact]
        public void RuleName()
        {
            var sut = NewRule();

            var result = sut.RuleName;

            Assert.Equal("LearnAimRef_87", result);
        }

        [Fact]
        public void FirstViableDateMeetsExpectation()
        {
            var result = LearnAimRef_87Rule.FirstViableDate;

            Assert.Equal(DateTime.Parse("2017-08-01"), result);
        }

        [Theory]
        [InlineData("ZVOC0001", true)]
        [InlineData("ZVOC0002", true)]
        [InlineData("ZVOC0003", true)]
        [InlineData("ZVOC0004", true)]
        [InlineData("ZVOC0005", true)]
        [InlineData("ZVOC0006", true)]
        [InlineData("ZVOC0007", true)]
        [InlineData("ZVOC0008", true)]
        [InlineData("ZVOC0009", true)]
        [InlineData("ZVOC0010", true)]
        [InlineData("ZVOC0011", true)]
        [InlineData("ZVOC0012", true)]
        [InlineData("ZVOC0013", true)]
        [InlineData("ZVOC0014", true)]
        [InlineData("ZVOC0015", true)]
        [InlineData("ZUXA103E", true)]
        [InlineData("ZUXA105C", true)]
        [InlineData("ZUXA107C", true)]
        [InlineData("ZUXA108B", true)]
        [InlineData("ZUXA203E", true)]
        [InlineData("ZUXA204A", true)]
        [InlineData("ZUXA204C", true)]
        [InlineData("ZUXA205C", true)]
        [InlineData("ZUXA206B", true)]
        [InlineData("ZUXA206C", true)]
        [InlineData("ZUXA207C", true)]
        [InlineData("ZUXA208B", true)]
        [InlineData("ZUXA209A", true)]
        [InlineData("ZUXA214A", true)]
        [InlineData("ZUXA214B", true)]
        [InlineData("ZUXA215A", true)]
        [InlineData("ZUXA301A", true)]
        [InlineData("ZUXA301B", true)]
        [InlineData("ZUXA302B", true)]
        [InlineData("ZUXA303E", true)]
        [InlineData("ZUXA304A", true)]
        [InlineData("ZUXA304C", true)]
        [InlineData("ZUXA305A", true)]
        [InlineData("ZUXA305C", true)]
        [InlineData("ZUXA306B", true)]
        [InlineData("ZUXA307C", true)]
        [InlineData("ZUXA314B", true)]
        [InlineData("ZUXA315A", true)]
        [InlineData("ZUXAE05C", true)]
        [InlineData("ZUXAE06B", true)]
        [InlineData("ZUXAE14A", true)]
        [InlineData("ZUXAH01B", true)]
        [InlineData("ZUXAH09C", true)]
        [InlineData("ZUXAH15A", true)]
        [InlineData("ZUXAH15B", true)]
        [InlineData("Z9OPE15B", true)]
        [InlineData("Z9OPE14A", true)]
        [InlineData("z9ope15B", true)]
        [InlineData("z9ope14A", true)]
        [InlineData("x9ope15B", false)]
        [InlineData("x9ope14A", false)]
        [InlineData(TypeOfAim.References.IndustryPlacement, false)]
        [InlineData(TypeOfAim.References.SupportedInternship16To19, false)]
        [InlineData(TypeOfAim.References.WorkExperience, false)]
        public void HasDisqualifyingVocationalAimMeetsExpectation(string candidate, bool expectation)
        {
            var sut = NewRule();
            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.LearnAimRef)
                .Returns(candidate);

            var result = sut.HasDisqualifyingVocationalAim(mockDelivery.Object);

            Assert.Equal(expectation, result);
        }

        [Theory]
        [InlineData("ZVOC0001")]
        [InlineData("ZVOC0002")]
        [InlineData("ZVOC0003")]
        [InlineData("ZVOC0004")]
        [InlineData("ZVOC0005")]
        [InlineData("ZVOC0006")]
        [InlineData("ZVOC0007")]
        [InlineData("ZVOC0008")]
        [InlineData("ZVOC0009")]
        [InlineData("ZVOC0010")]
        [InlineData("ZVOC0011")]
        [InlineData("ZVOC0012")]
        [InlineData("ZVOC0013")]
        [InlineData("ZVOC0014")]
        [InlineData("ZVOC0015")]
        [InlineData("ZUXA103E")]
        [InlineData("ZUXA105C")]
        [InlineData("ZUXA107C")]
        [InlineData("ZUXA108B")]
        [InlineData("ZUXA203E")]
        [InlineData("ZUXA204A")]
        [InlineData("ZUXA204C")]
        [InlineData("ZUXA205C")]
        [InlineData("ZUXA206B")]
        [InlineData("ZUXA206C")]
        [InlineData("ZUXA207C")]
        [InlineData("ZUXA208B")]
        [InlineData("ZUXA209A")]
        [InlineData("ZUXA214A")]
        [InlineData("ZUXA214B")]
        [InlineData("ZUXA215A")]
        [InlineData("ZUXA301A")]
        [InlineData("ZUXA301B")]
        [InlineData("ZUXA302B")]
        [InlineData("ZUXA303E")]
        [InlineData("ZUXA304A")]
        [InlineData("ZUXA304C")]
        [InlineData("ZUXA305A")]
        [InlineData("ZUXA305C")]
        [InlineData("ZUXA306B")]
        [InlineData("ZUXA307C")]
        [InlineData("ZUXA314B")]
        [InlineData("ZUXA315A")]
        [InlineData("ZUXAE05C")]
        [InlineData("ZUXAE06B")]
        [InlineData("ZUXAE14A")]
        [InlineData("ZUXAH01B")]
        [InlineData("ZUXAH09C")]
        [InlineData("ZUXAH15A")]
        [InlineData("ZUXAH15B")]
        [InlineData("Z9OPE15B")]
        [InlineData("Z9OPE14A")]
        [InlineData("z9ope15B")]
        [InlineData("z9ope14A")]
        public void InvalidItemRaisesValidationMessage(string candidate)
        {
            const string learnRefNumber = "123456789X";

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.LearnStartDate)
                .Returns(LearnAimRef_87Rule.FirstViableDate);
            mockDelivery
                .SetupGet(y => y.LearnAimRef)
                .Returns(candidate);

            var deliveries = new List<ILearningDelivery>();
            deliveries.Add(mockDelivery.Object);

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(learnRefNumber);
            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            handler
                .Setup(x => x.Handle("LearnAimRef_87", learnRefNumber, 0, Moq.It.IsAny<IEnumerable<IErrorMessageParameter>>()));
            handler
                .Setup(x => x.BuildErrorMessageParameter("LearnAimRef", candidate))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter("LearnStartDate", LearnAimRef_87Rule.FirstViableDate))
                .Returns(new Mock<IErrorMessageParameter>().Object);

            var commonChecks = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonChecks
                .Setup(x => x.HasQualifyingStart(mockDelivery.Object, LearnAimRef_87Rule.FirstViableDate, null))
                .Returns(true);

            var sut = new LearnAimRef_87Rule(handler.Object, commonChecks.Object);

            sut.Validate(mockLearner.Object);

            handler.VerifyAll();
            commonChecks.VerifyAll();
        }

        [Theory]
        [InlineData("x9ope15B")]
        [InlineData("x9ope14A")]
        [InlineData(TypeOfAim.References.IndustryPlacement)]
        [InlineData(TypeOfAim.References.SupportedInternship16To19)]
        [InlineData(TypeOfAim.References.WorkExperience)]
        public void ValidItemDoesNotRaiseValidationMessage(string candidate)
        {
            const string learnRefNumber = "123456789X";

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.LearnStartDate)
                .Returns(LearnAimRef_87Rule.FirstViableDate);
            mockDelivery
                .SetupGet(y => y.LearnAimRef)
                .Returns(candidate);

            var deliveries = new List<ILearningDelivery>();
            deliveries.Add(mockDelivery.Object);

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(learnRefNumber);
            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var commonChecks = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonChecks
                .Setup(x => x.HasQualifyingStart(mockDelivery.Object, LearnAimRef_87Rule.FirstViableDate, null))
                .Returns(true);

            var sut = new LearnAimRef_87Rule(handler.Object, commonChecks.Object);

            sut.Validate(mockLearner.Object);

            handler.VerifyAll();
            commonChecks.VerifyAll();
        }

        public LearnAimRef_87Rule NewRule()
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var commonChecks = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);

            return new LearnAimRef_87Rule(handler.Object, commonChecks.Object);
        }
    }
}
