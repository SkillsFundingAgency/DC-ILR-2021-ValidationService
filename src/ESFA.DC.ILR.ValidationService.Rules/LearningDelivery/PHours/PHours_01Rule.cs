using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.PHours
{
    public class PHours_01Rule : AbstractRule, IRule<ILearner>
    {
        private readonly DateTime _firstAugust2019 = new DateTime(2019, 08, 01);

        private readonly IEnumerable<int> _fundModels = new HashSet<int>()
        {
            TypeOfFunding.ApprenticeshipsFrom1May2017
        };

        public PHours_01Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.PHours_01)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate.LearningDeliveries != null)
            {
                foreach (var learningDelivery in objectToValidate.LearningDeliveries)
                {
                    if (ConditionMet(learningDelivery.LearnStartDate, learningDelivery.PHoursNullable, learningDelivery.FundModel, learningDelivery.AimType))
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

        public bool ConditionMet(DateTime startDate, int? plannedHours, int fundModel, int aimType)
        {
            return StartDateConditionMet(startDate)
                && PlannedHoursConditionMet(plannedHours)
                && FundModelConditionMet(fundModel)
                && AimTypeConditionMet(aimType);
        }

        public bool StartDateConditionMet(DateTime startDate)
        {
            return startDate >= _firstAugust2019;
        }

        public bool PlannedHoursConditionMet(int? plannedHours)
        {
            return plannedHours == null;
        }

        public bool FundModelConditionMet(int fundModel)
        {
            return _fundModels.Contains(fundModel);
        }

        public bool AimTypeConditionMet(int aimType)
        {
            return aimType == 1;
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
