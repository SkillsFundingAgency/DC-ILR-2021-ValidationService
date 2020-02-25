using System;

namespace ESFA.DC.ILR.ValidationService.Data.Interface
{
    public interface IFileDataCache
    {
        DateTime FilePreparationDate { get; }

        string FileName { get; set; }

        int? FileNameUKPRN { get; }

        int UKPRN { get; }
    }
}
