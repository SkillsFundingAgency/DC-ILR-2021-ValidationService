using System.Collections.Generic;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Interface;

namespace ESFA.DC.ILR.ValidationService.Data.Learner
{
    public class LearnerReferenceDataCache : ILearnerReferenceDataCache
    {
        public IReadOnlyDictionary<string, IEnumerable<ILearnerReferenceData>> Learners { get; set; }
    }
}
