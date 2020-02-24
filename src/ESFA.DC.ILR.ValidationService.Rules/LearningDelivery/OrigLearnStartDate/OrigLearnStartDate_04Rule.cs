using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Utility;
using System;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.OrigLearnStartDate
{
    public class OrigLearnStartDate_04Rule :
        IRule<ILearner>
    {
        /// <summary>
        /// Gets the name of the message property.
        /// </summary>
        public const string MessagePropertyName = "OrigLearnStartDate";

        /// <summary>
        /// Gets the name of the rule.
        /// </summary>
        public const string Name = RuleNameConstants.OrigLearnStartDate_04;

        /// <summary>
        /// The message handler
        /// </summary>
        private readonly IValidationErrorHandler _messageHandler;

        private readonly HashSet<int> fundModels = new HashSet<int> { 35, 36, 81, 99 };

        /// <summary>
        /// Initializes a new instance of the <see cref="OrigLearnStartDate_04Rule" /> class.
        /// </summary>
        /// <param name="validationErrorHandler">The validation error handler.</param>
        public OrigLearnStartDate_04Rule(IValidationErrorHandler validationErrorHandler)
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
        /// Determines whether [has original learning start date] [the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [has original learning start date] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasOriginalLearningStartDate(ILearningDelivery delivery) =>
            It.Has(delivery.OrigLearnStartDateNullable);

        /// <summary>
        /// Determines whether [has restart indicator] [the specified monitor].
        /// </summary>
        /// <param name="monitor">The monitor.</param>
        /// <returns>
        ///   <c>true</c> if [has restart indicator] [the specified monitor]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasRestartIndicator(ILearningDeliveryFAM monitor) =>
            It.IsInRange(monitor.LearnDelFAMType, Monitoring.Delivery.Types.Restart);

        /// <summary>
        /// Determines whether [has restart indicator] [the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [has restart indicator] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasRestartIndicator(ILearningDelivery delivery) =>
            delivery.LearningDeliveryFAMs.SafeAny(HasRestartIndicator);

        /// <summary>
        /// Determines whether [is not valid] [the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [is not valid] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsNotValid(ILearningDelivery delivery) =>
            HasOriginalLearningStartDate(delivery) &&
            HasValidFundModel(delivery) &&
            !HasRestartIndicator(delivery);

        public bool HasValidFundModel(ILearningDelivery delivery) =>
            fundModels.Contains(delivery.FundModel);

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
                .SafeWhere(IsNotValid)
                .ForEach(x => RaiseValidationMessage(learnRefNumber, x));
        }

        /// <summary>
        /// Raises the validation message.
        /// </summary>
        /// <param name="learnRefNumber">The learn reference number.</param>
        /// <param name="thisDelivery">this delivery.</param>
        public void RaiseValidationMessage(string learnRefNumber, ILearningDelivery thisDelivery)
        {
            var parameters = Collection.Empty<IErrorMessageParameter>();
            parameters.Add(_messageHandler.BuildErrorMessageParameter(MessagePropertyName, thisDelivery.OrigLearnStartDateNullable.Value));

            _messageHandler.Handle(RuleName, learnRefNumber, thisDelivery.AimSeqNumber, parameters);
        }
    }
}
