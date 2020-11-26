using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Query
{
    public class LearnerDPQueryServiceTests
    {
        [Fact]
        public void OutTypesForStartDate()
        {
            var outTypes = new List<string>
            {
                OutTypeConstants.PaidEmployment,
                OutTypeConstants.GapYear,
                OutTypeConstants.NotInPaidEmployment,
                OutTypeConstants.Other,
                OutTypeConstants.SocialDestination,
                OutTypeConstants.VoluntaryWork
            };

            var dpOutcomes = new List<TestDPOutcome>
            {
                new TestDPOutcome
                {
                    OutStartDate = new DateTime(2018, 8, 1),
                    OutType = "GAP"
                },
                new TestDPOutcome
                {
                    OutStartDate = new DateTime(2018, 8, 1),
                    OutType = "GAP"
                },
                new TestDPOutcome
                {
                    OutStartDate = new DateTime(2018, 9, 1),
                    OutType = "EMP"
                }
            };

            var dictionary = new Dictionary<DateTime, List<string>>
            {
                { new DateTime(2018, 8, 1), new List<string> { "GAP", "GAP" } },
                { new DateTime(2018, 9, 1), new List<string> { "EMP" } }
            };

            NewService().OutTypesForStartDateAndTypes(dpOutcomes, outTypes).Should().BeEquivalentTo(dictionary);
        }

        [Fact]
        public void OutTypesForStartDate_NullDPOutcomes()
        {
            var outTypes = new List<string>
            {
                OutTypeConstants.PaidEmployment,
                OutTypeConstants.GapYear,
                OutTypeConstants.NotInPaidEmployment,
                OutTypeConstants.Other,
                OutTypeConstants.SocialDestination,
                OutTypeConstants.VoluntaryWork
            };

            var dpOutcomes = new List<TestDPOutcome>();

            NewService().OutTypesForStartDateAndTypes(dpOutcomes, outTypes).Should().BeNull();
        }

        [Fact]
        public void OutTypesForStartDate_NullOutTypes()
        {
            var dpOutcomes = new List<TestDPOutcome>
            {
                new TestDPOutcome
                {
                    OutStartDate = new DateTime(2018, 8, 1),
                    OutType = "GAP"
                },
                new TestDPOutcome
                {
                    OutStartDate = new DateTime(2018, 8, 1),
                    OutType = "GAP"
                },
                new TestDPOutcome
                {
                    OutStartDate = new DateTime(2018, 9, 1),
                    OutType = "EMP"
                }
            };

            NewService().OutTypesForStartDateAndTypes(dpOutcomes, null).Should().BeNull();
        }

        [Fact]
        public void OutTypesForStartDate_MisMatch()
        {
            var outTypes = new List<string>
            {
                OutTypeConstants.PaidEmployment,
                OutTypeConstants.GapYear,
                OutTypeConstants.NotInPaidEmployment,
                OutTypeConstants.Other,
                OutTypeConstants.SocialDestination,
                OutTypeConstants.VoluntaryWork
            };

            var dpOutcomes = new List<TestDPOutcome>
            {
                new TestDPOutcome
                {
                    OutStartDate = new DateTime(2018, 8, 1),
                    OutType = "EDU"
                }
            };

            NewService().OutTypesForStartDateAndTypes(dpOutcomes, outTypes).Should().BeNull();
        }

        [Fact]
        public void ProvideDestinationAndProgressionForLearner()
        {
            const string testLearnRefNum = "12345";

            var learner = new TestLearner
            {
                LearnRefNumber = testLearnRefNum
            };

            var learnerDPs = new List<TestLearnerDestinationAndProgression>
            {
                new TestLearnerDestinationAndProgression
                {
                    LearnRefNumber = testLearnRefNum,
                    DPOutcomes = new List<TestDPOutcome>
                    {
                        new TestDPOutcome()
                    }
                }
            };

            var testMessage = new TestMessage()
            {
                LearnerDestinationAndProgressions = learnerDPs
            };

            var messageCacheMock = new Mock<ICache<IMessage>>();

            messageCacheMock.SetupGet(mc => mc.Item).Returns(testMessage);

            var learnerQueryService = NewService(messageCacheMock.Object);

            var result = learnerQueryService.GetDestinationAndProgressionForLearner(learner.LearnRefNumber);

            Assert.NotNull(result?.DPOutcomes);
            Assert.Equal(result.LearnRefNumber, testLearnRefNum);
            Assert.Equal(1, result.DPOutcomes.Count);
        }

        private ILearnerDestinationAndProgression[] SetupLearnerDPs()
        {
            var learnerDPs = new TestLearnerDestinationAndProgression[]
            {
                new TestLearnerDestinationAndProgression() { LearnRefNumber = "Learner1", ULN = 9999999999 },
                new TestLearnerDestinationAndProgression() { LearnRefNumber = "Learner1", ULN = 9999999998 },
                new TestLearnerDestinationAndProgression() { LearnRefNumber = "Learner3", ULN = 9999999997 },
                new TestLearnerDestinationAndProgression() { LearnRefNumber = "Learner4", ULN = 9999999999 },
            };

            return learnerDPs;
        }

        private LearnerDPQueryService NewService(ICache<IMessage> messageCache = null)
        {
            return new LearnerDPQueryService(messageCache);
        }
    }
}
