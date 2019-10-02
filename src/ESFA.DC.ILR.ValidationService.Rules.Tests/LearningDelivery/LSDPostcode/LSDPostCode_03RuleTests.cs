using System.Collections.Generic;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LSDPostcode;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.LSDPostcode
{
    public class LSDPostcode_03RuleTests : AbstractRuleTests<LSDPostcode_03Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("LSDPostcode_03");
        }

        [Fact]
        public void ConditionMet_True()
        {
            var postcode = "AA1AA";

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
        public void ConditionMet_FalseNull()
        {
            var postcodeQueryServiceMock = new Mock<IPostcodeQueryService>();
            postcodeQueryServiceMock.Setup(qs => qs.RegexValid(null)).Returns(false);

            NewRule(postcodeQueryServiceMock.Object).ConditionMet(null).Should().BeFalse();
        }

        [Fact]
        public void ValidateError()
        {
            var postcode = "AA1AA";

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                   new TestLearningDelivery
                   {
                       LSDPostcode = postcode
                   }
                }
            };

            var postcodeQueryServiceMock = new Mock<IPostcodeQueryService>();
            postcodeQueryServiceMock.Setup(qs => qs.RegexValid(postcode)).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(postcodeQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void ValidateNoError()
        {
            var postcode = "AA1 1AA";

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                   new TestLearningDelivery
                   {
                       LSDPostcode = postcode
                   }
                }
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
            var postcode = "A11AA";

            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter(PropertyNameConstants.LSDPostcode, postcode)).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(postcode);

            validationErrorHandlerMock.Verify();
        }

        private LSDPostcode_03Rule NewRule(
            IPostcodeQueryService postcodeQueryService = null,
            IValidationErrorHandler validationErrorHandler = null)
        {
            return new LSDPostcode_03Rule(postcodeQueryService, validationErrorHandler);
        }
    }
}
