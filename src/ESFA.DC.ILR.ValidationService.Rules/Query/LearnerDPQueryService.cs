using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Query
{
    public class LearnerDPQueryService : ILearnerDPQueryService
    {
        private readonly ICache<IMessage> _messageCache;

        public LearnerDPQueryService(ICache<IMessage> messageCache)
        {
            _messageCache = messageCache;
        }

        public IDictionary<DateTime, IEnumerable<string>> OutTypesForStartDateAndTypes(IEnumerable<IDPOutcome> dpOutcomes, IEnumerable<string> outTypes)
        {
            if (dpOutcomes != null && outTypes != null)
            {
                if (dpOutcomes.Any(dp => outTypes.Contains(dp.OutType)))
                {
                    return dpOutcomes
                    .GroupBy(g => g.OutStartDate)
                    .ToDictionary(k => k.Key, v => v.Select(o => o.OutType));
                }
            }

            return null;
        }

        public ILearnerDestinationAndProgression GetDestinationAndProgressionForLearner(string learnRefNumber)
        {
            return _messageCache.Item.LearnerDestinationAndProgressions
                .FirstOrDefault(ldp => ldp.LearnRefNumber.CaseInsensitiveEquals(learnRefNumber));
        }
    }
}
