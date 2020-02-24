using System;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Utility;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.ProgType
{
    /// <summary>
    /// from version 1.1 validation spread sheet
    /// these rules are singleton's; they can't hold state...
    /// </summary>
    /// <seealso cref="Interface.IRule{ILearner}" />
    public class ProgType_03Rule :
        IRule<ILearner>
    {
        /// <summary>
        /// Gets the name of the message property.
        /// </summary>
        public const string MessagePropertyName = "ProgType";

        /// <summary>
        /// Gets the name of the rule.
        /// </summary>
        public const string Name = RuleNameConstants.ProgType_03;

        /// <summary>
        /// The message handler
        /// </summary>
        private readonly IValidationErrorHandler _messageHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProgType_03Rule"/> class.
        /// </summary>
        /// <param name="validationErrorHandler">The validation error handler.</param>
        public ProgType_03Rule(IValidationErrorHandler validationErrorHandler)
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

            objectToValidate.LearningDeliveries
                .SafeWhere(x => It.Has(x.ProgTypeNullable))
                .ForEach(x =>
                {
                    var failedValidation = !ConditionMet(x);

                    if (failedValidation)
                    {
                        RaiseValidationMessage(learnRefNumber, x);
                    }
                });
        }

        /// <summary>
        /// Condition met.
        /// as this juncture the prog type cannot be null...
        /// </summary>
        /// <param name="thisDelivery">this delivery.</param>
        /// <returns>
        /// true if any any point the conditions are met
        /// </returns>
        public bool ConditionMet(ILearningDelivery thisDelivery)
        {
            return It.Has(thisDelivery)
                ? It.Has(thisDelivery.ProgTypeNullable) && TypeOfLearningProgramme.AsASet.Contains((int)thisDelivery.ProgTypeNullable)
                : true;
        }

        /// <summary>
        /// Raises the validation message.
        /// </summary>
        /// <param name="learnRefNumber">The learn reference number.</param>
        /// <param name="thisDelivery">this delivery.</param>
        public void RaiseValidationMessage(string learnRefNumber, ILearningDelivery thisDelivery)
        {
            var parameters = Collection.Empty<IErrorMessageParameter>();
            parameters.Add(_messageHandler.BuildErrorMessageParameter(PropertyNameConstants.ProgType, thisDelivery.ProgTypeNullable));
            parameters.Add(_messageHandler.BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, thisDelivery.LearnStartDate));

            _messageHandler.Handle(RuleName, learnRefNumber, thisDelivery.AimSeqNumber, parameters);
        }
    }
}
