using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Utility;
using System;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnAimRef
{
    public class LearnAimRef_87Rule :
        IRule<ILearner>
    {
        /// <summary>
        /// The (rule) name
        /// </summary>
        public const string Name = RuleNameConstants.LearnAimRef_87;

        /// <summary>
        /// the message handler
        /// </summary>
        private readonly IValidationErrorHandler _messageHandler;

        /// <summary>
        /// The common rule (operations provider)
        /// </summary>
        private readonly IProvideRuleCommonOperations _check;

        /// <summary>
        /// Initializes a new instance of the <see cref="LearnAimRef_87Rule" /> class.
        /// </summary>
        /// <param name="validationErrorHandler">The validation error handler.</param>
        /// <param name="commonChecks">The common checks.</param>
        public LearnAimRef_87Rule(
            IValidationErrorHandler validationErrorHandler,
            IProvideRuleCommonOperations commonChecks)
        {
            It.IsNull(validationErrorHandler)
                .AsGuard<ArgumentNullException>(nameof(validationErrorHandler));
            It.IsNull(commonChecks)
                .AsGuard<ArgumentNullException>(nameof(commonChecks));

            _messageHandler = validationErrorHandler;
            _check = commonChecks;
        }

        /// <summary>
        /// Gets the first viable date.
        /// </summary>
        public static DateTime FirstViableDate => new DateTime(2017, 08, 01);

        /// <summary>
        /// Gets the name of the rule.
        /// </summary>
        public string RuleName => Name;

        /// <summary>
        /// Determines whether [has disqualifying vocational aim] [the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [has disqualifying vocational aim] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasDisqualifyingVocationalAim(ILearningDelivery delivery) =>
            delivery.LearnAimRef.CommencesWith(
                TypeOfAim.Branches.VocationalStudiesNotLeadingToARecognisedQualification,
                TypeOfAim.Branches.NonExternallyCertificatedFEOtherProvision,
                TypeOfAim.Branches.UnitsOfApprovedNQFProvision);

        /// <summary>
        /// Determines whether [is not valid] [the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [is not valid] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsNotValid(ILearningDelivery delivery) =>
            _check.HasQualifyingStart(delivery, FirstViableDate)
            && HasDisqualifyingVocationalAim(delivery);

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
            parameters.Add(_messageHandler.BuildErrorMessageParameter(nameof(thisDelivery.LearnAimRef), thisDelivery.LearnAimRef));
            parameters.Add(_messageHandler.BuildErrorMessageParameter(nameof(thisDelivery.LearnStartDate), thisDelivery.LearnStartDate));

            _messageHandler.Handle(RuleName, learnRefNumber, thisDelivery.AimSeqNumber, parameters);
        }
    }
}
