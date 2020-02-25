using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.OrigLearnStartDate
{
    public class OrigLearnStartDate_03Rule : AbstractRule, IRule<ILearner>
    {
        private readonly HashSet<int> fundModels = new HashSet<int> { 25, 82, 10 };

        public OrigLearnStartDate_03Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.OrigLearnStartDate_03)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate?.LearningDeliveries == null)
            {
                return;
            }

            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(
                    learningDelivery.OrigLearnStartDateNullable,
                    learningDelivery.FundModel))
                {
                    HandleValidationError(
                        objectToValidate.LearnRefNumber,
                        learningDelivery.AimSeqNumber,
                        BuildErrorMessageParameters(learningDelivery.LearnStartDate, learningDelivery.FundModel));
                }
            }
        }

        public bool ConditionMet(DateTime? originalLearnStartDate, int fundModel)
        {
            return FundModelConditionMet(fundModel) &&
                   OriginalLearnStartDateConditionMet(originalLearnStartDate);
        }

        public bool OriginalLearnStartDateConditionMet(DateTime? originalLearnStartDate)
        {
            return originalLearnStartDate.HasValue;
        }

        public bool FundModelConditionMet(int fundModel)
        {
            return fundModels.Contains(fundModel);
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(DateTime? originalLearnStartDate, int fundModel)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.OrigLearnStartDate, originalLearnStartDate),
                BuildErrorMessageParameter(PropertyNameConstants.FundModel, fundModel)
            };
        }
    }
}
