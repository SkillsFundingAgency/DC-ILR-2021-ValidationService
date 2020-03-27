using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Learner.Postcode;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Learner.Postcode
{
    public class Postcode_15RuleTests : AbstractRuleTests<Postcode_15Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("Postcode_15");
        }

        [Fact]
        public void ConditionMet_True()
        {
            var postcode = "abc";

            var postcodeQueryServiceMock = new Mock<IPostcodeQueryService>();

            postcodeQueryServiceMock.Setup(qs => qs.RegexValid(postcode)).Returns(false);

            NewRule(postcodeQueryServiceMock.Object).ConditionMet(postcode).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False()
        {
            var postcode = "AA1 1AA";

            var postcodeQueryServiceMock = new Mock<IPostcodeQueryService>();

            postcodeQueryServiceMock.Setup(qs => qs.RegexValid(postcode)).Returns(true);

            NewRule(postcodeQueryServiceMock.Object).ConditionMet(postcode).Should().BeFalse();
        }

        [Fact]
        public void Validate_Error()
        {
            var postcode = "abc";

            var learner = new TestLearner()
            {
                Postcode = postcode
            };

            var postcodeQueryServiceMock = new Mock<IPostcodeQueryService>();

            postcodeQueryServiceMock.Setup(qs => qs.RegexValid(postcode)).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(postcodeQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoErrors()
        {
            var postcode = "AA1 1AA";

            var learner = new TestLearner()
            {
                Postcode = postcode
            };

            var postcodeQueryServiceMock = new Mock<IPostcodeQueryService>();

            postcodeQueryServiceMock.Setup(qs => qs.RegexValid(postcode)).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(postcodeQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("Postcode", "abc")).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters("abc");

            validationErrorHandlerMock.Verify();
        }

        private Postcode_15Rule NewRule(IPostcodeQueryService postcodeQueryService = null, IValidationErrorHandler validationErrorHandler = null)
        {
            return new Postcode_15Rule(postcodeQueryService, validationErrorHandler);
        }
    }
}
