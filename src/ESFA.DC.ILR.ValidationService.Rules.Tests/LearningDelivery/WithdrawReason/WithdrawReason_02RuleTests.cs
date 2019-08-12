using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.WithdrawReason;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.WithdrawReason
{
    public class WithdrawReason_02RuleTests : AbstractRuleTests<WithdrawReason_02Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("WithdrawReason_02");
        }

        [Fact]
        public void ConditionMet_True()
        {
            var lookupDetailsMock = new Mock<IProvideLookupDetails>();
            var withdrawReason = 1;

            NewRule(lookupDetails: lookupDetailsMock.Object).ConditionMet(withdrawReason).Should().BeTrue();
        }

        [Theory]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(7)]
        [InlineData(28)]
        [InlineData(29)]
        [InlineData(40)]
        [InlineData(41)]
        [InlineData(42)]
        [InlineData(43)]
        [InlineData(44)]
        [InlineData(45)]
        [InlineData(46)]
        [InlineData(47)]
        [InlineData(97)]
        [InlineData(98)]
        public void ConditionMet_False(int? withdrawReason)
        {
            var lookupDetailsMock = new Mock<IProvideLookupDetails>();
            lookupDetailsMock.Setup(l => l.Contains(TypeOfIntegerCodedLookup.WithdrawReason, withdrawReason.Value)).Returns(true);
            NewRule(lookupDetails: lookupDetailsMock.Object).ConditionMet(withdrawReason).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_FalseNull()
        {
            NewRule().ConditionMet(null).Should().BeFalse();
        }

        [Fact]
        public void ValidateError()
        {
            var lookupDetailsMock = new Mock<IProvideLookupDetails>();

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        WithdrawReasonNullable = 1
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandlerMock.Object, lookupDetailsMock.Object).Validate(learner);
            }
        }

        [Theory]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(7)]
        [InlineData(28)]
        [InlineData(29)]
        [InlineData(40)]
        [InlineData(41)]
        [InlineData(42)]
        [InlineData(43)]
        [InlineData(44)]
        [InlineData(45)]
        [InlineData(46)]
        [InlineData(47)]
        [InlineData(97)]
        [InlineData(98)]
        public void ValidateNoError(int? withdrawReason)
        {
            var lookupDetailsMock = new Mock<IProvideLookupDetails>();
            lookupDetailsMock.Setup(l => l.Contains(TypeOfIntegerCodedLookup.WithdrawReason, withdrawReason.Value))
                .Returns(true);

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        WithdrawReasonNullable = withdrawReason
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object, lookupDetailsMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            int? withdrawReason = 1;

            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("WithdrawReason", withdrawReason)).Verifiable();

            NewRule(validationErrorHandlerMock.Object).BuildErrorMessageParameters(withdrawReason);

            validationErrorHandlerMock.Verify();
        }

        [Fact]
        public void CallLookupProvider()
        {
            int? withdrawReason = 1;

            var lookupDetailsMock = new Mock<IProvideLookupDetails>();

            lookupDetailsMock.Setup(l => l.Contains(TypeOfIntegerCodedLookup.WithdrawReason, withdrawReason.Value)).Verifiable();

            NewRule(lookupDetails: lookupDetailsMock.Object).ConditionMet(withdrawReason);

            lookupDetailsMock.Verify();
        }

        private WithdrawReason_02Rule NewRule(IValidationErrorHandler validationErrorHandler = null, IProvideLookupDetails lookupDetails = null)
        {
            return new WithdrawReason_02Rule(validationErrorHandler, lookupDetails);
        }
    }
}
