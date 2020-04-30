using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.CrossEntity;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.CrossEntity
{
    public class R110RuleTests : AbstractRuleTests<R110Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("R110");
        }

        [Fact]
        public void Validate_Error()
        {
            var dateFrom = new DateTime(2017, 1, 1);

            var learningDeliveryFams = new List<ILearningDeliveryFAM>()
            {
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = LearningDeliveryFAMTypeConstants.ACT,
                    LearnDelFAMCode = LearningDeliveryFAMCodeConstants.ACT_ContractEmployer,
                    LearnDelFAMDateFromNullable = dateFrom
                }
            };

            var learnerEmploymentStatus = new TestLearnerEmploymentStatus()
            {
                EmpStat = EmploymentStatusEmpStats.NotEmployedNotSeekingOrNotAvailable
            };

            var learnerEmploymentStatuses = new List<ILearnerEmploymentStatus>()
            {
                learnerEmploymentStatus
            };

            var learner = new TestLearner
            {
                LearnerEmploymentStatuses = learnerEmploymentStatuses,
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        FundModel = FundModels.ApprenticeshipsFrom1May2017,
                        LearningDeliveryFAMs = learningDeliveryFams
                    }
                }
            };

            var learnerEmploymentStatusQueryServiceMock = new Mock<ILearnerEmploymentStatusQueryService>();

            learnerEmploymentStatusQueryServiceMock.Setup(qs => qs.LearnerEmploymentStatusForDate(learnerEmploymentStatuses, dateFrom)).Returns(learnerEmploymentStatus);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(learnerEmploymentStatusQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError()
        {
            var dateFrom = new DateTime(2017, 1, 1);
            var learningDeliveryFams = new List<ILearningDeliveryFAM>()
            {
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = LearningDeliveryFAMTypeConstants.ACT,
                    LearnDelFAMCode = LearningDeliveryFAMCodeConstants.ACT_ContractEmployer,
                    LearnDelFAMDateFromNullable = dateFrom
                }
            };

            var learnerEmploymentStatuses = new List<ILearnerEmploymentStatus>();

            var learnerEmploymentStatus = new TestLearnerEmploymentStatus()
            {
                EmpStat = EmploymentStatusEmpStats.NotEmployedNotSeekingOrNotAvailable
            };

            var learner = new TestLearner
            {
                LearnerEmploymentStatuses = learnerEmploymentStatuses,
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        FundModel = FundModels.ApprenticeshipsFrom1May2017,
                        LearningDeliveryFAMs = learningDeliveryFams
                    }
                }
            };

            var learnerEmploymentStatusQueryServiceMock = new Mock<ILearnerEmploymentStatusQueryService>();

            learnerEmploymentStatusQueryServiceMock.Setup(qs => qs.LearnerEmploymentStatusForDate(learnerEmploymentStatuses, dateFrom)).Returns(learnerEmploymentStatus);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(learnerEmploymentStatusQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void IsApprenticeshipProgramme_True()
        {
            var learningDelivery = new TestLearningDelivery()
            {
                FundModel = FundModels.ApprenticeshipsFrom1May2017
            };

            NewRule().IsApprenticeshipProgramme(learningDelivery).Should().BeTrue();
        }

        [Fact]
        public void IsApprenticeshipProgramme_False()
        {
            var learningDelivery = new TestLearningDelivery()
            {
                FundModel = FundModels.AdultSkills
            };

            NewRule().IsApprenticeshipProgramme(learningDelivery).Should().BeFalse();
        }

        [Fact]
        public void GetLearningDeliveryFAMsWhereApprenticeshipProgrammeFundedThroughContract_Match()
        {
            var matchOne = new TestLearningDeliveryFAM()
            {
                LearnDelFAMType = LearningDeliveryFAMTypeConstants.ACT,
                LearnDelFAMCode = LearningDeliveryFAMCodeConstants.ACT_ContractEmployer,
                LearnDelFAMDateFromNullable = new DateTime(2018, 1, 1),
            };

            var matchTwoCaseInsensitive = new TestLearningDeliveryFAM()
            {
                LearnDelFAMType = "act",
                LearnDelFAMCode = LearningDeliveryFAMCodeConstants.ACT_ContractEmployer,
                LearnDelFAMDateFromNullable = new DateTime(2018, 1, 1),
            };

            var nonMatchOneType = new TestLearningDeliveryFAM()
            {
                LearnDelFAMType = LearningDeliveryFAMTypeConstants.ADL,
                LearnDelFAMCode = LearningDeliveryFAMCodeConstants.ACT_ContractEmployer,
                LearnDelFAMDateFromNullable = new DateTime(2018, 1, 1),
            };

            var nonMatchTwoCode = new TestLearningDeliveryFAM()
            {
                LearnDelFAMType = LearningDeliveryFAMTypeConstants.ACT,
                LearnDelFAMCode = LearningDeliveryFAMCodeConstants.ACT_ContractESFA,
                LearnDelFAMDateFromNullable = new DateTime(2018, 1, 1),
            };

            var learningDeliveryFams = new List<ILearningDeliveryFAM>()
            {
                matchOne,
                matchTwoCaseInsensitive,
                nonMatchOneType,
                nonMatchTwoCode,
            };

            var result = NewRule().GetLearningDeliveryFAMsWhereApprenticeshipProgrammeFundedThroughContract(learningDeliveryFams).ToList();

            result.Should().HaveCount(2);
            result.Should().Contain(matchOne);
            result.Should().Contain(matchTwoCaseInsensitive);
        }

        [Fact]
        public void GetLearningDeliveryFAMsWhereApprenticeshipProgrammeFundedThroughContract_NoMatch()
        {
            var nonMatchOneType = new TestLearningDeliveryFAM()
            {
                LearnDelFAMType = LearningDeliveryFAMTypeConstants.ADL,
                LearnDelFAMCode = LearningDeliveryFAMCodeConstants.ACT_ContractEmployer,
                LearnDelFAMDateFromNullable = new DateTime(2018, 1, 1),
            };

            var nonMatchTwoCode = new TestLearningDeliveryFAM()
            {
                LearnDelFAMType = LearningDeliveryFAMTypeConstants.ACT,
                LearnDelFAMCode = LearningDeliveryFAMCodeConstants.ACT_ContractESFA,
                LearnDelFAMDateFromNullable = new DateTime(2018, 1, 1),
            };

            var learningDeliveryFams = new List<ILearningDeliveryFAM>()
            {
                nonMatchOneType,
                nonMatchTwoCode,
            };

            var result = NewRule().GetLearningDeliveryFAMsWhereApprenticeshipProgrammeFundedThroughContract(learningDeliveryFams);

            result.Should().BeEmpty();
        }

        [Fact]
        public void LearnerNotEmployedOnDate_True()
        {
            var learnerEmploymentStatuses = new List<ILearnerEmploymentStatus>();

            var learnerEmploymentStatus = new TestLearnerEmploymentStatus()
            {
                EmpStat = EmploymentStatusEmpStats.InPaidEmployment
            };

            var learningDeliveryFamDateFrom = new DateTime(2018, 1, 1);

            var learnerEmploymentStatusQueryServiceMock = new Mock<ILearnerEmploymentStatusQueryService>();

            learnerEmploymentStatusQueryServiceMock.Setup(qs => qs.LearnerEmploymentStatusForDate(learnerEmploymentStatuses, learningDeliveryFamDateFrom)).Returns(learnerEmploymentStatus);

            NewRule(learnerEmploymentStatusQueryServiceMock.Object).LearnerNotEmployedOnDate(learnerEmploymentStatuses, learningDeliveryFamDateFrom).Should().BeFalse();
        }

        [Fact]
        public void LearnerNotEmployedOnDate_False()
        {
            var learnerEmploymentStatus = new TestLearnerEmploymentStatus()
            {
                EmpStat = EmploymentStatusEmpStats.NotEmployedNotSeekingOrNotAvailable
            };

            var learnerEmploymentStatuses = new List<ILearnerEmploymentStatus>()
            {
                learnerEmploymentStatus
            };

            var learningDeliveryFamDateFrom = new DateTime(2018, 1, 1);

            var learnerEmploymentStatusQueryServiceMock = new Mock<ILearnerEmploymentStatusQueryService>();

            learnerEmploymentStatusQueryServiceMock.Setup(qs => qs.LearnerEmploymentStatusForDate(learnerEmploymentStatuses, learningDeliveryFamDateFrom)).Returns(learnerEmploymentStatus);

            NewRule(learnerEmploymentStatusQueryServiceMock.Object).LearnerNotEmployedOnDate(learnerEmploymentStatuses, learningDeliveryFamDateFrom).Should().BeTrue();
        }

        [Fact]
        public void LearnerNotEmployedOnDate_NullDate()
        {
            NewRule().LearnerNotEmployedOnDate(null, null).Should().BeFalse();
        }

        private R110Rule NewRule(ILearnerEmploymentStatusQueryService learnerEmploymentStatusQueryService = null, IValidationErrorHandler validationErrorHandler = null)
        {
            return new R110Rule(learnerEmploymentStatusQueryService, validationErrorHandler);
        }
    }
}
