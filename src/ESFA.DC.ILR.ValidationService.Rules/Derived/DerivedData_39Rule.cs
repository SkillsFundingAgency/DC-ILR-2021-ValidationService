using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Data.File.FileData.Interface;
using ESFA.DC.ILR.ValidationService.Data.Learner.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Derived
{
    public class DerivedData_39Rule : IDerivedData_39Rule
    {
        private readonly ILearnerReferenceDataService _learnerReferenceDataService;
        private readonly IFileDataService _fileDataService;

        public DerivedData_39Rule(ILearnerReferenceDataService learnerReferenceDataService, IFileDataService fileDataService)
        {
            _learnerReferenceDataService = learnerReferenceDataService;
            _fileDataService = fileDataService;
        }

        public ILearnerReferenceData GetMatchingLearningAimFromPreviousYear(ILearner learner, ILearningDelivery learningDelivery)
        {
            var learnRefNumbers = new List<string>
            {
                learner.LearnRefNumber,
            };

            if (learner.PrevLearnRefNumber != null)
            {
                learnRefNumbers.Add(learner.PrevLearnRefNumber);
            }

            var ukprns = new HashSet<long> { _fileDataService.UKPRN(), learner.PrevUKPRNNullable ?? 0, learner.PMUKPRNNullable ?? 0 };

            var previousLearnerData = _learnerReferenceDataService.GetLearnerDataForPreviousYear(learnRefNumbers);

            return previousLearnerData?.Where(x => HasMatch(ukprns, learner.ULN, learningDelivery, x)).FirstOrDefault();
        }

        public bool HasMatch(IEnumerable<long> currentUKPRNs, long uln, ILearningDelivery learningDelivery, ILearnerReferenceData previousLearnerData)
        {
            return uln == previousLearnerData.ULN
                   && HasUKPRNMatch(currentUKPRNs, previousLearnerData.UKPRN, previousLearnerData.PrevUKPRN, previousLearnerData.PMUKPRN)
                   && HasLearningDeliveryMatch(learningDelivery, previousLearnerData);
        }

        public bool HasUKPRNMatch(IEnumerable<long> currentUKPRNs, long UKPRN, long? prevUKPRN, long? pmUKPRN)
        {
            return currentUKPRNs.Contains(UKPRN)
                   || (prevUKPRN.HasValue ? currentUKPRNs.Contains(prevUKPRN.Value) : false)
                   || (pmUKPRN.HasValue ? currentUKPRNs.Contains(pmUKPRN.Value) : false);
        }

        public bool HasLearningDeliveryMatch(ILearningDelivery delivery, ILearnerReferenceData previousLearnerData)
        {
            return
                delivery.LearnAimRef.CaseInsensitiveEquals(previousLearnerData.LearnAimRef)
                && delivery.LearnStartDate == previousLearnerData.LearnStartDate
                && delivery.FundModel == previousLearnerData.FundModel
                && (!delivery.ProgTypeNullable.HasValue || delivery.ProgTypeNullable == previousLearnerData.ProgTypeNullable)
                && (!delivery.FworkCodeNullable.HasValue || delivery.FworkCodeNullable == previousLearnerData.FworkCodeNullable)
                && (!delivery.PwayCodeNullable.HasValue || delivery.PwayCodeNullable == previousLearnerData.PwayCodeNullable)
                && (!delivery.StdCodeNullable.HasValue || delivery.StdCodeNullable == previousLearnerData.StdCodeNullable);
        }
    }
}
