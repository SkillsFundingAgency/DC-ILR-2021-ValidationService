using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.File;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Data.Population.Interface;
using ESFA.DC.ILR.ValidationService.Interface;

namespace ESFA.DC.ILR.ValidationService.Data.Population
{
    public class FileDataCachePopulationService : IFileDataCachePopulationService
    {
        private readonly IFileDataCache _fileDataCache;
        private readonly ICache<IMessage> _messageCache;

        public FileDataCachePopulationService(IFileDataCache fileDataCache, ICache<IMessage> messageCache)
        {
            _fileDataCache = fileDataCache;
            _messageCache = messageCache;
        }

        public async Task PopulateAsync(IValidationContext validationContext, CancellationToken cancellationToken)
        {
            var fileDataCache = (FileDataCache)_fileDataCache;

            var message = _messageCache.Item;
            if (message != null)
            {
                fileDataCache.FilePreparationDate = message.HeaderEntity.CollectionDetailsEntity.FilePreparationDate;
                fileDataCache.UKPRN = message.LearningProviderEntity.UKPRN;
            }
        }
    }
}
