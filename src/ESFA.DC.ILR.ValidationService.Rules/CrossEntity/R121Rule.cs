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
    public class R121Rule : AbstractRule, IRule<ILearner>
    {
        private readonly ILearningDeliveryFAMQueryService _learningDeliveryFAMQueryService;

        public R121Rule(
                        ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService,
                        IValidationErrorHandler validationErrorHandler)
          : base(validationErrorHandler, RuleNameConstants.R121)
        {
            _learningDeliveryFAMQueryService = learningDeliveryFAMQueryService;
        }

        public void Validate(ILearner learner)
        {
            if (learner.LearningDeliveries == null)
            {
                return;
            }

            foreach (var learningDelivery in learner.LearningDeliveries)
            {
                if (ConditionMet(learningDelivery))
                {
                    var learnDelFAMDateTo = GetLearnDelFAMDate(learningDelivery.LearningDeliveryFAMs)?.LearnDelFAMDateToNullable;

                    HandleValidationError(
                                           learner.LearnRefNumber,
                                           learningDelivery.AimSeqNumber,
                                           BuildErrorMessageParameters(
                                                                LearningDeliveryFAMTypeConstants.ACT,
                                                                learnDelFAMDateTo,
                                                                learningDelivery.AchDateNullable));
                }
            }
        }

        public bool ConditionMet(ILearningDelivery learningDelivery)
        {
            return FundModelConditionMet(learningDelivery.FundModel)
                && ProgTypeConditionMet(learningDelivery.ProgTypeNullable)
                && ActEndDateIsKnown(learningDelivery.AchDateNullable)
                && FAMTypeConditionMet(learningDelivery.LearningDeliveryFAMs)
                && FAMDateConditionMet(learningDelivery.LearningDeliveryFAMs, learningDelivery.LearnActEndDateNullable);
        }

        public bool FundModelConditionMet(int fundModel)
        {
            return fundModel == TypeOfFunding.ApprenticeshipsFrom1May2017;
        }

        public bool ProgTypeConditionMet(int? progType)
        {
            return progType == TypeOfLearningProgramme.ApprenticeshipStandard;
        }

        public bool FAMTypeConditionMet(IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs)
        {
            return _learningDeliveryFAMQueryService.HasLearningDeliveryFAMType(learningDeliveryFAMs, LearningDeliveryFAMTypeConstants.ACT);
        }

        public bool ActEndDateIsKnown(DateTime? actEndDate)
        {
            return actEndDate.HasValue;
        }

        public bool FAMDateConditionMet(IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs, DateTime? actEndDate)
        {
            var learnDelFAMDateTo = GetLearnDelFAMDate(learningDeliveryFAMs)?.LearnDelFAMDateToNullable;
            return learnDelFAMDateTo.HasValue && actEndDate.HasValue && actEndDate != learnDelFAMDateTo;
        }

        private ILearningDeliveryFAM GetLearnDelFAMDate(IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs)
        {
            return learningDeliveryFAMs?
                        .Where(f => f.LearnDelFAMType.CaseInsensitiveEquals(LearningDeliveryFAMTypeConstants.ACT)
                        && f.LearnDelFAMDateFromNullable.HasValue)?
                        .OrderByDescending(o => o.LearnDelFAMDateFromNullable)
                        .FirstOrDefault();
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(string famType, DateTime? famDateTo, DateTime? achDate)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, famType),
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMDateTo, famDateTo),
                BuildErrorMessageParameter(PropertyNameConstants.AchDate, achDate)
            };
        }
    }
}
