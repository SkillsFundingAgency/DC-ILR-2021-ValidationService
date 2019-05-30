using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Query.Interface
{
    public interface ILearnerDPQueryService : IQueryService
    {
        IDictionary<DateTime, IEnumerable<string>> OutTypesForStartDateAndTypes(IEnumerable<IDPOutcome> dpOutcomes, IEnumerable<string> outTypes);

        ILearnerDestinationAndProgression GetDestinationAndProgressionForLearner(string learnRefNumber);
    }
}
