using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.EmploymentStatus.EmpStat;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.EmploymentStatus.EmpStat
{
    public class EmpStat_02RuleTests : AbstractRuleTests<EmpStat_02Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("EmpStat_02");
        }

        [Fact]
        public void AimTypeConditionMet_False()
        {
            NewRule().AimTypeConditionMet(2).Should().BeFalse();
        }

        [Fact]
        public void FundModelConditionMet_True()
        {
            NewRule().AimTypeConditionMet(1).Should().BeTrue();
        }

        [Fact]
        public void LearningDeliveryConditionMet_False()
        {
            NewRule().LearnStartDateConditionMet(new DateTime(2014, 09, 01)).Should().BeFalse();
        }

        [Fact]
        public void LearningDeliveryConditionMet_True()
        {
            NewRule().LearnStartDateConditionMet(new DateTime(2014, 07, 31)).Should().BeTrue();
        }

        [Theory]
        [InlineData(20)]
        [InlineData(2)]
        public void ApprenticeshipConditionMet_True_DD07(int? progType)
        {
            var dd07Mock = new Mock<IDerivedData_07Rule>();
            dd07Mock.Setup(dd => dd.IsApprenticeship(progType)).Returns(true);

            NewRule(dd07: dd07Mock.Object).ApprenticeshipConditionMet(progType).Should().BeTrue();
        }

        [Fact]
        public void ApprenticeshipConditionMet_True_ProgType()
        {
            var progType = 24;

            var dd07Mock = new Mock<IDerivedData_07Rule>();
            dd07Mock.Setup(dd => dd.IsApprenticeship(progType)).Returns(false);

            NewRule(dd07: dd07Mock.Object).ApprenticeshipConditionMet(progType).Should().BeTrue();
        }

        [Theory]
        [InlineData(25)]
        [InlineData(null)]
        public void ApprenticeshipConditionMet_False(int? progType)
        {
            var dd07Mock = new Mock<IDerivedData_07Rule>();
            dd07Mock.Setup(dd => dd.IsApprenticeship(progType)).Returns(false);

            NewRule(dd07: dd07Mock.Object).ApprenticeshipConditionMet(progType).Should().BeFalse();
        }

        [Fact]
        public void EmploymentStatusConditionMet_True()
        {
            var learnStartDate = new DateTime(2017, 8, 31);
            var learnerEmploymentStatuses = new List<TestLearnerEmploymentStatus>
            {
                new TestLearnerEmploymentStatus
                {
                    EmpStat = 11,
                    DateEmpStatApp = new DateTime(2018, 8, 1)
                }
            };

            var learnerEmploymentStatusQueryServiceMock = new Mock<ILearnerEmploymentStatusQueryService>();

            learnerEmploymentStatusQueryServiceMock.Setup(qs => qs.EmpStatsNotExistOnOrBeforeDate(learnerEmploymentStatuses, learnStartDate)).Returns(true);

            NewRule(learnerEmploymentStatusQueryService: learnerEmploymentStatusQueryServiceMock.Object).EmploymentStatusConditionMet(learnerEmploymentStatuses, learnStartDate).Should().BeTrue();
        }

        [Fact]
        public void EmploymentStatusConditionMet_True_Null()
        {
            var learnStartDate = new DateTime(2017, 8, 31);

            var learnerEmploymentStatusQueryServiceMock = new Mock<ILearnerEmploymentStatusQueryService>();

            learnerEmploymentStatusQueryServiceMock.Setup(qs => qs.EmpStatsNotExistOnOrBeforeDate(null, learnStartDate)).Returns(true);

            NewRule(learnerEmploymentStatusQueryService: learnerEmploymentStatusQueryServiceMock.Object).EmploymentStatusConditionMet(null, learnStartDate).Should().BeTrue();
        }

        [Fact]
        public void EmploymentStatusConditionMet_False()
        {
            var learnStartDate = new DateTime(2017, 8, 31);
            var learnerEmploymentStatuses = new List<TestLearnerEmploymentStatus>
            {
                new TestLearnerEmploymentStatus
                {
                    EmpStat = 11,
                    DateEmpStatApp = new DateTime(2017, 7, 1)
                }
            };

            var learnerEmploymentStatusQueryServiceMock = new Mock<ILearnerEmploymentStatusQueryService>();

            learnerEmploymentStatusQueryServiceMock.Setup(qs => qs.EmpStatsNotExistOnOrBeforeDate(learnerEmploymentStatuses, learnStartDate)).Returns(false);

            NewRule(learnerEmploymentStatusQueryService: learnerEmploymentStatusQueryServiceMock.Object).EmploymentStatusConditionMet(learnerEmploymentStatuses, learnStartDate).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_AimType()
        {
            var aimType = 2;
            var learnStartDate = new DateTime(2014, 7, 1);
            var progType = 24;
            var learnerEmploymentStatuses = new List<TestLearnerEmploymentStatus>
            {
                new TestLearnerEmploymentStatus
                {
                    EmpStat = 11,
                    DateEmpStatApp = new DateTime(2014, 7, 2)
                }
            };

            var dd07Mock = new Mock<IDerivedData_07Rule>();
            var learnerEmploymentStatusQueryServiceMock = new Mock<ILearnerEmploymentStatusQueryService>();

            dd07Mock.Setup(dd => dd.IsApprenticeship(progType)).Returns(true);
            learnerEmploymentStatusQueryServiceMock.Setup(qs => qs.EmpStatsNotExistOnOrBeforeDate(learnerEmploymentStatuses, learnStartDate)).Returns(true);

            NewRule(
                dd07: dd07Mock.Object,
                learnerEmploymentStatusQueryService: learnerEmploymentStatusQueryServiceMock.Object)
                .ConditionMet(aimType, learnStartDate, progType, learnerEmploymentStatuses).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_ProgType()
        {
            var aimType = 1;
            var learnStartDate = new DateTime(2014, 7, 1);
            var progType = 99;
            var learnerEmploymentStatuses = new List<TestLearnerEmploymentStatus>
            {
                new TestLearnerEmploymentStatus
                {
                    EmpStat = 11,
                    DateEmpStatApp = new DateTime(2014, 7, 2)
                }
            };

            var dd07Mock = new Mock<IDerivedData_07Rule>();
            var learnerEmploymentStatusQueryServiceMock = new Mock<ILearnerEmploymentStatusQueryService>();

            dd07Mock.Setup(dd => dd.IsApprenticeship(progType)).Returns(false);
            learnerEmploymentStatusQueryServiceMock.Setup(qs => qs.EmpStatsNotExistOnOrBeforeDate(learnerEmploymentStatuses, learnStartDate)).Returns(true);

            NewRule(
                dd07: dd07Mock.Object,
                learnerEmploymentStatusQueryService: learnerEmploymentStatusQueryServiceMock.Object)
                .ConditionMet(aimType, learnStartDate, progType, learnerEmploymentStatuses).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_LearnStartDate()
        {
            var aimType = 1;
            var learnStartDate = new DateTime(2015, 8, 1);
            var progType = 24;
            var learnerEmploymentStatuses = new List<TestLearnerEmploymentStatus>
            {
                new TestLearnerEmploymentStatus
                {
                    EmpStat = 11,
                    DateEmpStatApp = new DateTime(2014, 7, 1)
                }
            };

            var dd07Mock = new Mock<IDerivedData_07Rule>();
            var learnerEmploymentStatusQueryServiceMock = new Mock<ILearnerEmploymentStatusQueryService>();

            dd07Mock.Setup(dd => dd.IsApprenticeship(progType)).Returns(false);
            learnerEmploymentStatusQueryServiceMock.Setup(qs => qs.EmpStatsNotExistOnOrBeforeDate(learnerEmploymentStatuses, learnStartDate)).Returns(true);

            NewRule(
                dd07: dd07Mock.Object,
                learnerEmploymentStatusQueryService: learnerEmploymentStatusQueryServiceMock.Object)
                .ConditionMet(aimType, learnStartDate, progType, learnerEmploymentStatuses).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_LearnerEmploymentStatus()
        {
            var aimType = 1;
            var learnStartDate = new DateTime(2014, 7, 1);
            var progType = 24;
            var learnerEmploymentStatuses = new List<TestLearnerEmploymentStatus>
            {
                new TestLearnerEmploymentStatus
                {
                    EmpStat = 11,
                    DateEmpStatApp = new DateTime(2014, 7, 1)
                }
            };

            var dd07Mock = new Mock<IDerivedData_07Rule>();
            var learnerEmploymentStatusQueryServiceMock = new Mock<ILearnerEmploymentStatusQueryService>();

            dd07Mock.Setup(dd => dd.IsApprenticeship(progType)).Returns(false);
            learnerEmploymentStatusQueryServiceMock.Setup(qs => qs.EmpStatsNotExistOnOrBeforeDate(learnerEmploymentStatuses, learnStartDate)).Returns(false);

            NewRule(
                dd07: dd07Mock.Object,
                learnerEmploymentStatusQueryService: learnerEmploymentStatusQueryServiceMock.Object)
                .ConditionMet(aimType, learnStartDate, progType, learnerEmploymentStatuses).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_True()
        {
            var aimType = 1;
            var learnStartDate = new DateTime(2014, 7, 1);
            var progType = 24;
            var learnerEmploymentStatuses = new List<TestLearnerEmploymentStatus>
            {
                new TestLearnerEmploymentStatus
                {
                    EmpStat = 11,
                    DateEmpStatApp = new DateTime(2014, 7, 2)
                }
            };

            var dd07Mock = new Mock<IDerivedData_07Rule>();
            var learnerEmploymentStatusQueryServiceMock = new Mock<ILearnerEmploymentStatusQueryService>();

            dd07Mock.Setup(dd => dd.IsApprenticeship(progType)).Returns(false);
            learnerEmploymentStatusQueryServiceMock.Setup(qs => qs.EmpStatsNotExistOnOrBeforeDate(learnerEmploymentStatuses, learnStartDate)).Returns(true);

            NewRule(
                dd07: dd07Mock.Object,
                learnerEmploymentStatusQueryService: learnerEmploymentStatusQueryServiceMock.Object)
                .ConditionMet(aimType, learnStartDate, progType, learnerEmploymentStatuses).Should().BeTrue();
        }

        [Fact]
        public void Validate_Error()
        {
            var aimType = 1;
            var learnStartDate = new DateTime(2014, 7, 1);
            var progType = 24;
            var learnerEmploymentStatuses = new List<TestLearnerEmploymentStatus>
            {
                new TestLearnerEmploymentStatus
                {
                    EmpStat = 11,
                    DateEmpStatApp = new DateTime(2014, 7, 2)
                }
            };

            var dd07Mock = new Mock<IDerivedData_07Rule>();
            var learnerEmploymentStatusQueryServiceMock = new Mock<ILearnerEmploymentStatusQueryService>();

            dd07Mock.Setup(dd => dd.IsApprenticeship(progType)).Returns(false);
            learnerEmploymentStatusQueryServiceMock.Setup(qs => qs.EmpStatsNotExistOnOrBeforeDate(learnerEmploymentStatuses, learnStartDate)).Returns(true);

            ILearner learner = new TestLearner()
            {
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        AimType = aimType,
                        LearnStartDate = learnStartDate,
                        ProgTypeNullable = progType
                    }
                },
                LearnerEmploymentStatuses = learnerEmploymentStatuses
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(
                    dd07: dd07Mock.Object,
                    learnerEmploymentStatusQueryService: learnerEmploymentStatusQueryServiceMock.Object,
                    validationErrorHandler: validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_Error_Null_EmploymentStatus()
        {
            var aimType = 2;
            var learnStartDate = new DateTime(2014, 7, 1);
            var progType = 24;

            IReadOnlyCollection<ILearnerEmploymentStatus> learnerEmploymentStatuses = null;

            var dd07Mock = new Mock<IDerivedData_07Rule>();
            var learnerEmploymentStatusQueryServiceMock = new Mock<ILearnerEmploymentStatusQueryService>();

            dd07Mock.Setup(dd => dd.IsApprenticeship(progType)).Returns(false);
            learnerEmploymentStatusQueryServiceMock.Setup(qs => qs.EmpStatsNotExistOnOrBeforeDate(learnerEmploymentStatuses, learnStartDate)).Returns(true);

            ILearner learner = new TestLearner()
            {
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        AimType = aimType,
                        LearnStartDate = learnStartDate,
                        ProgTypeNullable = progType
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(
                    dd07: dd07Mock.Object,
                    learnerEmploymentStatusQueryService: learnerEmploymentStatusQueryServiceMock.Object,
                    validationErrorHandler: validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError()
        {
            var aimType = 2;
            var learnStartDate = new DateTime(2014, 7, 1);
            var progType = 24;
            var learnerEmploymentStatuses = new List<TestLearnerEmploymentStatus>
            {
                new TestLearnerEmploymentStatus
                {
                    EmpStat = 11,
                    DateEmpStatApp = new DateTime(2014, 7, 2)
                }
            };

            var dd07Mock = new Mock<IDerivedData_07Rule>();
            var learnerEmploymentStatusQueryServiceMock = new Mock<ILearnerEmploymentStatusQueryService>();

            dd07Mock.Setup(dd => dd.IsApprenticeship(progType)).Returns(false);
            learnerEmploymentStatusQueryServiceMock.Setup(qs => qs.EmpStatsNotExistOnOrBeforeDate(learnerEmploymentStatuses, learnStartDate)).Returns(true);

            ILearner learner = new TestLearner()
            {
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        AimType = aimType,
                        LearnStartDate = learnStartDate,
                        ProgTypeNullable = progType
                    }
                },
                LearnerEmploymentStatuses = learnerEmploymentStatuses
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(
                    dd07: dd07Mock.Object,
                    learnerEmploymentStatusQueryService: learnerEmploymentStatusQueryServiceMock.Object,
                    validationErrorHandler: validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();
            validationErrorHandlerMock.Setup(dd => dd.BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, "01/08/2014")).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(new DateTime(2014, 08, 01));

            validationErrorHandlerMock.Verify();
        }

        public EmpStat_02Rule NewRule(
            IDerivedData_07Rule dd07 = null,
            ILearnerEmploymentStatusQueryService learnerEmploymentStatusQueryService = null,
            IValidationErrorHandler validationErrorHandler = null)
        {
            return new EmpStat_02Rule(
                dd07: dd07,
                learnerEmploymentStatusQueryService: learnerEmploymentStatusQueryService,
                validationErrorHandler: validationErrorHandler);
        }
    }
}
