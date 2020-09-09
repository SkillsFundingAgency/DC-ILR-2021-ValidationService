using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.FileService.Interface;
using ESFA.DC.Serialization.Interfaces;

namespace ESFA.DC.ILR.ValidationService.Data.Population.FileProvider.Abstract
{
    public abstract class BaseFileProvider
    {
        private readonly ISerializationService _serializationService;
        private readonly IFileService _fileService;

        protected BaseFileProvider(
            ISerializationService serializationService,
            IFileService fileService)
        {
            _serializationService = serializationService;
            _fileService = fileService;
        }

        protected async Task<T> ProvideFileAsync<T>(string fileKey, string container, CancellationToken cancellationToken)
        {
            using (var stream = await _fileService.OpenReadStreamAsync(fileKey, container, cancellationToken))
            {
                return _serializationService.Deserialize<T>(stream);
            }
        }
    }
}
