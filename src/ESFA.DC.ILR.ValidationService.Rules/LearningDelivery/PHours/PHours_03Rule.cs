using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.PHours
{
    public class PHours_03Rule : AbstractRule, IRule<ILearner>
    {
        private readonly DateTime _firstAugust2020 = new DateTime(2020, 08, 01);

        public PHours_03Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.PHours_03)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate.LearningDeliveries != null)
            {
                foreach (var learningDelivery in objectToValidate.LearningDeliveries)
                {
                    if (ConditionMet(learningDelivery.LearnStartDate, learningDelivery.PHoursNullable, learningDelivery.FundModel, learningDelivery.AimType, learningDelivery.ProgTypeNullable))
                    {
                        HandleValidationError(
                            objectToValidate.LearnRefNumber,
                            learningDelivery.AimSeqNumber,
                            BuildErrorMessageParameters(learningDelivery.FundModel, learningDelivery.PHoursNullable, learningDelivery.AimType));
                        return;
                    }
                }
            }
        }

        public bool ConditionMet(DateTime startDate, int? plannedHours, int fundModel, int aimType, int? progType)
        {
            return AimTypeConditionMet(aimType)
                   && StartDateConditionMet(startDate)
                   && PlannedHoursConditionMet(plannedHours)
                   && TLevelConditionMet(fundModel, progType);
        }

        public bool AimTypeConditionMet(int aimType)
        {
            return aimType == AimTypes.ProgrammeAim;
        }

        public bool StartDateConditionMet(DateTime startDate)
        {
            return startDate >= _firstAugust2020;
        }

        public bool PlannedHoursConditionMet(int? plannedHours)
        {
            return plannedHours == null;
        }

        public bool TLevelConditionMet(int fundModel, int? progType)
        {
            return progType.HasValue
                   && fundModel == FundModels.Age16To19ExcludingApprenticeships
                   && (progType == ProgTypes.TLevel || progType == ProgTypes.TLevelTransition);
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(int fundModel, int? plannedHours, int aimType)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.FundModel, fundModel),
                BuildErrorMessageParameter(PropertyNameConstants.PHours, plannedHours),
                BuildErrorMessageParameter(PropertyNameConstants.AimType, aimType)
            };
        }
    }
}
