using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.Outcome
{
    public class Outcome_08Rule : AbstractRule, IRule<ILearner>
    {
        private readonly ILearnerDPQueryService _learnerDpQueryService;
        private readonly DateTime _ruleStartDate = new DateTime(2015, 8, 1);

        private readonly int[] _employedOutcomeCodes =
        {
            DPOutcomeCodeConstants.EMP_PaidEmployment16PlusHours,
            DPOutcomeCodeConstants.EMP_SelfEmployed16PlusHours
        };

        private readonly int[] _educationOutcomeCodes =
        {
            DPOutcomeCodeConstants.EDU_Apprenticeship,
            DPOutcomeCodeConstants.EDU_OtherFEFullTime,
            DPOutcomeCodeConstants.EDU_OtherFEPartTime
        };

        public Outcome_08Rule(
            ILearnerDPQueryService learnerDpQueryService,
            IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.Outcome_08)
        {
            _learnerDpQueryService = learnerDpQueryService;
        }

        public void Validate(ILearner learner)
        {
            if (learner?.LearningDeliveries == null)
            {
                return;
            }

            foreach (var learningDelivery in learner.LearningDeliveries)
            {
                if (learningDelivery.LearnStartDate < _ruleStartDate)
                {
                    continue;
                }

                if (learningDelivery.AimType != AimTypes.ProgrammeAim ||
                    ((learningDelivery.ProgTypeNullable ?? -1) != TypeOfLearningProgramme.Traineeship) ||
                    (learningDelivery.OutcomeNullable ?? -1) != OutcomeConstants.Achieved)
                {
                    continue;
                }

                if (MatchingDpOutcome(learner.LearnRefNumber, learningDelivery.LearnActEndDateNullable))
                {
                    continue;
                }

                HandleValidationError(
                        learner.LearnRefNumber,
                        learningDelivery.AimSeqNumber,
                        BuildErrorMessageParameters(learningDelivery));
            }
        }

        private bool MatchingDpOutcome(string learnRefNum, DateTime? endDate)
        {
            return _learnerDpQueryService.GetDestinationAndProgressionForLearner(learnRefNum)?.DPOutcomes
                ?.Any(dp => ((dp.OutType.CaseInsensitiveEquals(OutTypeConstants.PaidEmployment) && _employedOutcomeCodes.Contains(dp.OutCode)) ||
                             (dp.OutType.CaseInsensitiveEquals(OutTypeConstants.Education) && _educationOutcomeCodes.Contains(dp.OutCode)))
                            && dp.OutStartDate < (endDate?.AddMonths(6) ?? DateTime.MaxValue)) ?? false;
        }

        private IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(ILearningDelivery learningDelivery)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, learningDelivery.LearnStartDate),
                BuildErrorMessageParameter(PropertyNameConstants.ProgType, learningDelivery.ProgTypeNullable),
                BuildErrorMessageParameter(PropertyNameConstants.LearnActEndDate, learningDelivery.LearnActEndDateNullable),
                BuildErrorMessageParameter(PropertyNameConstants.Outcome, learningDelivery.OutcomeNullable)
            };
        }
    }
}
