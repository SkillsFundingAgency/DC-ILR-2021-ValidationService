using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.StdCode;
using Moq;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.StdCode
{
    public class StdCode_01RuleTests
    {
        [Fact]
        public void RuleName()
        {
            var sut = NewRule();

            var result = sut.RuleName;

            Assert.Equal("StdCode_01", result);
        }

        [Theory]
        [InlineData(2, true)]
        [InlineData(null, false)]
        public void HasStandardCodeMeetsExpectation(int? candidate, bool expectation)
        {
            var sut = NewRule();

            var mockItem = new Mock<ILearningDelivery>();
            mockItem
                .SetupGet(x => x.StdCodeNullable)
                .Returns(candidate);

            var result = sut.HasStandardCode(mockItem.Object);

            Assert.Equal(expectation, result);
        }

        [Theory]
        [InlineData(null, false)]
        [InlineData(TypeOfLearningProgramme.AdvancedLevelApprenticeship, false)]
        [InlineData(TypeOfLearningProgramme.ApprenticeshipStandard, true)]
        [InlineData(TypeOfLearningProgramme.HigherApprenticeshipLevel4, false)]
        [InlineData(TypeOfLearningProgramme.HigherApprenticeshipLevel5, false)]
        [InlineData(TypeOfLearningProgramme.HigherApprenticeshipLevel6, false)]
        [InlineData(TypeOfLearningProgramme.HigherApprenticeshipLevel7Plus, false)]
        [InlineData(TypeOfLearningProgramme.IntermediateLevelApprenticeship, false)]
        [InlineData(TypeOfLearningProgramme.Traineeship, false)]
        public void IsQualifyingLearningProgrammeMeetsExpectation(int? candidate, bool expectation)
        {
            var sut = NewRule();

            var mockItem = new Mock<ILearningDelivery>();
            mockItem
                .SetupGet(x => x.ProgTypeNullable)
                .Returns(candidate);

            var result = sut.IsQualifyingLearningProgramme(mockItem.Object);

            Assert.Equal(expectation, result);
        }

        [Fact]
        public void InvalidItemRaisesValidationMessage()
        {
            const string LearnRefNumber = "123456789X";

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(x => x.ProgTypeNullable)
                .Returns(TypeOfLearningProgramme.ApprenticeshipStandard);

            var deliveries = new List<ILearningDelivery>
            {
                mockDelivery.Object
            };

            var mock = new Mock<ILearner>();
            mock
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            mock
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            handler
                .Setup(x => x.Handle(
                    Moq.It.Is<string>(y => y == "StdCode_01"),
                    Moq.It.Is<string>(y => y == LearnRefNumber),
                    0,
                    Moq.It.IsAny<IEnumerable<IErrorMessageParameter>>()));
            handler
                .Setup(x => x.BuildErrorMessageParameter(
                    Moq.It.Is<string>(y => y == StdCode_01Rule.MessagePropertyName),
                    TypeOfLearningProgramme.ApprenticeshipStandard))
                .Returns(new Mock<IErrorMessageParameter>().Object);

            var sut = new StdCode_01Rule(handler.Object);

            sut.Validate(mock.Object);

            handler.VerifyAll();
        }

        [Fact]
        public void ValidItemDoesNotRaiseAValidationMessage()
        {
            const string LearnRefNumber = "123456789X";

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(x => x.ProgTypeNullable)
                .Returns(TypeOfLearningProgramme.ApprenticeshipStandard);
            mockDelivery
                .SetupGet(x => x.StdCodeNullable)
                .Returns(2);

            var deliveries = new List<ILearningDelivery>
            {
                mockDelivery.Object
            };

            var mock = new Mock<ILearner>();
            mock
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            mock
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            var sut = new StdCode_01Rule(handler.Object);

            sut.Validate(mock.Object);

            handler.VerifyAll();
        }

        public StdCode_01Rule NewRule()
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            return new StdCode_01Rule(handler.Object);
        }
    }
}
