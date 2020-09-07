using System.Collections.Generic;
using ESFA.DC.ILR.ValidationService.Interface;

namespace ESFA.DC.ILR.ValidationService.Data.Interface
{
    public interface ILearnerReferenceDataCache
    {
        IReadOnlyDictionary<string, IEnumerable<ILearnerReferenceData>> Learners { get; }
    }
}
