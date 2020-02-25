using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.DateOfBirth
{
    public class DateOfBirth_23Rule : AbstractRule, IRule<ILearner>
    {
        private readonly ILearningDeliveryFAMQueryService _learningDeliveryFAMQueryService;

        public DateOfBirth_23Rule(ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService, IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.DateOfBirth_23)
        {
            _learningDeliveryFAMQueryService = learningDeliveryFAMQueryService;
        }

        public void Validate(ILearner objectToValidate)
        {
            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(objectToValidate.DateOfBirthNullable, learningDelivery))
                {
                    HandleValidationError(objectToValidate.LearnRefNumber, errorMessageParameters: BuildErrorMessageParameters(objectToValidate.DateOfBirthNullable));
                    return;
                }
            }
        }

        public bool ConditionMet(DateTime? dateOfBirth, ILearningDelivery learningDelivery)
        {
            return FundModelConditionMet(learningDelivery.FundModel)
                && DateOfBirthConditionMet(dateOfBirth)
                && LearningDeliveryFAMConditionMet(learningDelivery.LearningDeliveryFAMs);
        }

        public bool FundModelConditionMet(int fundModel)
        {
            return fundModel == 99;
        }

        public bool DateOfBirthConditionMet(DateTime? dateOfBirth)
        {
            return !dateOfBirth.HasValue;
        }

        public bool LearningDeliveryFAMConditionMet(IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs)
        {
            return _learningDeliveryFAMQueryService.HasLearningDeliveryFAMType(learningDeliveryFAMs, "ADL")
            && !_learningDeliveryFAMQueryService.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, "LDM", "034");
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(DateTime? dateOfBirth)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.DateOfBirth, dateOfBirth)
            };
        }
    }
}