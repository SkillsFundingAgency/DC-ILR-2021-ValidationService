using System.Collections.Generic;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.External.Postcodes.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.DelLocPostCode;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.DelLocPostCode
{
    public class DelLocPostCode_16RuleTests : AbstractRuleTests<DelLocPostCode_16Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("DelLocPostCode_16");
        }

        [Fact]
        public void ConditionMet_True()
        {
            var fundModel = 1;
            var postCode = "abc";

            var mockRule = NewRuleMock();

            mockRule.Setup(r => r.FundModelConditionMet(fundModel)).Returns(true);
            mockRule.Setup(r => r.PostcodeConditionMet(postCode)).Returns(true);

            mockRule.Object.ConditionMet(fundModel, postCode).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False_FundModel()
        {
            var fundModel = 1;
            var postCode = "abc";

            var mockRule = NewRuleMock();

            mockRule.Setup(r => r.FundModelConditionMet(fundModel)).Returns(false);
            mockRule.Setup(r => r.PostcodeConditionMet(postCode)).Returns(true);

            mockRule.Object.ConditionMet(fundModel, postCode).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_Postcode()
        {
            var fundModel = 1;
            var postCode = "abc";

            var mockRule = NewRuleMock();

            mockRule.Setup(r => r.FundModelConditionMet(fundModel)).Returns(true);
            mockRule.Setup(r => r.PostcodeConditionMet(postCode)).Returns(false);

            mockRule.Object.ConditionMet(fundModel, postCode).Should().BeFalse();
        }

        [Fact]
        public void FundModelConditionMet_True()
        {
            NewRule().FundModelConditionMet(70).Should().BeTrue();
        }

        [Fact]
        public void FundModelConditionMet_False()
        {
            NewRule().FundModelConditionMet(71).Should().BeFalse();
        }

        [Fact]
        public void PostcodeConditionMet_True()
        {
            var postcode = "abc";

            var postcodeDataServiceMock = new Mock<IPostcodesDataService>();

            postcodeDataServiceMock.Setup(ds => ds.ONSPostcodeExists(postcode)).Returns(false);

            NewRule(postcodeDataServiceMock.Object).PostcodeConditionMet(postcode).Should().BeTrue();
        }

        [Fact]
        public void PostcodeConditionMet_False_Temporary()
        {
            NewRule().PostcodeConditionMet("ZZ99 9ZZ").Should().BeFalse();
        }

        [Fact]
        public void PostcodeConditionMet_False_Lookup()
        {
            var postcode = "abc";

            var postcodeDataServiceMock = new Mock<IPostcodesDataService>();

            postcodeDataServiceMock.Setup(ds => ds.ONSPostcodeExists(postcode)).Returns(true);

            NewRule(postcodeDataServiceMock.Object).PostcodeConditionMet(postcode).Should().BeFalse();
        }

        [Fact]
        public void Validate_Error()
        {
            var postcode = "abc";

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        FundModel = 70,
                        DelLocPostCode = postcode
                    }
                }
            };

            var postcodeDataServiceMock = new Mock<IPostcodesDataService>();

            postcodeDataServiceMock.Setup(ds => ds.ONSPostcodeExists(postcode)).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(postcodeDataServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoErrors()
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        FundModel = 71
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandler: validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("DelLocPostCode", "abc")).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters("abc");

            validationErrorHandlerMock.Verify();
        }

        private DelLocPostCode_16Rule NewRule(IPostcodesDataService postcodesDataService = null, IValidationErrorHandler validationErrorHandler = null)
        {
            return new DelLocPostCode_16Rule(postcodesDataService, validationErrorHandler);
        }
    }
}
