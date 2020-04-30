using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.EmploymentStatus.ESMType;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.EmploymentStatus.ESMType
{
    public class ESMType_14RuleTests
    {
        [Fact]
        public void RuleName()
        {
            var sut = NewRule();

            var result = sut.RuleName;

            Assert.Equal("ESMType_14", result);
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
            var ddRule26 = new Mock<IDerivedData_26Rule>(MockBehavior.Strict);
            var fcsData = new Mock<IFCSDataService>(MockBehavior.Strict);
            fcsData
                .Setup(x => x.GetEligibilityRuleFor(candidate))
                .Returns(new Mock<IEsfEligibilityRule>().Object);

            var sut = new ESMType_14Rule(handler.Object, ddRule26.Object, fcsData.Object);

            var result = sut.GetEligibilityRuleFor(mockItem.Object);

            Assert.NotNull(result);
            Assert.IsAssignableFrom<IEsfEligibilityRule>(result);

            handler.VerifyAll();
            ddRule26.VerifyAll();
            fcsData.VerifyAll();
        }

        [Theory]
        [InlineData("testConRef1", true)]
        [InlineData("testConRef2", true)]
        [InlineData("testConRef3", true)]
        [InlineData("testConRef1", false)]
        [InlineData("testConRef2", false)]
        [InlineData("testConRef3", false)]
        public void GetDerivedRuleBenefitsIndicatorForMeetsExpectation(string candidate, bool expectation)
        {
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

            var sut = new ESMType_14Rule(handler.Object, ddRule26.Object, fcsData.Object);

            var result = sut.GetDerivedRuleBenefitsIndicatorFor(mockLearner.Object, mockItem.Object);

            Assert.Equal(expectation, result);

            handler.VerifyAll();
            ddRule26.VerifyAll();
            fcsData.VerifyAll();
        }

        [Theory]
        [InlineData(true, true, true)]
        [InlineData(false, true, false)]
        [InlineData(true, false, false)]
        [InlineData(false, false, true)]
        [InlineData(null, false, true)]
        [InlineData(null, true, true)]
        public void HasMatchingBenefitsIndicatorMeetsExpectation(bool? eligibilty, bool derivedResult, bool expectation)
        {
            var mockItem = new Mock<IEsfEligibilityRule>();
            mockItem
                .SetupGet(x => x.Benefits)
                .Returns(eligibilty);

            var sut = NewRule();

            var result = sut.HasMatchingBenefitsIndicator(mockItem.Object, derivedResult);

            Assert.Equal(expectation, result);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void HasMatchingBenefitsIndicatorWithNullEligibilityMeetsExpectation(bool derivedResult)
        {
            var sut = NewRule();

            var result = sut.HasMatchingBenefitsIndicator(null, derivedResult);

            Assert.True(result);
        }

        [Theory]
        [InlineData("testConRef1", false, true)]
        [InlineData("testConRef2", true, false)]
        public void InvalidItemRaisesValidationMessage(string contractRef, bool eligibilty, bool derivedResult)
        {
            const string LearnRefNumber = "123456789X";

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.ConRefNumber)
                .Returns(contractRef);
            mockDelivery
                .SetupGet(y => y.FundModel)
                .Returns(70);

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

            var sut = new ESMType_14Rule(handler.Object, ddRule26.Object, fcsData.Object);

            sut.Validate(mockLearner.Object);

            handler.VerifyAll();
            ddRule26.VerifyAll();
            fcsData.VerifyAll();
        }

        [Theory]
        [InlineData("testConRef1", true, true)]
        [InlineData("testConRef2", false, false)]
        [InlineData("testConRef2", null, false)]
        public void ValidItemDoesNotRaiseValidationMessage(string contractRef, bool? eligibilty, bool derivedResult)
        {
            const string LearnRefNumber = "123456789X";

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.ConRefNumber)
                .Returns(contractRef);
            mockDelivery
                .SetupGet(y => y.FundModel)
                .Returns(70);

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

            var sut = new ESMType_14Rule(handler.Object, ddRule26.Object, fcsData.Object);

            sut.Validate(mockLearner.Object);

            handler.VerifyAll();
            ddRule26.VerifyAll();
            fcsData.VerifyAll();
        }

        public ESMType_14Rule NewRule()
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var ddRule26 = new Mock<IDerivedData_26Rule>(MockBehavior.Strict);
            var fcsData = new Mock<IFCSDataService>(MockBehavior.Strict);

            return new ESMType_14Rule(handler.Object, ddRule26.Object, fcsData.Object);
        }
    }
}
