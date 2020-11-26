using System.Collections.Generic;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Learner.DateOfBirth;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Learner.DateOfBirth
{
    public class DateOfBirth_43RuleTests : AbstractRuleTests<DateOfBirth_43Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("DateOfBirth_43");
        }

        [Fact]
        public void ValidatePasses_IrrelevantFundModel()
        {
            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError();

            var derivedData23Mock = new Mock<IDerivedData_23Rule>();
            derivedData23Mock
                .Setup(m => m.GetLearnersAgeAtStartOfESFContract(It.IsAny<TestLearner>(), It.IsAny<string>()))
                .Returns(14);

            var testLearner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        FundModel = FundModels.AdultSkills
                    }
                }
            };

            NewRule(validationErrorHandlerMock.Object, derivedData23Mock.Object).Validate(testLearner);
            VerifyErrorHandlerMock(validationErrorHandlerMock);
        }

        [Fact]
        public void ValidatePasses_LearnerOldEnough()
        {
            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError();

            var derivedData23Mock = new Mock<IDerivedData_23Rule>();
            derivedData23Mock
                .Setup(m => m.GetLearnersAgeAtStartOfESFContract(It.IsAny<TestLearner>(), It.IsAny<string>()))
                .Returns(15);

            var testLearner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        FundModel = FundModels.EuropeanSocialFund
                    }
                }
            };

            NewRule(validationErrorHandlerMock.Object, derivedData23Mock.Object).Validate(testLearner);
            VerifyErrorHandlerMock(validationErrorHandlerMock);
        }

        [Fact]
        public void ValidatePasses_NoLDs()
        {
            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError();

            var testLearner = new TestLearner();

            NewRule(validationErrorHandlerMock.Object).Validate(testLearner);
            VerifyErrorHandlerMock(validationErrorHandlerMock);
        }

        [Fact]
        public void ValidateFails()
        {
            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError();

            var derivedData23Mock = new Mock<IDerivedData_23Rule>();
            derivedData23Mock
                .Setup(m => m.GetLearnersAgeAtStartOfESFContract(It.IsAny<TestLearner>(), It.IsAny<string>()))
                .Returns(14);

            var testLearner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        FundModel = FundModels.EuropeanSocialFund
                    },
                    new TestLearningDelivery
                    {
                        FundModel = FundModels.AdultSkills
                    }
                }
            };

            NewRule(validationErrorHandlerMock.Object, derivedData23Mock.Object).Validate(testLearner);
            VerifyErrorHandlerMock(validationErrorHandlerMock, 1);
        }

        private DateOfBirth_43Rule NewRule(
            IValidationErrorHandler validationErrorHandler = null,
            IDerivedData_23Rule derivedData23 = null)
        {
            return new DateOfBirth_43Rule(derivedData23, validationErrorHandler);
        }
    }
}
