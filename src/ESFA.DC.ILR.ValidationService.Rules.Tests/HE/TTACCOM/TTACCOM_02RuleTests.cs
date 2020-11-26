using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.HE.TTACCOM;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.HE.TTACCOM
{
    public class TTACCOM_02RuleTests
    {
        [Fact]
        public void RuleName()
        {
            var sut = NewRule();

            var result = sut.RuleName;

            Assert.Equal("TTACCOM_02", result);
        }

        [Fact]
        public void ConditionMetWithNullTTAccomReturnsTrue()
        {
            var mockHandler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var mockService = new Mock<IProvideLookupDetails>(MockBehavior.Strict);
            var mockDerived = new Mock<IDerivedData_06Rule>(MockBehavior.Strict);
            var sut = new TTACCOM_02Rule(mockHandler.Object, mockService.Object, mockDerived.Object);

            var result = sut.ConditionMet(null, DateTime.MaxValue);

            Assert.True(result);
            mockHandler.VerifyAll();
            mockService.VerifyAll();
            mockDerived.VerifyAll();
        }

        [Theory]
        [InlineData(1, true, "2013-06-14", true)]
        [InlineData(2, true, "2015-09-03", true)]
        [InlineData(3, true, "2012-06-18", true)]
        [InlineData(1, true, "2013-06-14", false)]
        [InlineData(2, true, "2015-09-03", false)]
        [InlineData(3, true, "2012-06-18", false)]
        [InlineData(1, false, "2013-06-14", true)]
        [InlineData(2, false, "2015-09-03", true)]
        [InlineData(3, false, "2012-06-18", true)]
        public void ConditionMetWithPossibleCandidateExpectation(int candidate, bool present, string testCaseDate, bool expectation)
        {
            var testDate = DateTime.Parse(testCaseDate);

            var mockHandler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var mockService = new Mock<IProvideLookupDetails>(MockBehavior.Strict);
            mockService
                .Setup(x => x.Contains(TypeOfLimitedLifeLookup.TTAccom, candidate))
                .Returns(present);
            if (present)
            {
                mockService
                    .Setup(x => x.IsCurrent(TypeOfLimitedLifeLookup.TTAccom, candidate, testDate))
                    .Returns(expectation);
            }

            var mockDerived = new Mock<IDerivedData_06Rule>(MockBehavior.Strict);

            var sut = new TTACCOM_02Rule(mockHandler.Object, mockService.Object, mockDerived.Object);

            var result = sut.ConditionMet(candidate, testDate);

            Assert.Equal(expectation, result);
            mockHandler.VerifyAll();
            mockService.VerifyAll();
        }

        [Theory]
        [InlineData(1, "2013-06-14")]
        [InlineData(2, "2015-09-03")]
        [InlineData(3, "2012-06-18")]
        [InlineData(4, "2008-12-01")]
        public void InvalidItemRaisesValidationMessage(int candidate, string testCaseDate)
        {
            const string LearnRefNumber = "123456789X";
            var testDate = DateTime.Parse(testCaseDate);

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);

            var mockHE = new Mock<ILearnerHE>();
            mockHE
                .SetupGet(x => x.TTACCOMNullable)
                .Returns(candidate);
            mockLearner
                .SetupGet(x => x.LearnerHEEntity)
                .Returns(mockHE.Object);

            var mockDelivery = new Mock<ILearningDelivery>();

            var deliveries = new List<ILearningDelivery>();
            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries);

            var mockHandler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            mockHandler.Setup(x => x.Handle(
                Moq.It.Is<string>(y => y == TTACCOM_02Rule.Name),
                Moq.It.Is<string>(y => y == LearnRefNumber),
                null,
                Moq.It.IsAny<IEnumerable<IErrorMessageParameter>>()));
            mockHandler
                .Setup(x => x.BuildErrorMessageParameter(
                    Moq.It.Is<string>(y => y == TTACCOM_02Rule.MessagePropertyName),
                    Moq.It.Is<int>(y => y == candidate)))
                .Returns(new Mock<IErrorMessageParameter>().Object);

            var mockService = new Mock<IProvideLookupDetails>(MockBehavior.Strict);
            mockService
                .Setup(x => x.Contains(TypeOfLimitedLifeLookup.TTAccom, candidate))
                .Returns(true);
            mockService
                .Setup(x => x.IsCurrent(TypeOfLimitedLifeLookup.TTAccom, candidate, testDate))
                .Returns(false);

            var mockDerived = new Mock<IDerivedData_06Rule>(MockBehavior.Strict);
            mockDerived
                .Setup(x => x.Derive(deliveries))
                .Returns(testDate);

            var sut = new TTACCOM_02Rule(mockHandler.Object, mockService.Object, mockDerived.Object);

            sut.Validate(mockLearner.Object);

            mockHandler.VerifyAll();
            mockService.VerifyAll();
            mockDerived.VerifyAll();
        }

        [Theory]
        [InlineData(1, "2013-06-14")]
        [InlineData(2, "2015-09-03")]
        [InlineData(3, "2012-06-18")]
        [InlineData(4, "2008-12-01")]
        public void ValidItemDoesNotRaiseAValidationMessage(int candidate, string testCaseDate)
        {
            const string LearnRefNumber = "123456789X";
            var testDate = DateTime.Parse(testCaseDate);

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);

            var mockHE = new Mock<ILearnerHE>();
            mockHE
                .SetupGet(x => x.TTACCOMNullable)
                .Returns(candidate);
            mockLearner
                .SetupGet(x => x.LearnerHEEntity)
                .Returns(mockHE.Object);

            var mockDelivery = new Mock<ILearningDelivery>();

            var deliveries = new List<ILearningDelivery>();
            deliveries.Add(mockDelivery.Object);
            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries);

            var mockHandler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var mockService = new Mock<IProvideLookupDetails>(MockBehavior.Strict);
            mockService
                .Setup(x => x.Contains(TypeOfLimitedLifeLookup.TTAccom, candidate))
                .Returns(true);
            mockService
                .Setup(x => x.IsCurrent(TypeOfLimitedLifeLookup.TTAccom, candidate, testDate))
                .Returns(true);

            var mockDerived = new Mock<IDerivedData_06Rule>(MockBehavior.Strict);
            mockDerived
                .Setup(x => x.Derive(deliveries))
                .Returns(testDate);

            var sut = new TTACCOM_02Rule(mockHandler.Object, mockService.Object, mockDerived.Object);

            sut.Validate(mockLearner.Object);

            mockHandler.VerifyAll();
            mockService.VerifyAll();
            mockDerived.VerifyAll();
        }

        public TTACCOM_02Rule NewRule()
        {
            var handler = new Mock<IValidationErrorHandler>();
            var service = new Mock<IProvideLookupDetails>();
            var rule = new Mock<IDerivedData_06Rule>(MockBehavior.Strict);

            return new TTACCOM_02Rule(handler.Object, service.Object, rule.Object);
        }
    }
}
