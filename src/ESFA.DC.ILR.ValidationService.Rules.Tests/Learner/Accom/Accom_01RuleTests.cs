using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Learner.Accom;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Learner.Accom
{
    public class Accom_01RuleTests : AbstractRuleTests<Accom_01Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("Accom_01");
        }

        [Fact]
        public void ConditionMet_True()
        {
            int accomValue = 4;

            var provideLookupDetailsMockup = new Mock<IProvideLookupDetails>();

            provideLookupDetailsMockup.Setup(p => p.Contains(TypeOfIntegerCodedLookup.Accom, accomValue)).Returns(false);

            NewRule(provideLookupDetails: provideLookupDetailsMockup.Object).ConditionMet(accomValue).Should().BeTrue();
        }

        [Theory]
        [InlineData(5)]
        public void ConditionMet_False(int accomValue)
        {
            var provideLookupDetailsMockup = new Mock<IProvideLookupDetails>();

            provideLookupDetailsMockup.Setup(p => p.Contains(TypeOfIntegerCodedLookup.Accom, accomValue)).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandler: validationErrorHandlerMock.Object, provideLookupDetails: provideLookupDetailsMockup.Object).ConditionMet(accomValue).Should().BeFalse();
            }
        }

        [Fact]
        public void Validate_Error()
        {
            var testLearner = new TestLearner()
            {
                AccomNullable = 10
            };

            var provideLookupDetailsMockup = new Mock<IProvideLookupDetails>();

            provideLookupDetailsMockup.Setup(p => p.Contains(TypeOfIntegerCodedLookup.Accom, 10)).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandler: validationErrorHandlerMock.Object, provideLookupDetails: provideLookupDetailsMockup.Object).Validate(testLearner);
            }
        }

        [Fact]
        public void Validate_NoError()
        {
            var testLearner = new TestLearner()
            {
                Ethnicity = 4
            };

            var provideLookupDetailsMockup = new Mock<IProvideLookupDetails>();

            provideLookupDetailsMockup.Setup(p => p.Contains(TypeOfIntegerCodedLookup.Ethnicity, 4)).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandler: validationErrorHandlerMock.Object, provideLookupDetails: provideLookupDetailsMockup.Object).Validate(testLearner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            int accomValue = 5;

            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter(PropertyNameConstants.Accom, accomValue)).Verifiable();

            NewRule(validationErrorHandlerMock.Object).BuildErrorMessageParameters(accomValue);

            validationErrorHandlerMock.Verify();
        }

        private Accom_01Rule NewRule(IValidationErrorHandler validationErrorHandler = null, IProvideLookupDetails provideLookupDetails = null)
        {
            return new Accom_01Rule(provideLookupDetails: provideLookupDetails, validationErrorHandler: validationErrorHandler);
        }
    }
}
