using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.FileService.Interface;
using ESFA.DC.ILR.Model;
using ESFA.DC.ILR.ValidationService.Data.Population.FileProvider;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.Serialization.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Data.Population.Tests
{
    public class JsonFileProviderServiceTests
    {
        [Fact]
        public async Task ProvideAsync()
        {
            var cancellationToken = CancellationToken.None;

            var input = "ILR String";
            var container = "Container";

            var validationContextMock = new Mock<IValidationContext>();

            validationContextMock.SetupGet(c => c.Filename).Returns(input);
            validationContextMock.SetupGet(c => c.Container).Returns(container);

            using (var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes("Stream")))
            {
                var message = new Message();

                var fileServiceeMock = new Mock<IFileService>();
                fileServiceeMock.Setup(sps => sps.OpenReadStreamAsync(input, container, cancellationToken)).ReturnsAsync(memoryStream);

                var jsonSerializationService = new Mock<IJsonSerializationService>();
                jsonSerializationService.Setup(s => s.Deserialize<Message>(memoryStream)).Returns(message);

                (await NewService(jsonSerializationService.Object, fileServiceeMock.Object).ProvideAsync(input, container, cancellationToken)).Should().BeSameAs(message);
            }
        }

        private JsonFileProviderService<Message> NewService(
            IJsonSerializationService jsonSerializationService = null,
            IFileService fileService = null)
        {
            return new JsonFileProviderService<Message>(jsonSerializationService, fileService);
        }
    }
}
