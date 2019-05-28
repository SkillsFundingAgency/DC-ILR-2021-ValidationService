using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.FileService.Interface;
using ESFA.DC.ILR.Model;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.Serialization.Interfaces;

namespace ESFA.DC.ILR.ValidationService.Providers
{
    public class MessageFileProviderService : IValidationItemProviderService<IMessage>
    {
        private readonly IXmlSerializationService _xmlSerializationService;
        private readonly IValidationContext _preValidationContext;
        private readonly IFileService _fileService;

        public MessageFileProviderService(
            IXmlSerializationService xmlSerializationService,
            IValidationContext preValidationContext,
            IFileService fileService)
        {
            _xmlSerializationService = xmlSerializationService;
            _preValidationContext = preValidationContext;
            _fileService = fileService;
        }

        public async Task<IMessage> ProvideAsync(CancellationToken cancellationToken)
        {
            using (var stream = await _fileService.OpenReadStreamAsync(_preValidationContext.Filename, _preValidationContext.Container, cancellationToken))
            {
                stream.Position = 0;

                return _xmlSerializationService.Deserialize<Message>(stream);
            }
        }
    }
}
