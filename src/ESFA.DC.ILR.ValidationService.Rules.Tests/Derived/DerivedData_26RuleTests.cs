using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Derived
{
    public class DerivedData_26RuleTests
    {
        [Fact]
        public void LearnerOnBenefitsAtStartOfCompletedZESF0001AimForContract_True()
        {
            var conRefNumber = "conRefNumber";
            var learnStartDate = new DateTime(2017, 1, 1);

            var learnerEmploymentStatus = new TestLearnerEmploymentStatus()
            {
                EmploymentStatusMonitorings = new List<IEmploymentStatusMonitoring>()
                {
                    new TestEmploymentStatusMonitoring()
                    {
                        ESMType = Monitoring.EmploymentStatus.Types.BenefitStatusIndicator
                    }
                }
            };

            var learnerEmploymentStatuses = new List<ILearnerEmploymentStatus>()
            {
                learnerEmploymentStatus
            };

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<ILearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        CompStatus = CompletionState.HasCompleted,
                        ConRefNumber = conRefNumber,
                        LearnAimRef = ValidationConstants.ZESF0001,
                        LearnStartDate = learnStartDate,
                    }
                },
                LearnerEmploymentStatuses = learnerEmploymentStatuses
            };

            var learnerEmploymentStatusQueryServiceMock = new Mock<ILearnerEmploymentStatusQueryService>();

            learnerEmploymentStatusQueryServiceMock.Setup(qs => qs.LearnerEmploymentStatusForDate(learnerEmploymentStatuses, learnStartDate)).Returns(learnerEmploymentStatus);

            NewRule(learnerEmploymentStatusQueryServiceMock.Object).LearnerOnBenefitsAtStartOfCompletedZESF0001AimForContract(learner, conRefNumber).Should().BeTrue();
        }

        [Fact]
        public void LearnerOnBenefitsAtStartOfCompletedZESF0001AimForContract_False_LearnerNull()
        {
            var conRefNumber = "conRefNumber";

            NewRule().LearnerOnBenefitsAtStartOfCompletedZESF0001AimForContract(null, conRefNumber).Should().BeFalse();
        }

        [Fact]
        public void LearnerOnBenefitsAtStartOfCompletedZESF0001AimForContract_False_NoMatchingAim()
        {
            var conRefNumber = "conRefNumber";
            var learnStartDate = new DateTime(2017, 1, 1);

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<ILearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        CompStatus = CompletionState.HasTemporarilyWithdrawn,
                        ConRefNumber = conRefNumber,
                        LearnAimRef = ValidationConstants.ZESF0001,
                        LearnStartDate = learnStartDate,
                    }
                },
            };

            NewRule().LearnerOnBenefitsAtStartOfCompletedZESF0001AimForContract(learner, conRefNumber).Should().BeFalse();
        }

        [Fact]
        public void LearnerOnBenefitsAtStartOfCompletedZESF0001AimForContract_False_NoMatchingEmploymentStatusRecordForDate()
        {
            var conRefNumber = "conRefNumber";
            var learnStartDate = new DateTime(2017, 1, 1);

            var learnerEmploymentStatuses = new List<ILearnerEmploymentStatus>();

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<ILearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        CompStatus = CompletionState.HasCompleted,
                        ConRefNumber = conRefNumber,
                        LearnAimRef = ValidationConstants.ZESF0001,
                        LearnStartDate = learnStartDate,
                    }
                },
                LearnerEmploymentStatuses = learnerEmploymentStatuses
            };

            var learnerEmploymentStatusQueryServiceMock = new Mock<ILearnerEmploymentStatusQueryService>();

            learnerEmploymentStatusQueryServiceMock.Setup(qs => qs.LearnerEmploymentStatusForDate(learnerEmploymentStatuses, learnStartDate)).Returns(null as ILearnerEmploymentStatus);

            NewRule(learnerEmploymentStatusQueryServiceMock.Object).LearnerOnBenefitsAtStartOfCompletedZESF0001AimForContract(learner, conRefNumber).Should().BeFalse();
        }

        [Fact]
        public void LearnerOnBenefitsAtStartOfCompletedZESF0001AimForContract_False_NoMatchingMonitoringRecord()
        {
            var conRefNumber = "conRefNumber";
            var learnStartDate = new DateTime(2017, 1, 1);

            var learnerEmploymentStatus = new TestLearnerEmploymentStatus()
            {
                EmploymentStatusMonitorings = new List<IEmploymentStatusMonitoring>()
                {
                    new TestEmploymentStatusMonitoring()
                    {
                        ESMType = Monitoring.EmploymentStatus.Types.EmploymentIntensityIndicator
                    }
                }
            };

            var learnerEmploymentStatuses = new List<ILearnerEmploymentStatus>()
            {
                learnerEmploymentStatus
            };

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<ILearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        CompStatus = CompletionState.HasCompleted,
                        ConRefNumber = conRefNumber,
                        LearnAimRef = ValidationConstants.ZESF0001,
                        LearnStartDate = learnStartDate,
                    }
                },
                LearnerEmploymentStatuses = learnerEmploymentStatuses
            };

            var learnerEmploymentStatusQueryServiceMock = new Mock<ILearnerEmploymentStatusQueryService>();

            learnerEmploymentStatusQueryServiceMock.Setup(qs => qs.LearnerEmploymentStatusForDate(learnerEmploymentStatuses, learnStartDate)).Returns(learnerEmploymentStatus);

            NewRule(learnerEmploymentStatusQueryServiceMock.Object).LearnerOnBenefitsAtStartOfCompletedZESF0001AimForContract(learner, conRefNumber).Should().BeFalse();
        }

        [Theory]
        [InlineData("ZESF0001", "conRefNumber")]
        public void GetCompletedZESF0001AimForContract_Match(string learnAimRef, string conRefNumber)
        {
            var learningDelivery = new TestLearningDelivery()
            {
                CompStatus = CompletionState.HasCompleted,
                LearnAimRef = learnAimRef,
                ConRefNumber = conRefNumber
            };

            var learningDeliveries = new List<ILearningDelivery>()
            {
                learningDelivery
            };

            NewRule().GetCompletedZESF0001AimForContract(learningDeliveries, "conRefNumber").Should().Be(learningDelivery);
        }

        [Theory]
        [InlineData(CompletionState.HasTemporarilyWithdrawn, "ZESF0001", "conRefNumber")]
        [InlineData(CompletionState.HasCompleted, "noAim", "conRefNumber")]
        [InlineData(CompletionState.HasCompleted, "ZESF0001", "noContract")]
        public void GetCompletedZESF0001AimForContract_NoMatch(int compStatus, string learnAimRef, string conRefNumber)
        {
            var learningDelivery = new TestLearningDelivery()
            {
                CompStatus = compStatus,
                LearnAimRef = learnAimRef,
                ConRefNumber = conRefNumber
            };

            var learningDeliveries = new List<ILearningDelivery>()
            {
                learningDelivery
            };

            NewRule().GetCompletedZESF0001AimForContract(learningDeliveries, "conRefNumber").Should().BeNull();
        }

        [Fact]
        public void GetCompletedZESF0001AimForContract_NoMatch_NullLearningDeliveries()
        {
            NewRule().GetCompletedZESF0001AimForContract(null, "conRefNumber").Should().BeNull();
        }

        [Fact]
        public void HasEmploymentStatusMonitoringForTypeBSI_True()
        {
            var employmentStatusMonitorings = new List<IEmploymentStatusMonitoring>()
            {
                new TestEmploymentStatusMonitoring()
                {
                    ESMType = Monitoring.EmploymentStatus.Types.BenefitStatusIndicator,
                }
            };

            NewRule().HasEmploymentStatusMonitoringForTypeBSI(employmentStatusMonitorings).Should().BeTrue();
        }

        [Fact]
        public void HasEmploymentStatusMonitoringForTypeBSI_True_CaseInsensitive()
        {
            var employmentStatusMonitorings = new List<IEmploymentStatusMonitoring>()
            {
                new TestEmploymentStatusMonitoring()
                {
                    ESMType = "bsi"
                }
            };

            NewRule().HasEmploymentStatusMonitoringForTypeBSI(employmentStatusMonitorings).Should().BeTrue();
        }

        [Fact]
        public void HasEmploymentStatusMonitoringForTypeBSI_False()
        {
            var employmentStatusMonitorings = new List<IEmploymentStatusMonitoring>()
            {
                new TestEmploymentStatusMonitoring()
                {
                    ESMType = Monitoring.EmploymentStatus.Types.EmploymentIntensityIndicator
                }
            };

            NewRule().HasEmploymentStatusMonitoringForTypeBSI(employmentStatusMonitorings).Should().BeFalse();
        }

        [Fact]
        public void HasEmploymentStatusMonitoringForTypeBSI_False_NullMonitorings()
        {
            NewRule().HasEmploymentStatusMonitoringForTypeBSI(null).Should().BeFalse();
        }

        private DerivedData_26Rule NewRule(ILearnerEmploymentStatusQueryService learnerEmploymentStatusQueryService = null)
        {
            return new DerivedData_26Rule(learnerEmploymentStatusQueryService);
        }
    }
}
