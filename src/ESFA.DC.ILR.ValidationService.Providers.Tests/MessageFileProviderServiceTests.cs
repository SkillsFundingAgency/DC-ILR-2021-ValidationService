﻿using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.FileService.Interface;
using ESFA.DC.ILR.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Stateless.Models;
using ESFA.DC.Serialization.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Providers.Tests
{
    public class MessageFileProviderServiceTests
    {
        [Fact]
        public async Task ProvideAsync()
        {
            var cancellationToken = CancellationToken.None;
            
            var input = "ILR String";
            var container = "Container";

            var validationContextMock = new Mock<IPreValidationContext>();

            validationContextMock.SetupGet(c => c.Input).Returns(input);
            validationContextMock.SetupGet(c => c.Container).Returns(container);

            using (var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes("Stream")))
            {
                var message = new Message();

                var fileServiceeMock = new Mock<IFileService>();
                fileServiceeMock.Setup(sps => sps.OpenReadStreamAsync(input, container, cancellationToken)).ReturnsAsync(memoryStream);

                var xmlSerializationService = new Mock<IXmlSerializationService>();
                xmlSerializationService.Setup(s => s.Deserialize<Message>(memoryStream)).Returns(message);

                (await NewService(xmlSerializationService.Object, validationContextMock.Object, fileServiceeMock.Object).ProvideAsync(cancellationToken)).Should().BeSameAs(message);
            }
        }

        private MessageFileProviderService NewService(
            IXmlSerializationService xmlSerializationService = null,
            IPreValidationContext preValidationContext = null,
            IFileService fileService = null)
        {
            return new MessageFileProviderService(xmlSerializationService, preValidationContext, fileService);
        }
    }
}
