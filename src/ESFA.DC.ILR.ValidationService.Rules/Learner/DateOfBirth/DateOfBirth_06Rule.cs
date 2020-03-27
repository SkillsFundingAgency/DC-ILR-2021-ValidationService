using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Internal.AcademicYear.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.DateOfBirth
{
    public class DateOfBirth_06Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IDateTimeQueryService _dateTimeQueryService;
        private readonly IAcademicYearDataService _academicYearDataService;

        private readonly IEnumerable<long> _fundModels = new HashSet<long>() { 25, 82 };

        public DateOfBirth_06Rule(IDateTimeQueryService dateTimeQueryService, IAcademicYearDataService academicYearDataService, IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.DateOfBirth_06)
        {
            _dateTimeQueryService = dateTimeQueryService;
            _academicYearDataService = academicYearDataService;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate.LearningDeliveries != null)
            {
                foreach (var learningDelivery in objectToValidate.LearningDeliveries)
                {
                    if (ConditionMet(objectToValidate.DateOfBirthNullable, _academicYearDataService.AugustThirtyFirst(), learningDelivery.FundModel))
                    {
                        HandleValidationError(objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber, errorMessageParameters: BuildErrorMessageParameters(objectToValidate.DateOfBirthNullable));
                        return;
                    }
                }
            }
        }

        public bool ConditionMet(DateTime? dateOfBirth, DateTime academicYearAugustThirtyFirst, int fundModel)
        {
            return _fundModels.Contains(fundModel)
                && dateOfBirth.HasValue
                && _dateTimeQueryService.YearsBetween(dateOfBirth.Value, academicYearAugustThirtyFirst) < 13;
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(DateTime? dateOfBirth)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.DateOfBirth, dateOfBirth),
            };
        }
    }
}