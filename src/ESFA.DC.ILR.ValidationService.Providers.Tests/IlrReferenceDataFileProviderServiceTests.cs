using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.FileService.Interface;
using ESFA.DC.ILR.ReferenceDataService.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Stateless.Models;
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

            var preValidationContext = new PreValidationContext()
            {
                IlrReferenceDataKey = "ILR RD String",
                Container = "Container"
            };

            MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes("Stream"));
            var referenceData = new ReferenceDataRoot();

            var fileServiceeMock = new Mock<IFileService>();
            fileServiceeMock.Setup(sps => sps.OpenReadStreamAsync(preValidationContext.IlrReferenceDataKey, preValidationContext.Container, cancellationToken)).ReturnsAsync(memoryStream);
            
            var jsonSerializationService = new Mock<IJsonSerializationService>();
            jsonSerializationService.Setup(s => s.Deserialize<ReferenceDataRoot>(memoryStream)).Returns(referenceData);

            (await NewService(jsonSerializationService.Object, preValidationContext, fileServiceeMock.Object).ProvideAsync(cancellationToken)).Should().BeSameAs(referenceData);
        }

        private IlrReferenceDataFileProviderService NewService(
            IJsonSerializationService jsonSerializationService = null,
            IPreValidationContext preValidationContext = null,
            IFileService fileService = null)
        {
            return new IlrReferenceDataFileProviderService(jsonSerializationService, preValidationContext, fileService);
        }
    }
}
