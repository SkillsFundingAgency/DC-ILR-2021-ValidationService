using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Data.Population.Interface;
using ESFA.DC.ILR.ValidationService.Data.Learner;
using ESFA.DC.ILR.ValidationService.Data.Learner.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using LearnerReferenceDataJson = ESFA.DC.ILR.ReferenceDataService.Model.Learner.LearnerReferenceData;

namespace ESFA.DC.ILR.ValidationService.Data.Population
{
    public class LearnerReferenceDataCachePopulationService : ILearnerReferenceDataCachePopulationService
    {
        private readonly ILearnerReferenceDataCache _cache;

        public LearnerReferenceDataCachePopulationService(ILearnerReferenceDataCache cache)
        {
            _cache = cache;
        }

        public void Populate(LearnerReferenceDataJson learnerReferenceData)
        {
            var cache = (LearnerReferenceDataCache)_cache;

            cache.Learners = BuildDictionary(learnerReferenceData);
        }

        public virtual IReadOnlyDictionary<string, IEnumerable<ILearnerReferenceData>> BuildDictionary(LearnerReferenceDataJson learnerReferenceData)
        {
            var referenceData = new LearnerReferenceData();

            if (learnerReferenceData == null)
            {
                return new Dictionary<string, IEnumerable<ILearnerReferenceData>>();
            }

            var dictionary = learnerReferenceData?
                .Learners?
                .GroupBy(x => x.LearnRefNumber)
                .ToDictionary(
                    k => k.Key,
                    v => v.Select(l => (ILearnerReferenceData)new LearnerReferenceData
                    {
                        LearnRefNumber = l.LearnRefNumber,
                        UKPRN = l.UKPRN,
                        PMUKPRN = l.PMUKPRN,
                        PrevUKPRN = l.PrevUKPRN,
                        ULN = l.ULN,
                        PrevLearnRefNumber = l.PrevLearnRefNumber,
                        LearnAimRef = l.LearnAimRef,
                        ProgTypeNullable = l.ProgTypeNullable,
                        StdCodeNullable = l.StdCodeNullable,
                        FworkCodeNullable = l.FworkCodeNullable,
                        PwayCodeNullable = l.PwayCodeNullable,
                        LearnStartDate = l.LearnStartDate,
                        LearnActEndDate = l.LearnActEndDate,
                        FundModel = l.FundModel,
                    }), StringComparer.OrdinalIgnoreCase);

           return dictionary;
        }
    }
}
