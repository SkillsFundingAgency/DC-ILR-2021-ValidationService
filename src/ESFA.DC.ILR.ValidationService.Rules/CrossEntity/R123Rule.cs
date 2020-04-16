using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.CrossEntity
{
    public class R123Rule : AbstractRule, IRule<ILearner>
    {
        private readonly ILearningDeliveryFAMQueryService _learningDeliveryFAMQueryService;

        public R123Rule(ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService, IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.R123)
        {
            _learningDeliveryFAMQueryService = learningDeliveryFAMQueryService;
        }

        public void Validate(ILearner learner)
        {
            if (learner?.LearningDeliveries == null)
            {
                return;
            }

            foreach (var learningDelivery in learner.LearningDeliveries)
            {
                if (ConditionMet(learningDelivery))
                {
                    var learnDelFAMDateTo = GetMaxLearnDelFAMDateTo(learningDelivery.LearningDeliveryFAMs);

                    HandleValidationError(
                        learner.LearnRefNumber,
                        learningDelivery.AimSeqNumber,
                        BuildErrorMessageParameters(
                            learnDelFAMDateTo,
                            learningDelivery.CompStatus));
                }
            }
        }

        public bool ConditionMet(ILearningDelivery learningDelivery)
        {
            return FundModelConditionMet(learningDelivery.FundModel)
                   && ProgTypeConditionMet(learningDelivery.ProgTypeNullable)
                   && CompStatusConditionMet(learningDelivery.CompStatus)
                   && FAMTypeConditionMet(learningDelivery.LearningDeliveryFAMs)
                   && FAMDateConditionMet(learningDelivery.LearningDeliveryFAMs);
        }

        public bool FundModelConditionMet(int fundModel) => fundModel == TypeOfFunding.ApprenticeshipsFrom1May2017;

        public bool ProgTypeConditionMet(int? progType) => progType == TypeOfLearningProgramme.ApprenticeshipStandard;

        public bool CompStatusConditionMet(int compStatus) => compStatus == CompletionState.IsOngoing;

        public bool FAMDateConditionMet(IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs)
        {
            var learnDelFAMDateTo = GetMaxLearnDelFAMDateTo(learningDeliveryFAMs);
            return learnDelFAMDateTo.HasValue;
        }

        public bool FAMTypeConditionMet(IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs)
        {
            return _learningDeliveryFAMQueryService.HasLearningDeliveryFAMType(learningDeliveryFAMs, LearningDeliveryFAMTypeConstants.ACT);
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(DateTime? famDateTo, int compStatus)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMDateTo, famDateTo),
                BuildErrorMessageParameter(PropertyNameConstants.CompStatus, compStatus)
            };
        }

        private DateTime? GetMaxLearnDelFAMDateTo(IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs)
        {
            return learningDeliveryFAMs?
                .Where(f => f.LearnDelFAMType.CaseInsensitiveEquals(LearningDeliveryFAMTypeConstants.ACT))
                .OrderByDescending(o => o.LearnDelFAMDateFromNullable)
                .FirstOrDefault()
                .LearnDelFAMDateToNullable;
        }
    }
}