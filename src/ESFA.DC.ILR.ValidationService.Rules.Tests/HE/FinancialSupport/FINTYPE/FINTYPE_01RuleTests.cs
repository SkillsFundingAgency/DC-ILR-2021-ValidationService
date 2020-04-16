using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.HE.FinancialSupport.FINTYPE;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.HE.FinancialSupport.FINTYPE
{
    public class FINTYPE_01RuleTests
    {
        [Fact]
        public void RuleName()
        {
            var sut = NewRule();

            var result = sut.RuleName;

            Assert.Equal("FINTYPE_01", result);
        }

        [Fact]
        public void ConditionMetWithNullTTAccomReturnsTrue()
        {
            var sut = NewRule();

            var result = sut.ConditionMet(null);

            Assert.True(result);
        }

        [Fact]
        public void ConditionMetWithEmptyFinancialSupportReturnsTrue()
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var provider = new Mock<IProvideLookupDetails>(MockBehavior.Strict);

            var sut = new FINTYPE_01Rule(handler.Object, provider.Object);

            var result = sut.ConditionMet(new List<ILearnerHEFinancialSupport>());

            Assert.True(result);
            handler.VerifyAll();
            provider.VerifyAll();
        }

        [Theory]
        [InlineData(1, 1)]
        [InlineData(1, 2, 2)]
        [InlineData(1, 2, 3, 1)]
        [InlineData(1, 2, 3, 4, 3)]
        public void ConditionMetWithValidFinancialSupportCombinationsReturnsTrue(params int[] candidates)
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var provider = new Mock<IProvideLookupDetails>(MockBehavior.Strict);
            provider
                .Setup(x => x.Contains(TypeOfIntegerCodedLookup.FinType, Moq.It.IsAny<int>()))
                .Returns(true);

            var sut = new FINTYPE_01Rule(handler.Object, provider.Object);

            var result = sut.ConditionMet(GetFinancialSupport(candidates));

            Assert.True(result);
            handler.VerifyAll();
            provider.VerifyAll();
        }

        [Theory]
        [InlineData(1, 1)]
        [InlineData(1, 2, 2)]
        [InlineData(1, 2, 3, 1)]
        [InlineData(1, 2, 3, 4, 3)]
        public void ConditionMetWithInvalidFinancialSupportCombinationsReturnsFalse(params int[] candidates)
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var provider = new Mock<IProvideLookupDetails>(MockBehavior.Strict);
            provider
                .Setup(x => x.Contains(TypeOfIntegerCodedLookup.FinType, Moq.It.IsAny<int>()))
                .Returns(false);

            var sut = new FINTYPE_01Rule(handler.Object, provider.Object);

            var result = sut.ConditionMet(GetFinancialSupport(candidates));

            Assert.False(result);
            handler.VerifyAll();
            provider.VerifyAll();
        }

        [Theory]
        [InlineData(51)]
        [InlineData(16)]
        [InlineData(9)]
        [InlineData(0)]
        [InlineData(12)]
        public void ValidItemDoesNotRaiseAValidationMessage(int candidate)
        {
            const string LearnRefNumber = "123456789X";

            var mock = new Mock<ILearner>();
            mock.SetupGet(x => x.LearnRefNumber).Returns(LearnRefNumber);

            var mockHE = new Mock<ILearnerHE>();
            mockHE.SetupGet(x => x.LearnerHEFinancialSupports).Returns(GetFinancialSupport(candidate));
            mock.SetupGet(x => x.LearnerHEEntity).Returns(mockHE.Object);

            var mockDelivery = new Mock<ILearningDelivery>();
            var mockDeliveryHE = new Mock<ILearningDeliveryHE>();

            mockDelivery.SetupGet(x => x.LearningDeliveryHEEntity)
                .Returns(mockDeliveryHE.Object);

            var deliveries = new List<ILearningDelivery>();
            deliveries.Add(mockDelivery.Object);
            mock.SetupGet(x => x.LearningDeliveries).Returns(deliveries);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            var provider = new Mock<IProvideLookupDetails>(MockBehavior.Strict);
            provider
                .Setup(x => x.Contains(TypeOfIntegerCodedLookup.FinType, candidate))
                .Returns(true);

            var sut = new FINTYPE_01Rule(handler.Object, provider.Object);

            sut.Validate(mock.Object);

            handler.VerifyAll();
            provider.VerifyAll();
        }

        [Theory]
        [InlineData(51)]
        [InlineData(16)]
        [InlineData(9)]
        [InlineData(0)]
        [InlineData(12)]
        public void InvalidItemRaisesValidationMessage(int candidate)
        {
            const string LearnRefNumber = "123456789X";

            var mock = new Mock<ILearner>();
            mock.SetupGet(x => x.LearnRefNumber).Returns(LearnRefNumber);

            var mockHE = new Mock<ILearnerHE>();
            mockHE.SetupGet(x => x.LearnerHEFinancialSupports).Returns(GetFinancialSupport(candidate));
            mock.SetupGet(x => x.LearnerHEEntity).Returns(mockHE.Object);

            var mockDelivery = new Mock<ILearningDelivery>();

            var deliveries = new List<ILearningDelivery>();
            mock.SetupGet(x => x.LearningDeliveries).Returns(deliveries);

            var mockHandler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            mockHandler.Setup(x => x.Handle(
                Moq.It.Is<string>(y => y == FINTYPE_01Rule.Name),
                Moq.It.Is<string>(y => y == LearnRefNumber),
                null,
                Moq.It.IsAny<IEnumerable<IErrorMessageParameter>>()));
            mockHandler
                .Setup(x => x.BuildErrorMessageParameter(
                    Moq.It.Is<string>(y => y == FINTYPE_01Rule.MessagePropertyName),
                    Moq.It.IsAny<IReadOnlyCollection<ILearnerHEFinancialSupport>>()))
                .Returns(new Mock<IErrorMessageParameter>().Object);

            var mockProvider = new Mock<IProvideLookupDetails>(MockBehavior.Strict);
            mockProvider
                .Setup(x => x.Contains(TypeOfIntegerCodedLookup.FinType, candidate))
                .Returns(false);

            var sut = new FINTYPE_01Rule(mockHandler.Object, mockProvider.Object);

            sut.Validate(mock.Object);

            mockHandler.VerifyAll();
            mockProvider.VerifyAll();
        }

        public IReadOnlyCollection<ILearnerHEFinancialSupport> GetFinancialSupport(int[] candidates)
        {
            var collection = new List<ILearnerHEFinancialSupport>();

            candidates.ForEach(x =>
            {
                var mock = new Mock<ILearnerHEFinancialSupport>();
                mock.SetupGet(y => y.FINTYPE).Returns(x);
                collection.Add(mock.Object);
            });

            return collection;
        }

        public IReadOnlyCollection<ILearnerHEFinancialSupport> GetFinancialSupport(int candidate)
        {
            var collection = new List<ILearnerHEFinancialSupport>();

            var mock = new Mock<ILearnerHEFinancialSupport>();
            mock.SetupGet(y => y.FINTYPE).Returns(candidate);
            collection.Add(mock.Object);

            return collection;
        }

        public FINTYPE_01Rule NewRule()
        {
            var handler = new Mock<IValidationErrorHandler>();
            var provider = new Mock<IProvideLookupDetails>();

            return new FINTYPE_01Rule(handler.Object, provider.Object);
        }
    }
}
