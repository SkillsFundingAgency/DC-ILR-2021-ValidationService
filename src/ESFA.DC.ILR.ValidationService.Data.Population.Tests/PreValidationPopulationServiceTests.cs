using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.ValidationService.Data.Population.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Data.Population.Tests
{
    public class PreValidationPopulationServiceTests
    {
        [Fact]
        public async Task Populate()
        {
            var validationContextMock = new Mock<IValidationContext>();
            var cancellationToken = CancellationToken.None;
            var messageCachePopulationServiceMock = NewPopulationServiceMock<IMessageCachePopulationService>(validationContextMock.Object, cancellationToken);
            var referenceDataCachePopulationServiceMock = NewPopulationServiceMock<IReferenceDataCachePopulationService>(validationContextMock.Object, cancellationToken);
            var fileDataCachePopulationServiceMock = NewPopulationServiceMock<IFileDataCachePopulationService>(validationContextMock.Object, cancellationToken);
            var internalDataCachePopulationServiceMock = NewPopulationServiceMock<IInternalDataCachePopulationService>(validationContextMock.Object, cancellationToken);
            var externalDataCachePopulationServiceMock = NewPopulationServiceMock<IExternalDataCachePopulationService>(validationContextMock.Object, cancellationToken);

            messageCachePopulationServiceMock.Setup(x => x.PopulateAsync(validationContextMock.Object, cancellationToken)).Returns(Task.CompletedTask);
            referenceDataCachePopulationServiceMock.Setup(x => x.PopulateAsync(validationContextMock.Object, cancellationToken)).Returns(Task.CompletedTask);
            fileDataCachePopulationServiceMock.Setup(x => x.PopulateAsync(validationContextMock.Object, cancellationToken)).Returns(Task.CompletedTask);
            internalDataCachePopulationServiceMock.Setup(x => x.PopulateAsync(validationContextMock.Object, cancellationToken)).Returns(Task.CompletedTask);
            externalDataCachePopulationServiceMock.Setup(x => x.PopulateAsync(validationContextMock.Object, cancellationToken)).Returns(Task.CompletedTask);

            await NewService(messageCachePopulationServiceMock.Object, referenceDataCachePopulationServiceMock.Object, fileDataCachePopulationServiceMock.Object, internalDataCachePopulationServiceMock.Object, externalDataCachePopulationServiceMock.Object).PopulateAsync(validationContextMock.Object, cancellationToken);

            messageCachePopulationServiceMock.Verify();
            referenceDataCachePopulationServiceMock.Verify();
            fileDataCachePopulationServiceMock.Verify();
            internalDataCachePopulationServiceMock.Verify();
            externalDataCachePopulationServiceMock.Verify();
        }

        private Mock<T> NewPopulationServiceMock<T>(IValidationContext validationContext, CancellationToken cancellationToken)
            where T : class, IPopulationService
        {
            var mock = new Mock<T>();

            mock.Setup(ps => ps.PopulateAsync(validationContext, cancellationToken)).Verifiable();

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
