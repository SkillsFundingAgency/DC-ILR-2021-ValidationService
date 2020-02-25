using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Learner.PriorAttain;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Learner.PriorAttain
{
    public class PriorAttain_03RuleTests : AbstractRuleTests<PriorAttain_03Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("PriorAttain_03");
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        [InlineData(7)]
        [InlineData(9)]
        [InlineData(10)]
        [InlineData(11)]
        [InlineData(12)]
        [InlineData(13)]
        [InlineData(97)]
        [InlineData(98)]
        [InlineData(99)]
        public void ConditionMet_True(int priorAttainValue)
        {
            var provideLookupDetailsMockup = new Mock<IProvideLookupDetails>();

            provideLookupDetailsMockup.Setup(p => p.Contains(TypeOfIntegerCodedLookup.PriorAttain, priorAttainValue)).Returns(false);

            NewRule(provideLookupDetails: provideLookupDetailsMockup.Object).ConditionMet(priorAttainValue).Should().BeTrue();
        }

        [Theory]
        [InlineData(6)]
        [InlineData(8)]
        [InlineData(14)]
        [InlineData(15)]
        public void ConditionMet_False(int priorAttainValue)
        {
            var provideLookupDetailsMockup = new Mock<IProvideLookupDetails>();

            provideLookupDetailsMockup.Setup(p => p.Contains(TypeOfIntegerCodedLookup.PriorAttain, priorAttainValue)).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandler: validationErrorHandlerMock.Object, provideLookupDetails: provideLookupDetailsMockup.Object).ConditionMet(priorAttainValue).Should().BeFalse();
            }
        }

        [Theory]
        [InlineData(6)]
        [InlineData(8)]
        [InlineData(14)]
        [InlineData(15)]
        public void Validate_Error(int priorAttain)
        {
            var testLearner = new TestLearner()
            {
                PriorAttainNullable = priorAttain
            };

            var provideLookupDetailsMockup = new Mock<IProvideLookupDetails>();

            provideLookupDetailsMockup.Setup(p => p.Contains(TypeOfIntegerCodedLookup.PriorAttain, priorAttain)).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandler: validationErrorHandlerMock.Object, provideLookupDetails: provideLookupDetailsMockup.Object).Validate(testLearner);
            }
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        [InlineData(7)]
        [InlineData(9)]
        [InlineData(10)]
        [InlineData(11)]
        [InlineData(12)]
        [InlineData(13)]
        [InlineData(97)]
        [InlineData(98)]
        [InlineData(99)]
        public void Validate_NoError(int priorAttain)
        {
            var testLearner = new TestLearner()
            {
                PriorAttainNullable = priorAttain
            };

            var provideLookupDetailsMockup = new Mock<IProvideLookupDetails>();

            provideLookupDetailsMockup.Setup(p => p.Contains(TypeOfIntegerCodedLookup.PriorAttain, priorAttain)).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandler: validationErrorHandlerMock.Object, provideLookupDetails: provideLookupDetailsMockup.Object).Validate(testLearner);
            }
        }

        [Theory]
        [InlineData(6)]
        [InlineData(8)]
        [InlineData(14)]
        [InlineData(15)]
        public void BuildErrorMessageParameters(int priorAttain)
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter(PropertyNameConstants.PriorAttain, priorAttain)).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(priorAttain);

            validationErrorHandlerMock.Verify();
        }

        private PriorAttain_03Rule NewRule(IProvideLookupDetails provideLookupDetails = null, IValidationErrorHandler validationErrorHandler = null)
        {
            return new PriorAttain_03Rule(provideLookupDetails, validationErrorHandler);
        }
    }
}