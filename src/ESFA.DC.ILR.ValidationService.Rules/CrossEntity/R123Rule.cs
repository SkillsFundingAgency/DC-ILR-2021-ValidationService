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
                    var learnDelFAMDateToTuple = GetMaxLearnDelFAMDateTo(learningDelivery.LearningDeliveryFAMs);

                    HandleValidationError(
                        learner.LearnRefNumber,
                        learningDelivery.AimSeqNumber,
                        BuildErrorMessageParameters(
                            learnDelFAMDateToTuple.LearnDelFAMDateTo,
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
            var learnDelFAMDateToTuple = GetMaxLearnDelFAMDateTo(learningDeliveryFAMs);
            return learnDelFAMDateToTuple.Found && learnDelFAMDateToTuple.LearnDelFAMDateTo.HasValue;
        }

        public bool FAMTypeConditionMet(IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs)
        {
            return _learningDeliveryFAMQueryService.HasLearningDeliveryFAMType(learningDeliveryFAMs, LearningDeliveryFAMTypeConstants.ACT);
        }

        private (bool Found, DateTime? LearnDelFAMDateTo) GetMaxLearnDelFAMDateTo(IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs)
        {
            var learnDelFam = learningDeliveryFAMs?
                .Where(f => f.LearnDelFAMType.CaseInsensitiveEquals(LearningDeliveryFAMTypeConstants.ACT))
                .OrderByDescending(o => o.LearnDelFAMDateFromNullable)
                .FirstOrDefault();

            return learnDelFam != null ? (true, learnDelFam.LearnDelFAMDateToNullable) : (false, null);
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(DateTime? famDateTo, int compStatus)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMDateTo, famDateTo),
                BuildErrorMessageParameter(PropertyNameConstants.CompStatus, compStatus)
            };
        }
    }
}