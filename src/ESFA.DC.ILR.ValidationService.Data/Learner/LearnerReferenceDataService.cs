using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Data.Learner.Interface;
using ESFA.DC.ILR.ValidationService.Interface;

namespace ESFA.DC.ILR.ValidationService.Data.Learner
{
    public class LearnerReferenceDataService : ILearnerReferenceDataService
    {
        private readonly ILearnerReferenceDataCache _learnerReferenceDataCache;

        public LearnerReferenceDataService(ILearnerReferenceDataCache learnerReferenceDataCache)
        {
            _learnerReferenceDataCache = learnerReferenceDataCache;
        }

        public IEnumerable<ILearnerReferenceData> GetLearnerDataForPreviousYear(IEnumerable<string> learnRefNumbers)
        {
            var learnerData = new List<ILearnerReferenceData>();

            foreach (var learnRefNumber in learnRefNumbers)
            {
                learnerData.AddRange(_learnerReferenceDataCache.Learners.GetValueOrDefault(learnRefNumber, Enumerable.Empty<ILearnerReferenceData>()));
            }

            return learnerData;
        }
    }
}
