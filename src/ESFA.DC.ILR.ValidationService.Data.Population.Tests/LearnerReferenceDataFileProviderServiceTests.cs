using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.FileService.Interface;
using ESFA.DC.ILR.ReferenceDataService.Model.Learner;
using ESFA.DC.ILR.ValidationService.Data.Population.FileProvider;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.Serialization.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Data.Population.Tests
{
    public class LearnerReferenceDataFileProviderServiceTests
    {
        [Fact]
        public async Task ProvideAsync()
        {
            var cancellationToken = CancellationToken.None;

            var validationContextMock = new Mock<IValidationContext>();

            var learnerReferenceDataKey = "key";
            var container = "Container";

            validationContextMock.SetupGet(c => c.LearnerReferenceDataKey).Returns(learnerReferenceDataKey);
            validationContextMock.SetupGet(c => c.Container).Returns(container);

            using (var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes("Stream")))
            {
                var referenceData = new LearnerReferenceData();

                var fileServiceeMock = new Mock<IFileService>();
                fileServiceeMock.Setup(sps => sps.OpenReadStreamAsync(learnerReferenceDataKey, container, cancellationToken)).ReturnsAsync(memoryStream);

                var jsonSerializationService = new Mock<IJsonSerializationService>();
                jsonSerializationService.Setup(s => s.Deserialize<LearnerReferenceData>(memoryStream)).Returns(referenceData);

                (await NewService(jsonSerializationService.Object, fileServiceeMock.Object).ProvideAsync(validationContextMock.Object, cancellationToken)).Should().BeSameAs(referenceData);
            }
        }

        private LearnerReferenceDataFileProviderService NewService(
            IJsonSerializationService jsonSerializationService = null,
            IFileService fileService = null)
        {
            return new LearnerReferenceDataFileProviderService(jsonSerializationService, fileService);
        }
    }
}
