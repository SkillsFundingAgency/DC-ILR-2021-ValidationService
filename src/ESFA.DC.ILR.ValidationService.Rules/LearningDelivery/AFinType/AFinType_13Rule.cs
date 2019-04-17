using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Utility;
using System;
using System.Linq;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.AFinType
{
    /// <summary>
    /// from version 0.7.1 validation spread sheet
    /// these rules are singleton's; they can't hold state...
    /// </summary>
    /// <seealso cref="Interface.IRule{ILearner}" />
    public class AFinType_13Rule :
        IRule<ILearner>
    {
        /// <summary>
        /// Gets the name of the message property.
        /// </summary>
        public const string MessagePropertyName = "AFINDATE";

        /// <summary>
        /// Gets the name of the rule.
        /// </summary>
        public const string Name = "AFinType_13";

        /// <summary>
        /// The message handler
        /// </summary>
        private readonly IValidationErrorHandler _messageHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="AFinType_13Rule"/> class.
        /// </summary>
        /// <param name="validationErrorHandler">The validation error handler.</param>
        public AFinType_13Rule(IValidationErrorHandler validationErrorHandler)
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
        /// Determines whether the specified delivery is apprencticeship.
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if the specified delivery is apprencticeship; otherwise, <c>false</c>.
        /// </returns>
        public bool IsApprenticeshipFunded(ILearningDelivery delivery) =>
            It.IsInRange(delivery.FundModel, TypeOfFunding.ApprenticeshipsFrom1May2017);

        /// <summary>
        /// Determines whether [is right fund model] [for the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [is right fund model] [for the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsInAProgramme(ILearningDelivery delivery) =>
            It.IsInRange(delivery.AimType, TypeOfAim.ProgrammeAim);

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
                .SafeWhere(d => IsApprenticeshipFunded(d) && IsInAProgramme(d))
                .ForEach(x =>
                {
                    var failedValidation = !x.AppFinRecords
                        .SafeWhere(afr => afr.AFinType.CaseInsensitiveEquals(ApprenticeshipFinancialRecord.Types.TotalNegotiatedPrice))
                        .Any(y => ConditionMet(x, y));

                    if (failedValidation)
                    {
                        RaiseValidationMessage(learnRefNumber, x);
                    }
                });
        }

        /// <summary>
        /// Condition met.
        /// </summary>
        /// <param name="thisDelivery">this learning delivery.</param>
        /// <param name="thisFinancialRecord">this financial record.</param>
        /// <returns>
        /// true if any any point the conditions are met
        /// </returns>
        public bool ConditionMet(ILearningDelivery thisDelivery, IAppFinRecord thisFinancialRecord)
        {
            return It.Has(thisDelivery) && It.Has(thisFinancialRecord)
                ? thisFinancialRecord.AFinDate > DateTime.MinValue
                    && thisDelivery.LearnStartDate == thisFinancialRecord.AFinDate
                : true;
        }

        /// <summary>
        /// Raises the validation message.
        /// </summary>
        /// <param name="learnRefNumber">The learn reference number.</param>
        /// <param name="thisDelivery">this learning delivery.</param>
        /// AFinDate is unknown so returns string.Empty.
        public void RaiseValidationMessage(string learnRefNumber, ILearningDelivery thisDelivery)
        {
            var parameters = Collection.Empty<IErrorMessageParameter>();
            parameters.Add(_messageHandler.BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, thisDelivery.LearnStartDate));
            parameters.Add(_messageHandler.BuildErrorMessageParameter(PropertyNameConstants.AFinType, ApprenticeshipFinancialRecord.Types.TotalNegotiatedPrice));
            parameters.Add(_messageHandler.BuildErrorMessageParameter(PropertyNameConstants.AFinDate, string.Empty));

            _messageHandler.Handle(RuleName, learnRefNumber, thisDelivery.AimSeqNumber, parameters);
        }
    }
}
