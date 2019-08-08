using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Rules.Query;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Query
{
    public class LearnerEmploymentStatusMonitoringQueryServiceTests
    {
        [Fact]
        public void HasAnyESMTypeAndCodeForLES_True()
        {
            var learnerEmploymentStatuses = SetupLearnerEmploymentStatuses();

            NewService().HasAnyEmploymentStatusMonitoringTypeAndCodeForLearnerEmploymentStatus(learnerEmploymentStatuses, "EII", 2).Should().BeTrue();
        }

        [Fact]
        public void HasAnyESMCodesForType_False_NullLearnerEmploymentStatuses()
        {
            NewService().HasAnyEmploymentStatusMonitoringTypeAndCodeForLearnerEmploymentStatus(null, "EII", 2).Should().BeFalse();
        }

        [Fact]
        public void HasAnyESMCodesForType_False_NullLearnerEmploymentStatusMonitorings()
        {
            var learnerEmploymentStatuses = new TestLearnerEmploymentStatus[]
            {
                new TestLearnerEmploymentStatus
                {
                    EmploymentStatusMonitorings = null
                }
            };

            NewService().HasAnyEmploymentStatusMonitoringTypeAndCodeForLearnerEmploymentStatus(learnerEmploymentStatuses, "EII", 2).Should().BeFalse();
        }

        [Fact]
        public void HasAnyESMCodesForType_False_ESMTypesMissing()
        {
            var learnerEmploymentStatuses = new TestLearnerEmploymentStatus[]
            {
                new TestLearnerEmploymentStatus
                {
                    EmploymentStatusMonitorings = new TestEmploymentStatusMonitoring[]
                    {
                        new TestEmploymentStatusMonitoring() { ESMCode = 1 }
                    }
                }
            };

            NewService().HasAnyEmploymentStatusMonitoringTypeAndCodeForLearnerEmploymentStatus(learnerEmploymentStatuses, "EII", 2).Should().BeFalse();
        }

        [Fact]
        public void HasAnyESMCodesForType_False_ESMCodeMissing()
        {
            var learnerEmploymentStatuses = new TestLearnerEmploymentStatus[]
            {
                new TestLearnerEmploymentStatus
                {
                    EmploymentStatusMonitorings = new TestEmploymentStatusMonitoring[]
                    {
                        new TestEmploymentStatusMonitoring() { ESMType = "EII" }
                    }
                }
            };

            NewService().HasAnyEmploymentStatusMonitoringTypeAndCodeForLearnerEmploymentStatus(learnerEmploymentStatuses, "EII", 2).Should().BeFalse();
        }

        [Fact]
        public void HasAnyESMTypeAndCodeForLES_False_EsmTypeNull()
        {
            var learnerEmploymentStatuses = SetupLearnerEmploymentStatuses();

            NewService().HasAnyEmploymentStatusMonitoringTypeAndCodeForLearnerEmploymentStatus(learnerEmploymentStatuses, null, 2).Should().BeFalse();
        }

        [Fact]
        public void HasAnyESMTypeAndCodeForLES_False_EsmTypeMismatch()
        {
            var learnerEmploymentStatuses = SetupLearnerEmploymentStatuses();

            NewService().HasAnyEmploymentStatusMonitoringTypeAndCodeForLearnerEmploymentStatus(learnerEmploymentStatuses, "XX", 2).Should().BeFalse();
        }

        [Fact]
        public void HasAnyESMTypeAndCodeForLES_False_EsmCodeMismatch()
        {
            var learnerEmploymentStatuses = SetupLearnerEmploymentStatuses();

            NewService().HasAnyEmploymentStatusMonitoringTypeAndCodeForLearnerEmploymentStatus(learnerEmploymentStatuses, "SEI", 0).Should().BeFalse();
        }

        [Fact]
        public void HasAnyEmploymentStatusMonitoringTypeAndCodeForLearnerEmploymentStatus_True_WithDuplicates()
        {
            var learnerEmploymentStatuses = new TestLearnerEmploymentStatus[]
            {
                new TestLearnerEmploymentStatus
                {
                    EmploymentStatusMonitorings = new TestEmploymentStatusMonitoring[]
                    {
                        new TestEmploymentStatusMonitoring() { ESMType = "EII", ESMCode = 2 },
                        new TestEmploymentStatusMonitoring() { ESMType = "EII", ESMCode = 2 },
                    }
                },
                new TestLearnerEmploymentStatus
                {
                    EmploymentStatusMonitorings = new TestEmploymentStatusMonitoring[]
                    {
                        new TestEmploymentStatusMonitoring() { ESMType = "EII", ESMCode = 2 },
                        new TestEmploymentStatusMonitoring() { ESMType = "EII", ESMCode = 2 },
                    }
                }
            };

            NewService().HasAnyEmploymentStatusMonitoringTypeAndCodeForLearnerEmploymentStatus(learnerEmploymentStatuses, "EII", 2).Should().BeTrue();
        }

        [Theory]
        [InlineData("SEI")]
        [InlineData("eII")]
        [InlineData("LoU")]
        [InlineData("LOE")]
        [InlineData("BsI")]
        [InlineData("PEi")]
        [InlineData("SEm")]
        public void HasAnyEmploymentStatusMonitoringTypeMoreThanOnce_True(string duplicateType)
        {
            var esmTypes = new[] { "SEI", "EII", "LOU", "LOE", "BSI", "PEI", "SEM" };

            var employmentStatusMonitorings = new TestEmploymentStatusMonitoring[]
            {
                new TestEmploymentStatusMonitoring() { ESMType = "SeI" },
                new TestEmploymentStatusMonitoring() { ESMType = "EII" },
                new TestEmploymentStatusMonitoring() { ESMType = "lou" },
                new TestEmploymentStatusMonitoring() { ESMType = "LOE" },
                new TestEmploymentStatusMonitoring() { ESMType = "BSI" },
                new TestEmploymentStatusMonitoring() { ESMType = "PeI" },
                new TestEmploymentStatusMonitoring() { ESMType = "SEM" },
                new TestEmploymentStatusMonitoring() { ESMType = duplicateType }
            };

            NewService().HasAnyEmploymentStatusMonitoringTypeMoreThanOnce(employmentStatusMonitorings, esmTypes).Should().BeTrue();
        }

        [Fact]
        public void HasAnyEmploymentStatusMonitoringTypeMoreThanOnce_False()
        {
            var esmTypes = new[] { "SEI", "EII", "LOU", "LOE", "BSI", "PEI", "SEM" };

            var employmentStatusMonitorings = new TestEmploymentStatusMonitoring[]
            {
                new TestEmploymentStatusMonitoring() { ESMType = "sEI" },
                new TestEmploymentStatusMonitoring() { ESMType = "EII" },
                new TestEmploymentStatusMonitoring() { ESMType = "LoU" },
                new TestEmploymentStatusMonitoring() { ESMType = "LOE" },
                new TestEmploymentStatusMonitoring() { ESMType = "bsI" },
                new TestEmploymentStatusMonitoring() { ESMType = "PEI" },
                new TestEmploymentStatusMonitoring() { ESMType = "SEm" }
            };

            NewService().HasAnyEmploymentStatusMonitoringTypeMoreThanOnce(employmentStatusMonitorings, esmTypes).Should().BeFalse();
        }

        [Fact]
        public void HasAnyEmploymentStatusMonitoringTypeMoreThanOnce_FalseNoTypeMatch()
        {
            var esmTypes = new[] { "seI", "EiI", "LOU", "LOE", "BSI", "pEI", "SEM" };

            var employmentStatusMonitorings = new TestEmploymentStatusMonitoring[]
            {
                new TestEmploymentStatusMonitoring() { ESMType = "XXX" },
                new TestEmploymentStatusMonitoring() { ESMType = "XXX" }
            };

            NewService().HasAnyEmploymentStatusMonitoringTypeMoreThanOnce(employmentStatusMonitorings, esmTypes).Should().BeFalse();
        }

        [Fact]
        public void HasAnyEmploymentStatusMonitoringTypeMoreThanOnce_FalseNull()
        {
            var esmTypes = new[] { "SEI", "ii", "LOU", "LoE", "BSI", "Pei", "SEM" };

            NewService().HasAnyEmploymentStatusMonitoringTypeMoreThanOnce(null, esmTypes).Should().BeFalse();
        }

        [Fact]
        public void GetDuplicatedEmploymentStatusMonitoringTypesForTypes_OneMatch()
        {
            var esmTypes = new[] { "SEI", "eii", "LOU", "LOE", "bSI", "PEI", "sem" };

            var employmentStatusMonitorings = new TestEmploymentStatusMonitoring[]
            {
                new TestEmploymentStatusMonitoring() { ESMType = "SEI" },
                new TestEmploymentStatusMonitoring() { ESMType = "EII" },
                new TestEmploymentStatusMonitoring() { ESMType = "LoU" },
                new TestEmploymentStatusMonitoring() { ESMType = "LOE" },
                new TestEmploymentStatusMonitoring() { ESMType = "BsI" },
                new TestEmploymentStatusMonitoring() { ESMType = "PEI" },
                new TestEmploymentStatusMonitoring() { ESMType = "SEm" },
                new TestEmploymentStatusMonitoring() { ESMType = "SEm" },
            };

            NewService().GetDuplicatedEmploymentStatusMonitoringTypesForTypes(employmentStatusMonitorings, esmTypes).Count().Should().Be(1);
        }

        [Fact]
        public void GetDuplicatedEmploymentStatusMonitoringTypesForTypes_MultipleMatches()
        {
            var esmTypes = new[] { "SEI", "eii", "LOU", "LOE", "bsi", "PEI", "sem" };

            var employmentStatusMonitorings = new TestEmploymentStatusMonitoring[]
            {
                new TestEmploymentStatusMonitoring() { ESMType = "SEI" },
                new TestEmploymentStatusMonitoring() { ESMType = "EiI" },
                new TestEmploymentStatusMonitoring() { ESMType = "lOU" },
                new TestEmploymentStatusMonitoring() { ESMType = "LOE" },
                new TestEmploymentStatusMonitoring() { ESMType = "BsI" },
                new TestEmploymentStatusMonitoring() { ESMType = "BSI" },
                new TestEmploymentStatusMonitoring() { ESMType = "PEI" },
                new TestEmploymentStatusMonitoring() { ESMType = "Pei" },
                new TestEmploymentStatusMonitoring() { ESMType = "sEM" },
                new TestEmploymentStatusMonitoring() { ESMType = "Sem" },
            };

            NewService().GetDuplicatedEmploymentStatusMonitoringTypesForTypes(employmentStatusMonitorings, esmTypes).Count().Should().Be(3);
        }

        [Fact]
        public void GetDuplicatedEmploymentStatusMonitoringTypesForTypes_NoMatch()
        {
            var esmTypes = new[] { "seI", "EiI", "LoU", "LOE", "bsi", "PEI", "Sem" };

            var employmentStatusMonitorings = new TestEmploymentStatusMonitoring[]
            {
                new TestEmploymentStatusMonitoring() { ESMType = "SEI" },
                new TestEmploymentStatusMonitoring() { ESMType = "EII" },
                new TestEmploymentStatusMonitoring() { ESMType = "LOU" },
                new TestEmploymentStatusMonitoring() { ESMType = "LOE" },
                new TestEmploymentStatusMonitoring() { ESMType = "BSI" },
                new TestEmploymentStatusMonitoring() { ESMType = "pei" },
                new TestEmploymentStatusMonitoring() { ESMType = "SEM" },
            };

            NewService().GetDuplicatedEmploymentStatusMonitoringTypesForTypes(employmentStatusMonitorings, esmTypes).Count().Should().Be(0);
        }

        [Fact]
        public void GetDuplicatedEmploymentStatusMonitoringTypesForTypes_Null()
        {
            var esmTypes = new[] { "sei", "EII", "LOU", "loe", "BsI", "PEI", "sEm" };

            NewService().GetDuplicatedEmploymentStatusMonitoringTypesForTypes(null, esmTypes).Should().BeNull();
        }

        [Fact]
        public void HasAnyEmploymentStatusMonitoringTypeAndCodesForEmploymentStatus_False_Null()
        {
            NewService().HasAnyEmploymentStatusMonitoringTypeAndCodesForEmploymentStatus(null, It.IsAny<string>(), It.IsAny<IEnumerable<int>>()).Should().BeFalse();
        }

        [Theory]
        [InlineData("Sei", 1)]
        [InlineData("eII", 2)]
        [InlineData("LOu", 3)]
        [InlineData("lOe", 4)]
        public void HasAnyEmploymentStatusMonitoringTypeAndCodesForEmploymentStatus_True(string esmType, int esmCode)
        {
            var learnerEmploymentStatus = new TestLearnerEmploymentStatus
            {
                EmploymentStatusMonitorings = new TestEmploymentStatusMonitoring[]
                {
                    new TestEmploymentStatusMonitoring() { ESMType = "SeI", ESMCode = 1 },
                    new TestEmploymentStatusMonitoring() { ESMType = "eII", ESMCode = 2 },
                    new TestEmploymentStatusMonitoring() { ESMType = "LOu", ESMCode = 3 },
                    new TestEmploymentStatusMonitoring() { ESMType = "lOe", ESMCode = 4 },
                }
            };

            NewService()
                .HasAnyEmploymentStatusMonitoringTypeAndCodesForEmploymentStatus(
                    learnerEmploymentStatus,
                    esmType,
                    new List<int>() { esmCode, 999 }).Should().BeTrue();
        }

        [Theory]
        [InlineData("Sei", 1)]
        [InlineData("eII", 2)]
        [InlineData("LOu", 3)]
        [InlineData("lOe", 4)]
        public void HasAnyEmploymentStatusMonitoringTypeAndCodesForEmploymentStatus_False(string esmType, int esmCode)
        {
            var learnerEmploymentStatus = new TestLearnerEmploymentStatus
            {
                EmploymentStatusMonitorings = new TestEmploymentStatusMonitoring[]
                {
                    new TestEmploymentStatusMonitoring() { ESMType = "XYZ", ESMCode = 1 },
                }
            };

            NewService()
                .HasAnyEmploymentStatusMonitoringTypeAndCodesForEmploymentStatus(
                    learnerEmploymentStatus,
                    esmType,
                    new List<int>() { esmCode }).Should().BeFalse();
        }

        [Theory]
        [InlineData("Sei", 1)]
        [InlineData("eII", 2)]
        [InlineData("LOu", 3)]
        [InlineData("lOe", 4)]
        public void HasAnyEmploymentStatusMonitoringTypeAndCodeForEmploymentStatus_True(string esmType, int esmCode)
        {
            var learnerEmploymentStatus = new TestLearnerEmploymentStatus
            {
                EmploymentStatusMonitorings = new TestEmploymentStatusMonitoring[]
                {
                    new TestEmploymentStatusMonitoring() { ESMType = "SeI", ESMCode = 1 },
                    new TestEmploymentStatusMonitoring() { ESMType = "eII", ESMCode = 2 },
                    new TestEmploymentStatusMonitoring() { ESMType = "LOu", ESMCode = 3 },
                    new TestEmploymentStatusMonitoring() { ESMType = "lOe", ESMCode = 4 },
                }
            };

            NewService()
                .HasAnyEmploymentStatusMonitoringTypeAndCodeForEmploymentStatus(
                    learnerEmploymentStatus,
                    esmType,
                    esmCode).Should().BeTrue();
        }

        [Theory]
        [InlineData("Sei", 1)]
        [InlineData("eII", 2)]
        [InlineData("LOu", 3)]
        [InlineData("XYZ", 4)]
        public void HasAnyEmploymentStatusMonitoringTypeAndCodeForEmploymentStatus_False(string esmType, int esmCode)
        {
            var learnerEmploymentStatus = new TestLearnerEmploymentStatus
            {
                EmploymentStatusMonitorings = new TestEmploymentStatusMonitoring[]
                {
                    new TestEmploymentStatusMonitoring() { ESMType = "XYZ", ESMCode = 999 },
                }
            };

            NewService()
                .HasAnyEmploymentStatusMonitoringTypeAndCodesForEmploymentStatus(
                    learnerEmploymentStatus,
                    esmType,
                    new List<int>() { esmCode }).Should().BeFalse();
        }

        private ILearnerEmploymentStatus[] SetupLearnerEmploymentStatuses()
        {
            var learnerEmploymentStatuses = new TestLearnerEmploymentStatus[]
            {
                new TestLearnerEmploymentStatus
                {
                    EmploymentStatusMonitorings = new TestEmploymentStatusMonitoring[]
                    {
                        new TestEmploymentStatusMonitoring() { ESMType = "SeI", ESMCode = 1 },
                        new TestEmploymentStatusMonitoring() { ESMType = "eII", ESMCode = 2 },
                        new TestEmploymentStatusMonitoring() { ESMType = "LOu", ESMCode = 3 },
                        new TestEmploymentStatusMonitoring() { ESMType = "lOe", ESMCode = 4 },
                    }
                }
            };

            return learnerEmploymentStatuses;
        }

        private LearnerEmploymentStatusMonitoringQueryService NewService()
        {
            return new LearnerEmploymentStatusMonitoringQueryService();
        }
    }
}
