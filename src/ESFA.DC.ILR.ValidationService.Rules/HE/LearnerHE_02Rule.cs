using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ESFA.DC.ILR.ValidationService.Rules.HE
{
    /// <summary>
    /// from version 0.7.1 validation spread sheet
    /// these rules are singleton's; they can't hold state...
    /// </summary>
    /// <seealso cref="Interface.IRule{ILearner}" />
    public class LearnerHE_02Rule :
            IRule<ILearner>
    {
        /// <summary>
        /// Gets the name of the message property.
        /// </summary>
        public const string MessagePropertyName = "LearnerHE";

        /// <summary>
        /// Gets the name of the rule.
        /// </summary>
        public const string Name = RuleNameConstants.LearnerHE_02;

        /// <summary>
        /// The message handler
        /// </summary>
        private readonly IValidationErrorHandler _messageHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="LearnerHE_02Rule"/> class.
        /// </summary>
        /// <param name="validationErrorHandler">The validation error handler.</param>
        public LearnerHE_02Rule(IValidationErrorHandler validationErrorHandler)
        {
            It.IsNull(validationErrorHandler)
                .AsGuard<ArgumentNullException>(nameof(validationErrorHandler));

            _messageHandler = validationErrorHandler;
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
            var learningDeliveries = objectToValidate.LearningDeliveries;

            var failedValidation = !ConditionMet(learnerHE, learningDeliveries);

            if (failedValidation)
            {
                RaiseValidationMessage(learnRefNumber, learnerHE);
            }
        }

        /// <summary>
        /// Condition met.
        /// </summary>
        /// <param name="learnerHE">The learner he.</param>
        /// <param name="learningDeliveries">The learning deliveries.</param>
        /// <returns>true if any any point the conditions are met</returns>
        public bool ConditionMet(ILearnerHE learnerHE, IReadOnlyCollection<ILearningDelivery> learningDeliveries)
        {
            return It.Has(learnerHE)
                ? It.HasValues(learningDeliveries) && learningDeliveries.Any(d => It.Has(d.LearningDeliveryHEEntity))
                : true;
        }

        /// <summary>
        /// Raises the validation message.
        /// </summary>
        /// <param name="learnRefNumber">The learn reference number.</param>
        /// <param name="learnerHE">The learner HE.</param>
        public void RaiseValidationMessage(string learnRefNumber, ILearnerHE learnerHE)
        {
            var parameters = Collection.Empty<IErrorMessageParameter>();
            parameters.Add(_messageHandler.BuildErrorMessageParameter(MessagePropertyName, learnerHE));

            _messageHandler.Handle(RuleName, learnRefNumber, null, parameters);
        }
    }
}
