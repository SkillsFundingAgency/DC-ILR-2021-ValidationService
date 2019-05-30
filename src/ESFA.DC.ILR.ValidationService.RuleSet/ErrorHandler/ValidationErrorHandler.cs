using System.Collections.Generic;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Interface.Enum;
using ESFA.DC.ILR.ValidationService.RuleSet.ErrorHandler.Model;

namespace ESFA.DC.ILR.ValidationService.RuleSet.ErrorHandler
{
    public class ValidationErrorHandler : IValidationErrorHandler
    {
        private readonly IValidationErrorCache<IValidationError> _validationErrorCache;
        private readonly IValidationErrorsDataService _validationErrorsDataService;

        public ValidationErrorHandler(IValidationErrorCache<IValidationError> validationErrorCache, IValidationErrorsDataService validationErrorsDataService)
        {
            _validationErrorCache = validationErrorCache;
            _validationErrorsDataService = validationErrorsDataService;
        }

        public void Handle(string ruleName, string learnRefNumber = null, long? aimSequenceNumber = null, IEnumerable<IErrorMessageParameter> errorMessageParameters = null)
        {
            var severity = _validationErrorsDataService.SeverityForRuleName(ruleName);

            _validationErrorCache.Add(BuildValidationError(ruleName, learnRefNumber, aimSequenceNumber, severity, errorMessageParameters));
        }

        public IValidationError BuildValidationError(string ruleName, string learnRefNumber, long? aimSequenceNumber, Severity? severity, IEnumerable<IErrorMessageParameter> errorMessageParameters)
        {
            return new ValidationError()
            {
                RuleName = ruleName,
                LearnerReferenceNumber = learnRefNumber,
                AimSequenceNumber = aimSequenceNumber,
                Severity = severity,
                ErrorMessageParameters = errorMessageParameters
            };
        }

        public IErrorMessageParameter BuildErrorMessageParameter(string propertyName, object value)
        {
            return new ErrorMessageParameter(propertyName, value?.ToString());
        }
    }
}