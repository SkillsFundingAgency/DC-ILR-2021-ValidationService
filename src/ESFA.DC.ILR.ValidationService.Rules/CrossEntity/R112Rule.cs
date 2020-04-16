using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.CrossEntity
{
    public class R112Rule : AbstractRule, IRule<ILearner>
    {
        private readonly ILearningDeliveryFAMQueryService _learningDeliveryFAMQueryService;

        public R112Rule(
            IValidationErrorHandler validationErrorHandler,
            ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService)
            : base(validationErrorHandler, RuleNameConstants.R112)
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
                if (ExclusionConditionMet(learningDelivery.FundModel, learningDelivery.ProgTypeNullable))
                {
                    continue;
                }

                if (learningDelivery.LearnActEndDateNullable == null || learningDelivery.LearningDeliveryFAMs == null)
                {
                    continue;
                }

                var learningDeliveryFamToCheck = GetEligibleLearningDeliveryFam(learningDelivery.LearningDeliveryFAMs);

                if (learningDeliveryFamToCheck != null && ConditionMet(learningDelivery.FundModel, learningDeliveryFamToCheck, learningDelivery.LearnActEndDateNullable.Value))
                {
                    HandleValidationError(
                        learner.LearnRefNumber,
                        learningDelivery.AimSeqNumber,
                        BuildErrorMessageParameters(learningDelivery.LearnActEndDateNullable, LearningDeliveryFAMTypeConstants.ACT, learningDeliveryFamToCheck.LearnDelFAMDateToNullable));
                }
            }
        }

        public bool ConditionMet(int fundModel, ILearningDeliveryFAM learningDeliveryFam, DateTime learnActEndDate)
        {
            return FundModelConditionMet(fundModel) &&
             (!learningDeliveryFam.LearnDelFAMDateToNullable.HasValue
                || learningDeliveryFam.LearnDelFAMDateToNullable.Value != learnActEndDate);
        }

        public bool ExclusionConditionMet(int fundModel, int? progType)
        {
            return FundModelConditionMet(fundModel) && progType == TypeOfLearningProgramme.ApprenticeshipStandard;
        }

        public bool FundModelConditionMet(int fundModel)
        {
            return fundModel == TypeOfFunding.ApprenticeshipsFrom1May2017;
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(DateTime? learnActEndDateNullable, string learnDelFAMType, DateTime? learnDelFAMDateTo)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.LearnActEndDate, learnActEndDateNullable),
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, learnDelFAMType),
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMDateTo, learnDelFAMDateTo)
            };
        }

        private ILearningDeliveryFAM GetEligibleLearningDeliveryFam(IEnumerable<ILearningDeliveryFAM> learningDeliveryFams)
        {
            return _learningDeliveryFAMQueryService.GetLearningDeliveryFAMsForType(learningDeliveryFams, LearningDeliveryFAMTypeConstants.ACT)
                .Where(fam => fam.LearnDelFAMDateFromNullable.HasValue)
                .OrderByDescending(f => f.LearnDelFAMDateFromNullable)
                .FirstOrDefault();
        }
    }
}
