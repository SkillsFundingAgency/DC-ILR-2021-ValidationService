using System.Collections.Generic;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.PwayCode;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.PwayCode
{
    public class PwayCode_02RuleTests : AbstractRuleTests<PwayCode_02Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("PwayCode_02");
        }

        [Fact]
        public void PwayCodeConditionMet_True()
        {
            var pwayCode = 1;

            NewRule().PwayCodeConditionMet(pwayCode).Should().BeTrue();
        }

        [Fact]
        public void PwayCodeConditionMet_False()
        {
            int? pwayCode = null;

            NewRule().PwayCodeConditionMet(pwayCode).Should().BeFalse();
        }

        [Theory]
        [InlineData(50, false)]
        [InlineData(25, true)]
        public void ApprenticeshipConditionMet_True(int? progType, bool mockResult)
        {
            var dd07Mock = new Mock<IDerivedData_07Rule>();
            dd07Mock.Setup(dm => dm.IsApprenticeship(progType)).Returns(mockResult);

            NewRule(dd07Mock.Object).ApprenticeshipConditionMet(progType).Should().BeTrue();
        }

        [Theory]
        [InlineData(2, true)]
        [InlineData(3, true)]
        public void ApprenticeshipConditionMet_False(int? progType, bool mockResult)
        {
            var dd07Mock = new Mock<IDerivedData_07Rule>();
            dd07Mock.Setup(dm => dm.IsApprenticeship(progType)).Returns(mockResult);

            NewRule(dd07Mock.Object).ApprenticeshipConditionMet(progType).Should().BeFalse();
        }

        [Theory]
        [InlineData(1, 50, false)]
        [InlineData(1, 25, true)]
        public void ConditionMet_True(int? pwayCode, int? progType, bool mockResult)
        {
            var dd07Mock = new Mock<IDerivedData_07Rule>();
            dd07Mock.Setup(dm => dm.IsApprenticeship(progType)).Returns(mockResult);

            NewRule(dd07Mock.Object).ConditionMet(pwayCode, progType).Should().BeTrue();
        }

        [Theory]
        [InlineData(null, 50, false)]
        [InlineData(1, 2, true)]
        public void ConditionMet_False(int? pwayCode, int? progType, bool mockResult)
        {
            var dd07Mock = new Mock<IDerivedData_07Rule>();
            dd07Mock.Setup(dm => dm.IsApprenticeship(progType)).Returns(mockResult);

            NewRule(dd07Mock.Object).ConditionMet(pwayCode, progType).Should().BeFalse();
        }

        [Theory]
        [InlineData(1, 25, true)]
        [InlineData(1, 50, false)]
        public void Validate_error(int? pwayCode, int? progType, bool mockResult)
        {
            var learner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        PwayCodeNullable = pwayCode,
                        ProgTypeNullable = progType
                    },
                }
            };

            var dd07Mock = new Mock<IDerivedData_07Rule>();
            dd07Mock.Setup(dm => dm.IsApprenticeship(progType)).Returns(mockResult);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(dd07Mock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Theory]
        [InlineData(null, 25, true)]
        [InlineData(1, 2, true)]
        public void Validate_NoError(int? pwayCode, int? progType, bool mockResult)
        {
            var learner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        PwayCodeNullable = pwayCode,
                        ProgTypeNullable = progType
                    },
                }
            };

            var dd07Mock = new Mock<IDerivedData_07Rule>();
            dd07Mock.Setup(dm => dm.IsApprenticeship(progType)).Returns(mockResult);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(dd07Mock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var pwayCode = 1;
            var progType = 25;

            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("PwayCode", pwayCode)).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("ProgType", progType)).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(pwayCode, progType);

            validationErrorHandlerMock.Verify();
        }

        private PwayCode_02Rule NewRule(
            IDerivedData_07Rule dd07 = null,
            IValidationErrorHandler validationErrorHandler = null)
        {
            return new PwayCode_02Rule(dd07, validationErrorHandler);
        }
    }
}
