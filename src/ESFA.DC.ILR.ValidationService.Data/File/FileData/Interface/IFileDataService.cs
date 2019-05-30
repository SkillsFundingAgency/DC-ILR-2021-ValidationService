using System;
using ESFA.DC.ILR.ValidationService.Interface;

namespace ESFA.DC.ILR.ValidationService.Data.File.FileData.Interface
{
    public interface IFileDataService : IDataService
    {
        int UKPRN();

        DateTime FilePreparationDate();

        string FileName();

        int? FileNameUKPRN();
    }
}
