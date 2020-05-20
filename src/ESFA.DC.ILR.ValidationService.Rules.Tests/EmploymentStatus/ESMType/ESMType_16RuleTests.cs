using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.EmploymentStatus.ESMType;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.EmploymentStatus.ESMType
{
    public class ESMType_16RuleTests : AbstractRuleTests<ESMType_16Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("ESMType_16");
        }

        [Fact]
        public void LearningDeliveryConditionMet_True()
        {
            var learningDeliveries = new List<TestLearningDelivery>()
            {
                new TestLearningDelivery()
                {
                    ProgTypeNullable = 25
                }
            };

            NewRule().LearningDeliveryConditionMet(learningDeliveries).Should().BeTrue();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(null)]
        public void LearningDeliveryConditionMet_False(int? progType)
        {
            var learningDeliveries = new List<TestLearningDelivery>()
            {
                new TestLearningDelivery()
                {
                    ProgTypeNullable = progType
                }
            };

            NewRule().LearningDeliveryConditionMet(learningDeliveries).Should().BeFalse();
        }

        [Fact]
        public void LearningDeliveryConditionMet_FalseNullDelivery()
        {
            NewRule().LearningDeliveryConditionMet(null).Should().BeFalse();
        }

        [Fact]
        public void Validate_Error()
        {
            var learnerEmploymentStatuses = new TestLearnerEmploymentStatus[]
            {
                new TestLearnerEmploymentStatus
                {
                    EmpStat = 12,
                    EmploymentStatusMonitorings = new TestEmploymentStatusMonitoring[]
                    {
                        new TestEmploymentStatusMonitoring
                        {
                            ESMType = "SEM",
                            ESMCode = 1
                        }
                    }
                }
            };

            var learner = new TestLearner
            {
                LearnerEmploymentStatuses = learnerEmploymentStatuses,
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        ProgTypeNullable = 25,
                    },
                }
            };

            var learnerEmploymentStatusMonitoringQueryServiceMock = new Mock<ILearnerEmploymentStatusMonitoringQueryService>();
            learnerEmploymentStatusMonitoringQueryServiceMock
                .Setup(ds => ds.HasAnyEmploymentStatusMonitoringTypeAndCodeForEmploymentStatus(It.IsAny<ILearnerEmploymentStatus>(), "SEM", 1))
                .Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(learnerEmploymentStatusMonitoringQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError_ValidEmpStat()
        {
            var learnerEmploymentStatuses = new TestLearnerEmploymentStatus[]
            {
                new TestLearnerEmploymentStatus
                {
                    EmpStat = 10,
                    EmploymentStatusMonitorings = new TestEmploymentStatusMonitoring[]
                    {
                        new TestEmploymentStatusMonitoring
                        {
                            ESMType = "SEM",
                            ESMCode = 1
                        }
                    }
                }
            };

            var learner = new TestLearner
            {
                LearnerEmploymentStatuses = learnerEmploymentStatuses,
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        ProgTypeNullable = 25,
                    },
                }
            };

            var learnerEmploymentStatusMonitoringQueryServiceMock = new Mock<ILearnerEmploymentStatusMonitoringQueryService>();
            learnerEmploymentStatusMonitoringQueryServiceMock
                .Setup(ds => ds.HasAnyEmploymentStatusMonitoringTypeAndCodeForEmploymentStatus(It.IsAny<ILearnerEmploymentStatus>(), "SEM", 1))
                .Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(learnerEmploymentStatusMonitoringQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError_ValidESM()
        {
            var learnerEmploymentStatuses = new TestLearnerEmploymentStatus[]
            {
                new TestLearnerEmploymentStatus
                {
                    EmpStat = 12,
                    EmploymentStatusMonitorings = new TestEmploymentStatusMonitoring[]
                    {
                        new TestEmploymentStatusMonitoring
                        {
                            ESMType = "SEM",
                            ESMCode = 2
                        }
                    }
                }
            };

            var learner = new TestLearner
            {
                LearnerEmploymentStatuses = learnerEmploymentStatuses,
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        ProgTypeNullable = 25,
                    },
                }
            };

            var learnerEmploymentStatusMonitoringQueryServiceMock = new Mock<ILearnerEmploymentStatusMonitoringQueryService>();
            learnerEmploymentStatusMonitoringQueryServiceMock
                .Setup(ds => ds.HasAnyEmploymentStatusMonitoringTypeAndCodeForEmploymentStatus(It.IsAny<ILearnerEmploymentStatus>(), "SEM", 2))
                .Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(learnerEmploymentStatusMonitoringQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("ProgType", 25)).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("EmpStat", 10)).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("ESMType", "SEM")).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("ESMCode", 1)).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(10);

            validationErrorHandlerMock.Verify();
        }

        private ESMType_16Rule NewRule(ILearnerEmploymentStatusMonitoringQueryService learnerEmploymentStatusMonitoringQueryService = null, IValidationErrorHandler validationErrorHandler = null)
        {
            return new ESMType_16Rule(learnerEmploymentStatusMonitoringQueryService, validationErrorHandler);
        }
    }
}
