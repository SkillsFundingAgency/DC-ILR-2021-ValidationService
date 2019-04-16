using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.ValidationService.Data.Population.Interface;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Data.Population.Tests
{
    public class PreValidationPopulationServiceTests
    {
        [Fact]
        public async Task Populate()
        {
            var messageCachePopulationServiceMock = NewPopulationServiceMock<IMessageCachePopulationService>();
            var referenceDataCachePopulationServiceMock = NewPopulationServiceMock<IReferenceDataCachePopulationService>();
            var fileDataCachePopulationServiceMock = NewPopulationServiceMock<IFileDataCachePopulationService>();
            var internalDataCachePopulationServiceMock = NewPopulationServiceMock<IInternalDataCachePopulationService>();
            var externalDataCachePopulationServiceMock = NewPopulationServiceMock<IExternalDataCachePopulationService>();

            messageCachePopulationServiceMock.Setup(x => x.PopulateAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            referenceDataCachePopulationServiceMock.Setup(x => x.PopulateAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            fileDataCachePopulationServiceMock.Setup(x => x.PopulateAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            internalDataCachePopulationServiceMock.Setup(x => x.PopulateAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            externalDataCachePopulationServiceMock.Setup(x => x.PopulateAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            await NewService(messageCachePopulationServiceMock.Object, referenceDataCachePopulationServiceMock.Object, fileDataCachePopulationServiceMock.Object, internalDataCachePopulationServiceMock.Object, externalDataCachePopulationServiceMock.Object).PopulateAsync(CancellationToken.None);

            messageCachePopulationServiceMock.Verify();
            referenceDataCachePopulationServiceMock.Verify();
            fileDataCachePopulationServiceMock.Verify();
            internalDataCachePopulationServiceMock.Verify();
            externalDataCachePopulationServiceMock.Verify();
        }

        private Mock<T> NewPopulationServiceMock<T>()
            where T : class, IPopulationService
        {
            var mock = new Mock<T>();

            mock.Setup(ps => ps.PopulateAsync(It.IsAny<CancellationToken>())).Verifiable();

            return mock;
        }

        private PreValidationPopulationService NewService(
            IMessageCachePopulationService messageCachePopulationService = null,
            IReferenceDataCachePopulationService referenceDataCachePopulationService = null,
            IFileDataCachePopulationService fileDataCachePopulationService = null,
            IInternalDataCachePopulationService internalDataCachePopulationService = null,
            IExternalDataCachePopulationService externalDataCachePopulationService = null)
        {
            return new PreValidationPopulationService(
                messageCachePopulationService,
                referenceDataCachePopulationService,
                fileDataCachePopulationService,
                internalDataCachePopulationService,
                externalDataCachePopulationService);
        }
    }
}
