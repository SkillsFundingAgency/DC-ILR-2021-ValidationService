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
    public class FINTYPE_02RuleTests
    {
        [Fact]
        public void NewRuleWithNullMessageHandlerThrows()
        {
            var provider = new Mock<IProvideLookupDetails>(MockBehavior.Strict);

            Assert.Throws<ArgumentNullException>(() => new FINTYPE_02Rule(null, provider.Object));
        }

        [Fact]
        public void NewRuleWithNullProviderThrows()
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            Assert.Throws<ArgumentNullException>(() => new FINTYPE_02Rule(handler.Object, null));
        }

        [Fact]
        public void RuleName1()
        {
            var sut = NewRule();

            var result = sut.RuleName;

            Assert.Equal("FINTYPE_02", result);
        }

        [Fact]
        public void RuleName2()
        {
            var sut = NewRule();

            var result = sut.RuleName;

            Assert.Equal(FINTYPE_02Rule.Name, result);
        }

        [Fact]
        public void RuleName3()
        {
            var sut = NewRule();

            var result = sut.RuleName;

            Assert.NotEqual("SomeOtherRuleName_07", result);
        }

        [Fact]
        public void ValidateWithNullLearnerThrows()
        {
            var sut = NewRule();

            Assert.Throws<ArgumentNullException>(() => sut.Validate(null));
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
            var sut = NewRule();

            var result = sut.ConditionMet(new List<ILearnerHEFinancialSupport>());

            Assert.True(result);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(1, 2)]
        [InlineData(1, 2, 3)]
        [InlineData(1, 2, 3, 4)]
        public void ConditionMetWithValidFinancialSupportCombinationsReturnsTrue(params int[] candidates)
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var provider = new Mock<IProvideLookupDetails>(MockBehavior.Strict);
            provider
                .Setup(x => x.Get(TypeOfIntegerCodedLookup.FinType))
                .Returns(candidates);

            var sut = new FINTYPE_02Rule(handler.Object, provider.Object);

            var result = sut.ConditionMet(GetFinancialSupport(candidates));

            Assert.True(result);
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
                .Setup(x => x.Get(TypeOfIntegerCodedLookup.FinType))
                .Returns(candidates);

            var sut = new FINTYPE_02Rule(handler.Object, provider.Object);

            var result = sut.ConditionMet(GetFinancialSupport(candidates));

            Assert.False(result);
            handler.VerifyAll();
            provider.VerifyAll();
        }

        [Theory]
        [InlineData(1, 2)]
        [InlineData(1, 2, 3)]
        [InlineData(1, 2, 3, 4)]
        [InlineData(1, 2, 3, 4, 5)]
        public void ValidItemDoesNotRaiseAValidationMessage(params int[] candidates)
        {
            const string LearnRefNumber = "123456789X";

            var mock = new Mock<ILearner>();
            mock.SetupGet(x => x.LearnRefNumber).Returns(LearnRefNumber);

            var mockHE = new Mock<ILearnerHE>();
            mockHE.SetupGet(x => x.LearnerHEFinancialSupports).Returns(GetFinancialSupport(candidates));
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
                .Setup(x => x.Get(TypeOfIntegerCodedLookup.FinType))
                .Returns(candidates);

            var sut = new FINTYPE_02Rule(handler.Object, provider.Object);

            sut.Validate(mock.Object);

            handler.VerifyAll();
            provider.VerifyAll();
        }

        [Theory]
        [InlineData(2, 2)]
        [InlineData(4, 3, 4)]
        [InlineData(1, 2, 1, 3)]
        [InlineData(3, 4, 5, 6, 7, 6)]
        public void InvalidItemRaisesValidationMessage(params int[] candidates)
        {
            const string LearnRefNumber = "123456789X";

            var mock = new Mock<ILearner>();
            mock.SetupGet(x => x.LearnRefNumber).Returns(LearnRefNumber);

            var mockHE = new Mock<ILearnerHE>();
            mockHE.SetupGet(x => x.LearnerHEFinancialSupports).Returns(GetFinancialSupport(candidates));
            mock.SetupGet(x => x.LearnerHEEntity).Returns(mockHE.Object);

            var mockDelivery = new Mock<ILearningDelivery>();

            var deliveries = new List<ILearningDelivery>();
            mock.SetupGet(x => x.LearningDeliveries).Returns(deliveries);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            handler.Setup(x => x.Handle(
                Moq.It.Is<string>(y => y == "FINTYPE_02"),
                Moq.It.Is<string>(y => y == LearnRefNumber),
                null,
                Moq.It.IsAny<IEnumerable<IErrorMessageParameter>>()));
            handler
                .Setup(x => x.BuildErrorMessageParameter(
                    Moq.It.Is<string>(y => y == FINTYPE_02Rule.MessagePropertyName),
                    Moq.It.IsAny<IReadOnlyCollection<ILearnerHEFinancialSupport>>()))
                .Returns(new Mock<IErrorMessageParameter>().Object);

            var provider = new Mock<IProvideLookupDetails>(MockBehavior.Strict);
            provider
                .Setup(x => x.Get(TypeOfIntegerCodedLookup.FinType))
                .Returns(candidates);

            var sut = new FINTYPE_02Rule(handler.Object, provider.Object);

            sut.Validate(mock.Object);

            handler.VerifyAll();
            provider.VerifyAll();
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

        public FINTYPE_02Rule NewRule()
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var provider = new Mock<IProvideLookupDetails>(MockBehavior.Strict);

            return new FINTYPE_02Rule(handler.Object, provider.Object);
        }
    }
}
