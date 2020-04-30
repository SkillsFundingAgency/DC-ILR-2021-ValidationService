using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.ProgType;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.ProgType
{
    public class ProgType_14RuleTests
    {
        [Fact]
        public void RuleName()
        {
            var sut = NewRule();

            var result = sut.RuleName;

            Assert.Equal("ProgType_14", result);
        }

        [Fact]
        public void ConditionMetWithNullLearningDeliveryReturnsTrue()
        {
            var sut = NewRule();

            var result = sut.ConditionMet(null);

            Assert.True(result);
        }

        [Theory]
        [InlineData(AimTypes.References.IndustryPlacement, false)]
        [InlineData("asdflkasroas i", true)]
        [InlineData("w;oraeijwq rf;oiew ", true)]
        [InlineData(null, true)]
        public void ConditionMetForLearningDeliveriesWithTrainingNotInWorkPlacementMeetsExpectation(string aimReference, bool expectation)
        {
            var sut = NewRule();
            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.LearnAimRef)
                .Returns(aimReference);

            var result = sut.ConditionMet(mockDelivery.Object);

            Assert.Equal(expectation, result);
        }

        [Theory]
        [InlineData(AimTypes.References.IndustryPlacement, ProgTypes.Traineeship)]
        public void InvalidItemRaisesValidationMessage(string aimReference, int typeOfProgramme)
        {
            const string LearnRefNumber = "123456789X";

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.ProgTypeNullable)
                .Returns(typeOfProgramme);
            mockDelivery
                .SetupGet(y => y.LearnAimRef)
                .Returns(aimReference);

            var deliveries = new List<ILearningDelivery>();
            deliveries.Add(mockDelivery.Object);

            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries);

            var mockHandler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            mockHandler.Setup(x => x.Handle(
                Moq.It.Is<string>(y => y == ProgType_14Rule.Name),
                Moq.It.Is<string>(y => y == LearnRefNumber),
                0,
                Moq.It.IsAny<IEnumerable<IErrorMessageParameter>>()));
            mockHandler
                .Setup(x => x.BuildErrorMessageParameter(
                    Moq.It.Is<string>(y => y == ProgType_14Rule.MessagePropertyName),
                    Moq.It.IsAny<ILearningDelivery>()))
                .Returns(new Mock<IErrorMessageParameter>().Object);

            var sut = new ProgType_14Rule(mockHandler.Object);

            sut.Validate(mockLearner.Object);

            mockHandler.VerifyAll();
        }

        [Theory]
        [InlineData("SSER SUR I", ProgTypes.AdvancedLevelApprenticeship)]
        [InlineData("VCMWAPOASFM", ProgTypes.ApprenticeshipStandard)]
        [InlineData("CMASLFDASEJEF", ProgTypes.HigherApprenticeshipLevel4)]
        [InlineData("CASLFAIWEJ", ProgTypes.HigherApprenticeshipLevel5)]
        [InlineData("2AAWSFPOASERGK", ProgTypes.HigherApprenticeshipLevel6)]
        [InlineData("SMAFAIJ", ProgTypes.HigherApprenticeshipLevel7Plus)]
        [InlineData("sdfaseira", ProgTypes.IntermediateLevelApprenticeship)]
        [InlineData("cansefaEEfasoeif", ProgTypes.Traineeship)]
        [InlineData(null, ProgTypes.AdvancedLevelApprenticeship)]
        [InlineData(null, ProgTypes.ApprenticeshipStandard)]
        [InlineData(null, ProgTypes.HigherApprenticeshipLevel4)]
        [InlineData(null, ProgTypes.HigherApprenticeshipLevel5)]
        [InlineData(null, ProgTypes.HigherApprenticeshipLevel6)]
        [InlineData(null, ProgTypes.HigherApprenticeshipLevel7Plus)]
        [InlineData(null, ProgTypes.IntermediateLevelApprenticeship)]
        [InlineData(null, ProgTypes.Traineeship)]
        public void ValidItemDoesNotRaiseAValidationMessage(string aimReference, int typeOfProgramme)
        {
            const string LearnRefNumber = "123456789X";

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.ProgTypeNullable)
                .Returns(typeOfProgramme);
            mockDelivery
                .SetupGet(y => y.LearnAimRef)
                .Returns(aimReference);

            var deliveries = new List<ILearningDelivery>();
            deliveries.Add(mockDelivery.Object);

            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries);

            var mockHandler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            var sut = new ProgType_14Rule(mockHandler.Object);

            sut.Validate(mockLearner.Object);

            mockHandler.VerifyAll();
        }

        public ProgType_14Rule NewRule()
        {
            var mock = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            return new ProgType_14Rule(mock.Object);
        }
    }
}
