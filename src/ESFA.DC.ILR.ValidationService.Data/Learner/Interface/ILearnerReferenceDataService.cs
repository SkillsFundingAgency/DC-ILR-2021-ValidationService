using ESFA.DC.ILR.ValidationService.Interface;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Data.Learner.Interface
{
    public interface ILearnerReferenceDataService : IDataService
    {
        IEnumerable<ILearnerReferenceData> GetLearnerDataForPreviousYear(IEnumerable<string> learnRefNumbers);
    }
}
