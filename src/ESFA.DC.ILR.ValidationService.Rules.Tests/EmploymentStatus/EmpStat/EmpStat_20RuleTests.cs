using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.EmploymentStatus.EmpStat;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.EmploymentStatus.EmpStat
{
    public class EmpStat_20RuleTests : AbstractRuleTests<EmpStat_20Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("EmpStat_20");
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
        public void EmpStatConditionMet_False()
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
            var famCode = "363";
            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "LDM",
                    LearnDelFAMCode = famCode
                }
            };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, "LDM", famCode)).Returns(true);

            NewRule(learningDeliveryFAMQueryService: learningDeliveryFAMQueryServiceMock.Object).LearningDeliveryFAMConditionMet(learningDeliveryFAMs).Should().BeTrue();
        }

        [Fact]
        public void LearningDeliveryFAMConditionMet_False()
        {
            var famCode = "363";
            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "LDM",
                    LearnDelFAMCode = famCode
                }
            };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, "LDM", famCode)).Returns(false);

            NewRule(learningDeliveryFAMQueryService: learningDeliveryFAMQueryServiceMock.Object).LearningDeliveryFAMConditionMet(learningDeliveryFAMs).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_True()
        {
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

            var famCode = "363";

            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "LDM",
                    LearnDelFAMCode = famCode
                }
            };

            var learnerEmploymentStatusQueryServiceMock = new Mock<ILearnerEmploymentStatusQueryService>();
            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learnerEmploymentStatusQueryServiceMock.Setup(qs => qs.LearnerEmploymentStatusForDate(learnerEmploymentStatuses, learnStartDate)).Returns(learnerEmploymentStatus);
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, "LDM", famCode)).Returns(true);

            NewRule(learnerEmploymentStatusQueryServiceMock.Object, learningDeliveryFAMQueryServiceMock.Object)
                .ConditionMet(learnStartDate, learnerEmploymentStatuses, learningDeliveryFAMs).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False()
        {
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

            var famCode = "363";

            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "LDM",
                    LearnDelFAMCode = famCode
                }
            };

            var learnerEmploymentStatusQueryServiceMock = new Mock<ILearnerEmploymentStatusQueryService>();
            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learnerEmploymentStatusQueryServiceMock.Setup(qs => qs.LearnerEmploymentStatusForDate(learnerEmploymentStatuses, learnStartDate)).Returns(learnerEmploymentStatus);
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, "LDM", famCode)).Returns(false);

            NewRule(learnerEmploymentStatusQueryServiceMock.Object, learningDeliveryFAMQueryServiceMock.Object)
                .ConditionMet(learnStartDate, learnerEmploymentStatuses, learningDeliveryFAMs).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_True_NoApplicableEmpStat()
        {
            var learnStartDate = new DateTime(2018, 8, 1);
            var learnerEmploymentStatuses = new List<TestLearnerEmploymentStatus>();

            var famCode = "363";

            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "LDM",
                    LearnDelFAMCode = famCode
                }
            };

            var learnerEmploymentStatusQueryServiceMock = new Mock<ILearnerEmploymentStatusQueryService>();
            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learnerEmploymentStatusQueryServiceMock.Setup(qs => qs.LearnerEmploymentStatusForDate(learnerEmploymentStatuses, learnStartDate)).Returns(null as ILearnerEmploymentStatus);
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, "LDM", famCode)).Returns(true);

            NewRule(learnerEmploymentStatusQueryServiceMock.Object, learningDeliveryFAMQueryServiceMock.Object)
                .ConditionMet(learnStartDate, learnerEmploymentStatuses, learningDeliveryFAMs).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False_FAMCodes()
        {
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

            var famCode = "363";

            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "LDM",
                    LearnDelFAMCode = famCode
                }
            };

            var learnerEmploymentStatusQueryServiceMock = new Mock<ILearnerEmploymentStatusQueryService>();
            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learnerEmploymentStatusQueryServiceMock.Setup(qs => qs.LearnerEmploymentStatusForDate(learnerEmploymentStatuses, learnStartDate)).Returns(learnerEmploymentStatus);
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, "LDM", famCode)).Returns(false);

            NewRule(learnerEmploymentStatusQueryServiceMock.Object, learningDeliveryFAMQueryServiceMock.Object)
                .ConditionMet(learnStartDate, learnerEmploymentStatuses, learningDeliveryFAMs).Should().BeFalse();
        }

        [Fact]
        public void Validate_Error()
        {
            var learnStartDate = new DateTime(2018, 8, 1);

            var learnerEmploymentStatus = new TestLearnerEmploymentStatus
            {
                EmpStat = EmploymentStatusEmpStats.NotEmployedSeekingAndAvailable,
                DateEmpStatApp = new DateTime(2018, 8, 1)
            };

            var learnerEmploymentStatuses = new List<TestLearnerEmploymentStatus>();

            var famCode = "363";

            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "LDM",
                    LearnDelFAMCode = famCode
                }
            };

            var learner = new TestLearner()
            {
                LearnerEmploymentStatuses = learnerEmploymentStatuses,
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        LearnStartDate = learnStartDate,
                        LearningDeliveryFAMs = learningDeliveryFAMs
                    }
                }
            };

            var learnerEmploymentStatusQueryServiceMock = new Mock<ILearnerEmploymentStatusQueryService>();
            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learnerEmploymentStatusQueryServiceMock.Setup(qs => qs.LearnerEmploymentStatusForDate(learnerEmploymentStatuses, learnStartDate)).Returns(learnerEmploymentStatus);
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, "LDM", famCode)).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(learnerEmploymentStatusQueryServiceMock.Object, learningDeliveryFAMQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_No_Error()
        {
            var learnStartDate = new DateTime(2018, 8, 1);
            var learnerEmploymentStatuses = new List<TestLearnerEmploymentStatus>();
            var famCode = "363";

            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "LDM",
                    LearnDelFAMCode = famCode
                }
            };

            var learner = new TestLearner()
            {
                LearnerEmploymentStatuses = learnerEmploymentStatuses,
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        LearnStartDate = learnStartDate,
                        LearningDeliveryFAMs = learningDeliveryFAMs
                    }
                }
            };

            var learnerEmploymentStatusQueryServiceMock = new Mock<ILearnerEmploymentStatusQueryService>();
            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learnerEmploymentStatusQueryServiceMock.Setup(qs => qs.LearnerEmploymentStatusForDate(learnerEmploymentStatuses, learnStartDate)).Returns((ILearnerEmploymentStatus)null);
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, "LDM", famCode)).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(learnerEmploymentStatusQueryServiceMock.Object, learningDeliveryFAMQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_WithNoLearningDeliveries_Returns_NoError()
        {
            var learner = new TestLearner();
            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandler: validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, "01/08/2018")).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, "LDM")).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMCode, "363")).Verifiable();

            NewRule(null, validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(new DateTime(2018, 8, 1), "LDM", "363");

            validationErrorHandlerMock.Verify();
        }

        private EmpStat_20Rule NewRule(
            ILearnerEmploymentStatusQueryService learnerEmploymentStatusQueryService = null,
            ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService = null,
            IValidationErrorHandler validationErrorHandler = null)
        {
            return new EmpStat_20Rule(learnerEmploymentStatusQueryService, learningDeliveryFAMQueryService, validationErrorHandler);
        }
    }
}
