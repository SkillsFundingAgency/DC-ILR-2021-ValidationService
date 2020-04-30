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
    public class EmpStat_12RuleTests : AbstractRuleTests<EmpStat_12Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("EmpStat_12");
        }

        [Fact]
        public void AimTypeConditionMet_True()
        {
            NewRule().AimTypeConditionMet(1).Should().BeTrue();
        }

        [Fact]
        public void AimTypeConditionMet_False()
        {
            NewRule().AimTypeConditionMet(2).Should().BeFalse();
        }

        [Fact]
        public void DD07ConditionMet_True()
        {
            var progType = 24;

            var dd07Mock = new Mock<IDerivedData_07Rule>();

            dd07Mock.Setup(dd => dd.IsApprenticeship(progType)).Returns(true);

            NewRule(dd07: dd07Mock.Object).DD07ConditionMet(progType).Should().BeTrue();
        }

        [Fact]
        public void DD07ConditionMet_False()
        {
            var progType = 25;

            var dd07Mock = new Mock<IDerivedData_07Rule>();

            dd07Mock.Setup(dd => dd.IsApprenticeship(progType)).Returns(false);

            NewRule(dd07: dd07Mock.Object).DD07ConditionMet(progType).Should().BeFalse();
        }

        [Fact]
        public void DD07ConditionMet_False_Null()
        {
            int? progType = null;

            var dd07Mock = new Mock<IDerivedData_07Rule>();

            dd07Mock.Setup(dd => dd.IsApprenticeship(progType)).Returns(false);

            NewRule(dd07: dd07Mock.Object).DD07ConditionMet(progType).Should().BeFalse();
        }

        [Fact]
        public void EmpStatConditionMet_True()
        {
            var learnerEmploymentStatus = new TestLearnerEmploymentStatus
            {
                EmpStat = EmploymentStatusEmpStats.NotEmployedSeekingAndAvailable,
                DateEmpStatApp = new DateTime(2018, 8, 1)
            };

            var learnStartDate = new DateTime(2018, 8, 1);

            var learnerEmploymentStatuses = new List<TestLearnerEmploymentStatus>
            {
                learnerEmploymentStatus
            };

            var learnerEmploymentStatusQueryServiceMock = new Mock<ILearnerEmploymentStatusQueryService>();

            learnerEmploymentStatusQueryServiceMock.Setup(qs => qs.LearnerEmploymentStatusForDate(learnerEmploymentStatuses, learnStartDate)).Returns(learnerEmploymentStatus);

            NewRule(learnerEmploymentStatusQueryServiceMock.Object).EmpStatConditionMet(learnStartDate, learnerEmploymentStatuses).Should().BeTrue();
        }

        [Fact]
        public void EmpStatConditionMet_False_EmpStat()
        {
            var learnStartDate = new DateTime(2018, 8, 1);

            var learnerEmploymentStatus = new TestLearnerEmploymentStatus
            {
                EmpStat = EmploymentStatusEmpStats.InPaidEmployment,
                DateEmpStatApp = new DateTime(2018, 8, 1)
            };

            var learnerEmploymentStatuses = new List<TestLearnerEmploymentStatus>
            {
                learnerEmploymentStatus
            };

            var learnerEmploymentStatusQueryServiceMock = new Mock<ILearnerEmploymentStatusQueryService>();

            learnerEmploymentStatusQueryServiceMock.Setup(qs => qs.LearnerEmploymentStatusForDate(learnerEmploymentStatuses, learnStartDate)).Returns(learnerEmploymentStatus);

            NewRule(learnerEmploymentStatusQueryServiceMock.Object).EmpStatConditionMet(learnStartDate, learnerEmploymentStatuses).Should().BeFalse();
        }

        [Fact]
        public void EmpStatConditionMet_True_NoDateMatch()
        {
            var learnStartDate = new DateTime(2018, 7, 1);
            var learnerEmploymentStatuses = new List<TestLearnerEmploymentStatus>();

            var learnerEmploymentStatusQueryServiceMock = new Mock<ILearnerEmploymentStatusQueryService>();

            learnerEmploymentStatusQueryServiceMock.Setup(qs => qs.LearnerEmploymentStatusForDate(learnerEmploymentStatuses, learnStartDate)).Returns(null as ILearnerEmploymentStatus);

            NewRule(learnerEmploymentStatusQueryServiceMock.Object).EmpStatConditionMet(learnStartDate, learnerEmploymentStatuses).Should().BeTrue();
        }

        [Fact]
        public void LearningDeliveryFAMConditionMet_True()
        {
            var famCodes = new List<string> { "353", "354", "355" };
            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "LDM",
                    LearnDelFAMCode = "100"
                }
            };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasAnyLearningDeliveryFAMCodesForType(learningDeliveryFAMs, "LDM", famCodes)).Returns(false);

            NewRule(learningDeliveryFAMQueryService: learningDeliveryFAMQueryServiceMock.Object).LearningDeliveryFAMConditionMet(learningDeliveryFAMs).Should().BeTrue();
        }

        [Fact]
        public void LearningDeliveryFAMConditionMet_False()
        {
            var famCodes = new List<string> { "353", "354", "355" };
            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "LDM",
                    LearnDelFAMCode = "353"
                }
            };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasAnyLearningDeliveryFAMCodesForType(learningDeliveryFAMs, "LDM", famCodes)).Returns(true);

            NewRule(learningDeliveryFAMQueryService: learningDeliveryFAMQueryServiceMock.Object).LearningDeliveryFAMConditionMet(learningDeliveryFAMs).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_True()
        {
            var aimType = 1;
            var progType = 24;
            var learnStartDate = new DateTime(2018, 8, 1);

            var learnerEmploymentStatus = new TestLearnerEmploymentStatus
            {
                EmpStat = EmploymentStatusEmpStats.NotEmployedSeekingAndAvailable,
                DateEmpStatApp = new DateTime(2018, 8, 1)
            };

            var learnerEmploymentStatuses = new List<ILearnerEmploymentStatus>
            {
                learnerEmploymentStatus
            };

            var famCodes = new List<string> { "353", "354", "355" };
            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "LDM",
                    LearnDelFAMCode = "100"
                }
            };

            var dd07Mock = new Mock<IDerivedData_07Rule>();
            var learnerEmploymentStatusQueryServiceMock = new Mock<ILearnerEmploymentStatusQueryService>();
            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            dd07Mock.Setup(dd => dd.IsApprenticeship(progType)).Returns(true);
            learnerEmploymentStatusQueryServiceMock.Setup(qs => qs.LearnerEmploymentStatusForDate(learnerEmploymentStatuses, learnStartDate)).Returns(learnerEmploymentStatus);
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasAnyLearningDeliveryFAMCodesForType(learningDeliveryFAMs, "LDM", famCodes)).Returns(false);

            NewRule(learnerEmploymentStatusQueryServiceMock.Object, dd07Mock.Object, learningDeliveryFAMQueryServiceMock.Object)
                .ConditionMet(progType, aimType, learnStartDate, learnerEmploymentStatuses, learningDeliveryFAMs).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False_AimType()
        {
            var aimType = 2;
            var progType = 24;
            var learnStartDate = new DateTime(2018, 8, 1);

            var learnerEmploymentStatus = new TestLearnerEmploymentStatus
            {
                EmpStat = EmploymentStatusEmpStats.NotEmployedSeekingAndAvailable,
                DateEmpStatApp = new DateTime(2018, 8, 1)
            };

            var learnerEmploymentStatuses = new List<TestLearnerEmploymentStatus>
            {
                learnerEmploymentStatus
            };

            var famCodes = new List<string> { "353", "354", "355" };
            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "LDM",
                    LearnDelFAMCode = "100"
                }
            };

            var dd07Mock = new Mock<IDerivedData_07Rule>();
            var learnerEmploymentStatusQueryServiceMock = new Mock<ILearnerEmploymentStatusQueryService>();
            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            dd07Mock.Setup(dd => dd.IsApprenticeship(progType)).Returns(true);
            learnerEmploymentStatusQueryServiceMock.Setup(qs => qs.LearnerEmploymentStatusForDate(learnerEmploymentStatuses, learnStartDate)).Returns(learnerEmploymentStatus);
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasAnyLearningDeliveryFAMCodesForType(learningDeliveryFAMs, "LDM", famCodes)).Returns(false);

            NewRule(learnerEmploymentStatusQueryServiceMock.Object, dd07Mock.Object, learningDeliveryFAMQueryServiceMock.Object)
                .ConditionMet(progType, aimType, learnStartDate, learnerEmploymentStatuses, learningDeliveryFAMs).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_DD07()
        {
            var aimType = 1;
            var progType = 24;
            var learnStartDate = new DateTime(2018, 8, 1);

            var learnerEmploymentStatus = new TestLearnerEmploymentStatus
            {
                EmpStat = EmploymentStatusEmpStats.NotEmployedSeekingAndAvailable,
                DateEmpStatApp = new DateTime(2018, 8, 1)
            };

            var learnerEmploymentStatuses = new List<TestLearnerEmploymentStatus>
            {
                learnerEmploymentStatus
            };

            var famCodes = new List<string> { "353", "354", "355" };
            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "LDM",
                    LearnDelFAMCode = "100"
                }
            };

            var dd07Mock = new Mock<IDerivedData_07Rule>();
            var learnerEmploymentStatusQueryServiceMock = new Mock<ILearnerEmploymentStatusQueryService>();
            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            dd07Mock.Setup(dd => dd.IsApprenticeship(progType)).Returns(false);
            learnerEmploymentStatusQueryServiceMock.Setup(qs => qs.LearnerEmploymentStatusForDate(learnerEmploymentStatuses, learnStartDate)).Returns(learnerEmploymentStatus);
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasAnyLearningDeliveryFAMCodesForType(learningDeliveryFAMs, "LDM", famCodes)).Returns(false);

            NewRule(learnerEmploymentStatusQueryServiceMock.Object, dd07Mock.Object, learningDeliveryFAMQueryServiceMock.Object)
                .ConditionMet(progType, aimType, learnStartDate, learnerEmploymentStatuses, learningDeliveryFAMs).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_True_NoApplicableEmpStat()
        {
            var aimType = 1;
            var progType = 24;
            var learnStartDate = new DateTime(2018, 8, 1);
            var learnerEmploymentStatuses = new List<TestLearnerEmploymentStatus>();

            var famCodes = new List<string> { "353", "354", "355" };
            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "LDM",
                    LearnDelFAMCode = "100"
                }
            };

            var dd07Mock = new Mock<IDerivedData_07Rule>();
            var learnerEmploymentStatusQueryServiceMock = new Mock<ILearnerEmploymentStatusQueryService>();
            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            dd07Mock.Setup(dd => dd.IsApprenticeship(progType)).Returns(true);
            learnerEmploymentStatusQueryServiceMock.Setup(qs => qs.LearnerEmploymentStatusForDate(learnerEmploymentStatuses, learnStartDate)).Returns(null as ILearnerEmploymentStatus);
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasAnyLearningDeliveryFAMCodesForType(learningDeliveryFAMs, "LDM", famCodes)).Returns(false);

            NewRule(learnerEmploymentStatusQueryServiceMock.Object, dd07Mock.Object, learningDeliveryFAMQueryServiceMock.Object)
                .ConditionMet(progType, aimType, learnStartDate, learnerEmploymentStatuses, learningDeliveryFAMs).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False_FAMCodes()
        {
            var aimType = 1;
            var progType = 24;
            var learnStartDate = new DateTime(2018, 8, 1);

            var learnerEmploymentStatus = new TestLearnerEmploymentStatus
            {
                EmpStat = EmploymentStatusEmpStats.NotEmployedSeekingAndAvailable,
                DateEmpStatApp = new DateTime(2018, 8, 1)
            };

            var learnerEmploymentStatuses = new List<TestLearnerEmploymentStatus>
            {
                learnerEmploymentStatus
            };

            var famCodes = new List<string> { "353", "354", "355" };
            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "LDM",
                    LearnDelFAMCode = "353"
                }
            };

            var dd07Mock = new Mock<IDerivedData_07Rule>();
            var learnerEmploymentStatusQueryServiceMock = new Mock<ILearnerEmploymentStatusQueryService>();
            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            dd07Mock.Setup(dd => dd.IsApprenticeship(progType)).Returns(true);
            learnerEmploymentStatusQueryServiceMock.Setup(qs => qs.LearnerEmploymentStatusForDate(learnerEmploymentStatuses, learnStartDate)).Returns(learnerEmploymentStatus);
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasAnyLearningDeliveryFAMCodesForType(learningDeliveryFAMs, "LDM", famCodes)).Returns(true);

            NewRule(learnerEmploymentStatusQueryServiceMock.Object, dd07Mock.Object, learningDeliveryFAMQueryServiceMock.Object)
                .ConditionMet(progType, aimType, learnStartDate, learnerEmploymentStatuses, learningDeliveryFAMs).Should().BeFalse();
        }

        [Fact]
        public void Validate_Error()
        {
            var aimType = 1;
            var progType = 24;
            var learnStartDate = new DateTime(2018, 8, 1);

            var learnerEmploymentStatus = new TestLearnerEmploymentStatus
            {
                EmpStat = EmploymentStatusEmpStats.NotEmployedSeekingAndAvailable,
                DateEmpStatApp = new DateTime(2018, 8, 1)
            };

            var learnerEmploymentStatuses = new List<TestLearnerEmploymentStatus>();

            var famCodes = new List<string> { "353", "354", "355" };
            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "LDM",
                    LearnDelFAMCode = "100"
                }
            };

            var learner = new TestLearner()
            {
                LearnerEmploymentStatuses = learnerEmploymentStatuses,
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        AimType = aimType,
                        ProgTypeNullable = progType,
                        LearnStartDate = learnStartDate,
                        LearningDeliveryFAMs = learningDeliveryFAMs
                    }
                }
            };

            var dd07Mock = new Mock<IDerivedData_07Rule>();
            var learnerEmploymentStatusQueryServiceMock = new Mock<ILearnerEmploymentStatusQueryService>();
            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            dd07Mock.Setup(dd => dd.IsApprenticeship(progType)).Returns(true);
            learnerEmploymentStatusQueryServiceMock.Setup(qs => qs.LearnerEmploymentStatusForDate(learnerEmploymentStatuses, learnStartDate)).Returns(learnerEmploymentStatus);
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasAnyLearningDeliveryFAMCodesForType(learningDeliveryFAMs, "LDM", famCodes)).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(learnerEmploymentStatusQueryServiceMock.Object, dd07Mock.Object, learningDeliveryFAMQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError()
        {
            var aimType = 1;
            var progType = 24;
            var learnStartDate = new DateTime(2018, 8, 1);

            var learnerEmploymentStatus = new TestLearnerEmploymentStatus
            {
                EmpStat = EmploymentStatusEmpStats.NotEmployedSeekingAndAvailable,
                DateEmpStatApp = new DateTime(2018, 8, 1)
            };

            var learnerEmploymentStatuses = new List<TestLearnerEmploymentStatus>();

            var famCodes = new List<string> { "353", "354", "355" };
            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "LDM",
                    LearnDelFAMCode = "353"
                }
            };

            var learner = new TestLearner()
            {
                LearnerEmploymentStatuses = learnerEmploymentStatuses,
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        AimType = aimType,
                        ProgTypeNullable = progType,
                        LearnStartDate = learnStartDate,
                        LearningDeliveryFAMs = learningDeliveryFAMs
                    }
                }
            };

            var dd07Mock = new Mock<IDerivedData_07Rule>();
            var learnerEmploymentStatusQueryServiceMock = new Mock<ILearnerEmploymentStatusQueryService>();
            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            dd07Mock.Setup(dd => dd.IsApprenticeship(progType)).Returns(true);
            learnerEmploymentStatusQueryServiceMock.Setup(qs => qs.LearnerEmploymentStatusForDate(learnerEmploymentStatuses, learnStartDate)).Returns(learnerEmploymentStatus);
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasAnyLearningDeliveryFAMCodesForType(learningDeliveryFAMs, "LDM", famCodes)).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(learnerEmploymentStatusQueryServiceMock.Object, dd07Mock.Object, learningDeliveryFAMQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("LearnStartDate", "01/08/2018")).Verifiable();

            NewRule(null, validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(new DateTime(2018, 8, 1));

            validationErrorHandlerMock.Verify();
        }

        private EmpStat_12Rule NewRule(
            ILearnerEmploymentStatusQueryService learnerEmploymentStatusQueryService = null,
            IDerivedData_07Rule dd07 = null,
            ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService = null,
            IValidationErrorHandler validationErrorHandler = null)
        {
            return new EmpStat_12Rule(learnerEmploymentStatusQueryService, dd07, learningDeliveryFAMQueryService, validationErrorHandler);
        }
    }
}
