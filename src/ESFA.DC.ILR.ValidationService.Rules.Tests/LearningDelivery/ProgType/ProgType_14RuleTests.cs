using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.ProgType;
using ESFA.DC.ILR.ValidationService.Utility;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.ProgType
{
    /// <summary>
    /// from version 1.1 validation spread sheet
    /// </summary>
    public class ProgType_14RuleTests
    {
        /// <summary>
        /// New rule with null message handler throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullMessageHandlerThrows()
        {
            Assert.Throws<ArgumentNullException>(() => new ProgType_14Rule(null));
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
            Assert.Equal("ProgType_14", result);
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
            Assert.Equal(ProgType_14Rule.Name, result);
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
        /// Condition met with null learning delivery returns true.
        /// </summary>
        [Fact]
        public void ConditionMetWithNullLearningDeliveryReturnsTrue()
        {
            // arrange
            var sut = NewRule();

            // act
            var result = sut.ConditionMet(null);

            // assert
            Assert.True(result);
        }

        /// <summary>
        /// Condition met for learning deliveries with training not in work placement meets expectation.
        /// </summary>
        /// <param name="aimReference">The aim reference.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(TypeOfAim.References.IndustryPlacement, false)]
        [InlineData("asdflkasroas i", true)]
        [InlineData("w;oraeijwq rf;oiew ", true)]
        [InlineData(null, true)]
        public void ConditionMetForLearningDeliveriesWithTrainingNotInWorkPlacementMeetsExpectation(string aimReference, bool expectation)
        {
            // arrange
            var sut = NewRule();
            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.LearnAimRef)
                .Returns(aimReference);

            // act
            var result = sut.ConditionMet(mockDelivery.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Invalid item raises validation message.
        /// </summary>
        /// <param name="aimReference">The aim reference.</param>
        /// <param name="typeOfProgramme">The type of programme.</param>
        [Theory]
        [InlineData(TypeOfAim.References.IndustryPlacement, TypeOfLearningProgramme.Traineeship)]
        public void InvalidItemRaisesValidationMessage(string aimReference, int typeOfProgramme)
        {
            // arrange
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

            // act
            sut.Validate(mockLearner.Object);

            // assert
            mockHandler.VerifyAll();
        }

        /// <summary>
        /// Valid item does not raise a validation message.
        /// </summary>
        /// <param name="aimReference">The aim reference.</param>
        /// <param name="typeOfProgramme">The type of programme.</param>
        [Theory]
        [InlineData("SSER SUR I", TypeOfLearningProgramme.AdvancedLevelApprenticeship)]
        [InlineData("VCMWAPOASFM", TypeOfLearningProgramme.ApprenticeshipStandard)]
        [InlineData("CMASLFDASEJEF", TypeOfLearningProgramme.HigherApprenticeshipLevel4)]
        [InlineData("CASLFAIWEJ", TypeOfLearningProgramme.HigherApprenticeshipLevel5)]
        [InlineData("2AAWSFPOASERGK", TypeOfLearningProgramme.HigherApprenticeshipLevel6)]
        [InlineData("SMAFAIJ", TypeOfLearningProgramme.HigherApprenticeshipLevel7Plus)]
        [InlineData("sdfaseira", TypeOfLearningProgramme.IntermediateLevelApprenticeship)]
        [InlineData("cansefaEEfasoeif", TypeOfLearningProgramme.Traineeship)]
        [InlineData(null, TypeOfLearningProgramme.AdvancedLevelApprenticeship)]
        [InlineData(null, TypeOfLearningProgramme.ApprenticeshipStandard)]
        [InlineData(null, TypeOfLearningProgramme.HigherApprenticeshipLevel4)]
        [InlineData(null, TypeOfLearningProgramme.HigherApprenticeshipLevel5)]
        [InlineData(null, TypeOfLearningProgramme.HigherApprenticeshipLevel6)]
        [InlineData(null, TypeOfLearningProgramme.HigherApprenticeshipLevel7Plus)]
        [InlineData(null, TypeOfLearningProgramme.IntermediateLevelApprenticeship)]
        [InlineData(null, TypeOfLearningProgramme.Traineeship)]
        public void ValidItemDoesNotRaiseAValidationMessage(string aimReference, int typeOfProgramme)
        {
            // arrange
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

            // act
            sut.Validate(mockLearner.Object);

            // assert
            mockHandler.VerifyAll();
        }

        /// <summary>
        /// New rule.
        /// </summary>
        /// <returns>a constructed and mocked up validation rule</returns>
        public ProgType_14Rule NewRule()
        {
            var mock = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            return new ProgType_14Rule(mock.Object);
        }
    }
}
