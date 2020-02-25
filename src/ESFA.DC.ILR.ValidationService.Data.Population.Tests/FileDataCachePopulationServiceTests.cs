using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.File;
using ESFA.DC.ILR.ValidationService.Data.Population;
using ESFA.DC.ILR.ValidationService.Interface;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.FileData.Tests
{
    public class FileDataCachePopulationServiceTests
    {
        [Fact]
        public async Task Populate()
        {
            var filePreparationDate = new DateTime(2018, 1, 5);
            var ukprn = 1;
            var learnerDestinationAndProgressions = new List<TestLearnerDestinationAndProgression>
            {
                new TestLearnerDestinationAndProgression
                {
                    LearnRefNumber = "Learner1",
                    ULN = 9999999999,
                    DPOutcomes = new List<TestDPOutcome>
                    {
                        new TestDPOutcome
                        {
                            OutCode = 1,
                            OutType = "Type",
                            OutCollDate = new DateTime(2018, 8, 1),
                            OutStartDate = new DateTime(2018, 8, 1)
                        }
                    }
                }
            };

            var message = new TestMessage()
            {
                HeaderEntity = new TestHeader()
                {
                    CollectionDetailsEntity = new TestCollectionDetails()
                    {
                        FilePreparationDate = filePreparationDate
                    }
                },
                LearningProviderEntity = new TestLearningProvider()
                {
                    UKPRN = ukprn,
                },
                LearnerDestinationAndProgressions = learnerDestinationAndProgressions
            };

            var fileDataCache = new FileDataCache();

            var validationContextMock = new Mock<IValidationContext>();

            var fileDataCachePopulationService = new FileDataCachePopulationService(fileDataCache);

            fileDataCachePopulationService.Populate(validationContextMock.Object, message);

            fileDataCache.FilePreparationDate.Should().Be(filePreparationDate);
            fileDataCache.UKPRN.Should().Be(ukprn);
        }
    }
}
