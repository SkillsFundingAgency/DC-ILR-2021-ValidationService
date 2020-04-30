using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Data.File.FileData.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.CrossEntity
{
    public class R108Rule : AbstractRule, IRule<IMessage>
    {
        private readonly HashSet<int> _fundModels = new HashSet<int>()
        {
            FundModels.Age16To19ExcludingApprenticeships,
            FundModels.AdultSkills,
            FundModels.EuropeanSocialFund,
            FundModels.OtherAdult
        };

        private readonly HashSet<int?> _progTypes = new HashSet<int?>()
        {
            ProgTypes.Traineeship,
            ProgTypes.ApprenticeshipStandard
        };

        private readonly IFileDataService _fileDataService;

        public R108Rule(
            IValidationErrorHandler validationErrorHandler,
            IFileDataService fileDataService)
            : base(validationErrorHandler, RuleNameConstants.R108)
        {
            _fileDataService = fileDataService;
        }

        public void Validate(IMessage message)
        {
            if (message.Learners != null)
            {
                foreach (ILearner learner in message.Learners)
                {
                    DateTime? learnActEndDateLatest = null;

                    if (learner?.LearningDeliveries == null
                        || !FundModelConditionMet(learner.LearningDeliveries)
                        || !AllAimsClosedConditionMet(learner.LearningDeliveries)
                        || !CompStatusConditionMet(learner.LearningDeliveries, out learnActEndDateLatest)
                        || (!learnActEndDateLatest.HasValue
                            || (!FilePreparationDateConditionMet(learnActEndDateLatest.Value)
                                || !DPOutComeConditionMet(
                                    learner.LearnRefNumber,
                                    message?.LearnerDestinationAndProgressions,
                                    learnActEndDateLatest.Value))))
                    {
                        continue;
                    }

                    if (learner.LearningDeliveries.Any(ld => ProgTypeConditionMet(ld.ProgTypeNullable)))
                    {
                        HandleValidationError(learner.LearnRefNumber);
                    }
                }
            }
        }

        public bool ProgTypeConditionMet(int? progTypeNullable) => !_progTypes.Contains(progTypeNullable);

        public bool FundModelConditionMet(IReadOnlyCollection<ILearningDelivery> learningDeliveries) =>
            learningDeliveries.Any(ld => _fundModels.Contains(ld.FundModel));

        public bool AllAimsClosedConditionMet(IReadOnlyCollection<ILearningDelivery> learningDeliveries) =>
            learningDeliveries.All(ld => ld.LearnActEndDateNullable.HasValue);

        public bool CompStatusConditionMet(IReadOnlyCollection<ILearningDelivery> learningDeliveries, out DateTime? learnActEndDateLatest)
        {
            bool compStatusCondition = false;
            learnActEndDateLatest = null;

            ILearningDelivery learningDelivery = learningDeliveries
                .Where(ld => ld.LearnActEndDateNullable.HasValue)
                .OrderByDescending(ld => ld.LearnActEndDateNullable)
                .FirstOrDefault();
            if (learningDelivery != null)
            {
                learnActEndDateLatest = learningDelivery.LearnActEndDateNullable;
                compStatusCondition = learningDelivery.CompStatus != CompletionState.HasTemporarilyWithdrawn;
            }

            return compStatusCondition;
        }

        public bool FilePreparationDateConditionMet(DateTime learnActEndDateLatest) =>
            _fileDataService.FilePreparationDate() >= learnActEndDateLatest.AddMonths(2);

        public bool DPOutComeConditionMet(
            string learnRefNumber,
            IEnumerable<ILearnerDestinationAndProgression> lDAPs,
            DateTime learnActEndDateLatest) =>
             (lDAPs?
                    .Where(p => p.LearnRefNumber.CaseInsensitiveEquals(learnRefNumber) && p.DPOutcomes != null)
                    .SelectMany(p => p.DPOutcomes)
                    .Any(d => d.OutStartDate >= learnActEndDateLatest) ?? false) ? false : true;
    }
}
