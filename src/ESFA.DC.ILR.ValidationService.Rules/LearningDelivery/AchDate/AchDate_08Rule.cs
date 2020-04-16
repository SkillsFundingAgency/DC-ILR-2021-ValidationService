using System;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.AchDate
{
    public class AchDate_08Rule : AbstractRule, IRule<ILearner>
    {
        public AchDate_08Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.AchDate_08)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(
                    learningDelivery.ProgTypeNullable,
                    learningDelivery.FundModel,
                    learningDelivery.AimType,
                    learningDelivery.OutcomeNullable,
                    learningDelivery.AchDateNullable))
                {
                    HandleValidationError(objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber);
                }
            }
        }

        public bool ConditionMet(int? progType, int fundModel, int aimType, int? outcome, DateTime? achDate)
        {
            return ProgTypeConditionMet(progType, fundModel)
                   && AimTypeConditionMet(aimType)
                   && OutcomeConditionMet(outcome)
                   && AchDateConditionMet(achDate);
        }

        public bool ProgTypeConditionMet(int? progType, int fundModel)
        {
            return progType == 24 || (progType == 25 && fundModel == 81);
        }

        public bool AimTypeConditionMet(int aimType)
        {
            return aimType == 1;
        }

        public bool OutcomeConditionMet(int? outcome)
        {
            return outcome == 1;
        }

        public bool AchDateConditionMet(DateTime? achDate)
        {
            return !achDate.HasValue;
        }
    }
}
