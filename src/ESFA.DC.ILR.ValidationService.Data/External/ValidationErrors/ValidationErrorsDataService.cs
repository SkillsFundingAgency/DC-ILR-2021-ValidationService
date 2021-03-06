﻿using ESFA.DC.ILR.ValidationService.Data.External.ValidationErrors.Model;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Interface.Enum;

namespace ESFA.DC.ILR.ValidationService.Data.External.ValidationErrors
{
    public class ValidationErrorsDataService : IValidationErrorsDataService
    {
        private readonly Severity? _nullSeverity = null;
        private readonly IExternalDataCache _externalDataCache;

        public ValidationErrorsDataService(IExternalDataCache externalDataCache)
        {
            _externalDataCache = externalDataCache;
        }

        public string MessageforRuleName(string ruleName) => GetRule(ruleName)?.Message;

        public Severity? SeverityForRuleName(string ruleName) => GetRule(ruleName)?.Severity ?? _nullSeverity;

        private ValidationError GetRule(string ruleName)
        {
            ValidationError validationError = null;

            _externalDataCache.ValidationErrors?.TryGetValue(ruleName, out validationError);

            return validationError;
        }
    }
}
