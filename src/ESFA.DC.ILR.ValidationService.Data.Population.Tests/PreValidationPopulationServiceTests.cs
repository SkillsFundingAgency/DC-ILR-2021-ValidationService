using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ReferenceDataService.Model;
using ESFA.DC.ILR.ReferenceDataService.Model.Learner;
using ESFA.DC.ILR.ValidationService.Data.Population.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Data.Population.Tests
{
    public class PreValidationPopulationServiceTests
    {
        [Fact]
        public void Populate()
        {
            var validationContextMock = Mock.Of<IValidationContext>();

            var messageMock = Mock.Of<IMessage>();
            var referenceDataRootMock = Mock.Of<ReferenceDataRoot>();
            var learnerReferenceDataMock = Mock.Of<LearnerReferenceData>();

            var messageCachePopulationServiceMock = new Mock<IMessageCachePopulationService>();
            var fileDataCachePopulationServiceMock = new Mock<IFileDataCachePopulationService>();
            var internalDataCachePopulationServiceMock = new Mock<IInternalDataCachePopulationService>();
            var externalDataCachePopulationServiceMock = new Mock<IExternalDataCachePopulationService>();
            var learnerReferenceDataCachePopulationServiceMock = new Mock<ILearnerReferenceDataCachePopulationService>();

            NewService(
                messageCachePopulationServiceMock.Object,
                fileDataCachePopulationServiceMock.Object,
                internalDataCachePopulationServiceMock.Object,
                externalDataCachePopulationServiceMock.Object,
                learnerReferenceDataCachePopulationServiceMock.Object)
                .Populate(validationContextMock, messageMock, referenceDataRootMock, learnerReferenceDataMock);

            messageCachePopulationServiceMock.Verify(ps => ps.Populate(messageMock));
            fileDataCachePopulationServiceMock.Verify(ps => ps.Populate(validationContextMock, messageMock));
            internalDataCachePopulationServiceMock.Verify(ps => ps.Populate(referenceDataRootMock));
            externalDataCachePopulationServiceMock.Verify(ps => ps.Populate(referenceDataRootMock, validationContextMock));
        }

        private PreValidationPopulationService NewService(
            IMessageCachePopulationService messageCachePopulationService = null,
            IFileDataCachePopulationService fileDataCachePopulationService = null,
            IInternalDataCachePopulationService internalDataCachePopulationService = null,
            IExternalDataCachePopulationService externalDataCachePopulationService = null,
            ILearnerReferenceDataCachePopulationService learnerReferenceDataCachePopulationService = null)
        {
            return new PreValidationPopulationService(
                messageCachePopulationService,
                fileDataCachePopulationService,
                internalDataCachePopulationService,
                externalDataCachePopulationService,
                learnerReferenceDataCachePopulationService);
        }
    }
}
