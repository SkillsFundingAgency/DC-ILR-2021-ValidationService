using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.FileService.Interface;
using ESFA.DC.ILR.ReferenceDataService.Model.Learner;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.Serialization.Interfaces;

namespace ESFA.DC.ILR.ValidationService.Data.Population.FileProvider
{
    public class LearnerReferenceDataFileProviderService : IProvider<LearnerReferenceData>
    {
        private readonly IJsonSerializationService _jsonSerializationService;
        private readonly IFileService _fileService;

        public LearnerReferenceDataFileProviderService(
            IJsonSerializationService jsonSerializationService,
            IFileService fileService)
        {
            _jsonSerializationService = jsonSerializationService;
            _fileService = fileService;
        }

        public async Task<LearnerReferenceData> ProvideAsync(IValidationContext validationContext, CancellationToken cancellationToken)
        {
            using (var stream = await _fileService.OpenReadStreamAsync(validationContext.LearnerReferenceDataKey, validationContext.Container, cancellationToken))
            {
                return _jsonSerializationService.Deserialize<LearnerReferenceData>(stream);
            }
        }
    }
}
