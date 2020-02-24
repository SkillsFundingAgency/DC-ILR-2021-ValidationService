using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Utility;
using System;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.OrigLearnStartDate
{
    public class OrigLearnStartDate_02Rule :
        IRule<ILearner>
    {
        /// <summary>
        /// Gets the name of the message property.
        /// </summary>
        public const string MessagePropertyName = "OrigLearnStartDate";

        /// <summary>
        /// Gets the name of the rule.
        /// </summary>
        public const string Name = RuleNameConstants.OrigLearnStartDate_02;

        /// <summary>
        /// The message handler
        /// </summary>
        private readonly IValidationErrorHandler _messageHandler;

        /// <summary>
        /// The fund models
        /// </summary>
        private readonly HashSet<int> fundModels = new HashSet<int>
        {
            TypeOfFunding.AdultSkills,
            TypeOfFunding.ApprenticeshipsFrom1May2017,
            TypeOfFunding.OtherAdult,
            TypeOfFunding.NotFundedByESFA
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="OrigLearnStartDate_02Rule" /> class.
        /// </summary>
        /// <param name="validationErrorHandler">The validation error handler.</param>
        public OrigLearnStartDate_02Rule(IValidationErrorHandler validationErrorHandler)
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
        /// Determines whether [has qualifying dates] [the specified delivery].
        /// must be less than the learn start date
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [has qualifying dates] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasQualifyingDates(ILearningDelivery delivery) =>
            It.IsBetween(delivery.OrigLearnStartDateNullable.Value, DateTime.MinValue, delivery.LearnStartDate, false);

        /// <summary>
        /// Determines whether [has valid fund model] [the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [has valid fund model] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasValidFundModel(ILearningDelivery delivery) =>
            fundModels.Contains(delivery.FundModel);

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
            !HasQualifyingDates(delivery);

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
            parameters.Add(_messageHandler.BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, thisDelivery.LearnStartDate));
            parameters.Add(_messageHandler.BuildErrorMessageParameter(MessagePropertyName, thisDelivery.OrigLearnStartDateNullable.Value));

            _messageHandler.Handle(RuleName, learnRefNumber, thisDelivery.AimSeqNumber, parameters);
        }
    }
}
