using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Utility;
using System;

namespace ESFA.DC.ILR.ValidationService.Rules.HE.TTACCOM
{
    /// <summary>
    /// from version 0.7.1 validation spread sheet
    /// these rules are singleton's; they can't hold state...
    /// </summary>
    /// <seealso cref="Interface.IRule{ILearner}" />
    public class TTACCOM_02Rule :
        IRule<ILearner>
    {
        /// <summary>
        /// Gets the name of the message property.
        /// </summary>
        public const string MessagePropertyName = "TTACCOM";

        /// <summary>
        /// Gets the name of the rule.
        /// </summary>
        public const string Name = RuleNameConstants.TTACCOM_02;

        /// <summary>
        /// The message handler
        /// </summary>
        private readonly IValidationErrorHandler _messageHandler;

        /// <summary>
        /// The lookup details (provider)
        /// </summary>
        private readonly IProvideLookupDetails _lookupDetails;

        /// <summary>
        /// The derived data06
        /// </summary>
        private readonly IDerivedData_06Rule _derivedData06;

        /// <summary>
        /// Initializes a new instance of the <see cref="TTACCOM_02Rule" /> class.
        /// </summary>
        /// <param name="validationErrorHandler">The validation error handler.</param>
        /// <param name="lookupDetails">The lookup details (provider).</param>
        /// <param name="derivedData06">The derived data 06.</param>
        public TTACCOM_02Rule(IValidationErrorHandler validationErrorHandler, IProvideLookupDetails lookupDetails, IDerivedData_06Rule derivedData06)
        {
            It.IsNull(validationErrorHandler)
                .AsGuard<ArgumentNullException>(nameof(validationErrorHandler));
            It.IsNull(lookupDetails)
                .AsGuard<ArgumentNullException>(nameof(lookupDetails));
            It.IsNull(derivedData06)
                .AsGuard<ArgumentNullException>(nameof(derivedData06));

            _messageHandler = validationErrorHandler;
            _lookupDetails = lookupDetails;
            _derivedData06 = derivedData06;
        }

        /// <summary>
        /// Gets the name of the rule.
        /// </summary>
        public string RuleName => Name;

        /// <summary>
        /// Validates the specified object.
        /// </summary>
        /// <param name="objectToValidate">The object to validate.</param>
        public void Validate(ILearner objectToValidate)
        {
            It.IsNull(objectToValidate)
                .AsGuard<ArgumentNullException>(nameof(objectToValidate));

            var learnRefNumber = objectToValidate.LearnRefNumber;
            var learnerHE = objectToValidate.LearnerHEEntity;
            var tTAccom = learnerHE?.TTACCOMNullable;
            var referenceDate = _derivedData06.Derive(objectToValidate.LearningDeliveries);
            var failedValidation = !ConditionMet(tTAccom, referenceDate);

            if (failedValidation)
            {
                RaiseValidationMessage(learnRefNumber, tTAccom.Value);
            }
        }

        /// <summary>
        /// Condition met.
        /// </summary>
        /// <param name="tTAccom">The term time accomodation (code).</param>
        /// <param name="referenceDate">The reference date.</param>
        /// <returns>
        /// true if any any point the conditions are met
        /// </returns>
        public bool ConditionMet(int? tTAccom, DateTime referenceDate)
        {
            if (!tTAccom.HasValue)
            { // No value is supplied, pass, only fail is this is a valid tTAccom with invalid date
                return true;
            }

            if (!_lookupDetails.Contains(TypeOfLimitedLifeLookup.TTAccom, tTAccom.Value))
            { // Not a valid value, pass, only fail is this is a valid tTAccom with invalid date
                return true;
            }

            // Check if the existing tTAccom is valid date wise.
            return _lookupDetails.IsCurrent(TypeOfLimitedLifeLookup.TTAccom, tTAccom.Value, referenceDate);
        }

        /// <summary>
        /// Raises the validation message.
        /// </summary>
        /// <param name="learnRefNumber">The learn reference number.</param>
        /// <param name="tTAccom">term time accomodation.</param>
        public void RaiseValidationMessage(string learnRefNumber, int tTAccom)
        {
            var parameters = Collection.Empty<IErrorMessageParameter>();
            parameters.Add(_messageHandler.BuildErrorMessageParameter(MessagePropertyName, tTAccom));

            _messageHandler.Handle(RuleName, learnRefNumber, null, parameters);
        }
    }
}
