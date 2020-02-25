using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Internal.AcademicYear.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.DateOfBirth
{
    public class DateOfBirth_13Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IAcademicYearDataService _academicYearDataService;
        private readonly IDateTimeQueryService _dateTimeQueryService;
        private readonly ILearningDeliveryFAMQueryService _learningDeliveryFAMQueryService;

        public DateOfBirth_13Rule(IAcademicYearDataService academicYearDataService, IDateTimeQueryService dateTimeQueryService, ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService, IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.DateOfBirth_13)
        {
            _academicYearDataService = academicYearDataService;
            _dateTimeQueryService = dateTimeQueryService;
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
            return dateOfBirth.HasValue
                && _dateTimeQueryService.YearsBetween(dateOfBirth.Value, _academicYearDataService.JulyThirtyFirst()) < 16;
        }

        public bool LearningDeliveryFAMConditionMet(IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs)
        {
            return _learningDeliveryFAMQueryService.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, "SOF", "1");
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