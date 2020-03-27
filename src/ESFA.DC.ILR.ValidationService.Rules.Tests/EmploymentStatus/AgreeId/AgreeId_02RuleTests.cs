using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.EmploymentStatus.AgreeId;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.EmploymentStatus.AgreeId
{
    public class AgreeId_02RuleTests : AbstractRuleTests<AgreeId_02Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("AgreeId_02");
        }

        [Fact]
        public void AgreeIdExists_True()
        {
            NewRule().AgreeIdExists("AgreeIdValue").Should().BeTrue();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void FundModelConditionMet_True(string agreeId)
        {
            NewRule().AgreeIdExists(agreeId).Should().BeFalse();
        }

        [Fact]
        public void ACTOneDoesNotExistOnOrAfterDateEmpStat_False()
        {
            var dateEmpStat = new DateTime(2018, 8, 1);

            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "ACT",
                    LearnDelFAMCode = "1",
                    LearnDelFAMDateFromNullable = new DateTime(2018, 9, 1)
                }
            };

            NewRule().ACTOneDoesNotExistOnOrAfterDateEmpStat(learningDeliveryFAMs, dateEmpStat).Should().BeFalse();
        }

        [Fact]
        public void ACTOneDoesNotExistOnOrAfterDateEmpStat_True_DateMisMatch()
        {
            var dateEmpStat = new DateTime(2018, 9, 1);

            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "ACT",
                    LearnDelFAMCode = "1",
                    LearnDelFAMDateFromNullable = new DateTime(2018, 8, 1)
                }
            };

            NewRule().ACTOneDoesNotExistOnOrAfterDateEmpStat(learningDeliveryFAMs, dateEmpStat).Should().BeTrue();
        }

        [Fact]
        public void ACTOneDoesNotExistOnOrAfterDateEmpStat_True_FAMTypeMisMatch()
        {
            var dateEmpStat = new DateTime(2018, 8, 1);

            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "LSF",
                    LearnDelFAMCode = "1",
                    LearnDelFAMDateFromNullable = new DateTime(2018, 9, 1)
                }
            };

            NewRule().ACTOneDoesNotExistOnOrAfterDateEmpStat(learningDeliveryFAMs, dateEmpStat).Should().BeTrue();
        }

        [Fact]
        public void ACTOneDoesNotExistOnOrAfterDateEmpStat_True_NullFams()
        {
            NewRule().ACTOneDoesNotExistOnOrAfterDateEmpStat(null, new DateTime(2018, 8, 1)).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_True_FamMisMatch()
        {
            var learnerEmploymentStatus = new TestLearnerEmploymentStatus
            {
                DateEmpStatApp = new DateTime(2018, 8, 1),
                AgreeId = "AgreeId"
            };

            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "LSF",
                    LearnDelFAMCode = "1",
                    LearnDelFAMDateFromNullable = new DateTime(2018, 9, 1)
                }
            };

            NewRule().ConditionMet(learnerEmploymentStatus, learningDeliveryFAMs).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_True_FamDateFromMisMatch()
        {
            var learnerEmploymentStatus = new TestLearnerEmploymentStatus
            {
                DateEmpStatApp = new DateTime(2018, 9, 1),
                AgreeId = "AgreeId"
            };

            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "ACT",
                    LearnDelFAMCode = "1",
                    LearnDelFAMDateFromNullable = new DateTime(2018, 8, 1)
                }
            };

            NewRule().ConditionMet(learnerEmploymentStatus, learningDeliveryFAMs).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_True_FamNull()
        {
            var learnerEmploymentStatus = new TestLearnerEmploymentStatus
            {
                DateEmpStatApp = new DateTime(2018, 8, 1),
                AgreeId = "AgreeId"
            };

            NewRule().ConditionMet(learnerEmploymentStatus, null).Should().BeTrue();
        }

        [Theory]
        [InlineData("", "ACT")]
        [InlineData("AgreeId", "ACT")]
        public void ConditionMet_False(string agreeId, string famType)
        {
            var learnerEmploymentStatus = new TestLearnerEmploymentStatus
            {
                DateEmpStatApp = new DateTime(2018, 8, 1),
                AgreeId = agreeId
            };

            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = famType,
                    LearnDelFAMCode = "1",
                    LearnDelFAMDateFromNullable = new DateTime(2018, 9, 1)
                }
            };

            NewRule().ConditionMet(learnerEmploymentStatus, learningDeliveryFAMs).Should().BeFalse();
        }

        [Fact]
        public void Validate_Error()
        {
            var learnerEmploymentStatuses = new List<TestLearnerEmploymentStatus>
            {
                new TestLearnerEmploymentStatus
                {
                    EmpStat = 11,
                    DateEmpStatApp = new DateTime(2018, 9, 1),
                    AgreeId = "AgreeId"
                }
            };

            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "ACT",
                    LearnDelFAMCode = "1",
                    LearnDelFAMDateFromNullable = new DateTime(2018, 8, 1)
                }
            };

            ILearner learner = new TestLearner()
            {
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        LearningDeliveryFAMs = learningDeliveryFAMs
                    }
                },
                LearnerEmploymentStatuses = learnerEmploymentStatuses
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandler: validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_Error_NullFAMs()
        {
            var learnerEmploymentStatuses = new List<TestLearnerEmploymentStatus>
            {
                new TestLearnerEmploymentStatus
                {
                    EmpStat = 11,
                    DateEmpStatApp = new DateTime(2018, 9, 1),
                    AgreeId = "AgreeId"
                }
            };

            ILearner learner = new TestLearner()
            {
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                    }
                },
                LearnerEmploymentStatuses = learnerEmploymentStatuses
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandler: validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError_AgreeIdNull()
        {
            var learnerEmploymentStatuses = new List<TestLearnerEmploymentStatus>
            {
                new TestLearnerEmploymentStatus
                {
                    EmpStat = 11,
                    DateEmpStatApp = new DateTime(2018, 8, 1),
                }
            };

            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "ACT",
                    LearnDelFAMCode = "1",
                    LearnDelFAMDateFromNullable = new DateTime(2018, 9, 1)
                }
            };

            ILearner learner = new TestLearner()
            {
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        LearningDeliveryFAMs = learningDeliveryFAMs
                    }
                },
                LearnerEmploymentStatuses = learnerEmploymentStatuses
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandler: validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError_LearnerEmploymentStatusNull()
        {
            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "ACT",
                    LearnDelFAMCode = "1",
                    LearnDelFAMDateFromNullable = new DateTime(2018, 8, 1)
                }
            };

            ILearner learner = new TestLearner()
            {
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        LearningDeliveryFAMs = learningDeliveryFAMs
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandler: validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();
            validationErrorHandlerMock.Setup(dd => dd.BuildErrorMessageParameter(PropertyNameConstants.DateEmpStatApp, "01/08/2014")).Verifiable();
            validationErrorHandlerMock.Setup(dd => dd.BuildErrorMessageParameter(PropertyNameConstants.AgreeId, "AgreeIdValue")).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(new DateTime(2014, 08, 01), "AgreeIdValue");

            validationErrorHandlerMock.Verify();
        }

        public AgreeId_02Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new AgreeId_02Rule(validationErrorHandler: validationErrorHandler);
        }
    }
}
