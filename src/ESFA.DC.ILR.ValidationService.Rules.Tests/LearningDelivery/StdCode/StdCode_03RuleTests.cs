using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.StdCode;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.StdCode
{
    public class StdCode_03RuleTests
    {
        [Fact]
        public void RuleName()
        {
            var sut = NewRule();

            var result = sut.RuleName;

            Assert.Equal("StdCode_03", result);
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
        [InlineData(ProgTypes.AdvancedLevelApprenticeship, false)]
        [InlineData(ProgTypes.ApprenticeshipStandard, true)]
        [InlineData(ProgTypes.HigherApprenticeshipLevel4, false)]
        [InlineData(ProgTypes.HigherApprenticeshipLevel5, false)]
        [InlineData(ProgTypes.HigherApprenticeshipLevel6, false)]
        [InlineData(ProgTypes.HigherApprenticeshipLevel7Plus, false)]
        [InlineData(ProgTypes.IntermediateLevelApprenticeship, false)]
        [InlineData(ProgTypes.Traineeship, false)]
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
                .Returns(ProgTypes.AdvancedLevelApprenticeship);
            mockDelivery
                .SetupGet(x => x.StdCodeNullable)
                .Returns(3);

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
                    Moq.It.Is<string>(y => y == "StdCode_03"),
                    Moq.It.Is<string>(y => y == LearnRefNumber),
                    0,
                    Moq.It.IsAny<IEnumerable<IErrorMessageParameter>>()));
            handler
                .Setup(x => x.BuildErrorMessageParameter(
                    Moq.It.Is<string>(y => y == StdCode_03Rule.MessagePropertyName),
                    3))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter(
                    Moq.It.Is<string>(y => y == PropertyNameConstants.ProgType),
                    ProgTypes.AdvancedLevelApprenticeship))
                .Returns(new Mock<IErrorMessageParameter>().Object);

            var sut = new StdCode_03Rule(handler.Object);

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
                .Returns(ProgTypes.AdvancedLevelApprenticeship);

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

            var sut = new StdCode_03Rule(handler.Object);

            sut.Validate(mock.Object);

            handler.VerifyAll();
        }

        public StdCode_03Rule NewRule()
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            return new StdCode_03Rule(handler.Object);
        }
    }
}
