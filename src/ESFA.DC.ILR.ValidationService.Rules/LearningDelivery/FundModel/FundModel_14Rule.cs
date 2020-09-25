using System;
using System.Collections.Generic;
using System.Text;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.FundModel
{
    public class FundModel_14Rule : AbstractRule, IRule<ILearner>
    {
        public FundModel_14Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.FundModel_14)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate.LearningDeliveries == null)
            {
                return;
            }

            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(learningDelivery.FundModel, learningDelivery.OtjActHoursNullable))
                {
                    HandleValidationError(objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber, BuildErrorMessageParameters(learningDelivery.FundModel, learningDelivery.OtjActHoursNullable));
                }
            }
        }

        public bool ConditionMet(int fundModel, int? otjActHours)
        {
            return otjActHours.HasValue && fundModel != FundModels.ApprenticeshipsFrom1May2017;
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(int fundModel, int? otjActHours)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.FundModel, fundModel),
                BuildErrorMessageParameter(PropertyNameConstants.OtjActHours, otjActHours),
            };
        }
    }
}
