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
        private readonly IFileService _fileService;

        public IlrReferenceDataFileProviderService(
            IJsonSerializationService jsonSerializationService,
            IFileService fileService)
        {
            _jsonSerializationService = jsonSerializationService;
            _fileService = fileService;
        }

        public async Task<ReferenceDataRoot> ProvideAsync(IValidationContext validationContext, CancellationToken cancellationToken)
        {
            using (var stream = await _fileService.OpenReadStreamAsync(validationContext.IlrReferenceDataKey, validationContext.Container, cancellationToken))
            {
                stream.Position = 0;

                return _jsonSerializationService.Deserialize<ReferenceDataRoot>(stream);
            }
        }
    }
}
