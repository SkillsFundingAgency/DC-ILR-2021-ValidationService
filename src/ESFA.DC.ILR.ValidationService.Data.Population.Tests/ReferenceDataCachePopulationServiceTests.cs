using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.ReferenceDataService.Model;
using ESFA.DC.ILR.ValidationService.Data.Cache;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Data.Population.Tests
{
    public class ReferenceDataCachePopulationServiceTests
    {
        [Fact]
        public async Task Populate()
        {
            var referenceData = new Mock<ReferenceDataRoot>().Object;

            var referenceDataCacheMock = new Mock<Cache<ReferenceDataRoot>>();

            referenceDataCacheMock.SetupSet(mc => mc.Item = referenceData).Verifiable();

            var validationItemProviderServiceMock = new Mock<IValidationItemProviderService<ReferenceDataRoot>>();

            validationItemProviderServiceMock.Setup(ps => ps.ProvideAsync(It.IsAny<CancellationToken>())).ReturnsAsync(referenceData);

            await NewService(referenceDataCacheMock.Object, validationItemProviderServiceMock.Object).PopulateAsync(CancellationToken.None);

            validationItemProviderServiceMock.Verify();
            referenceDataCacheMock.Verify();
        }

        private ReferenceDataCachePopulationService NewService(ICache<ReferenceDataRoot> referenceDataCache = null, IValidationItemProviderService<ReferenceDataRoot> validationItemProviderService = null)
        {
            return new ReferenceDataCachePopulationService(referenceDataCache, validationItemProviderService);
        }
    }
}
