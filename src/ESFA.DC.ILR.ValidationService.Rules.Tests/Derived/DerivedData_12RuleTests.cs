using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Rules.Derived;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Derived
{
    public class DerivedData_12RuleTests
    {
        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public void IsAdultSkillsFundedOnBenefits_True(int esmCode)
        {
            var learningDelivery = new TestLearningDelivery()
            {
                FundModel = 35,
                LearnStartDate = new DateTime(2017, 1, 2)
            };

            var latestLearnerEmploymentStatus = new TestLearnerEmploymentStatus()
            {
                DateEmpStatApp = new DateTime(2017, 1, 2),
                EmploymentStatusMonitorings = new List<IEmploymentStatusMonitoring>()
                {
                    new TestEmploymentStatusMonitoring()
                    {
                        ESMCode = esmCode,
                        ESMType = "BSI"
                    }
                }
            };
            var learnerEmploymentStatuses = new List<ILearnerEmploymentStatus>()
            {
                latestLearnerEmploymentStatus,
                new TestLearnerEmploymentStatus()
                {
                    DateEmpStatApp = new DateTime(2017, 1, 1),
                    EmploymentStatusMonitorings = new List<IEmploymentStatusMonitoring>()
                    {
                        new TestEmploymentStatusMonitoring()
                        {
                            ESMCode = 23,
                            ESMType = "BSI"
                        }
                    }
                }
            };

            var learnerEmploymentStatusMonitoringQueryServiceMock = new Mock<ILearnerEmploymentStatusMonitoringQueryService>();
            learnerEmploymentStatusMonitoringQueryServiceMock.Setup(x =>
                x.HasAnyEmploymentStatusMonitoringTypeAndCodesForEmploymentStatus(
                    It.IsAny<ILearnerEmploymentStatus>(),
                    "BSI",
                    It.IsAny<IEnumerable<int>>())).Returns(true);

            NewRule(learnerEmploymentStatusMonitoringQueryServiceMock.Object)
                .IsAdultSkillsFundedOnBenefits(learnerEmploymentStatuses, learningDelivery).Should().BeTrue();

            learnerEmploymentStatusMonitoringQueryServiceMock.Verify(
                x => x.HasAnyEmploymentStatusMonitoringTypeAndCodesForEmploymentStatus(
                latestLearnerEmploymentStatus,
                "BSI",
                It.IsAny<IEnumerable<int>>()),
                Times.Once);
        }

        [Fact]
        public void IsAdultSkillsFundedOnBenefits_True_LDM()
        {
            var learningDelivery = new TestLearningDelivery()
            {
                FundModel = 35,
                LearnStartDate = new DateTime(2017, 1, 2),
                LearningDeliveryFAMs = new List<ILearningDeliveryFAM>()
                {
                    new TestLearningDeliveryFAM()
                    {
                        LearnDelFAMCode = "318",
                        LearnDelFAMType = "LDM"
                    },
                    new TestLearningDeliveryFAM()
                    {
                        LearnDelFAMCode = "999",
                        LearnDelFAMType = "LDM"
                    }
                }
            };

            var latestLearnerEmploymentStatus = new TestLearnerEmploymentStatus()
            {
                DateEmpStatApp = new DateTime(2017, 1, 2),
                EmploymentStatusMonitorings = new List<IEmploymentStatusMonitoring>()
                {
                    new TestEmploymentStatusMonitoring()
                    {
                        ESMCode = 4,
                        ESMType = "BSI"
                    }
                }
            };
            var learnerEmploymentStatuses = new List<ILearnerEmploymentStatus>()
            {
                latestLearnerEmploymentStatus,
                new TestLearnerEmploymentStatus()
                {
                    DateEmpStatApp = new DateTime(2017, 1, 1),
                    EmploymentStatusMonitorings = new List<IEmploymentStatusMonitoring>()
                    {
                        new TestEmploymentStatusMonitoring()
                        {
                            ESMCode = 23,
                            ESMType = "BSI"
                        }
                    }
                }
            };

            var learnerEmploymentStatusMonitoringQueryServiceMock = new Mock<ILearnerEmploymentStatusMonitoringQueryService>();
            learnerEmploymentStatusMonitoringQueryServiceMock.Setup(x =>
                x.HasAnyEmploymentStatusMonitoringTypeAndCodesForEmploymentStatus(
                    It.IsAny<ILearnerEmploymentStatus>(),
                    "BSI",
                    It.IsAny<IEnumerable<int>>())).Returns(false);

            learnerEmploymentStatusMonitoringQueryServiceMock.Setup(x =>
                x.HasAnyEmploymentStatusMonitoringTypeAndCodeForEmploymentStatus(
                    It.IsAny<ILearnerEmploymentStatus>(),
                    "BSI",
                    4)).Returns(true);

            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQueryServiceMock.Setup(x =>
                    x.HasLearningDeliveryFAMCodeForType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), "LDM", "318"))
                .Returns(true);

            NewRule(learnerEmploymentStatusMonitoringQueryServiceMock.Object, learningDeliveryFamQueryServiceMock.Object)
                   .IsAdultSkillsFundedOnBenefits(learnerEmploymentStatuses, learningDelivery).Should().BeTrue();

            learnerEmploymentStatusMonitoringQueryServiceMock.Verify(
                x => x.HasAnyEmploymentStatusMonitoringTypeAndCodeForEmploymentStatus(
                latestLearnerEmploymentStatus,
                "BSI",
                4),
                Times.Once);

            learningDeliveryFamQueryServiceMock.Verify(
                x =>
                    x.HasLearningDeliveryFAMCodeForType(learningDelivery.LearningDeliveryFAMs, "LDM", "318"),
                Times.Once);
        }

        [Theory]
        [InlineData("BSI", 999, "LDM", "318")]
        [InlineData("XYZ", 1, "LDM", "318")]
        [InlineData("XYZ", 2, "LDM", "318")]
        [InlineData("BSI", 4, "LDM", "9999")]
        [InlineData("BSI", 4, "XYZ", "318")]
        public void IsAdultSkillsFundedOnBenefits_False(string esmType, int esmCode, string famType, string famCode)
        {
            var learningDelivery = new TestLearningDelivery()
            {
                FundModel = 35,
                LearnStartDate = new DateTime(2017, 1, 2),
                LearningDeliveryFAMs = new List<ILearningDeliveryFAM>()
                {
                    new TestLearningDeliveryFAM()
                    {
                        LearnDelFAMCode = famType,
                        LearnDelFAMType = famCode
                    },
                    new TestLearningDeliveryFAM()
                    {
                        LearnDelFAMCode = "10000",
                        LearnDelFAMType = "LDM"
                    }
                }
            };

            var latestLearnerEmploymentStatus = new TestLearnerEmploymentStatus()
            {
                DateEmpStatApp = new DateTime(2017, 1, 2),
                EmploymentStatusMonitorings = new List<IEmploymentStatusMonitoring>()
                {
                    new TestEmploymentStatusMonitoring()
                    {
                        ESMCode = esmCode,
                        ESMType = esmType
                    }
                }
            };
            var learnerEmploymentStatuses = new List<ILearnerEmploymentStatus>()
            {
                latestLearnerEmploymentStatus,
                new TestLearnerEmploymentStatus()
                {
                    DateEmpStatApp = new DateTime(2017, 1, 1),
                    EmploymentStatusMonitorings = new List<IEmploymentStatusMonitoring>()
                    {
                        new TestEmploymentStatusMonitoring()
                        {
                            ESMCode = 23,
                            ESMType = "BSI"
                        }
                    }
                }
            };

            var learnerEmploymentStatusMonitoringQueryServiceMock = new Mock<ILearnerEmploymentStatusMonitoringQueryService>();
            learnerEmploymentStatusMonitoringQueryServiceMock.Setup(x =>
                x.HasAnyEmploymentStatusMonitoringTypeAndCodesForEmploymentStatus(
                    It.IsAny<ILearnerEmploymentStatus>(),
                    It.IsAny<string>(),
                    It.IsAny<IEnumerable<int>>())).Returns(false);

            learnerEmploymentStatusMonitoringQueryServiceMock.Setup(x =>
                x.HasAnyEmploymentStatusMonitoringTypeAndCodeForEmploymentStatus(
                    It.IsAny<ILearnerEmploymentStatus>(),
                    It.IsAny<string>(),
                    4)).Returns(false);

            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQueryServiceMock.Setup(x =>
                    x.HasLearningDeliveryFAMCodeForType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), "LDM", "318"))
                .Returns(true);

            NewRule(learnerEmploymentStatusMonitoringQueryServiceMock.Object, learningDeliveryFamQueryServiceMock.Object)
                   .IsAdultSkillsFundedOnBenefits(learnerEmploymentStatuses, learningDelivery).Should().BeFalse();
        }

        private DerivedData_12Rule NewRule(
            ILearnerEmploymentStatusMonitoringQueryService learnerEmploymentStatusMonitoringQueryService = null,
            ILearningDeliveryFAMQueryService learningDeliveryFamQueryService = null)
        {
            return new DerivedData_12Rule(learnerEmploymentStatusMonitoringQueryService, learningDeliveryFamQueryService);
        }
    }
}
