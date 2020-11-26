using System;
using System.Collections.Generic;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Data.Learner;
using ESFA.DC.ILR.ValidationService.Interface;
using FluentAssertions;
using Moq;
using Xunit;
using LearnerReferenceDataJson = ESFA.DC.ILR.ReferenceDataService.Model.Learner.LearnerReferenceData;
using LearnerReferenceData = ESFA.DC.ILR.ValidationService.Data.Learner.Model.LearnerReferenceData;

namespace ESFA.DC.ILR.ValidationService.Data.Population.Tests
{
    public class LearnerReferenceDataCachePopulationServiceTests
    {
        [Fact]
        public void Populate()
        {
            var refDataFromJson = new LearnerReferenceDataJson();
            var cacheRefData = new Dictionary<string, IEnumerable<ILearnerReferenceData>>();

            ILearnerReferenceDataCache cacheMock = new LearnerReferenceDataCache();

            var service = NewMockService(cacheMock);
            service.Setup(x => x.BuildDictionary(refDataFromJson)).Returns(cacheRefData).Verifiable();

            service.Object.Populate(refDataFromJson);

            cacheMock.Learners.Should().NotBeNull();
        }

        [Fact]
        public void BuildDictionary_NullInput()
        {
            var refData = NewService().BuildDictionary(null);

            refData.Should().NotBeNull();
            refData.Keys.Should().HaveCount(0);
        }

        [Fact]
        public void BuildDictionary_EmptyLearners()
        {
            var refDataFromJson = new LearnerReferenceDataJson
            {
                Learners = new List<ReferenceDataService.Model.Learner.Learner>(),
            };

            var refData = NewService().BuildDictionary(refDataFromJson);

            refData.Should().NotBeNull();
            refData.Keys.Should().HaveCount(0);
        }

        [Fact]
        public void BuildDictionary()
        {
            var refDataFromJson = new LearnerReferenceDataJson
            {
                Learners = new List<ReferenceDataService.Model.Learner.Learner>
                {
                    new ReferenceDataService.Model.Learner.Learner
                    {
                        LearnRefNumber = "Learner1",
                        UKPRN = 1000000,
                        PMUKPRN = null,
                        ULN = 10000000,
                        PrevLearnRefNumber = null,
                        LearnAimRef = "LearnAimRef1a",
                        ProgTypeNullable = 1,
                        StdCodeNullable = 1,
                        FworkCodeNullable = 1,
                        PwayCodeNullable = 1,
                        LearnStartDate = new DateTime(2019, 8, 1),
                        LearnActEndDate = null,
                        FundModel = 35,
                    },
                    new ReferenceDataService.Model.Learner.Learner
                    {
                        LearnRefNumber = "Learner1",
                        UKPRN = 1000000,
                        PMUKPRN = null,
                        ULN = 10000000,
                        PrevLearnRefNumber = null,
                        LearnAimRef = "LearnAimRef1b",
                        ProgTypeNullable = 1,
                        StdCodeNullable = 1,
                        FworkCodeNullable = 1,
                        PwayCodeNullable = 1,
                        LearnStartDate = new DateTime(2019, 8, 1),
                        LearnActEndDate = null,
                        FundModel = 35,
                    },
                    new ReferenceDataService.Model.Learner.Learner
                    {
                        LearnRefNumber = "Learner2",
                        UKPRN = 2000000,
                        PMUKPRN = null,
                        ULN = 20000000,
                        PrevLearnRefNumber = null,
                        LearnAimRef = "LearnAimRef2a",
                        ProgTypeNullable = 1,
                        StdCodeNullable = 1,
                        FworkCodeNullable = 1,
                        PwayCodeNullable = 1,
                        LearnStartDate = new DateTime(2019, 8, 1),
                        LearnActEndDate = new DateTime(2020, 9, 1),
                        FundModel = 35,
                    },
                },
            };

            var expectedResult = new Dictionary<string, List<ILearnerReferenceData>>
            {
                {
                    "Learner1", new List<ILearnerReferenceData>
                    {
                        new LearnerReferenceData
                        {
                            LearnRefNumber = "Learner1",
                            UKPRN = 1000000,
                            PMUKPRN = null,
                            ULN = 10000000,
                            PrevLearnRefNumber = null,
                            LearnAimRef = "LearnAimRef1a",
                            ProgTypeNullable = 1,
                            StdCodeNullable = 1,
                            FworkCodeNullable = 1,
                            PwayCodeNullable = 1,
                            LearnStartDate = new DateTime(2019, 8, 1),
                            LearnActEndDate = null,
                            FundModel = 35,
                        },
                        new LearnerReferenceData
                        {
                            LearnRefNumber = "Learner1",
                            UKPRN = 1000000,
                            PMUKPRN = null,
                            ULN = 10000000,
                            PrevLearnRefNumber = null,
                            LearnAimRef = "LearnAimRef1b",
                            ProgTypeNullable = 1,
                            StdCodeNullable = 1,
                            FworkCodeNullable = 1,
                            PwayCodeNullable = 1,
                            LearnStartDate = new DateTime(2019, 8, 1),
                            LearnActEndDate = null,
                            FundModel = 35,
                        },
                    }
                },
                {
                    "Learner2", new List<ILearnerReferenceData>
                    {
                        new LearnerReferenceData
                        {
                            LearnRefNumber = "Learner2",
                            UKPRN = 2000000,
                            PMUKPRN = null,
                            ULN = 20000000,
                            PrevLearnRefNumber = null,
                            LearnAimRef = "LearnAimRef2a",
                            ProgTypeNullable = 1,
                            StdCodeNullable = 1,
                            FworkCodeNullable = 1,
                            PwayCodeNullable = 1,
                            LearnStartDate = new DateTime(2019, 8, 1),
                            LearnActEndDate = new DateTime(2020, 9, 1),
                            FundModel = 35,
                        },
                    }
                },
            };

            var result = NewService().BuildDictionary(refDataFromJson);

            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedResult);
        }

        private Mock<LearnerReferenceDataCachePopulationService> NewMockService(ILearnerReferenceDataCache cache = null)
        {
            return new Mock<LearnerReferenceDataCachePopulationService>(cache);
        }

        private LearnerReferenceDataCachePopulationService NewService(ILearnerReferenceDataCache cache = null)
        {
            return new LearnerReferenceDataCachePopulationService(cache);
        }
    }
}
