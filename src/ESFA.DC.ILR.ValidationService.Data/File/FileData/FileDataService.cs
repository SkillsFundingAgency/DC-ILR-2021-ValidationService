using System;
using ESFA.DC.ILR.ValidationService.Data.File.FileData.Interface;
using ESFA.DC.ILR.ValidationService.Data.Interface;

namespace ESFA.DC.ILR.ValidationService.Data.File.FileData
{
    public class FileDataService :
        IFileDataService
    {
        private readonly IFileDataCache _fileDataCache;

        public FileDataService(IFileDataCache fileDataCache)
        {
            _fileDataCache = fileDataCache;
        }

        public int UKPRN()
        {
            return _fileDataCache.UKPRN;
        }

        public DateTime FilePreparationDate()
        {
            return _fileDataCache.FilePreparationDate;
        }

        public string FileName()
        {
            return _fileDataCache.FileName;
        }

        public int? FileNameUKPRN()
        {
            return _fileDataCache.FileNameUKPRN;
        }
    }
}
