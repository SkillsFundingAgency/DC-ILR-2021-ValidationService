using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.FileService.Interface;
using ESFA.DC.ILR.Model;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Providers.Output;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Serialization.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Providers.Tests
{
    public class ValidIlrFileOutputServiceTests
    {
        [Fact]
        public async Task ProcessAsync()
        {
            IMessage ilrMessage = new Message
            {
                Learner = new MessageLearner[]
                {
                    new MessageLearner
                    {
                        LearnRefNumber = "Learner1"
                    },
                    new MessageLearner
                    {
                        LearnRefNumber = "Learner2"
                    },
                     new MessageLearner
                    {
                        LearnRefNumber = "Learner3"
                    }
                }
            };
            IEnumerable<string> serializedValidLearners = new List<string>
            {
                "Learner1",
                "Learner2"
            };
           
            var fileNameKeyTight = "FileName-Tight";
            var fileNameKeyValid = "FileName-Valid";
            var validLearnRefNumbersKey = "Valid Learn Ref Numbers Key";
            var container = "Container";

            var xmlSerializationServiceMock = new Mock<IXmlSerializationService>();
            var jsonSerializationServiceMock = new Mock<IJsonSerializationService>();
            var validationContextMock = new Mock<IValidationContext>();
            var fileServiceMock = new Mock<IFileService>();


            jsonSerializationServiceMock.Setup(s => s.Deserialize<IEnumerable<string>>(It.IsAny<Stream>())).Returns(serializedValidLearners);
            xmlSerializationServiceMock.Setup(s => s.Serialize(ilrMessage, It.IsAny<Stream>())).Verifiable();

            validationContextMock.SetupGet(c => c.ValidLearnRefNumbersKey).Returns(validLearnRefNumbersKey);
            validationContextMock.SetupProperty(c => c.Filename, fileNameKeyTight);
            validationContextMock.SetupGet(c => c.Container).Returns(container);

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes("Stream")))
            {
                fileServiceMock.Setup(s => s.OpenReadStreamAsync(validationContextMock.Object.ValidLearnRefNumbersKey, validationContextMock.Object.Container, default(CancellationToken))).ReturnsAsync(stream);
                fileServiceMock.Setup(s => s.OpenWriteStreamAsync(validationContextMock.Object.Filename, validationContextMock.Object.Container, default(CancellationToken))).ReturnsAsync(stream);

                var service = NewService(fileServiceMock.Object, xmlSerializationServiceMock.Object, jsonSerializationServiceMock.Object);

                await service.ProcessAsync(validationContextMock.Object, ilrMessage, CancellationToken.None);
            }

            validationContextMock.Object.Filename.Should().Be(fileNameKeyValid);
            xmlSerializationServiceMock.VerifyAll();
            validationContextMock.VerifyAll();
        }

        private ValidIlrFileOutputService NewService(
            IFileService fileService = null,
            IXmlSerializationService xmlSerializationService = null,
            IJsonSerializationService jsonSerializationService = null,
            IValidationErrorsDataService validationErrorsDataService = null
            )
        {
            return new ValidIlrFileOutputService(
                fileService,
                xmlSerializationService,
                jsonSerializationService,
                new Mock<ILogger>().Object);
        }
    }
}
