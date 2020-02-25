using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using System;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.OrigLearnStartDate
{
    public class OrigLearnStartDate_09Rule : AbstractRule, IRule<ILearner>
    {
        private const int FundModel36 = 36;
        private readonly DateTime startDateConditionDateTime = new DateTime(2017, 5, 1);

        public OrigLearnStartDate_09Rule(
            IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.OrigLearnStartDate_09)
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
                        BuildErrorMessageParameters(learningDelivery.OrigLearnStartDateNullable));
                }
            }
        }

        public bool ConditionMet(DateTime? origLearnStartDate, int fundModel)
        {
            return OrigLearnStartDateConditionMet(origLearnStartDate)
                   && FundModelConditionMet(fundModel);
        }

        public bool OrigLearnStartDateConditionMet(DateTime? origLearnStartDate)
        {
            return origLearnStartDate.HasValue && origLearnStartDate < startDateConditionDateTime;
        }

        public bool FundModelConditionMet(int fundModel)
        {
            return fundModel == FundModel36;
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(DateTime? origLearnStartDate)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.OrigLearnStartDate, origLearnStartDate)
            };
        }
    }
}
