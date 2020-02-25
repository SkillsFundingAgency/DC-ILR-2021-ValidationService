using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.DateOfBirth
{
    public class DateOfBirth_12Rule : AbstractRule, IRule<ILearner>
    {
        private readonly HashSet<string> _famCodes = new HashSet<string> { "1", "2" };

        private readonly IDateTimeQueryService _dateTimeQueryService;
        private readonly ILearningDeliveryFAMQueryService _learningDeliveryFAMQueryService;

        public DateOfBirth_12Rule(IDateTimeQueryService dateTimeQueryService, ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService, IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.DateOfBirth_12)
        {
            _dateTimeQueryService = dateTimeQueryService;
            _learningDeliveryFAMQueryService = learningDeliveryFAMQueryService;
        }

        public void Validate(ILearner objectToValidate)
        {
            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(objectToValidate.DateOfBirthNullable, learningDelivery))
                {
                    HandleValidationError(objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber, errorMessageParameters: BuildErrorMessageParameters(objectToValidate.DateOfBirthNullable, learningDelivery.FundModel));
                    return;
                }
            }
        }

        public bool ConditionMet(DateTime? dateOfBirth, ILearningDelivery learningDelivery)
        {
            return FundModelConditionMet(learningDelivery.FundModel)
                && DateOfBirthConditionMet(dateOfBirth, learningDelivery.LearnStartDate)
                && LearningDeliveryFAMConditionMet(learningDelivery.LearningDeliveryFAMs);
        }

        public bool FundModelConditionMet(int fundModel)
        {
            return fundModel == 10;
        }

        public bool DateOfBirthConditionMet(DateTime? dateOfBirth, DateTime learnStartDate)
        {
            return dateOfBirth.HasValue
                && _dateTimeQueryService.YearsBetween(dateOfBirth.Value, learnStartDate) < 19;
        }

        public bool LearningDeliveryFAMConditionMet(IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs)
        {
            return _learningDeliveryFAMQueryService.HasAnyLearningDeliveryFAMCodesForType(learningDeliveryFAMs, "ASL", _famCodes);
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(DateTime? dateOfBirth, int fundModel)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.DateOfBirth, dateOfBirth),
                BuildErrorMessageParameter(PropertyNameConstants.FundModel, fundModel)
            };
        }
    }
}