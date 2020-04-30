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
    public class R113Rule : AbstractRule, IRule<ILearner>
    {
        private readonly ILearningDeliveryFAMQueryService _learningDeliveryFAMQueryService;

        public R113Rule(
            ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService,
            IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.R113)
        {
            _learningDeliveryFAMQueryService = learningDeliveryFAMQueryService;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate?.LearningDeliveries == null)
            {
                return;
            }

            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                // Excluding condition
                if (FundModelConditionMet(learningDelivery.FundModel) && ProgTypeConditionMet(learningDelivery.ProgTypeNullable))
                {
                    continue;
                }

                if (ConditionMet(
                               learningDelivery.FundModel,
                               learningDelivery.LearnActEndDateNullable,
                               learningDelivery.LearningDeliveryFAMs))
                {
                    var learnDelFAMDateTo = GetLearnDelFAMDate(learningDelivery.LearningDeliveryFAMs)?.LearnDelFAMDateToNullable;

                    HandleValidationError(
                                    objectToValidate.LearnRefNumber,
                                    learningDelivery.AimSeqNumber,
                                    BuildErrorMessageParameters(
                                                             learningDelivery.LearnActEndDateNullable,
                                                             LearningDeliveryFAMTypeConstants.ACT,
                                                             learnDelFAMDateTo));
                }
            }
        }

        public bool ConditionMet(int fundModel, DateTime? actEndDate, IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs)
        {
            return FundModelConditionMet(fundModel)
                && LearnActEndDateNotKnown(actEndDate)
                && ContractTypeConditionMet(learningDeliveryFAMs)
                && FAMDateConditionMet(learningDeliveryFAMs);
        }

        public bool FundModelConditionMet(int fundModel)
        {
            return fundModel == FundModels.ApprenticeshipsFrom1May2017;
        }

        public bool ProgTypeConditionMet(int? progType)
        {
            return progType == ProgTypes.ApprenticeshipStandard;
        }

        public bool LearnActEndDateNotKnown(DateTime? actEndDate)
        {
            return actEndDate == null;
        }

        public bool ContractTypeConditionMet(IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs)
        {
            return _learningDeliveryFAMQueryService.HasLearningDeliveryFAMType(learningDeliveryFAMs, LearningDeliveryFAMTypeConstants.ACT);
        }

        public bool FAMDateConditionMet(IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs)
        {
            var learnDelFAMDateTo = GetLearnDelFAMDate(learningDeliveryFAMs)?.LearnDelFAMDateToNullable;
            return learnDelFAMDateTo.HasValue;
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(
            DateTime? learnActEndDateNullable,
            string learnDelFAMType,
            DateTime? learnDelFAMDateTo)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.LearnActEndDate, learnActEndDateNullable),
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, learnDelFAMType),
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMDateTo, learnDelFAMDateTo)
            };
        }

        private ILearningDeliveryFAM GetLearnDelFAMDate(IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs)
        {
            return learningDeliveryFAMs?
                        .Where(f => f.LearnDelFAMType.CaseInsensitiveEquals(LearningDeliveryFAMTypeConstants.ACT)
                        && f.LearnDelFAMDateFromNullable.HasValue)?
                        .OrderByDescending(o => o.LearnDelFAMDateFromNullable)
                        .FirstOrDefault();
        }
    }
}
