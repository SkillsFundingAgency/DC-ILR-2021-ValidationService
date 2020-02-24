using System;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Utility;

namespace ESFA.DC.ILR.ValidationService.Rules.HE.DOMICILE
{
    public class DOMICILE_01Rule :
        IRule<ILearner>
    {
        /// <summary>
        /// Gets the name of the message property.
        /// </summary>
        public const string MessagePropertyName = "DOMICILE";

        /// <summary>
        /// Gets the name of the rule.
        /// </summary>
        public const string Name = RuleNameConstants.Domicile_01;

        /// <summary>
        /// The message handler
        /// </summary>
        private readonly IValidationErrorHandler _messageHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="DOMICILE_01Rule"/> class.
        /// </summary>
        /// <param name="validationErrorHandler">The validation error handler.</param>
        public DOMICILE_01Rule(
            IValidationErrorHandler validationErrorHandler)
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
        /// Gets the last inviable date.
        /// </summary>
        public DateTime LastInviableDate => new DateTime(2013, 07, 31);

        /// <summary>
        /// Determines whether [is qualifying start date] [the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [is qualifying start date] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsQualifyingStartDate(ILearningDelivery delivery) =>
            delivery.LearnStartDate > LastInviableDate;

        /// <summary>
        /// Determines whether [has higher ed] [the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [has higher ed] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasHigherEd(ILearningDelivery delivery) =>
            It.Has(delivery.LearningDeliveryHEEntity);

        /// <summary>
        /// Determines whether the specified the HE has a domicile.
        /// </summary>
        /// <param name="he">The higher ed.</param>
        /// <returns>
        ///   <c>true</c> if the specified he has domicile; otherwise, <c>false</c>.
        /// </returns>
        public bool HasDomicile(ILearningDeliveryHE he) =>
            It.Has(he.DOMICILE);

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
                .SafeWhere(x => IsQualifyingStartDate(x) && HasHigherEd(x) && !HasDomicile(x.LearningDeliveryHEEntity))
                .ForEach(x =>
                {
                    RaiseValidationMessage(learnRefNumber, x);
                });
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

            _messageHandler.Handle(RuleName, learnRefNumber, thisDelivery.AimSeqNumber, parameters);
        }
    }
}