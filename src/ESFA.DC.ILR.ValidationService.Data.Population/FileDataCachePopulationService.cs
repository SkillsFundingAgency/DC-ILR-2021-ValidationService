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

        public FileDataCachePopulationService(IFileDataCache fileDataCache)
        {
            _fileDataCache = fileDataCache;
        }

        public void Populate(IValidationContext validationContext, IMessage message)
        {
            var fileDataCache = (FileDataCache)_fileDataCache;

            fileDataCache.FileName = validationContext.Filename;

            if (message != null)
            {
                fileDataCache.FilePreparationDate = message.HeaderEntity.CollectionDetailsEntity.FilePreparationDate;
                fileDataCache.UKPRN = message.LearningProviderEntity.UKPRN;
            }
        }
    }
}
