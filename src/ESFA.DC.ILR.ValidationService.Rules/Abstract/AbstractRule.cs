using ESFA.DC.ILR.ValidationService.Interface;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace ESFA.DC.ILR.ValidationService.Rules.Abstract
{
    public abstract class AbstractRule
    {
        /// <summary>
        /// The required culture (for date time formatting)
        /// </summary>
        public static readonly IFormatProvider RequiredCulture = new CultureInfo("en-GB");

        private readonly IValidationErrorHandler _validationErrorHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractRule"/> class.
        /// </summary>
        /// <param name="validationErrorHandler">The validation error handler.</param>
        /// <param name="ruleName">Name of the rule.</param>
        protected AbstractRule(IValidationErrorHandler validationErrorHandler, string ruleName)
        {
            // TODO: these need to be in place, but they can't be at present as they induce thousands of test errors...
            // It.IsNull(validationErrorHandler)
            //    .AsGuard<ArgumentNullException>(nameof(validationErrorHandler));
            // It.IsEmpty(ruleName)
            //   .AsGuard<ArgumentNullException>(nameof(ruleName));
            _validationErrorHandler = validationErrorHandler;
            RuleName = ruleName;
        }

        /// <summary>
        /// Gets the name of the rule.
        /// </summary>
        public string RuleName { get; }

        public static string AsRequiredCultureDate(DateTime? candidate) =>
            candidate?.ToString("d", RequiredCulture);

        /// <summary>
        /// Raises a validation message using a collection of message parameters.
        /// </summary>
        /// <param name="learnRefNumber">The learn reference number.</param>
        /// <param name="aimSequenceNumber">The aim sequence number.</param>
        /// <param name="errorMessageParameters">The error message parameters.</param>
        protected void HandleValidationError(string learnRefNumber = null, long? aimSequenceNumber = null, IEnumerable<IErrorMessageParameter> errorMessageParameters = null)
        {
            _validationErrorHandler.Handle(RuleName, learnRefNumber, aimSequenceNumber, errorMessageParameters);
        }

        /// <summary>
        /// Builds the error message parameter.
        /// </summary>
        /// <typeparam name="T">a parameter of type</typeparam>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="value">The value.</param>
        /// <returns>a date message parameter with the required culture</returns>
        protected IErrorMessageParameter BuildErrorMessageParameter<T>(string propertyName, T value)
        {
            return _validationErrorHandler.BuildErrorMessageParameter(propertyName, value);
        }

        /// <summary>
        /// Builds a date message parameter with the required culture.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="value">The value.</param>
        /// <returns>a date message parameter with the required culture</returns>
        protected IErrorMessageParameter BuildErrorMessageParameter(string propertyName, DateTime value)
        {
            return _validationErrorHandler.BuildErrorMessageParameter(propertyName, AsRequiredCultureDate(value));
        }

        /// <summary>
        /// Builds a nullable date message parameter with the required culture.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="value">The value.</param>
        /// <returns>a nullable date message parameter with the required culture</returns>
        protected IErrorMessageParameter BuildErrorMessageParameter(string propertyName, DateTime? value)
        {
            return _validationErrorHandler.BuildErrorMessageParameter(propertyName, AsRequiredCultureDate(value));
        }
    }
}
