using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using System;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnPlanEndDate
{
    public class LearnPlanEndDate_02Rule : AbstractRule, IRule<ILearner>
    {
        public LearnPlanEndDate_02Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.LearnPlanEndDate_02)
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
                    learningDelivery.LearnPlanEndDate,
                    learningDelivery.LearnStartDate))
                {
                    HandleValidationError(
                        objectToValidate.LearnRefNumber,
                        learningDelivery.AimSeqNumber,
                        BuildErrorMessageParameters(learningDelivery.LearnPlanEndDate, learningDelivery.LearnStartDate));
                }
            }
        }

        public bool ConditionMet(DateTime learnPlanEndDate, DateTime learnStartDate)
        {
            return learnPlanEndDate < learnStartDate;
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(DateTime learnPlanEndDate, DateTime learnStartDate)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.LearnPlanEndDate, learnPlanEndDate),
                BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, learnStartDate)
            };
        }
    }
}
