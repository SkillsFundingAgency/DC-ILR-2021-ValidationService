using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ESFA.DC.ILR.ValidationService.Rules.HE.FinancialSupport.FINTYPE
{
    /// <summary>
    /// from version 0.7.1 validation spread sheet
    /// these rules are singleton's; they can't hold state...
    /// </summary>
    /// <seealso cref="Interface.IRule{ILearner}" />
    public class FINTYPE_02Rule :
        IRule<ILearner>
    {
        /// <summary>
        /// Gets the name of the message property.
        /// </summary>
        public const string MessagePropertyName = "FINTYPE";

        /// <summary>
        /// Gets the name of the rule.
        /// </summary>
        public const string Name = "FINTYPE_02";

        /// <summary>
        /// The message handler
        /// </summary>
        private readonly IValidationErrorHandler _messageHandler;

        /// <summary>
        /// The lookup details (provider)
        /// </summary>
        private readonly IProvideLookupDetails _lookupDetails;

        /// <summary>
        /// Initializes a new instance of the <see cref="FINTYPE_02Rule" /> class.
        /// </summary>
        /// <param name="validationErrorHandler">The validation error handler.</param>
        /// <param name="lookupDetails">The lookup details.</param>
        public FINTYPE_02Rule(IValidationErrorHandler validationErrorHandler, IProvideLookupDetails lookupDetails)
        {
            It.IsNull(validationErrorHandler)
                .AsGuard<ArgumentNullException>(nameof(validationErrorHandler));
            It.IsNull(lookupDetails)
                .AsGuard<ArgumentNullException>(nameof(lookupDetails));

            _messageHandler = validationErrorHandler;
            _lookupDetails = lookupDetails;
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
            var financialSupport = learnerHE?.LearnerHEFinancialSupports;

            var failedValidation = !ConditionMet(financialSupport);

            if (failedValidation)
            {
                RaiseValidationMessage(learnRefNumber, financialSupport);
            }
        }

        /// <summary>
        /// Condition met.
        /// </summary>
        /// <param name="financialSupport">The financial support.</param>
        /// <returns>
        /// true if any any point the conditions are met
        /// </returns>
        public bool ConditionMet(IReadOnlyCollection<ILearnerHEFinancialSupport> financialSupport)
        {
            return It.HasValues(financialSupport)
                ? _lookupDetails.Get(TypeOfIntegerCodedLookup.FinType).All(x => financialSupport.Count(y => y.FINTYPE == x) <= 1)
                : true;
        }

        /// <summary>
        /// Raises the validation message.
        /// </summary>
        /// <param name="learnRefNumber">The learn reference number.</param>
        /// <param name="financialSupport">The financial support.</param>
        public void RaiseValidationMessage(string learnRefNumber, IReadOnlyCollection<ILearnerHEFinancialSupport> financialSupport)
        {
            var parameters = Collection.Empty<IErrorMessageParameter>();
            parameters.Add(_messageHandler.BuildErrorMessageParameter(MessagePropertyName, financialSupport));

            _messageHandler.Handle(RuleName, learnRefNumber, null, parameters);
        }
    }
}
