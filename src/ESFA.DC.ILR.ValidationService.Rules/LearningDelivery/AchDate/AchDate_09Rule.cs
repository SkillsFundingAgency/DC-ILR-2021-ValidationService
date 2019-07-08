using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.AchDate
{
    public class AchDate_09Rule : AbstractRule, IRule<ILearner>
    {
        private readonly DateTime _learnStartDate = new DateTime(2015, 7, 31);

        public AchDate_09Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.AchDate_09)
        {
        }

        public AchDate_09Rule()
            : base(null, RuleNameConstants.AchDate_09)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(learningDelivery.AchDateNullable, learningDelivery.LearnStartDate, learningDelivery.AimType, learningDelivery.ProgTypeNullable, learningDelivery.FundModel))
                {
                    HandleValidationError(
                        objectToValidate.LearnRefNumber,
                        learningDelivery.AimSeqNumber,
                        BuildErrorMessageParameters(
                            learningDelivery.AimType,
                            learningDelivery.LearnStartDate,
                            learningDelivery.ProgTypeNullable,
                            learningDelivery.AchDateNullable));
                }
            }
        }

        public bool ConditionMet(DateTime? achDate, DateTime learnStartDate, int aimType, int? progType, int fundModel)
        {
            return AchDateConditionMet(achDate)
                   && LearnStartDateConditionMet(learnStartDate)
                   && ApprenticeshipConditionMet(aimType, progType, fundModel);
        }

        public virtual bool AchDateConditionMet(DateTime? achDate)
        {
            return achDate.HasValue;
        }

        public virtual bool LearnStartDateConditionMet(DateTime learnStartDate)
        {
            return learnStartDate > _learnStartDate;
        }

        public virtual bool ApprenticeshipConditionMet(int aimType, int? progType, int fundModel)
        {
            return !(aimType == 1 &&
                     (progType == 24 ||
                      (progType == 25 && fundModel == 81) || (progType == 25 && fundModel == 36)));
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(int aimType, DateTime learnStartDate, int? progType, DateTime? achDate)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.AimType, aimType),
                BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, learnStartDate),
                BuildErrorMessageParameter(PropertyNameConstants.ProgType, progType),
                BuildErrorMessageParameter(PropertyNameConstants.AchDate, achDate),
            };
        }
    }
}
