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
        private DateTime? _learnDelFAMDateTo = null;
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

            foreach (var delivery in learner.LearningDeliveries)
            {
                if (ConditionMet(delivery))
                {
                    HandleValidationError(
                                           learner.LearnRefNumber,
                                           delivery.AimSeqNumber,
                                           BuildErrorMessageParameters(
                                                                LearningDeliveryFAMTypeConstants.ACT,
                                                                _learnDelFAMDateTo,
                                                                delivery.AchDateNullable));
                }
            }
        }

        public bool ConditionMet(ILearningDelivery learningDelivery)
        {
            return FundModelConditionMet(learningDelivery.FundModel)
                && ProgTypeConditionMet(learningDelivery.ProgTypeNullable)
                && AchDateConditionMet(learningDelivery.AchDateNullable)
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

        public bool AchDateConditionMet(DateTime? achDate)
        {
            return achDate.HasValue;
        }
              
        public bool FAMDateConditionMet(IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs, DateTime? actEndDate)
        {
            var latestLearnDelFAMDate = learningDeliveryFAMs?
                                          .Where(f => f.LearnDelFAMType.CaseInsensitiveEquals(LearningDeliveryFAMTypeConstants.ACT)
                                          && f.LearnDelFAMDateFromNullable.HasValue)?
                                          .OrderByDescending(o => o.LearnDelFAMDateFromNullable)
                                          .FirstOrDefault();

            if (latestLearnDelFAMDate != null && latestLearnDelFAMDate.LearnDelFAMDateToNullable.HasValue)
            {
                _learnDelFAMDateTo = latestLearnDelFAMDate.LearnDelFAMDateToNullable;
               return actEndDate != latestLearnDelFAMDate.LearnDelFAMDateToNullable;
            }
            else
            {
                return false;
            }
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
