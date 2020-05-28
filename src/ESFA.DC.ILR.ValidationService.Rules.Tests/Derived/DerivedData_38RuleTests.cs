using System;
using System.Collections.Generic;
using System.Linq;
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
    public class DerivedData_38RuleTests
    {
        private readonly HashSet<int> _empStatsLookupRange = new HashSet<int>()
        {
            LearnerEmploymentStatusConstants.EmpStats.InPaidEmployment,
            LearnerEmploymentStatusConstants.EmpStats.NotEmployedSeekingAndAvailable,
            LearnerEmploymentStatusConstants.EmpStats.NotEmployedNotSeekingOrNotAvailable,
            LearnerEmploymentStatusConstants.EmpStats.NotKnownProvided
        };

        private readonly HashSet<int> _empStatsConditionTwo = new HashSet<int>()
        {
            LearnerEmploymentStatusConstants.EmpStats.NotEmployedSeekingAndAvailable,
            LearnerEmploymentStatusConstants.EmpStats.NotEmployedNotSeekingOrNotAvailable
        };

        private readonly HashSet<int> _esmCodesConditionTwo = new HashSet<int>()
        {
            LearnerEmploymentStatusConstants.ESMCodes.BSI_ReceiptOfUniversalCredit,
            LearnerEmploymentStatusConstants.ESMCodes.BSI_ReceiptOfEmploymentAndSupport
        };

        private readonly HashSet<int> _esmEIICodesConditionThree = new HashSet<int>()
        {
            LearnerEmploymentStatusConstants.ESMCodes.Retired_EEI_EmployedLessThan16Hours,
            LearnerEmploymentStatusConstants.ESMCodes.EEI_Employed0To10Hours,
            LearnerEmploymentStatusConstants.ESMCodes.EEI_Employed11To20Hours
        };

        private readonly HashSet<int> _esmBSICodesConditionThree = new HashSet<int>()
        {
            LearnerEmploymentStatusConstants.ESMCodes.BSI_ReceiptOfUniversalCredit,
            LearnerEmploymentStatusConstants.ESMCodes.BSI_ReceiptOfEmploymentAndSupport
        };

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

        [Theory]
        [InlineData(4, 2)]
        [InlineData(4, 5)]
        [InlineData(4, 6)]
        [InlineData(5, 2)]
        [InlineData(5, 5)]
        [InlineData(5, 6)]
        public void EmpStatMonitoringConditionThree_True(int bsiCode, int eiiCode)
        {
            var employmentStatus = new TestLearnerEmploymentStatus
            {
                EmpStat = 10,
                EmploymentStatusMonitorings = new List<TestEmploymentStatusMonitoring>
                {
                    new TestEmploymentStatusMonitoring
                    {
                        ESMType = "BSI",
                        ESMCode = bsiCode
                    },
                    new TestEmploymentStatusMonitoring
                    {
                        ESMType = "EII",
                        ESMCode = eiiCode
                    }
                }
            };

            var empStatusQSMock = new Mock<ILearnerEmploymentStatusMonitoringQueryService>();
            empStatusQSMock.Setup(x => x.HasAnyEmploymentStatusMonitoringTypeAndCodesForEmploymentStatus(employmentStatus, "BSI", _esmBSICodesConditionThree)).Returns(true);
            empStatusQSMock.Setup(x => x.HasAnyEmploymentStatusMonitoringTypeAndCodesForEmploymentStatus(employmentStatus, "EII", _esmEIICodesConditionThree)).Returns(true);

            DD(learnerEmploymentStatusMonitoringQueryService: empStatusQSMock.Object).EmpStatMonitoringConditionThree(employmentStatus).Should().BeTrue();
        }

        [Fact]
        public void EmpStatMonitoringConditionThree_False_EmpStat()
        {
            var employmentStatus = new TestLearnerEmploymentStatus
            {
                EmpStat = 11,
                EmploymentStatusMonitorings = new List<TestEmploymentStatusMonitoring>
                {
                    new TestEmploymentStatusMonitoring
                    {
                        ESMType = "BSI",
                        ESMCode = 5
                    },
                    new TestEmploymentStatusMonitoring
                    {
                        ESMType = "EII",
                        ESMCode = 5
                    }
                }
            };

            var empStatusQSMock = new Mock<ILearnerEmploymentStatusMonitoringQueryService>();
            empStatusQSMock.Setup(x => x.HasAnyEmploymentStatusMonitoringTypeAndCodesForEmploymentStatus(employmentStatus, "BSI", _esmBSICodesConditionThree)).Returns(true);
            empStatusQSMock.Setup(x => x.HasAnyEmploymentStatusMonitoringTypeAndCodesForEmploymentStatus(employmentStatus, "EII", _esmEIICodesConditionThree)).Returns(true);

            DD(learnerEmploymentStatusMonitoringQueryService: empStatusQSMock.Object).EmpStatMonitoringConditionThree(employmentStatus).Should().BeFalse();
        }

        [Fact]
        public void EmpStatMonitoringConditionThree_False_SingleESM()
        {
            var employmentStatus = new TestLearnerEmploymentStatus
            {
                EmpStat = 10,
                EmploymentStatusMonitorings = new List<TestEmploymentStatusMonitoring>
                {
                    new TestEmploymentStatusMonitoring
                    {
                        ESMType = "BSI",
                        ESMCode = 5
                    }
                }
            };

            var empStatusQSMock = new Mock<ILearnerEmploymentStatusMonitoringQueryService>();
            empStatusQSMock.Setup(x => x.HasAnyEmploymentStatusMonitoringTypeAndCodesForEmploymentStatus(employmentStatus, "BSI", _esmBSICodesConditionThree)).Returns(true);
            empStatusQSMock.Setup(x => x.HasAnyEmploymentStatusMonitoringTypeAndCodesForEmploymentStatus(employmentStatus, "EII", _esmEIICodesConditionThree)).Returns(false);

            DD(learnerEmploymentStatusMonitoringQueryService: empStatusQSMock.Object).EmpStatMonitoringConditionThree(employmentStatus).Should().BeFalse();
        }

        [Fact]
        public void EmpStatMonitoringConditionThree_False_NoESMs()
        {
            var employmentStatus = new TestLearnerEmploymentStatus
            {
                EmpStat = 10,
            };

            var empStatusQSMock = new Mock<ILearnerEmploymentStatusMonitoringQueryService>();
            empStatusQSMock.Setup(x => x.HasAnyEmploymentStatusMonitoringTypeAndCodesForEmploymentStatus(employmentStatus, "BSI", _esmBSICodesConditionThree)).Returns(false);
            empStatusQSMock.Setup(x => x.HasAnyEmploymentStatusMonitoringTypeAndCodesForEmploymentStatus(employmentStatus, "EII", _esmEIICodesConditionThree)).Returns(false);

            DD(learnerEmploymentStatusMonitoringQueryService: empStatusQSMock.Object).EmpStatMonitoringConditionThree(employmentStatus).Should().BeFalse();
        }

        [Theory]
        [InlineData(11, 5, 5, true, true)]
        [InlineData(10, 99, 5, false, true)]
        [InlineData(10, 5, 99, true, false)]
        [InlineData(10, 99, 99, false, false)]
        [InlineData(11, 99, 99, false, false)]
        public void EmpStatMonitoringConditionThree_False_InvalidCombinations(int empStat, int bsiCode, int eiiCode, bool mockOne, bool mockTwo)
        {
            var employmentStatus = new TestLearnerEmploymentStatus
            {
                EmpStat = empStat,
                EmploymentStatusMonitorings = new List<TestEmploymentStatusMonitoring>
                {
                    new TestEmploymentStatusMonitoring
                    {
                        ESMType = "BSI",
                        ESMCode = bsiCode
                    },
                    new TestEmploymentStatusMonitoring
                    {
                        ESMType = "EII",
                        ESMCode = eiiCode
                    }
                }
            };

            var empStatusQSMock = new Mock<ILearnerEmploymentStatusMonitoringQueryService>();
            empStatusQSMock.Setup(x => x.HasAnyEmploymentStatusMonitoringTypeAndCodesForEmploymentStatus(employmentStatus, "BSI", _esmBSICodesConditionThree)).Returns(mockOne);
            empStatusQSMock.Setup(x => x.HasAnyEmploymentStatusMonitoringTypeAndCodesForEmploymentStatus(employmentStatus, "EII", _esmEIICodesConditionThree)).Returns(mockTwo);

            DD(learnerEmploymentStatusMonitoringQueryService: empStatusQSMock.Object).EmpStatMonitoringConditionThree(employmentStatus).Should().BeFalse();
        }

        [Theory]
        [InlineData(11, 4)]
        [InlineData(11, 5)]
        [InlineData(12, 4)]
        [InlineData(12, 5)]
        public void EmpStatMonitoringConditionTwo_True(int empStat, int bsiCode)
        {
            var employmentStatus = new TestLearnerEmploymentStatus
            {
                EmpStat = empStat,
                EmploymentStatusMonitorings = new List<TestEmploymentStatusMonitoring>
                {
                    new TestEmploymentStatusMonitoring
                    {
                        ESMType = "BSI",
                        ESMCode = bsiCode
                    }
                }
            };

            var empStatusQSMock = new Mock<ILearnerEmploymentStatusMonitoringQueryService>();
            empStatusQSMock.Setup(x => x.HasAnyEmploymentStatusMonitoringTypeAndCodesForEmploymentStatus(employmentStatus, "BSI", _esmCodesConditionTwo)).Returns(true);

            DD(learnerEmploymentStatusMonitoringQueryService: empStatusQSMock.Object).EmpStatMonitoringConditionTwo(employmentStatus).Should().BeTrue();
        }

        [Fact]
        public void EmpStatMonitoringConditionTwo_False_EmpStat()
        {
            var employmentStatus = new TestLearnerEmploymentStatus
            {
                EmpStat = 10,
                EmploymentStatusMonitorings = new List<TestEmploymentStatusMonitoring>
                {
                    new TestEmploymentStatusMonitoring
                    {
                        ESMType = "BSI",
                        ESMCode = 5
                    }
                }
            };

            var empStatusQSMock = new Mock<ILearnerEmploymentStatusMonitoringQueryService>();
            empStatusQSMock.Setup(x => x.HasAnyEmploymentStatusMonitoringTypeAndCodesForEmploymentStatus(employmentStatus, "BSI", _esmCodesConditionTwo)).Returns(true);

            DD(learnerEmploymentStatusMonitoringQueryService: empStatusQSMock.Object).EmpStatMonitoringConditionTwo(employmentStatus).Should().BeFalse();
        }

        [Fact]
        public void EmpStatMonitoringConditionTwo_False_NoESMs()
        {
            var employmentStatus = new TestLearnerEmploymentStatus
            {
                EmpStat = 11,
            };

            var empStatusQSMock = new Mock<ILearnerEmploymentStatusMonitoringQueryService>();
            empStatusQSMock.Setup(x => x.HasAnyEmploymentStatusMonitoringTypeAndCodesForEmploymentStatus(employmentStatus, "BSI", _esmCodesConditionTwo)).Returns(false);

            DD(learnerEmploymentStatusMonitoringQueryService: empStatusQSMock.Object).EmpStatMonitoringConditionTwo(employmentStatus).Should().BeFalse();
        }

        [Theory]
        [InlineData(10, 5, true)]
        [InlineData(11, 2, false)]
        [InlineData(12, 2, false)]
        public void EmpStatMonitoringConditionTwo_False_InvalidCombinations(int empStat, int bsiCode, bool mock)
        {
            var employmentStatus = new TestLearnerEmploymentStatus
            {
                EmpStat = empStat,
                EmploymentStatusMonitorings = new List<TestEmploymentStatusMonitoring>
                {
                    new TestEmploymentStatusMonitoring
                    {
                        ESMType = "BSI",
                        ESMCode = bsiCode
                    }
                }
            };

            var empStatusQSMock = new Mock<ILearnerEmploymentStatusMonitoringQueryService>();
            empStatusQSMock.Setup(x => x.HasAnyEmploymentStatusMonitoringTypeAndCodesForEmploymentStatus(employmentStatus, "BSI", _esmBSICodesConditionThree)).Returns(mock);

            DD(learnerEmploymentStatusMonitoringQueryService: empStatusQSMock.Object).EmpStatMonitoringConditionThree(employmentStatus).Should().BeFalse();
        }

        [Fact]
        public void EmpStatMonitoringConditionOne_True()
        {
            var employmentStatus = new TestLearnerEmploymentStatus
            {
                EmpStat = 10,
                EmploymentStatusMonitorings = new List<TestEmploymentStatusMonitoring>
                {
                    new TestEmploymentStatusMonitoring
                    {
                        ESMType = "BSI",
                        ESMCode = 1
                    }
                }
            };

            var empStatusQSMock = new Mock<ILearnerEmploymentStatusMonitoringQueryService>();
            empStatusQSMock.Setup(x => x.HasAnyEmploymentStatusMonitoringTypeAndCodeForEmploymentStatus(employmentStatus, "BSI", 1)).Returns(true);

            DD(learnerEmploymentStatusMonitoringQueryService: empStatusQSMock.Object).EmpStatMonitoringConditionOne(employmentStatus).Should().BeTrue();
        }

        [Fact]
        public void EmpStatMonitoringConditionOne_False_NoESMs()
        {
            var employmentStatus = new TestLearnerEmploymentStatus
            {
                EmpStat = 11,
            };

            var empStatusQSMock = new Mock<ILearnerEmploymentStatusMonitoringQueryService>();
            empStatusQSMock.Setup(x => x.HasAnyEmploymentStatusMonitoringTypeAndCodeForEmploymentStatus(employmentStatus, "BSI", 1)).Returns(false);

            DD(learnerEmploymentStatusMonitoringQueryService: empStatusQSMock.Object).EmpStatMonitoringConditionOne(employmentStatus).Should().BeFalse();
        }

        [Fact]
        public void EmpStatMonitoringConditionTwo_False()
        {
            var employmentStatus = new TestLearnerEmploymentStatus
            {
                EmpStat = 10,
                EmploymentStatusMonitorings = new List<TestEmploymentStatusMonitoring>
                {
                    new TestEmploymentStatusMonitoring
                    {
                        ESMType = "BSI",
                        ESMCode = 2
                    }
                }
            };

            var empStatusQSMock = new Mock<ILearnerEmploymentStatusMonitoringQueryService>();
            empStatusQSMock.Setup(x => x.HasAnyEmploymentStatusMonitoringTypeAndCodeForEmploymentStatus(employmentStatus, "BSI", 1)).Returns(false);

            DD(learnerEmploymentStatusMonitoringQueryService: empStatusQSMock.Object).EmpStatMonitoringConditionOne(employmentStatus).Should().BeFalse();
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

        [Fact]
        public void EmpStatusCondition_True()
        {
            var empStatus = new TestLearnerEmploymentStatus
            {
                EmpStat = 10,
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

            var empStatMonQSMock = new Mock<ILearnerEmploymentStatusMonitoringQueryService>();
            empStatMonQSMock.Setup(x => x.HasAnyEmploymentStatusMonitoringTypeAndCodeForEmploymentStatus(empStatus, "BSI", 1)).Returns(true);

            DD(learnerEmploymentStatusMonitoringQueryService: empStatMonQSMock.Object).EmpStatusCondition(empStatus).Should().BeTrue();
        }

        [Fact]
        public void EmpStatusCondition_False_EmpStat()
        {
            var empStatus = new TestLearnerEmploymentStatus
            {
                EmpStat = 100,
                DateEmpStatApp = new DateTime(2018, 8, 1),
                EmploymentStatusMonitorings = new List<TestEmploymentStatusMonitoring>
                {
                    new TestEmploymentStatusMonitoring
                    {
                        ESMType = "BSI",
                        ESMCode = 1
                    }
                }
            };

            var empStatMonQSMock = new Mock<ILearnerEmploymentStatusMonitoringQueryService>();
            empStatMonQSMock.Setup(x => x.HasAnyEmploymentStatusMonitoringTypeAndCodeForEmploymentStatus(empStatus, "BSI", 1)).Returns(true);

            DD(learnerEmploymentStatusMonitoringQueryService: empStatMonQSMock.Object).EmpStatusCondition(empStatus).Should().BeFalse();
        }

        [Fact]
        public void EmpStatusCondition_False_ESMMisMatch()
        {
            var empStatus = new TestLearnerEmploymentStatus
            {
                EmpStat = 100,
                DateEmpStatApp = new DateTime(2018, 9, 1),
                EmploymentStatusMonitorings = new List<TestEmploymentStatusMonitoring>
                {
                    new TestEmploymentStatusMonitoring
                    {
                        ESMType = "BSI",
                        ESMCode = 2
                    }
                }
            };

            var empStatMonQSMock = new Mock<ILearnerEmploymentStatusMonitoringQueryService>();
            empStatMonQSMock.Setup(x => x.HasAnyEmploymentStatusMonitoringTypeAndCodeForEmploymentStatus(empStatus, "BSI", 1)).Returns(false);

            DD(learnerEmploymentStatusMonitoringQueryService: empStatMonQSMock.Object).EmpStatusCondition(empStatus).Should().BeFalse();
        }

        [Fact]
        public void Derive_True()
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
                        ESMCode = 1
                    }
                }
            };

            employmentStatuses.Add(empStatus);

            var empStatQSMock = new Mock<ILearnerEmploymentStatusQueryService>();
            empStatQSMock.Setup(x => x.LearnerEmploymentStatusesForDate(employmentStatuses, learnStartDate)).Returns(employmentStatuses);

            var empStatMonQSMock = new Mock<ILearnerEmploymentStatusMonitoringQueryService>();
            empStatMonQSMock.Setup(x => x.HasAnyEmploymentStatusMonitoringTypeAndCodeForEmploymentStatus(empStatus, "BSI", 1)).Returns(true);

            DD(empStatQSMock.Object, empStatMonQSMock.Object).Derive(35, learnStartDate, employmentStatuses).Should().BeTrue();
        }

        [Fact]
        public void Derive_True_Multiple()
        {
            var learnStartDate = new DateTime(2018, 9, 1);
            var employmentStatuses = new List<ILearnerEmploymentStatus>();
            var empStatusOne = new TestLearnerEmploymentStatus
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

            var empStatusTwo = new TestLearnerEmploymentStatus
            {
                EmpStat = 10,
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

            employmentStatuses.Add(empStatusOne);
            employmentStatuses.Add(empStatusTwo);

            var empStatQSMock = new Mock<ILearnerEmploymentStatusQueryService>();
            empStatQSMock.Setup(x => x.LearnerEmploymentStatusesForDate(employmentStatuses, learnStartDate)).Returns(employmentStatuses);

            var empStatMonQSMock = new Mock<ILearnerEmploymentStatusMonitoringQueryService>();
            empStatMonQSMock.Setup(x => x.HasAnyEmploymentStatusMonitoringTypeAndCodeForEmploymentStatus(empStatusTwo, "BSI", 1)).Returns(true);

            DD(empStatQSMock.Object, empStatMonQSMock.Object).Derive(35, learnStartDate, employmentStatuses).Should().BeTrue();
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
                        ESMCode = 1
                    }
                }
            };

            employmentStatuses.Add(empStatus);

            var empStatQSMock = new Mock<ILearnerEmploymentStatusQueryService>();
            empStatQSMock.Setup(x => x.LearnerEmploymentStatusesForDate(employmentStatuses, learnStartDate)).Returns(employmentStatuses);

            var empStatMonQSMock = new Mock<ILearnerEmploymentStatusMonitoringQueryService>();
            empStatMonQSMock.Setup(x => x.HasAnyEmploymentStatusMonitoringTypeAndCodeForEmploymentStatus(empStatus, "BSI", 1)).Returns(true);

            DD(empStatQSMock.Object, empStatMonQSMock.Object).Derive(25, learnStartDate, employmentStatuses).Should().BeFalse();
        }

        [Fact]
        public void Derive_False_NullEmploymentStatuses()
        {
            var learnStartDate = new DateTime(2018, 9, 1);

            DD().Derive(35, learnStartDate, null).Should().BeFalse();
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

            var empStatQSMock = new Mock<ILearnerEmploymentStatusQueryService>();
            empStatQSMock.Setup(x => x.LearnerEmploymentStatusesForDate(employmentStatuses, learnStartDate)).Returns(Enumerable.Empty<ILearnerEmploymentStatus>());

            DD(empStatQSMock.Object).Derive(35, learnStartDate, employmentStatuses).Should().BeFalse();
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

            var empStatQSMock = new Mock<ILearnerEmploymentStatusQueryService>();
            empStatQSMock.Setup(x => x.LearnerEmploymentStatusesForDate(employmentStatuses, learnStartDate)).Returns(employmentStatuses);

            DD(empStatQSMock.Object).Derive(35, learnStartDate, employmentStatuses).Should().BeFalse();
        }

        [Fact]
        public void Derive_False()
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
                        ESMCode = 1
                    }
                }
            };

            employmentStatuses.Add(empStatus);

            var empStatQSMock = new Mock<ILearnerEmploymentStatusQueryService>();
            empStatQSMock.Setup(x => x.LearnerEmploymentStatusesForDate(employmentStatuses, learnStartDate)).Returns(employmentStatuses);

            var empStatMonQSMock = new Mock<ILearnerEmploymentStatusMonitoringQueryService>();
            empStatMonQSMock.Setup(x => x.HasAnyEmploymentStatusMonitoringTypeAndCodeForEmploymentStatus(empStatus, "BSI", 1)).Returns(false);
            empStatMonQSMock.Setup(x => x.HasAnyEmploymentStatusMonitoringTypeAndCodesForEmploymentStatus(empStatus, "BSI", _esmCodesConditionTwo)).Returns(false);
            empStatMonQSMock.Setup(x => x.HasAnyEmploymentStatusMonitoringTypeAndCodesForEmploymentStatus(empStatus, "BSI", _esmBSICodesConditionThree)).Returns(false);
            empStatMonQSMock.Setup(x => x.HasAnyEmploymentStatusMonitoringTypeAndCodesForEmploymentStatus(empStatus, "EII", _esmEIICodesConditionThree)).Returns(false);

            DD(empStatQSMock.Object, empStatMonQSMock.Object).Derive(35, learnStartDate, employmentStatuses).Should().BeFalse();
        }

        private DerivedData_38Rule DD(ILearnerEmploymentStatusQueryService learnerEmploymentStatusQueryService = null, ILearnerEmploymentStatusMonitoringQueryService learnerEmploymentStatusMonitoringQueryService = null)
        {
            return new DerivedData_38Rule(learnerEmploymentStatusQueryService, learnerEmploymentStatusMonitoringQueryService);
        }
    }
}
