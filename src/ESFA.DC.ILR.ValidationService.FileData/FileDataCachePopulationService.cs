﻿using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.File;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Data.Population.Interface;

namespace ESFA.DC.ILR.ValidationService.FileData
{
    public class FileDataCachePopulationService : IFileDataCachePopulationService
    {
        private readonly IFileDataCache _fileDataCache;

        public FileDataCachePopulationService(IFileDataCache fileDataCache)
        {
            _fileDataCache = fileDataCache;
        }

        public void Populate(IMessage message)
        {
            var fileDataCache = (FileDataCache)_fileDataCache;

            fileDataCache.FilePreparationDate = message.HeaderEntity.CollectionDetailsEntity.FilePreparationDate;
        }
    }
}
