using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Internal.AcademicYear.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using System;
using System.Collections.Generic;
using System.Linq;

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
                    if (ConditionMet(learningDelivery.LearnStartDate, learningDelivery.PHoursNullable, learningDelivery.FundModel))
                    {
                        HandleValidationError(
                                 objectToValidate.LearnRefNumber,
                                 learningDelivery.AimSeqNumber,
                                 BuildErrorMessageParameters(learningDelivery.FundModel, learningDelivery.PHoursNullable));
                        return;
                    }
                }
            }
        }

        public bool ConditionMet(DateTime startDate, int? plannedHours, int fundModel)
        {
            return StartDateConditionMet(startDate)
                && PlannedHoursConditionMet(plannedHours)
                && FundModelConditionMet(fundModel);
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

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(int fundModel, int? plannedHours)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.FundModel, fundModel),
                BuildErrorMessageParameter(PropertyNameConstants.PHours, plannedHours)
            };
        }
    }
}
