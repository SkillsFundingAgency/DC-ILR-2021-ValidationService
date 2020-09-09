using System.Collections.Generic;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Data.Learner;
using ESFA.DC.ILR.ValidationService.Data.Learner.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Data.Tests.Learner
{
    public class LearnerReferenceDataServiceTests
    {
        [Fact]
        public void GetLearnerDataForPreviousYear()
        {
            var learner1 = new LearnerReferenceData
            {
                LearnRefNumber = "Learner1"
            };

            var learnerCache = new Dictionary<string, IEnumerable<ILearnerReferenceData>>
            {
                {
                    "Learner1", new List<ILearnerReferenceData>
                    {
                        learner1
                    }
                },
                {
                    "Learner2", new List<ILearnerReferenceData>
                    {
                        new LearnerReferenceData
                        {
                            LearnRefNumber = "Learner2"
                        }
                    }
                },
            };

            var cache = new Mock<ILearnerReferenceDataCache>();
            cache.Setup(x => x.Learners).Returns(learnerCache);

            var result = NewService(cache.Object).GetLearnerDataForPreviousYear(new List<string> { "Learner1" });

            result.Should().BeEquivalentTo(new List<ILearnerReferenceData> { learner1 });
        }

        [Fact]
        public void GetLearnerDataForPreviousYear_Multiple()
        {
            var learner1 = new LearnerReferenceData
            {
                LearnRefNumber = "Learner1"
            };

            var learner2 = new LearnerReferenceData
            {
                LearnRefNumber = "Learner2"
            };

            var learnerCache = new Dictionary<string, IEnumerable<ILearnerReferenceData>>
            {
                {
                    "Learner1", new List<ILearnerReferenceData>
                    {
                        learner1
                    }
                },
                {
                    "Learner2", new List<ILearnerReferenceData>
                    {
                        learner2
                    }
                },
            };

            var cache = new Mock<ILearnerReferenceDataCache>();
            cache.Setup(x => x.Learners).Returns(learnerCache);

            var result = NewService(cache.Object).GetLearnerDataForPreviousYear(new List<string> { "Learner1", "Learner2" });

            result.Should().BeEquivalentTo(new List<ILearnerReferenceData> { learner1, learner2 });
        }

        [Fact]
        public void GetLearnerDataForPreviousYear_MultipleOneMatch()
        {
            var learner1 = new LearnerReferenceData
            {
                LearnRefNumber = "Learner1"
            };

            var learnerCache = new Dictionary<string, IEnumerable<ILearnerReferenceData>>
            {
                {
                    "Learner1", new List<ILearnerReferenceData>
                    {
                        learner1
                    }
                },
                {
                    "Learner2", new List<ILearnerReferenceData>
                    {
                        new LearnerReferenceData
                        {
                            LearnRefNumber = "Learner2"
                        }
                    }
                },
            };

            var cache = new Mock<ILearnerReferenceDataCache>();
            cache.Setup(x => x.Learners).Returns(learnerCache);

            var result = NewService(cache.Object).GetLearnerDataForPreviousYear(new List<string> { "Learner1", "Learner3" });

            result.Should().BeEquivalentTo(new List<ILearnerReferenceData> { learner1 });
        }

        [Fact]
        public void GetLearnerDataForPreviousYear_NoMatch()
        {
            var learnerCache = new Dictionary<string, IEnumerable<ILearnerReferenceData>>
            {
                {
                    "Learner1", new List<ILearnerReferenceData>
                    {
                        new LearnerReferenceData
                        {
                            LearnRefNumber = "Learner1"
                        }
                    }
                },
            };

            var cache = new Mock<ILearnerReferenceDataCache>();
            cache.Setup(x => x.Learners).Returns(learnerCache);

            NewService(cache.Object).GetLearnerDataForPreviousYear(new List<string> { "Learner2" }).Should().BeNullOrEmpty();
        }

        public LearnerReferenceDataService NewService(ILearnerReferenceDataCache learnerReferenceDataCache = null)
        {
            return new LearnerReferenceDataService(learnerReferenceDataCache);
        }
    }
}