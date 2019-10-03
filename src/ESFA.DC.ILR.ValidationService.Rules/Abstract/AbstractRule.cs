using System;
using System.Collections.Generic;
using System.Globalization;
using ESFA.DC.ILR.ValidationService.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Abstract
{
    public abstract class AbstractRule
    {
        public static readonly IFormatProvider RequiredCulture = new CultureInfo("en-GB");

        private readonly IValidationErrorHandler _validationErrorHandler;

        protected AbstractRule(IValidationErrorHandler validationErrorHandler, string ruleName)
        {
            _validationErrorHandler = validationErrorHandler;
            RuleName = ruleName;
        }

        public string RuleName { get; }

        public static string AsRequiredCultureDate(DateTime? candidate) =>
            candidate?.ToString("d", RequiredCulture);

        protected void HandleValidationError(string learnRefNumber = null, long? aimSequenceNumber = null, IEnumerable<IErrorMessageParameter> errorMessageParameters = null)
        {
            _validationErrorHandler.Handle(RuleName, learnRefNumber, aimSequenceNumber, errorMessageParameters);
        }

        protected IErrorMessageParameter BuildErrorMessageParameter<T>(string propertyName, T value)
        {
            return _validationErrorHandler.BuildErrorMessageParameter(propertyName, value);
        }

        protected IErrorMessageParameter BuildErrorMessageParameter(string propertyName, DateTime value)
        {
            return _validationErrorHandler.BuildErrorMessageParameter(propertyName, AsRequiredCultureDate(value));
        }

        protected IErrorMessageParameter BuildErrorMessageParameter(string propertyName, DateTime? value)
        {
            return _validationErrorHandler.BuildErrorMessageParameter(propertyName, AsRequiredCultureDate(value));
        }
    }
}
