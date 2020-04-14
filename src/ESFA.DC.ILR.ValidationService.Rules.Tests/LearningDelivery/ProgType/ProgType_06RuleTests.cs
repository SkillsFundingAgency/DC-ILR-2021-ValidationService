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
    public class ProgType_06RuleTests
    {
        /// <summary>
        /// New rule with null message handler throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullMessageHandlerThrows()
        {
            Assert.Throws<ArgumentNullException>(() => new ProgType_06Rule(null));
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
            Assert.Equal("ProgType_06", result);
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
            Assert.Equal(ProgType_06Rule.Name, result);
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
        /// Condition met with learning delivery containing null fund model returns false.
        /// </summary>
        [Fact]
        public void ConditionMetWithLearningDeliveryContainingNullFundModelReturnsFalse()
        {
            // arrange
            var sut = NewRule();
            var mockDelivery = new Mock<ILearningDelivery>();

            // act
            var result = sut.ConditionMet(mockDelivery.Object);

            // assert
            Assert.False(result);
        }

        /// <summary>
        /// Condition met with learning deliveries containing fund models meets expectation.
        /// </summary>
        /// <param name="fundModel">The fund model.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(TypeOfFunding.EuropeanSocialFund, false)]
        [InlineData(TypeOfFunding.CommunityLearning, false)]
        [InlineData(TypeOfFunding.Age16To19ExcludingApprenticeships, false)]
        [InlineData(TypeOfFunding.AdultSkills, false)]
        [InlineData(TypeOfFunding.Other16To19, false)]
        [InlineData(TypeOfFunding.NotFundedByESFA, true)]
        [InlineData(TypeOfFunding.OtherAdult, true)]
        [InlineData(TypeOfFunding.ApprenticeshipsFrom1May2017, true)]
        public void ConditionMetWithLearningDeliveriesContainingFundModelsMeetsExpectation(int fundModel, bool expectation)
        {
            // arrange
            var sut = NewRule();
            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.FundModel)
                .Returns(fundModel);

            // act
            var result = sut.ConditionMet(mockDelivery.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Invalid item raises validation message.
        /// </summary>
        /// <param name="fundModel">The fund model.</param>
        [Theory]
        [InlineData(TypeOfFunding.EuropeanSocialFund)]
        [InlineData(TypeOfFunding.CommunityLearning)]
        [InlineData(TypeOfFunding.Age16To19ExcludingApprenticeships)]
        [InlineData(TypeOfFunding.AdultSkills)]
        [InlineData(TypeOfFunding.Other16To19)]
        public void InvalidItemRaisesValidationMessage(int fundModel)
        {
            // arrange
            const string LearnRefNumber = "123456789X";

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.FundModel)
                .Returns(fundModel);
            mockDelivery
                .SetupGet(y => y.ProgTypeNullable)
                .Returns(TypeOfLearningProgramme.ApprenticeshipStandard);

            var deliveries = new List<ILearningDelivery>();
            deliveries.Add(mockDelivery.Object);

            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries);

            var mockHandler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            mockHandler.Setup(x => x.Handle(
                Moq.It.Is<string>(y => y == ProgType_06Rule.Name),
                Moq.It.Is<string>(y => y == LearnRefNumber),
                0,
                Moq.It.IsAny<IEnumerable<IErrorMessageParameter>>()));

            mockHandler
                .Setup(x => x.BuildErrorMessageParameter(
                    Moq.It.Is<string>(y => y == "FundModel"),
                    Moq.It.Is<int>(y => y == fundModel)))
                .Returns(new Mock<IErrorMessageParameter>().Object);

            var sut = new ProgType_06Rule(mockHandler.Object);

            // act
            sut.Validate(mockLearner.Object);

            // assert
            mockHandler.VerifyAll();
        }

        /// <summary>
        /// Valid item does not raise a validation message.
        /// </summary>
        /// <param name="fundModel">The fund model.</param>
        /// <param name="programmeType">Type of the programme.</param>
        [Theory]
        [InlineData(TypeOfFunding.NotFundedByESFA, 26)]
        [InlineData(TypeOfFunding.OtherAdult, 27)]
        [InlineData(TypeOfFunding.ApprenticeshipsFrom1May2017, 28)]
        [InlineData(TypeOfFunding.NotFundedByESFA, TypeOfLearningProgramme.ApprenticeshipStandard)]
        [InlineData(TypeOfFunding.OtherAdult, TypeOfLearningProgramme.ApprenticeshipStandard)]
        [InlineData(TypeOfFunding.ApprenticeshipsFrom1May2017, TypeOfLearningProgramme.ApprenticeshipStandard)]
        [InlineData(TypeOfFunding.EuropeanSocialFund, null)]
        [InlineData(TypeOfFunding.CommunityLearning, null)]
        [InlineData(TypeOfFunding.Age16To19ExcludingApprenticeships, null)]
        [InlineData(TypeOfFunding.AdultSkills, null)]
        [InlineData(TypeOfFunding.Other16To19, null)]
        public void ValidItemDoesNotRaiseAValidationMessage(int fundModel, int? programmeType)
        {
            // arrange
            const string LearnRefNumber = "123456789X";

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.FundModel)
                .Returns(fundModel);
            mockDelivery
                .SetupGet(y => y.ProgTypeNullable)
                .Returns(programmeType);

            var deliveries = new List<ILearningDelivery>();
            deliveries.Add(mockDelivery.Object);

            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries);

            var mockHandler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            var sut = new ProgType_06Rule(mockHandler.Object);

            // act
            sut.Validate(mockLearner.Object);

            // assert
            mockHandler.VerifyAll();
        }

        /// <summary>
        /// New rule.
        /// </summary>
        /// <returns>a constructed and mocked up validation rule</returns>
        public ProgType_06Rule NewRule()
        {
            var mock = new Mock<IValidationErrorHandler>();

            return new ProgType_06Rule(mock.Object);
        }
    }
}
