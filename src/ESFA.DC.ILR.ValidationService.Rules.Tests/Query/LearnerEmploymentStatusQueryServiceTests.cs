using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Rules.Query;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Query
{
    public class LearnerEmploymentStatusQueryServiceTests
    {
        [Fact]
        public void EmpStatForDateEmpStatApp_Zero()
        {
            NewService().LearnerEmploymentStatusForDate(null, new DateTime(2018, 8, 1)).Should().BeNull();
        }

        [Fact]
        public void EmpStatForDateEmpStatApp_Zero_DateTooLate()
        {
            var learnStartDate = new DateTime(2018, 7, 1);

            var learnerEmploymentStatuses = new List<TestLearnerEmploymentStatus>
            {
                new TestLearnerEmploymentStatus
                {
                    EmpStat = 10,
                    DateEmpStatApp = new DateTime(2018, 8, 1)
                }
            };

            NewService().LearnerEmploymentStatusForDate(learnerEmploymentStatuses, learnStartDate).Should().BeNull();
        }

        [Fact]
        public void EmpStatForDateEmpStatApp()
        {
            var learnStartDate = new DateTime(2018, 8, 1);

            var matchingLearnerEmploymentStatus = new TestLearnerEmploymentStatus
            {
                EmpStat = 10,
                DateEmpStatApp = new DateTime(2018, 8, 1)
            };

            var learnerEmploymentStatuses = new List<TestLearnerEmploymentStatus>
            {
                matchingLearnerEmploymentStatus
            };

            NewService().LearnerEmploymentStatusForDate(learnerEmploymentStatuses, learnStartDate).Should().Be(matchingLearnerEmploymentStatus);
        }

        [Fact]
        public void EmpStatForDateEmpStatApp_MiddleOne()
        {
            var learnStartDate = new DateTime(2018, 8, 13);

            var earlyLearnerEmploymentStatus = new TestLearnerEmploymentStatus
            {
                DateEmpStatApp = new DateTime(2018, 8, 1)
            };

            var matchingLearnerEmploymentStatus = new TestLearnerEmploymentStatus
            {
                DateEmpStatApp = new DateTime(2018, 8, 13)
            };

            var laterLearningEmploymentStatus = new TestLearnerEmploymentStatus
            {
                DateEmpStatApp = new DateTime(2018, 8, 19)
            };

            var learnerEmploymentStatuses = new List<TestLearnerEmploymentStatus>
            {
                earlyLearnerEmploymentStatus,
                matchingLearnerEmploymentStatus,
                laterLearningEmploymentStatus
            };

            NewService().LearnerEmploymentStatusForDate(learnerEmploymentStatuses, learnStartDate).Should().Be(matchingLearnerEmploymentStatus);
        }

        [Fact]
        public void LearnerEmploymentStatusesForDate_Single()
        {
            var learnStartDate = new DateTime(2018, 8, 13);

            var matchingLearnerEmploymentStatus = new TestLearnerEmploymentStatus
            {
                DateEmpStatApp = new DateTime(2018, 8, 13)
            };

            var laterLearningEmploymentStatus = new TestLearnerEmploymentStatus
            {
                DateEmpStatApp = new DateTime(2018, 8, 19)
            };

            var learnerEmploymentStatuses = new List<TestLearnerEmploymentStatus>
            {
                matchingLearnerEmploymentStatus,
                laterLearningEmploymentStatus
            };

            NewService().LearnerEmploymentStatusesForDate(learnerEmploymentStatuses, learnStartDate).Should().BeEquivalentTo(new List<ILearnerEmploymentStatus> { matchingLearnerEmploymentStatus });
        }

        [Fact]
        public void LearnerEmploymentStatusesForDate_Multiple()
        {
            var learnStartDate = new DateTime(2018, 8, 13);

            var matchingLearnerEmploymentStatusOne = new TestLearnerEmploymentStatus
            {
                DateEmpStatApp = new DateTime(2018, 8, 13)
            };

            var matchingLearnerEmploymentStatusTwo = new TestLearnerEmploymentStatus
            {
                DateEmpStatApp = new DateTime(2018, 8, 13)
            };

            var learnerEmploymentStatuses = new List<TestLearnerEmploymentStatus>
            {
                matchingLearnerEmploymentStatusOne,
                matchingLearnerEmploymentStatusTwo
            };

            NewService().LearnerEmploymentStatusesForDate(learnerEmploymentStatuses, learnStartDate).Should().BeEquivalentTo(new List<ILearnerEmploymentStatus> { matchingLearnerEmploymentStatusOne, matchingLearnerEmploymentStatusTwo });
        }

        [Fact]
        public void LearnerEmploymentStatusesForDate_NoMatch()
        {
            var learnStartDate = new DateTime(2018, 8, 1);

            var earlyLearnerEmploymentStatus = new TestLearnerEmploymentStatus
            {
                DateEmpStatApp = new DateTime(2018, 8, 10)
            };

            var middleLearnerEmploymentStatus = new TestLearnerEmploymentStatus
            {
                DateEmpStatApp = new DateTime(2018, 8, 13)
            };

            var laterLearnerEmploymentStatus = new TestLearnerEmploymentStatus
            {
                DateEmpStatApp = new DateTime(2018, 8, 15)
            };

            var learnerEmploymentStatuses = new List<TestLearnerEmploymentStatus>
            {
                earlyLearnerEmploymentStatus,
                middleLearnerEmploymentStatus,
                laterLearnerEmploymentStatus
            };

            NewService().LearnerEmploymentStatusesForDate(learnerEmploymentStatuses, learnStartDate).Should().BeEquivalentTo(Enumerable.Empty<ILearnerEmploymentStatus>());
        }

        [Fact]
        public void LearnerEmploymentStatusesForDate_NoMatch_NoStatuses()
        {
            var learnStartDate = new DateTime(2018, 8, 1);

            NewService().LearnerEmploymentStatusesForDate(null, learnStartDate).Should().BeEquivalentTo(Enumerable.Empty<ILearnerEmploymentStatus>());
        }

        [Fact]
        public void EmpStatsNotExistBeforeLearnStartDate_True_Null()
        {
            NewService().EmpStatsNotExistBeforeDate(null, new DateTime(2018, 8, 1)).Should().BeTrue();
        }

        [Fact]
        public void EmpStatsNotExistBeforeLearnStartDate_True()
        {
            var learnStartDate = new DateTime(2018, 7, 1);
            var learnerEmploymentStatuses = new List<TestLearnerEmploymentStatus>
            {
                new TestLearnerEmploymentStatus
                {
                    EmpStat = 10,
                    DateEmpStatApp = new DateTime(2018, 8, 1)
                }
            };

            NewService().EmpStatsNotExistBeforeDate(learnerEmploymentStatuses, learnStartDate).Should().BeTrue();
        }

        [Fact]
        public void EmpStatsNotExistBeforeLearnStartDate_False()
        {
            var learnStartDate = new DateTime(2018, 9, 1);
            var learnerEmploymentStatuses = new List<TestLearnerEmploymentStatus>
            {
                new TestLearnerEmploymentStatus
                {
                    EmpStat = 10,
                    DateEmpStatApp = new DateTime(2018, 8, 1)
                }
            };

            NewService().EmpStatsNotExistBeforeDate(learnerEmploymentStatuses, learnStartDate).Should().BeFalse();
        }

        [Fact]
        public void EmpStatsNotExistOnOrBeforeLearnStartDate_True_Null()
        {
            NewService().EmpStatsNotExistOnOrBeforeDate(null, new DateTime(2018, 8, 1)).Should().BeTrue();
        }

        [Fact]
        public void EmpStatsNotExistOnOrBeforeLearnStartDate_True()
        {
            var learnStartDate = new DateTime(2018, 7, 1);
            var learnerEmploymentStatuses = new List<TestLearnerEmploymentStatus>
            {
                new TestLearnerEmploymentStatus
                {
                    EmpStat = 10,
                    DateEmpStatApp = new DateTime(2018, 8, 1)
                }
            };

            NewService().EmpStatsNotExistOnOrBeforeDate(learnerEmploymentStatuses, learnStartDate).Should().BeTrue();
        }

        [Fact]
        public void EmpStatsNotExistOnOrBeforeLearnStartDate_False()
        {
            var learnStartDate = new DateTime(2018, 9, 1);
            var learnerEmploymentStatuses = new List<TestLearnerEmploymentStatus>
            {
                new TestLearnerEmploymentStatus
                {
                    EmpStat = 10,
                    DateEmpStatApp = new DateTime(2018, 8, 1)
                }
            };

            NewService().EmpStatsNotExistOnOrBeforeDate(learnerEmploymentStatuses, learnStartDate).Should().BeFalse();
        }

        private LearnerEmploymentStatusQueryService NewService()
        {
            return new LearnerEmploymentStatusQueryService();
        }
    }
}
