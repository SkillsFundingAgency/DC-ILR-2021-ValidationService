using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.CrossEntity;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.CrossEntity
{
    public class R122RuleTests
    {
        /// <summary>
        /// New rule with null message handler throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullMessageHandlerThrows()
        {
            // arrange
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new R122Rule(null, commonOps.Object));
        }

        /// <summary>
        /// New rule with null common operations throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullCommonOperationsThrows()
        {
            // arrange
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new R122Rule(handler.Object, null));
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
            Assert.Equal("R122", result);
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
            Assert.Equal(RuleNameConstants.R122, result);
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
        /// Gets the nullable date.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <returns>a nullable date</returns>
        public DateTime? GetNullableDate(string candidate) =>
            string.IsNullOrWhiteSpace(candidate) ? (DateTime?)null : DateTime.Parse(candidate);

        /// <summary>
        /// Has qualifying model meets expectation
        /// </summary>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void HasQualifyingModelMeetsExpectation(bool expectation)
        {
            // arrange
            var delivery = new Mock<ILearningDelivery>();

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonOps
                .Setup(x => x.HasQualifyingFunding(delivery.Object, 36)) // TypeOfFunding.ApprenticeshipsFrom1May2017
                .Returns(expectation);

            var sut = new R122Rule(handler.Object, commonOps.Object);

            // act
            var result = sut.HasQualifyingModel(delivery.Object);

            // assert
            Assert.Equal(expectation, result);

            handler.VerifyAll();
            commonOps.VerifyAll();
        }

        /// <summary>
        /// Is standard apprenticeship meets expectation
        /// </summary>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void IsStandardApprenticeshipMeetsExpectation(bool expectation)
        {
            // arrange
            var delivery = new Mock<ILearningDelivery>();

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonOps
                .Setup(x => x.IsStandardApprenticeship(delivery.Object))
                .Returns(expectation);

            var sut = new R122Rule(handler.Object, commonOps.Object);

            // act
            var result = sut.IsStandardApprenticeship(delivery.Object);

            // assert
            Assert.Equal(expectation, result);

            handler.VerifyAll();
            commonOps.VerifyAll();
        }

        /// <summary>
        /// Has apprenticeship contract meets expectation
        /// </summary>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void HasApprenticeshipContractMeetsExpectation(bool expectation)
        {
            // arrange
            var delivery = new Mock<ILearningDelivery>();

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);

            var sut = new R122Rule(handler.Object, commonOps.Object);

            // post construction set up
            commonOps
                .Setup(x => x.CheckDeliveryFAMs(delivery.Object, sut.IsApprenticeshipContract))
                .Returns(expectation);

            // act
            var result = sut.HasApprenticeshipContract(delivery.Object);

            // assert
            Assert.Equal(expectation, result);

            handler.VerifyAll();
            commonOps.VerifyAll();
        }

        [Theory]
        [InlineData("SOF", false)]
        [InlineData("ACT", true)]
        [InlineData("WPP", false)]
        [InlineData("EEF", false)]
        [InlineData("POD", false)]
        public void IsApprenticeshipContractMeetsExpectation(string candidate, bool expectation)
        {
            // arrange
            var monitor = new Mock<ILearningDeliveryFAM>();
            monitor
                .SetupGet(x => x.LearnDelFAMType)
                .Returns(candidate);

            var sut = NewRule();

            // act
            var result = sut.IsApprenticeshipContract(monitor.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        [Theory]
        [InlineData(null, false)]
        [InlineData("1996-06-19", true)]
        [InlineData("2018-02-14", true)]
        [InlineData("2021-11-24", true)]
        public void HasAchievementDateMeetsExpectation(string candidate, bool expectation)
        {
            // arrange
            var delivery = new Mock<ILearningDelivery>();
            delivery
                .SetupGet(x => x.AchDateNullable)
                .Returns(GetNullableDate(candidate));

            var sut = NewRule();

            // act
            var result = sut.HasAchievementDate(delivery.Object);

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

            var monitor = new Mock<ILearningDeliveryFAM>();
            monitor
                .SetupGet(x => x.LearnDelFAMType)
                .Returns("ACT");
            monitor
                .SetupGet(x => x.LearnDelFAMDateFromNullable)
                .Returns(DateTime.Today);
            monitor
                .SetupGet(x => x.LearnDelFAMDateToNullable)
                .Returns(DateTime.Today);

            var delivery = new Mock<ILearningDelivery>(MockBehavior.Strict);
            delivery
                .SetupGet(x => x.AimSeqNumber)
                .Returns(1);
            delivery
                .Setup(x => x.AchDateNullable)
                .Returns((DateTime?)null);
            delivery
                .SetupGet(x => x.LearningDeliveryFAMs)
                .Returns(new ILearningDeliveryFAM[] { monitor.Object });

            var temp = new Mock<ILearningDelivery>(MockBehavior.Strict);
            temp
                .SetupGet(x => x.AimSeqNumber)
                .Returns(2);
            temp
                .Setup(x => x.AchDateNullable)
                .Returns(DateTime.Today);
            temp
                .SetupGet(x => x.LearningDeliveryFAMs)
                .Returns(new ILearningDeliveryFAM[] { });

            var deliveries = new ILearningDelivery[] { delivery.Object, temp.Object };

            var learner = new Mock<ILearner>();
            learner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            learner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            handler
                .Setup(x => x.Handle(RuleNameConstants.R122, LearnRefNumber, 1, It.IsAny<IEnumerable<IErrorMessageParameter>>()));
            handler
                .Setup(x => x.BuildErrorMessageParameter("LearnDelFAMType", "ACT"))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter("LearnDelFAMDateTo", AbstractRule.AsRequiredCultureDate(DateTime.Today)))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter("AchDate", null))
                .Returns(new Mock<IErrorMessageParameter>().Object);

            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonOps
                .Setup(x => x.HasQualifyingFunding(delivery.Object, 36))
                .Returns(true);
            commonOps
                .Setup(x => x.HasQualifyingFunding(temp.Object, 36))
                .Returns(true);
            commonOps
                .Setup(x => x.IsStandardApprenticeship(delivery.Object))
                .Returns(true);
            commonOps
                .Setup(x => x.IsStandardApprenticeship(temp.Object))
                .Returns(true);

            var sut = new R122Rule(handler.Object, commonOps.Object);

            // post construction set up
            commonOps
                .Setup(x => x.CheckDeliveryFAMs(delivery.Object, sut.IsApprenticeshipContract))
                .Returns(true);
            commonOps
                .Setup(x => x.CheckDeliveryFAMs(temp.Object, sut.IsApprenticeshipContract))
                .Returns(true);

            // act
            sut.Validate(learner.Object);

            // assert
            handler.VerifyAll();
            commonOps.VerifyAll();
        }

        /// <summary>
        /// Valid item does not raise validation message.
        /// </summary>
        [Fact]
        public void ValidItemDoesNotRaiseValidationMessage()
        {
            // arrange
            const string LearnRefNumber = "123456789X";

            var monitor = new Mock<ILearningDeliveryFAM>();
            monitor
                .SetupGet(x => x.LearnDelFAMType)
                .Returns("ACT");
            monitor
                .SetupGet(x => x.LearnDelFAMDateFromNullable)
                .Returns(DateTime.Today);
            monitor
                .SetupGet(x => x.LearnDelFAMDateToNullable)
                .Returns(DateTime.Today);

            var delivery = new Mock<ILearningDelivery>(MockBehavior.Strict);
            delivery
                .SetupGet(x => x.AimSeqNumber)
                .Returns(1);
            delivery
                .Setup(x => x.AchDateNullable)
                .Returns(DateTime.Today);
            delivery
                .SetupGet(x => x.LearningDeliveryFAMs)
                .Returns(new ILearningDeliveryFAM[] { monitor.Object });

            var temp = new Mock<ILearningDelivery>(MockBehavior.Strict);
            temp
                .SetupGet(x => x.AimSeqNumber)
                .Returns(2);
            temp
                .Setup(x => x.AchDateNullable)
                .Returns(DateTime.Today);
            temp
                .SetupGet(x => x.LearningDeliveryFAMs)
                .Returns(new ILearningDeliveryFAM[] { });

            var deliveries = new ILearningDelivery[] { delivery.Object, temp.Object };

            var learner = new Mock<ILearner>();
            learner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            learner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonOps
                .Setup(x => x.HasQualifyingFunding(delivery.Object, 36))
                .Returns(true);
            commonOps
                .Setup(x => x.HasQualifyingFunding(temp.Object, 36))
                .Returns(true);
            commonOps
                .Setup(x => x.IsStandardApprenticeship(delivery.Object))
                .Returns(true);
            commonOps
                .Setup(x => x.IsStandardApprenticeship(temp.Object))
                .Returns(true);

            var sut = new R122Rule(handler.Object, commonOps.Object);

            // post construction set up
            commonOps
                .Setup(x => x.CheckDeliveryFAMs(delivery.Object, sut.IsApprenticeshipContract))
                .Returns(true);
            commonOps
                .Setup(x => x.CheckDeliveryFAMs(temp.Object, sut.IsApprenticeshipContract))
                .Returns(true);

            // act
            sut.Validate(learner.Object);

            // assert
            handler.VerifyAll();
            commonOps.VerifyAll();
        }

        /// <summary>
        /// New rule.
        /// </summary>
        /// <returns>a constructed and mocked up validation rule</returns>
        public R122Rule NewRule()
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);

            return new R122Rule(handler.Object, commonOps.Object);
        }
    }
}
