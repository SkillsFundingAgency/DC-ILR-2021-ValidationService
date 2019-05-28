using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.FileService.Interface;
using ESFA.DC.ILR.ReferenceDataService.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.Serialization.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Providers.Tests
{
    public class IlrReferenceDataFileProviderServiceTests
    {
        [Fact]
        public async Task ProvideAsync()
        {
            var cancellationToken = CancellationToken.None;

            var validationContextMock = new Mock<IValidationContext>();

            var ilrReferenceDataKey = "ILR RD String";
            var container = "Container";

            validationContextMock.SetupGet(c => c.IlrReferenceDataKey).Returns(ilrReferenceDataKey);
            validationContextMock.SetupGet(c => c.Container).Returns(container);

            using (var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes("Stream")))
            {
                var referenceData = new ReferenceDataRoot();

                var fileServiceeMock = new Mock<IFileService>();
                fileServiceeMock.Setup(sps => sps.OpenReadStreamAsync(ilrReferenceDataKey, container, cancellationToken)).ReturnsAsync(memoryStream);

                var jsonSerializationService = new Mock<IJsonSerializationService>();
                jsonSerializationService.Setup(s => s.Deserialize<ReferenceDataRoot>(memoryStream)).Returns(referenceData);

                (await NewService(jsonSerializationService.Object, validationContextMock.Object, fileServiceeMock.Object).ProvideAsync(cancellationToken)).Should().BeSameAs(referenceData);
            }
        }

        private IlrReferenceDataFileProviderService NewService(
            IJsonSerializationService jsonSerializationService = null,
            IValidationContext preValidationContext = null,
            IFileService fileService = null)
        {
            return new IlrReferenceDataFileProviderService(jsonSerializationService, preValidationContext, fileService);
        }
    }
}
