using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnDelFAMDateFrom
{
    public class LearnDelFAMDateFrom_02Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IEnumerable<int> _fundModels = new HashSet<int>()
        {
            FundModels.AdultSkills,
            FundModels.ApprenticeshipsFrom1May2017,
            FundModels.OtherAdult,
            FundModels.NotFundedByESFA,
        };

        public LearnDelFAMDateFrom_02Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.LearnDelFAMDateFrom_02)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (learningDelivery.LearningDeliveryFAMs != null && FundModelConditionMet(learningDelivery.FundModel))
                {
                    foreach (var learningDeliveryFam in learningDelivery.LearningDeliveryFAMs)
                    {
                        if (ConditionMet(learningDeliveryFam.LearnDelFAMDateFromNullable, learningDelivery.LearnStartDate))
                        {
                            HandleValidationError(objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber, BuildErrorMessageParameters(learningDelivery.LearnStartDate, learningDelivery.FundModel, learningDeliveryFam.LearnDelFAMDateFromNullable));
                        }
                    }
                }
            }
        }

        public bool ConditionMet(DateTime? learnDelFamDateFrom, DateTime learnStartDate)
        {
            return learnDelFamDateFrom.HasValue && learnDelFamDateFrom < learnStartDate;
        }

        public bool FundModelConditionMet(int fundModel)
        {
            return _fundModels.Contains(fundModel);
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(DateTime learnStartDate, int fundModel, DateTime? learnDelFamDateFrom)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, learnStartDate),
                BuildErrorMessageParameter(PropertyNameConstants.FundModel, fundModel),
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMDateFrom, learnDelFamDateFrom)
            };
        }
    }
}
