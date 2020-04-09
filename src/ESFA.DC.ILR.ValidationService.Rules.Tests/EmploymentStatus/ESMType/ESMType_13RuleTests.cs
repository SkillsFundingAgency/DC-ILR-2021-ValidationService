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
    public class ESMType_13RuleTests
    {
        /// <summary>
        /// New rule with null message handler throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullMessageHandlerThrows()
        {
            // arrange
            var ddRule25 = new Mock<IDerivedData_25Rule>(MockBehavior.Strict);
            var fcsData = new Mock<IFCSDataService>(MockBehavior.Strict);
            var common = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new ESMType_13Rule(null, ddRule25.Object, fcsData.Object, common.Object));
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
            Assert.Throws<ArgumentNullException>(() => new ESMType_13Rule(handler.Object, null, fcsData.Object, common.Object));
        }

        /// <summary>
        /// New rule with null FCS data throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullFCSDataThrows()
        {
            // arrange
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var ddRule25 = new Mock<IDerivedData_25Rule>(MockBehavior.Strict);
            var common = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new ESMType_13Rule(handler.Object, ddRule25.Object, null, common.Object));
        }

        /// <summary>
        /// New rule with null common operations throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullCommonOperationsThrows()
        {
            // arrange
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var ddRule25 = new Mock<IDerivedData_25Rule>(MockBehavior.Strict);
            var fcsData = new Mock<IFCSDataService>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new ESMType_13Rule(handler.Object, ddRule25.Object, fcsData.Object, null));
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
            Assert.Equal("ESMType_13", result);
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
            Assert.Equal(RuleNameConstants.ESMType_13, result);
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
            var ddRule25 = new Mock<IDerivedData_25Rule>(MockBehavior.Strict);
            var fcsData = new Mock<IFCSDataService>(MockBehavior.Strict);
            fcsData
                .Setup(x => x.GetEligibilityRuleFor(candidate))
                .Returns(new Mock<IEsfEligibilityRule>().Object);

            var common = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);

            var sut = new ESMType_13Rule(handler.Object, ddRule25.Object, fcsData.Object, common.Object);

            // act
            var result = sut.GetEligibilityRuleFor(mockItem.Object);

            // assert
            Assert.NotNull(result);
            Assert.IsAssignableFrom<IEsfEligibilityRule>(result);

            handler.VerifyAll();
            ddRule25.VerifyAll();
            fcsData.VerifyAll();
            common.VerifyAll();
        }

        /// <summary>
        /// Get derived rule benefits indicator for, meets expectation.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData("testConRef1", null)]
        [InlineData("testConRef2", 1)]
        [InlineData("testConRef3", 2)]
        [InlineData("testConRef1", 3)]
        [InlineData("testConRef2", 4)]
        [InlineData("testConRef3", 5)]
        public void GetDerivedRuleBenefitsIndicatorForMeetsExpectation(string candidate, int? expectation)
        {
            // arrange
            var mockItem = new Mock<ILearningDelivery>();
            mockItem
                .SetupGet(x => x.ConRefNumber)
                .Returns(candidate);
            var mockLearner = new Mock<ILearner>();

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var ddRule25 = new Mock<IDerivedData_25Rule>(MockBehavior.Strict);
            ddRule25
                .Setup(x => x.GetLengthOfUnemployment(mockLearner.Object, candidate))
                .Returns(expectation);

            var fcsData = new Mock<IFCSDataService>(MockBehavior.Strict);
            var common = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);

            var sut = new ESMType_13Rule(handler.Object, ddRule25.Object, fcsData.Object, common.Object);

            // act
            var result = sut.GetDerivedRuleLOUIndicatorFor(mockLearner.Object, mockItem.Object);

            // assert
            Assert.Equal(expectation, result);

            handler.VerifyAll();
            ddRule25.VerifyAll();
            fcsData.VerifyAll();
            common.VerifyAll();
        }

        /// <summary>
        /// Has matching benefits indicator meets expectation
        /// </summary>
        /// <param name="minLOU">The minimum lou.</param>
        /// <param name="maxLOU">The maximum lou.</param>
        /// <param name="derivedResult">if set to <c>true</c> [derived result].</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(null, 1, null, false)]
        [InlineData(1, 1, null, false)]
        [InlineData(null, 1, 1, false)]
        [InlineData(1, null, 1, false)]
        [InlineData(1, 1, 1, false)]
        [InlineData(1, 3, 2, false)]
        [InlineData(3, 5, 4, false)]
        [InlineData(1, 3, 4, true)]
        [InlineData(3, 5, 2, true)]
        public void HasDisqualifyingLOUIndicatorMeetsExpectation(int? minLOU, int? maxLOU, int? derivedResult, bool expectation)
        {
            // arrange
            var mockItem = new Mock<IEsfEligibilityRule>();
            mockItem
                .SetupGet(x => x.MinLengthOfUnemployment)
                .Returns(minLOU);
            mockItem
                .SetupGet(x => x.MaxLengthOfUnemployment)
                .Returns(maxLOU);

            var sut = NewRule();

            // act
            var result = sut.HasDisqualifyingLOUIndicator(mockItem.Object, derivedResult);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Has disqualifying lou indicator with null derived result returns false
        /// </summary>
        [Fact]
        public void HasDisqualifyingLOUIndicatorWithNullDerivedResultReturnsFalse()
        {
            // arrange
            var mockItem = new Mock<IEsfEligibilityRule>();
            mockItem
                .SetupGet(x => x.MinLengthOfUnemployment)
                .Returns(3);
            mockItem
                .SetupGet(x => x.MaxLengthOfUnemployment)
                .Returns(4);

            var sut = NewRule();

            // act
            var result = sut.HasDisqualifyingLOUIndicator(mockItem.Object, null);

            // assert
            Assert.False(result);
        }

        /// <summary>
        /// Has disqualifying lou indicator with null eligibility returns false
        /// </summary>
        [Fact]
        public void HasDisqualifyingLOUIndicatorWithNullEligibilityReturnsFalse()
        {
            // arrange
            var sut = NewRule();

            // act
            var result = sut.HasDisqualifyingLOUIndicator(null, 5);

            // assert
            Assert.False(result);
        }

        /// <summary>
        /// Invalid item raises validation message.
        /// </summary>
        /// <param name="contractRef">The contract reference.</param>
        /// <param name="minLOU">The minimum lou.</param>
        /// <param name="maxLOU">The maximum lou.</param>
        /// <param name="derivedResult">if set to <c>true</c> [derived result].</param>
        [Theory]
        [InlineData("testConRef1", 1, 3, 4)]
        [InlineData("testConRef2", 3, 5, 2)]
        public void InvalidItemRaisesValidationMessage(string contractRef, int? minLOU, int? maxLOU, int? derivedResult)
        {
            // arrange
            const string LearnRefNumber = "123456789X";

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.ConRefNumber)
                .Returns(contractRef);

            var deliveries = new List<ILearningDelivery>();
            deliveries.Add(mockDelivery.Object);

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            handler
                .Setup(x => x.Handle(RuleNameConstants.ESMType_13, LearnRefNumber, null, Moq.It.IsAny<IEnumerable<IErrorMessageParameter>>()));
            handler
                .Setup(x => x.BuildErrorMessageParameter("ConRefNumber", contractRef))
                .Returns(new Mock<IErrorMessageParameter>().Object);

            var ddRule25 = new Mock<IDerivedData_25Rule>(MockBehavior.Strict);
            ddRule25
                .Setup(x => x.GetLengthOfUnemployment(mockLearner.Object, contractRef))
                .Returns(derivedResult);

            var mockItem = new Mock<IEsfEligibilityRule>();
            mockItem
                .SetupGet(x => x.MinLengthOfUnemployment)
                .Returns(minLOU);
            mockItem
                .SetupGet(x => x.MaxLengthOfUnemployment)
                .Returns(maxLOU);

            var fcsData = new Mock<IFCSDataService>(MockBehavior.Strict);
            fcsData
                .Setup(x => x.GetEligibilityRuleFor(contractRef))
                .Returns(mockItem.Object);

            var common = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            common
                .Setup(x => x.HasQualifyingFunding(mockDelivery.Object, 70)) // TypeOfFunding.EuropeanSocialFund
                .Returns(true);

            var sut = new ESMType_13Rule(handler.Object, ddRule25.Object, fcsData.Object, common.Object);

            // act
            sut.Validate(mockLearner.Object);

            // assert
            handler.VerifyAll();
            ddRule25.VerifyAll();
            fcsData.VerifyAll();
            common.VerifyAll();
        }

        /// <summary>
        /// Valid item does not raise validation message.
        /// </summary>
        /// <param name="contractRef">The contract reference.</param>
        /// <param name="minLOU">The minimum lou.</param>
        /// <param name="maxLOU">The maximum lou.</param>
        /// <param name="derivedResult">if set to <c>true</c> [derived result].</param>
        [Theory]
        [InlineData("testConRef1", null, 1, null)]
        [InlineData("testConRef2", 1, 1, null)]
        [InlineData("testConRef2", null, 1, 1)]
        [InlineData("testConRef2", 1, 1, 1)]
        public void ValidItemDoesNotRaiseValidationMessage(string contractRef, int? minLOU, int? maxLOU, int? derivedResult)
        {
            // arrange
            const string LearnRefNumber = "123456789X";

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.ConRefNumber)
                .Returns(contractRef);

            var deliveries = new List<ILearningDelivery>();
            deliveries.Add(mockDelivery.Object);

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            var ddRule25 = new Mock<IDerivedData_25Rule>(MockBehavior.Strict);
            ddRule25
                .Setup(x => x.GetLengthOfUnemployment(mockLearner.Object, contractRef))
                .Returns(derivedResult);

            var mockItem = new Mock<IEsfEligibilityRule>();
            mockItem
                .SetupGet(x => x.MinLengthOfUnemployment)
                .Returns(minLOU);
            mockItem
                .SetupGet(x => x.MaxLengthOfUnemployment)
                .Returns(maxLOU);

            var fcsData = new Mock<IFCSDataService>(MockBehavior.Strict);
            fcsData
                .Setup(x => x.GetEligibilityRuleFor(contractRef))
                .Returns(mockItem.Object);

            var common = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            common
                .Setup(x => x.HasQualifyingFunding(mockDelivery.Object, 70)) // TypeOfFunding.EuropeanSocialFund
                .Returns(true);

            var sut = new ESMType_13Rule(handler.Object, ddRule25.Object, fcsData.Object, common.Object);

            // act
            sut.Validate(mockLearner.Object);

            // assert
            handler.VerifyAll();
            ddRule25.VerifyAll();
            fcsData.VerifyAll();
            common.VerifyAll();
        }

        /// <summary>
        /// New rule.
        /// </summary>
        /// <returns>a constructed and mocked up validation rule</returns>
        public ESMType_13Rule NewRule()
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var ddRule25 = new Mock<IDerivedData_25Rule>(MockBehavior.Strict);
            var fcsData = new Mock<IFCSDataService>(MockBehavior.Strict);
            var common = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);

            return new ESMType_13Rule(handler.Object, ddRule25.Object, fcsData.Object, common.Object);
        }
    }
}
