using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Rules.Derived;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Derived
{
    public class DerivedData_37RuleTests
    {
        [Fact]
        public void FundModelCondition_True()
        {
            DD().FundModelCondition(35).Should().BeTrue();
        }

        [Fact]
        public void FundModelCondition_Fact()
        {
            DD().FundModelCondition(99).Should().BeFalse();
        }

        [Fact]
        public void ValidEmpstat_True()
        {
            DD().ValidEmpstat(12).Should().BeTrue();
        }

        [Fact]
        public void ValidEmpstat()
        {
            DD().ValidEmpstat(1200).Should().BeFalse();
        }

        [Theory]
        [InlineData(5, true, false)]
        [InlineData(4, false, true)]
        public void EmpStatMonitoringCondition_True(int bsiCode, bool mockOne, bool mockTwo)
        {
            var empStatus = new TestLearnerEmploymentStatus
            {
                EmpStat = 12,
                DateEmpStatApp = new DateTime(2018, 9, 1),
                EmploymentStatusMonitorings = new List<TestEmploymentStatusMonitoring>
                {
                    new TestEmploymentStatusMonitoring
                    {
                        ESMType = "BSI",
                        ESMCode = bsiCode
                    }
                }
            };

            var learningDeliveryFAMs = new List<ILearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "LDM",
                    LearnDelFAMCode = "317"
                }
            };

            var empStatusQSMock = new Mock<ILearnerEmploymentStatusMonitoringQueryService>();
            empStatusQSMock.Setup(x => x.HasAnyEmploymentStatusMonitoringTypeAndCodeForEmploymentStatus(empStatus, "BSI", 6)).Returns(mockOne);
            empStatusQSMock.Setup(x => x.HasAnyEmploymentStatusMonitoringTypeAndCodeForEmploymentStatus(empStatus, "BSI", 4)).Returns(mockTwo);

            var learningDeliveryFAMQSMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFAMQSMock.Setup(x => x.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, "LDM", "318")).Returns(false);

            DD(
                learnerEmploymentStatusMonitoringQueryService: empStatusQSMock.Object,
                learningDeliveryFAMQueryService: learningDeliveryFAMQSMock.Object).EmpStatMonitoringCondition(empStatus, learningDeliveryFAMs).Should().BeTrue();
        }

        [Theory]
        [InlineData(6, false, false, true)]
        [InlineData(4, false, true, true)]
        public void EmpStatMonitoringCondition_False(int bsiCode, bool mockOne, bool mockTwo, bool mockThree)
        {
            var empStatus = new TestLearnerEmploymentStatus
            {
                EmpStat = 12,
                DateEmpStatApp = new DateTime(2018, 9, 1),
                EmploymentStatusMonitorings = new List<TestEmploymentStatusMonitoring>
                {
                    new TestEmploymentStatusMonitoring
                    {
                        ESMType = "BSI",
                        ESMCode = bsiCode
                    }
                }
            };

            var learningDeliveryFAMs = new List<ILearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "LDM",
                    LearnDelFAMCode = "318"
                }
            };

            var empStatusQSMock = new Mock<ILearnerEmploymentStatusMonitoringQueryService>();
            empStatusQSMock.Setup(x => x.HasAnyEmploymentStatusMonitoringTypeAndCodeForEmploymentStatus(empStatus, "BSI", 5)).Returns(mockOne);
            empStatusQSMock.Setup(x => x.HasAnyEmploymentStatusMonitoringTypeAndCodeForEmploymentStatus(empStatus, "BSI", 4)).Returns(mockTwo);

            var learningDeliveryFAMQSMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFAMQSMock.Setup(x => x.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, "LDM", "318")).Returns(mockThree);

            DD(
                learnerEmploymentStatusMonitoringQueryService: empStatusQSMock.Object,
                learningDeliveryFAMQueryService: learningDeliveryFAMQSMock.Object).EmpStatMonitoringCondition(empStatus, learningDeliveryFAMs).Should().BeFalse();
        }

        [Fact]
        public void EmpStatMonitoringCondition_True_NullFams()
        {
            var empStatus = new TestLearnerEmploymentStatus
            {
                EmpStat = 12,
                DateEmpStatApp = new DateTime(2018, 9, 1),
                EmploymentStatusMonitorings = new List<TestEmploymentStatusMonitoring>
                {
                    new TestEmploymentStatusMonitoring
                    {
                        ESMType = "BSI",
                        ESMCode = 4
                    }
                }
            };

            var empStatusQSMock = new Mock<ILearnerEmploymentStatusMonitoringQueryService>();
            empStatusQSMock.Setup(x => x.HasAnyEmploymentStatusMonitoringTypeAndCodeForEmploymentStatus(empStatus, "BSI", 5)).Returns(false);
            empStatusQSMock.Setup(x => x.HasAnyEmploymentStatusMonitoringTypeAndCodeForEmploymentStatus(empStatus, "BSI", 4)).Returns(true);

            var learningDeliveryFAMQSMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFAMQSMock.Setup(x => x.HasLearningDeliveryFAMCodeForType(null, "LDM", "318")).Returns(false);

            DD(
                learnerEmploymentStatusMonitoringQueryService: empStatusQSMock.Object,
                learningDeliveryFAMQueryService: learningDeliveryFAMQSMock.Object).EmpStatMonitoringCondition(empStatus, null).Should().BeTrue();
        }

        [Fact]
        public void EmpStatusCondition_True()
        {
            var empStatus = new TestLearnerEmploymentStatus
            {
                EmpStat = 12,
                DateEmpStatApp = new DateTime(2018, 9, 1),
                EmploymentStatusMonitorings = new List<TestEmploymentStatusMonitoring>
                {
                    new TestEmploymentStatusMonitoring
                    {
                        ESMType = "BSI",
                        ESMCode = 4
                    }
                }
            };

            var learningDeliveryFAMs = new List<ILearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "LDM",
                    LearnDelFAMCode = "318"
                }
            };

            var empStatusQSMock = new Mock<ILearnerEmploymentStatusMonitoringQueryService>();
            empStatusQSMock.Setup(x => x.HasAnyEmploymentStatusMonitoringTypeAndCodeForEmploymentStatus(empStatus, "BSI", 5)).Returns(false);
            empStatusQSMock.Setup(x => x.HasAnyEmploymentStatusMonitoringTypeAndCodeForEmploymentStatus(empStatus, "BSI", 4)).Returns(true);

            var learningDeliveryFAMQSMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFAMQSMock.Setup(x => x.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, "LDM", "318")).Returns(false);

            DD(
                learnerEmploymentStatusMonitoringQueryService: empStatusQSMock.Object,
                learningDeliveryFAMQueryService: learningDeliveryFAMQSMock.Object).EmpStatusCondition(empStatus, learningDeliveryFAMs).Should().BeTrue();
        }

        [Fact]
        public void EmpStatusCondition_False()
        {
            var empStatus = new TestLearnerEmploymentStatus
            {
                EmpStat = 12,
                DateEmpStatApp = new DateTime(2018, 9, 1),
                EmploymentStatusMonitorings = new List<TestEmploymentStatusMonitoring>
                {
                    new TestEmploymentStatusMonitoring
                    {
                        ESMType = "BSI",
                        ESMCode = 4
                    }
                }
            };

            var learningDeliveryFAMs = new List<ILearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "LDM",
                    LearnDelFAMCode = "318"
                }
            };

            var empStatusQSMock = new Mock<ILearnerEmploymentStatusMonitoringQueryService>();
            empStatusQSMock.Setup(x => x.HasAnyEmploymentStatusMonitoringTypeAndCodeForEmploymentStatus(empStatus, "BSI", 5)).Returns(false);
            empStatusQSMock.Setup(x => x.HasAnyEmploymentStatusMonitoringTypeAndCodeForEmploymentStatus(empStatus, "BSI", 4)).Returns(true);

            var learningDeliveryFAMQSMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFAMQSMock.Setup(x => x.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, "LDM", "318")).Returns(true);

            DD(
                learnerEmploymentStatusMonitoringQueryService: empStatusQSMock.Object,
                learningDeliveryFAMQueryService: learningDeliveryFAMQSMock.Object).EmpStatusCondition(empStatus, learningDeliveryFAMs).Should().BeFalse();
        }

        [Fact]
        public void Derive_True()
        {
            var learnStartDate = new DateTime(2018, 9, 1);
            var employmentStatuses = new List<ILearnerEmploymentStatus>();
            var empStatus = new TestLearnerEmploymentStatus
            {
                EmpStat = 12,
                DateEmpStatApp = new DateTime(2018, 9, 1),
                EmploymentStatusMonitorings = new List<TestEmploymentStatusMonitoring>
                {
                    new TestEmploymentStatusMonitoring
                    {
                        ESMType = "BSI",
                        ESMCode = 5
                    }
                }
            };

            employmentStatuses.Add(empStatus);

            var learningDeliveryFAMs = new List<ILearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "LDM",
                    LearnDelFAMCode = "318"
                }
            };

            var empStatQSMock = new Mock<ILearnerEmploymentStatusQueryService>();
            empStatQSMock.Setup(x => x.LearnerEmploymentStatusesForDate(employmentStatuses, learnStartDate)).Returns(employmentStatuses);

            var empStatusQSMock = new Mock<ILearnerEmploymentStatusMonitoringQueryService>();
            empStatusQSMock.Setup(x => x.HasAnyEmploymentStatusMonitoringTypeAndCodeForEmploymentStatus(empStatus, "BSI", 5)).Returns(true);
            empStatusQSMock.Setup(x => x.HasAnyEmploymentStatusMonitoringTypeAndCodeForEmploymentStatus(empStatus, "BSI", 4)).Returns(true);

            var learningDeliveryFAMQSMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFAMQSMock.Setup(x => x.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, "LDM", "318")).Returns(false);

            DD(empStatQSMock.Object, empStatusQSMock.Object, learningDeliveryFAMQSMock.Object).Derive(35, learnStartDate, employmentStatuses, learningDeliveryFAMs).Should().BeTrue();
        }

        [Fact]
        public void Derive_True_Multiple()
        {
            var learnStartDate = new DateTime(2018, 9, 1);
            var employmentStatuses = new List<ILearnerEmploymentStatus>();
            var empStatusOne = new TestLearnerEmploymentStatus
            {
                EmpStat = 12,
                DateEmpStatApp = new DateTime(2018, 9, 1),
                EmploymentStatusMonitorings = new List<TestEmploymentStatusMonitoring>
                {
                    new TestEmploymentStatusMonitoring
                    {
                        ESMType = "BSI",
                        ESMCode = 5
                    }
                }
            };

            var empStatusTwo = new TestLearnerEmploymentStatus
            {
                EmpStat = 10,
                DateEmpStatApp = new DateTime(2018, 9, 1),
                EmploymentStatusMonitorings = new List<TestEmploymentStatusMonitoring>
                {
                    new TestEmploymentStatusMonitoring
                    {
                        ESMType = "BSI",
                        ESMCode = 5
                    }
                }
            };

            employmentStatuses.Add(empStatusOne);
            employmentStatuses.Add(empStatusTwo);

            var learningDeliveryFAMs = new List<ILearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "LDM",
                    LearnDelFAMCode = "318"
                }
            };

            var empStatQSMock = new Mock<ILearnerEmploymentStatusQueryService>();
            empStatQSMock.Setup(x => x.LearnerEmploymentStatusesForDate(employmentStatuses, learnStartDate)).Returns(employmentStatuses);

            var empStatusQSMock = new Mock<ILearnerEmploymentStatusMonitoringQueryService>();
            empStatusQSMock.Setup(x => x.HasAnyEmploymentStatusMonitoringTypeAndCodeForEmploymentStatus(empStatusOne, "BSI", 5)).Returns(true);
            empStatusQSMock.Setup(x => x.HasAnyEmploymentStatusMonitoringTypeAndCodeForEmploymentStatus(empStatusOne, "BSI", 4)).Returns(true);

            var learningDeliveryFAMQSMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFAMQSMock.Setup(x => x.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, "LDM", "318")).Returns(false);

            DD(empStatQSMock.Object, empStatusQSMock.Object, learningDeliveryFAMQSMock.Object).Derive(35, learnStartDate, employmentStatuses, learningDeliveryFAMs).Should().BeTrue();
        }

        [Fact]
        public void Derive_False_FundModel()
        {
            var learnStartDate = new DateTime(2018, 9, 1);
            var employmentStatuses = new List<ILearnerEmploymentStatus>();
            var empStatus = new TestLearnerEmploymentStatus
            {
                EmpStat = 10,
                DateEmpStatApp = new DateTime(2018, 9, 1),
                EmploymentStatusMonitorings = new List<TestEmploymentStatusMonitoring>
                {
                    new TestEmploymentStatusMonitoring
                    {
                        ESMType = "BSI",
                        ESMCode = 5
                    }
                }
            };

            employmentStatuses.Add(empStatus);

            var learningDeliveryFAMs = new List<ILearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "LDM",
                    LearnDelFAMCode = "318"
                }
            };

            var empStatQSMock = new Mock<ILearnerEmploymentStatusQueryService>();
            empStatQSMock.Setup(x => x.LearnerEmploymentStatusesForDate(employmentStatuses, learnStartDate)).Returns(employmentStatuses);

            var empStatusQSMock = new Mock<ILearnerEmploymentStatusMonitoringQueryService>();
            empStatusQSMock.Setup(x => x.HasAnyEmploymentStatusMonitoringTypeAndCodeForEmploymentStatus(empStatus, "BSI", 5)).Returns(true);
            empStatusQSMock.Setup(x => x.HasAnyEmploymentStatusMonitoringTypeAndCodeForEmploymentStatus(empStatus, "BSI", 4)).Returns(true);

            var learningDeliveryFAMQSMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFAMQSMock.Setup(x => x.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, "LDM", "318")).Returns(false);

            DD(empStatQSMock.Object, empStatusQSMock.Object, learningDeliveryFAMQSMock.Object).Derive(25, learnStartDate, employmentStatuses, learningDeliveryFAMs).Should().BeFalse();
        }

        [Fact]
        public void Derive_False_NullEmploymentStatuses()
        {
            var learnStartDate = new DateTime(2018, 9, 1);

            var learningDeliveryFAMs = new List<ILearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "LDM",
                    LearnDelFAMCode = "318"
                }
            };

            DD().Derive(35, learnStartDate, null, learningDeliveryFAMs).Should().BeFalse();
        }

        [Fact]
        public void Derive_False_EmploymentStatusesDateMismatch()
        {
            var learnStartDate = new DateTime(2018, 9, 1);
            var employmentStatuses = new List<ILearnerEmploymentStatus>();
            var empStatus = new TestLearnerEmploymentStatus
            {
                EmpStat = 10,
                DateEmpStatApp = new DateTime(2018, 10, 1),
                EmploymentStatusMonitorings = new List<TestEmploymentStatusMonitoring>
                {
                    new TestEmploymentStatusMonitoring
                    {
                        ESMType = "BSI",
                        ESMCode = 1
                    }
                }
            };

            employmentStatuses.Add(empStatus);

            var learningDeliveryFAMs = new List<ILearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "LDM",
                    LearnDelFAMCode = "318"
                }
            };

            var empStatQSMock = new Mock<ILearnerEmploymentStatusQueryService>();
            empStatQSMock.Setup(x => x.LearnerEmploymentStatusesForDate(employmentStatuses, learnStartDate)).Returns(Enumerable.Empty<ILearnerEmploymentStatus>());

            var learningDeliveryFAMQSMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFAMQSMock.Setup(x => x.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, "LDM", "318")).Returns(false);

            DD(empStatQSMock.Object, learningDeliveryFAMQueryService: learningDeliveryFAMQSMock.Object).Derive(35, learnStartDate, employmentStatuses, learningDeliveryFAMs).Should().BeFalse();
        }

        [Fact]
        public void Derive_False_EmpStat()
        {
            var learnStartDate = new DateTime(2018, 9, 1);
            var employmentStatuses = new List<ILearnerEmploymentStatus>();
            var empStatus = new TestLearnerEmploymentStatus
            {
                EmpStat = 100,
                DateEmpStatApp = new DateTime(2018, 9, 1),
                EmploymentStatusMonitorings = new List<TestEmploymentStatusMonitoring>
                {
                    new TestEmploymentStatusMonitoring
                    {
                        ESMType = "BSI",
                        ESMCode = 1
                    }
                }
            };

            employmentStatuses.Add(empStatus);

            var learningDeliveryFAMs = new List<ILearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "LDM",
                    LearnDelFAMCode = "318"
                }
            };

            var empStatQSMock = new Mock<ILearnerEmploymentStatusQueryService>();
            empStatQSMock.Setup(x => x.LearnerEmploymentStatusesForDate(employmentStatuses, learnStartDate)).Returns(employmentStatuses);

            var empStatusQSMock = new Mock<ILearnerEmploymentStatusMonitoringQueryService>();
            empStatusQSMock.Setup(x => x.HasAnyEmploymentStatusMonitoringTypeAndCodeForEmploymentStatus(empStatus, "BSI", 5)).Returns(true);
            empStatusQSMock.Setup(x => x.HasAnyEmploymentStatusMonitoringTypeAndCodeForEmploymentStatus(empStatus, "BSI", 4)).Returns(true);

            var learningDeliveryFAMQSMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFAMQSMock.Setup(x => x.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, "LDM", "318")).Returns(false);

            DD(empStatQSMock.Object, empStatusQSMock.Object, learningDeliveryFAMQSMock.Object).Derive(35, learnStartDate, employmentStatuses, learningDeliveryFAMs).Should().BeFalse();
        }

        [Fact]
        public void Derive_False()
        {
            var learnStartDate = new DateTime(2018, 9, 1);
            var employmentStatuses = new List<ILearnerEmploymentStatus>();
            var empStatus = new TestLearnerEmploymentStatus
            {
                EmpStat = 12,
                DateEmpStatApp = new DateTime(2018, 9, 1),
                EmploymentStatusMonitorings = new List<TestEmploymentStatusMonitoring>
                {
                    new TestEmploymentStatusMonitoring
                    {
                        ESMType = "BSI",
                        ESMCode = 4
                    }
                }
            };

            employmentStatuses.Add(empStatus);

            var learningDeliveryFAMs = new List<ILearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "LDM",
                    LearnDelFAMCode = "318"
                }
            };

            var empStatQSMock = new Mock<ILearnerEmploymentStatusQueryService>();
            empStatQSMock.Setup(x => x.LearnerEmploymentStatusesForDate(employmentStatuses, learnStartDate)).Returns(employmentStatuses);

            var empStatusQSMock = new Mock<ILearnerEmploymentStatusMonitoringQueryService>();
            empStatusQSMock.Setup(x => x.HasAnyEmploymentStatusMonitoringTypeAndCodeForEmploymentStatus(empStatus, "BSI", 5)).Returns(false);
            empStatusQSMock.Setup(x => x.HasAnyEmploymentStatusMonitoringTypeAndCodeForEmploymentStatus(empStatus, "BSI", 4)).Returns(true);

            var learningDeliveryFAMQSMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFAMQSMock.Setup(x => x.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, "LDM", "318")).Returns(true);

            DD(empStatQSMock.Object, empStatusQSMock.Object, learningDeliveryFAMQSMock.Object).Derive(35, learnStartDate, employmentStatuses, learningDeliveryFAMs).Should().BeFalse();
        }

        private DerivedData_37Rule DD(
            ILearnerEmploymentStatusQueryService learnerEmploymentStatusQueryService = null,
            ILearnerEmploymentStatusMonitoringQueryService learnerEmploymentStatusMonitoringQueryService = null,
            ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService = null)
        {
            return new DerivedData_37Rule(learnerEmploymentStatusQueryService, learnerEmploymentStatusMonitoringQueryService, learningDeliveryFAMQueryService);
        }
    }
}
