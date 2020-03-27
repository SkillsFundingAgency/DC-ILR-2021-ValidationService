using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Utility;
using System;
using System.Linq;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnDelFAMType
{
    public class LearnDelFAMType_04Rule :
        IRule<ILearner>
    {
        /// <summary>
        /// Gets the name of the rule.
        /// </summary>
        public const string Name = RuleNameConstants.LearnDelFAMType_04;

        /// <summary>
        /// The message handler
        /// </summary>
        private readonly IValidationErrorHandler _messageHandler;

        /// <summary>
        /// The lookup details (provider)
        /// </summary>
        private readonly IProvideLookupDetails _lookupDetails;

        /// <summary>
        /// Initializes a new instance of the <see cref="LearnDelFAMType_04Rule" /> class.
        /// </summary>
        /// <param name="validationErrorHandler">The validation error handler.</param>
        /// <param name="lookupDetails">The lookup details.</param>
        public LearnDelFAMType_04Rule(
            IValidationErrorHandler validationErrorHandler,
            IProvideLookupDetails lookupDetails)
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
        /// Determines whether [is not valid] [the specified monitor].
        /// </summary>
        /// <param name="monitor">The monitor.</param>
        /// <returns>
        ///   <c>true</c> if [is not valid] [the specified monitor]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsNotValid(ILearningDeliveryFAM monitor) =>
            !_lookupDetails.Contains(TypeOfLimitedLifeLookup.LearnDelFAMType, $"{monitor.LearnDelFAMType}{monitor.LearnDelFAMCode}");

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
                .AsSafeReadOnlyList()
                .SelectMany(x => x.LearningDeliveryFAMs.AsSafeReadOnlyList())
                .Where(IsNotValid)
                .ForEach(x => RaiseValidationMessage(learnRefNumber, x));
        }

        /// <summary>
        /// Raises the validation message.
        /// </summary>
        /// <param name="learnRefNumber">The learn reference number.</param>
        /// <param name="thisMonitor">this monitor.</param>
        public void RaiseValidationMessage(string learnRefNumber, ILearningDeliveryFAM thisMonitor)
        {
            var parameters = Collection.Empty<IErrorMessageParameter>();
            parameters.Add(_messageHandler.BuildErrorMessageParameter(nameof(thisMonitor.LearnDelFAMType), thisMonitor.LearnDelFAMType));
            parameters.Add(_messageHandler.BuildErrorMessageParameter(nameof(thisMonitor.LearnDelFAMCode), thisMonitor.LearnDelFAMCode));

            _messageHandler.Handle(RuleName, learnRefNumber, null, parameters);
        }
    }
}
