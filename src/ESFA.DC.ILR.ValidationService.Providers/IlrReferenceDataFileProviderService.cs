using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.FileService.Interface;
using ESFA.DC.ILR.ReferenceDataService.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.Serialization.Interfaces;

namespace ESFA.DC.ILR.ValidationService.Providers
{
    public class IlrReferenceDataFileProviderService : IValidationItemProviderService<ReferenceDataRoot>
    {
        private readonly IJsonSerializationService _jsonSerializationService;
        private readonly IPreValidationContext _preValidationContext;
        private readonly IFileService _fileService;

        public IlrReferenceDataFileProviderService(
            IJsonSerializationService jsonSerializationService,
            IPreValidationContext preValidationContext,
            IFileService fileService)
        {
            _jsonSerializationService = jsonSerializationService;
            _preValidationContext = preValidationContext;
            _fileService = fileService;
        }

        public async Task<ReferenceDataRoot> ProvideAsync(CancellationToken cancellationToken)
        {
            using (var stream = await _fileService.OpenReadStreamAsync(_preValidationContext.IlrReferenceDataKey, _preValidationContext.Container, cancellationToken))
            {
                stream.Position = 0;

                return _jsonSerializationService.Deserialize<ReferenceDataRoot>(stream);
            }
        }
    }
}
