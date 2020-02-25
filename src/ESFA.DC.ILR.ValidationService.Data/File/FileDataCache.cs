using ESFA.DC.ILR.ValidationService.Data.Interface;
using System;

namespace ESFA.DC.ILR.ValidationService.Data.File
{
    public class FileDataCache : IFileDataCache
    {
        public DateTime FilePreparationDate { get; set; }

        public int UKPRN { get; set; }

        public string FileName { get; set; }

        public int? FileNameUKPRN
        {
            get
            {
                try
                {
                    return int.Parse(FileName.Split('-')[1]);
                }
                catch (Exception)
                {
                }

                return null;
            }
        }
    }
}
