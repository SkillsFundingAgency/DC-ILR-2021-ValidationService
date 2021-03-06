﻿using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.PlanLearnHours
{
    public class PlanLearnHours_03Rule : AbstractRule, IRule<ILearner>
    {
        public PlanLearnHours_03Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.PlanLearnHours_03)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            if (
                objectToValidate?.PlanLearnHoursNullable == null
                || !LearnerConditionMet(
                    objectToValidate.PlanLearnHoursNullable,
                    objectToValidate.PlanEEPHoursNullable)
                || objectToValidate.LearningDeliveries == null)
            {
                return;
            }

            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (!Excluded(learningDelivery.FundModel, learningDelivery.ProgTypeNullable) && ConditionMet(learningDelivery.FundModel))
                {
                    HandleValidationError(
                        objectToValidate.LearnRefNumber,
                        learningDelivery.AimSeqNumber,
                        BuildErrorMessageParameters(
                            objectToValidate.PlanLearnHoursNullable,
                            objectToValidate.PlanEEPHoursNullable,
                            learningDelivery.FundModel));
                }
            }
        }

        public bool ConditionMet(int fundModel) => FundModelConditionMet(fundModel);

        public bool LearnerConditionMet(int? planLearnHours, int? planEEPHours)
            => (planLearnHours ?? 0) + (planEEPHours ?? 0) == 0;

        public bool FundModelConditionMet(int fundModel) => fundModel == FundModels.Age16To19ExcludingApprenticeships;

        public bool Excluded(int fundModel, int? progType)
        {
            return fundModel == FundModels.Age16To19ExcludingApprenticeships
                   && progType == ProgTypes.TLevel;
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(
            int? planLearnHours,
            int? planEEPHours,
            int fundModel)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.PlanLearnHours, planLearnHours),
                BuildErrorMessageParameter(PropertyNameConstants.PlanEEPHours, planEEPHours),
                BuildErrorMessageParameter(PropertyNameConstants.FundModel, fundModel)
            };
        }
    }
}
