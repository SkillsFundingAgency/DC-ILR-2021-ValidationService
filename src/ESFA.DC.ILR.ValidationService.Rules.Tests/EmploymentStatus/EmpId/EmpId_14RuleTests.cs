using System.Collections.Generic;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.EmploymentStatus.EmpId;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.EmploymentStatus.EmpId
{
    public class EmpId_14RuleTests : AbstractRuleTests<EmpId_14Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("EmpId_14");
        }

        [Theory]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(20)]
        [InlineData(21)]
        [InlineData(22)]
        [InlineData(23)]
        [InlineData(25)]
        public void ConditionMet_True(int progType)
        {
            NewRule().ConditionMet(progType).Should().BeTrue();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(null)]
        public void ConditionMet_False(int? progType)
        {
            NewRule().ConditionMet(progType).Should().BeFalse();
        }

        [Fact]
        public void Validate_Error()
        {
            var learnerEmploymentStatuses = new List<TestLearnerEmploymentStatus>()
            {
                new TestLearnerEmploymentStatus
                {
                    EmpIdNullable = 999999999,
                    EmpStat = 11
                },
                new TestLearnerEmploymentStatus
                {
                    EmpIdNullable = 999999999,
                    EmpStat = 12
                },
                new TestLearnerEmploymentStatus
                {
                    EmpIdNullable = null,
                    EmpStat = 0
                },
            };

            var learner = new TestLearner
            {
                LearnerEmploymentStatuses = learnerEmploymentStatuses,
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        ProgTypeNullable = 2,
                    },
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError()
        {
            var learnerEmploymentStatuses = new List<TestLearnerEmploymentStatus>()
            {
                new TestLearnerEmploymentStatus
                {
                    EmpIdNullable = null,
                    EmpStat = 0
                },
            };

            var learner = new TestLearner
            {
                LearnerEmploymentStatuses = learnerEmploymentStatuses,
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        ProgTypeNullable = 2,
                    },
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoErrorNullLes()
        {
            var learner = new TestLearner
            {
                LearnerEmploymentStatuses = null,
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        ProgTypeNullable = 2,
                    },
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("ProgType", 1)).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("EmpStat", 2)).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("EmpId", 3)).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(1, 2, 3);

            validationErrorHandlerMock.Verify();
        }

        private EmpId_14Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new EmpId_14Rule(validationErrorHandler);
        }
    }
}
