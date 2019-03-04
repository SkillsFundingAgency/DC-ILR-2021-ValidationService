using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.EmploymentStatus.ESMType;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Utility;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.EmploymentStatus.ESMType
{
    public class ESMType_14RuleTests
    {
        /// <summary>
        /// New rule with null message handler throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullMessageHandlerThrows()
        {
            // arrange
            var ddRule26 = new Mock<IDerivedData_26Rule>(MockBehavior.Strict);
            var fcsData = new Mock<IFCSDataService>(MockBehavior.Strict);
            var common = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new ESMType_14Rule(null, ddRule26.Object, fcsData.Object, common.Object));
        }

        /// <summary>
        /// New rule with null derived data throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullDerivedDataThrows()
        {
            // arrange
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var fcsData = new Mock<IFCSDataService>(MockBehavior.Strict);
            var common = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new ESMType_14Rule(handler.Object, null, fcsData.Object, common.Object));
        }

        /// <summary>
        /// New rule with null FCS data throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullFCSDataThrows()
        {
            // arrange
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var ddRule26 = new Mock<IDerivedData_26Rule>(MockBehavior.Strict);
            var common = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new ESMType_14Rule(handler.Object, ddRule26.Object, null, common.Object));
        }

        /// <summary>
        /// New rule with null common operations throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullCommonOperationsThrows()
        {
            // arrange
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var ddRule26 = new Mock<IDerivedData_26Rule>(MockBehavior.Strict);
            var fcsData = new Mock<IFCSDataService>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new ESMType_14Rule(handler.Object, ddRule26.Object, fcsData.Object, null));
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
            Assert.Equal("ESMType_14", result);
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
            Assert.Equal(RuleNameConstants.ESMType_14, result);
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

            // act / assert
            Assert.Throws<ArgumentNullException>(() => sut.Validate(null));
        }

        /// <summary>
        /// Get eligibility rule for, meets expectation.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        [Theory]
        [InlineData("testconRef1")]
        [InlineData("testconRef2")]
        [InlineData("testconRef3")]
        public void GetEligibilityRuleForMeetsExpectation(string candidate)
        {
            // arrange
            var mockItem = new Mock<ILearningDelivery>();
            mockItem
                .SetupGet(x => x.ConRefNumber)
                .Returns(candidate);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var ddRule26 = new Mock<IDerivedData_26Rule>(MockBehavior.Strict);
            var fcsData = new Mock<IFCSDataService>(MockBehavior.Strict);
            fcsData
                .Setup(x => x.GetEligibilityRuleFor(candidate))
                .Returns(new Mock<IEsfEligibilityRule>().Object);

            var common = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);

            var sut = new ESMType_14Rule(handler.Object, ddRule26.Object, fcsData.Object, common.Object);

            // act
            var result = sut.GetEligibilityRuleFor(mockItem.Object);

            // assert
            Assert.NotNull(result);
            Assert.IsAssignableFrom<IEsfEligibilityRule>(result);

            handler.VerifyAll();
            ddRule26.VerifyAll();
            fcsData.VerifyAll();
            common.VerifyAll();
        }

        /// <summary>
        /// Get derived rule benefits indicator for, meets expectation.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData("testConRef1", true)]
        [InlineData("testConRef2", true)]
        [InlineData("testConRef3", true)]
        [InlineData("testConRef1", false)]
        [InlineData("testConRef2", false)]
        [InlineData("testConRef3", false)]
        public void GetDerivedRuleBenefitsIndicatorForMeetsExpectation(string candidate, bool expectation)
        {
            // arrange
            var mockItem = new Mock<ILearningDelivery>();
            mockItem
                .SetupGet(x => x.ConRefNumber)
                .Returns(candidate);
            var mockLearner = new Mock<ILearner>();

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var ddRule26 = new Mock<IDerivedData_26Rule>(MockBehavior.Strict);
            ddRule26
                .Setup(x => x.LearnerOnBenefitsAtStartOfCompletedZESF0001AimForContract(mockLearner.Object, candidate))
                .Returns(expectation);

            var fcsData = new Mock<IFCSDataService>(MockBehavior.Strict);
            var common = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);

            var sut = new ESMType_14Rule(handler.Object, ddRule26.Object, fcsData.Object, common.Object);

            // act
            var result = sut.GetDerivedRuleBenefitsIndicatorFor(mockLearner.Object, mockItem.Object);

            // assert
            Assert.Equal(expectation, result);

            handler.VerifyAll();
            ddRule26.VerifyAll();
            fcsData.VerifyAll();
            common.VerifyAll();
        }

        /// <summary>
        /// Has matching benefits indicator meets expectation
        /// </summary>
        /// <param name="eligibilty">if set to <c>true</c> [eligibilty].</param>
        /// <param name="derivedResult">if set to <c>true</c> [derived result].</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(true, true, true)]
        [InlineData(false, true, false)]
        [InlineData(true, false, false)]
        [InlineData(false, false, true)]
        [InlineData(null, false, true)]
        [InlineData(null, true, true)]
        public void HasMatchingBenefitsIndicatorMeetsExpectation(bool? eligibilty, bool derivedResult, bool expectation)
        {
            // arrange
            var mockItem = new Mock<IEsfEligibilityRule>();
            mockItem
                .SetupGet(x => x.Benefits)
                .Returns(eligibilty);

            var sut = NewRule();

            // act
            var result = sut.HasMatchingBenefitsIndicator(mockItem.Object, derivedResult);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Has matching benefits indicator with null eligibility meets expectation
        /// </summary>
        /// <param name="derivedResult">if set to <c>true</c> [derived result].</param>
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void HasMatchingBenefitsIndicatorWithNullEligibilityMeetsExpectation(bool derivedResult)
        {
            // arrange
            var sut = NewRule();

            // act
            var result = sut.HasMatchingBenefitsIndicator(null, derivedResult);

            // assert
            Assert.True(result);
        }

        /// <summary>
        /// Invalid item raises validation message.
        /// </summary>
        /// <param name="contractRef">The contract reference.</param>
        /// <param name="eligibilty">if set to <c>true</c> [eligibilty].</param>
        /// <param name="derivedResult">if set to <c>true</c> [derived result].</param>
        [Theory]
        [InlineData("testConRef1", false, true)]
        [InlineData("testConRef2", true, false)]
        public void InvalidItemRaisesValidationMessage(string contractRef, bool eligibilty, bool derivedResult)
        {
            // arrange
            const string LearnRefNumber = "123456789X";

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.ConRefNumber)
                .Returns(contractRef);

            var deliveries = Collection.Empty<ILearningDelivery>();
            deliveries.Add(mockDelivery.Object);

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries.AsSafeReadOnlyList());

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            handler
                .Setup(x => x.Handle(RuleNameConstants.ESMType_14, LearnRefNumber, null, Moq.It.IsAny<IEnumerable<IErrorMessageParameter>>()));
            handler
                .Setup(x => x.BuildErrorMessageParameter("ConRefNumber", contractRef))
                .Returns(new Mock<IErrorMessageParameter>().Object);

            var ddRule26 = new Mock<IDerivedData_26Rule>(MockBehavior.Strict);
            ddRule26
                .Setup(x => x.LearnerOnBenefitsAtStartOfCompletedZESF0001AimForContract(mockLearner.Object, contractRef))
                .Returns(derivedResult);

            var mockItem = new Mock<IEsfEligibilityRule>();
            mockItem
                .SetupGet(x => x.Benefits)
                .Returns(eligibilty);

            var fcsData = new Mock<IFCSDataService>(MockBehavior.Strict);
            fcsData
                .Setup(x => x.GetEligibilityRuleFor(contractRef))
                .Returns(mockItem.Object);

            var common = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            common
                .Setup(x => x.HasQualifyingFunding(mockDelivery.Object, 70)) // TypeOfFunding.EuropeanSocialFund
                .Returns(true);

            var sut = new ESMType_14Rule(handler.Object, ddRule26.Object, fcsData.Object, common.Object);

            // act
            sut.Validate(mockLearner.Object);

            // assert
            handler.VerifyAll();
            ddRule26.VerifyAll();
            fcsData.VerifyAll();
            common.VerifyAll();
        }

        /// <summary>
        /// Valid item does not raise validation message.
        /// </summary>
        /// <param name="contractRef">The contract reference.</param>
        /// <param name="eligibilty">if set to <c>true</c> [eligibilty].</param>
        /// <param name="derivedResult">if set to <c>true</c> [derived result].</param>
        [Theory]
        [InlineData("testConRef1", true, true)]
        [InlineData("testConRef2", false, false)]
        [InlineData("testConRef2", null, false)]
        public void ValidItemDoesNotRaiseValidationMessage(string contractRef, bool? eligibilty, bool derivedResult)
        {
            // arrange
            const string LearnRefNumber = "123456789X";

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.ConRefNumber)
                .Returns(contractRef);

            var deliveries = Collection.Empty<ILearningDelivery>();
            deliveries.Add(mockDelivery.Object);

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries.AsSafeReadOnlyList());

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            var ddRule26 = new Mock<IDerivedData_26Rule>(MockBehavior.Strict);
            ddRule26
                .Setup(x => x.LearnerOnBenefitsAtStartOfCompletedZESF0001AimForContract(mockLearner.Object, contractRef))
                .Returns(derivedResult);

            var mockItem = new Mock<IEsfEligibilityRule>();
            mockItem
                .SetupGet(x => x.Benefits)
                .Returns(eligibilty);

            var fcsData = new Mock<IFCSDataService>(MockBehavior.Strict);
            fcsData
                .Setup(x => x.GetEligibilityRuleFor(contractRef))
                .Returns(mockItem.Object);

            var common = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            common
                .Setup(x => x.HasQualifyingFunding(mockDelivery.Object, 70)) // TypeOfFunding.EuropeanSocialFund
                .Returns(true);

            var sut = new ESMType_14Rule(handler.Object, ddRule26.Object, fcsData.Object, common.Object);

            // act
            sut.Validate(mockLearner.Object);

            // assert
            handler.VerifyAll();
            ddRule26.VerifyAll();
            fcsData.VerifyAll();
            common.VerifyAll();
        }

        /// <summary>
        /// New rule.
        /// </summary>
        /// <returns>a constructed and mocked up validation rule</returns>
        public ESMType_14Rule NewRule()
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var ddRule26 = new Mock<IDerivedData_26Rule>(MockBehavior.Strict);
            var fcsData = new Mock<IFCSDataService>(MockBehavior.Strict);
            var common = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);

            return new ESMType_14Rule(handler.Object, ddRule26.Object, fcsData.Object, common.Object);
        }
    }
}
