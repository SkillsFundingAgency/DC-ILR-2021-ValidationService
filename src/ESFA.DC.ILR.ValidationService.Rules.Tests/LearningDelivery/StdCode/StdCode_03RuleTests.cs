using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.StdCode;
using ESFA.DC.ILR.ValidationService.Utility;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.StdCode
{
    public class StdCode_03RuleTests
    {
        /// <summary>
        /// New rule with null message handler throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullMessageHandlerThrows()
        {
            // arrange / act / assert
            Assert.Throws<ArgumentNullException>(() => new StdCode_03Rule(null));
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
            Assert.Equal("StdCode_03", result);
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
            Assert.Equal(StdCode_03Rule.Name, result);
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

        [Theory]
        [InlineData(2, true)]
        [InlineData(null, false)]
        public void HasStandardCodeMeetsExpectation(int? candidate, bool expectation)
        {
            // arrange
            var sut = NewRule();

            var mockItem = new Mock<ILearningDelivery>();
            mockItem
                .SetupGet(x => x.StdCodeNullable)
                .Returns(candidate);

            // act
            var result = sut.HasStandardCode(mockItem.Object);

            // assert
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
            // arrange
            var sut = NewRule();

            var mockItem = new Mock<ILearningDelivery>();
            mockItem
                .SetupGet(x => x.ProgTypeNullable)
                .Returns(candidate);

            // act
            var result = sut.IsQualifyingLearningProgramme(mockItem.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Invalid item raises validation message.
        /// </summary>
        [Fact]
        public void InvalidItemRaisesValidationMessage()
        {
            // arrange
            const string LearnRefNumber = "123456789X";

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(x => x.ProgTypeNullable)
                .Returns(TypeOfLearningProgramme.AdvancedLevelApprenticeship);
            mockDelivery
                .SetupGet(x => x.StdCodeNullable)
                .Returns(3);

            var deliveries = new List<ILearningDelivery>();
            deliveries.Add(mockDelivery.Object);

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
                    TypeOfLearningProgramme.AdvancedLevelApprenticeship))
                .Returns(new Mock<IErrorMessageParameter>().Object);

            var sut = new StdCode_03Rule(handler.Object);

            // act
            sut.Validate(mock.Object);

            // assert
            handler.VerifyAll();
        }

        /// <summary>
        /// Valid item does not raise a validation message.
        /// </summary>
        [Fact]
        public void ValidItemDoesNotRaiseAValidationMessage()
        {
            // arrange
            const string LearnRefNumber = "123456789X";

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(x => x.ProgTypeNullable)
                .Returns(TypeOfLearningProgramme.AdvancedLevelApprenticeship);

            var deliveries = new List<ILearningDelivery>();
            deliveries.Add(mockDelivery.Object);

            var mock = new Mock<ILearner>();
            mock
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            mock
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            var sut = new StdCode_03Rule(handler.Object);

            // act
            sut.Validate(mock.Object);

            // assert
            handler.VerifyAll();
        }

        /// <summary>
        /// New rule.
        /// </summary>
        /// <returns>a constructed and mocked up validation rule</returns>
        public StdCode_03Rule NewRule()
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            return new StdCode_03Rule(handler.Object);
        }
    }
}
