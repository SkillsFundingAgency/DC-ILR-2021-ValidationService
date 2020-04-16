using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.EmploymentStatus.ESMType;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.EmploymentStatus.ESMType
{
    public class ESMType_13RuleTests
    {
        [Fact]
        public void RuleName()
        {
            var sut = NewRule();

            var result = sut.RuleName;

            Assert.Equal("ESMType_13", result);
        }

        [Theory]
        [InlineData("testconRef1")]
        [InlineData("testconRef2")]
        [InlineData("testconRef3")]
        public void GetEligibilityRuleForMeetsExpectation(string candidate)
        {
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

            var result = sut.GetEligibilityRuleFor(mockItem.Object);

            Assert.NotNull(result);
            Assert.IsAssignableFrom<IEsfEligibilityRule>(result);

            handler.VerifyAll();
            ddRule25.VerifyAll();
            fcsData.VerifyAll();
            common.VerifyAll();
        }

        [Theory]
        [InlineData("testConRef1", null)]
        [InlineData("testConRef2", 1)]
        [InlineData("testConRef3", 2)]
        [InlineData("testConRef1", 3)]
        [InlineData("testConRef2", 4)]
        [InlineData("testConRef3", 5)]
        public void GetDerivedRuleBenefitsIndicatorForMeetsExpectation(string candidate, int? expectation)
        {
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

            var result = sut.GetDerivedRuleLOUIndicatorFor(mockLearner.Object, mockItem.Object);

            Assert.Equal(expectation, result);

            handler.VerifyAll();
            ddRule25.VerifyAll();
            fcsData.VerifyAll();
            common.VerifyAll();
        }

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
            var mockItem = new Mock<IEsfEligibilityRule>();
            mockItem
                .SetupGet(x => x.MinLengthOfUnemployment)
                .Returns(minLOU);
            mockItem
                .SetupGet(x => x.MaxLengthOfUnemployment)
                .Returns(maxLOU);

            var sut = NewRule();

            var result = sut.HasDisqualifyingLOUIndicator(mockItem.Object, derivedResult);

            Assert.Equal(expectation, result);
        }

        [Fact]
        public void HasDisqualifyingLOUIndicatorWithNullDerivedResultReturnsFalse()
        {
            var mockItem = new Mock<IEsfEligibilityRule>();
            mockItem
                .SetupGet(x => x.MinLengthOfUnemployment)
                .Returns(3);
            mockItem
                .SetupGet(x => x.MaxLengthOfUnemployment)
                .Returns(4);

            var sut = NewRule();

            var result = sut.HasDisqualifyingLOUIndicator(mockItem.Object, null);

            Assert.False(result);
        }

        [Fact]
        public void HasDisqualifyingLOUIndicatorWithNullEligibilityReturnsFalse()
        {
            var sut = NewRule();

            var result = sut.HasDisqualifyingLOUIndicator(null, 5);

            Assert.False(result);
        }

        [Theory]
        [InlineData("testConRef1", 1, 3, 4)]
        [InlineData("testConRef2", 3, 5, 2)]
        public void InvalidItemRaisesValidationMessage(string contractRef, int? minLOU, int? maxLOU, int? derivedResult)
        {
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
                .Setup(x => x.HasQualifyingFunding(mockDelivery.Object, 70))
                .Returns(true);

            var sut = new ESMType_13Rule(handler.Object, ddRule25.Object, fcsData.Object, common.Object);

            sut.Validate(mockLearner.Object);

            handler.VerifyAll();
            ddRule25.VerifyAll();
            fcsData.VerifyAll();
            common.VerifyAll();
        }

        [Theory]
        [InlineData("testConRef1", null, 1, null)]
        [InlineData("testConRef2", 1, 1, null)]
        [InlineData("testConRef2", null, 1, 1)]
        [InlineData("testConRef2", 1, 1, 1)]
        public void ValidItemDoesNotRaiseValidationMessage(string contractRef, int? minLOU, int? maxLOU, int? derivedResult)
        {
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
                .Setup(x => x.HasQualifyingFunding(mockDelivery.Object, 70))
                .Returns(true);

            var sut = new ESMType_13Rule(handler.Object, ddRule25.Object, fcsData.Object, common.Object);

            sut.Validate(mockLearner.Object);

            handler.VerifyAll();
            ddRule25.VerifyAll();
            fcsData.VerifyAll();
            common.VerifyAll();
        }

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
