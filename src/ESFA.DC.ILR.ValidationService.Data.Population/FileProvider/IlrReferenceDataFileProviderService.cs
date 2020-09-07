using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.FileService.Interface;
using ESFA.DC.ILR.ReferenceDataService.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.Serialization.Interfaces;

namespace ESFA.DC.ILR.ValidationService.Data.Population.FileProvider
{
    public class IlrReferenceDataFileProviderService : IProvider<ReferenceDataRoot>
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
                return _jsonSerializationService.Deserialize<ReferenceDataRoot>(stream);
            }
        }
    }
}
