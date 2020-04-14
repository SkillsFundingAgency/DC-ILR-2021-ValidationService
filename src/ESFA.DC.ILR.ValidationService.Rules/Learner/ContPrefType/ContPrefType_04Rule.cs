using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.ContPrefType
{
    public class ContPrefType_04Rule :
        AbstractRule,
        IRule<ILearner>
    {
        /// <summary>
        /// The compatibility message
        /// </summary>
        private const string CompatibilityMessage = "(incompatible combination)";

        /// <summary>
        /// Initializes a new instance of the <see cref="ContPrefType_04Rule" /> class.
        /// </summary>
        /// <param name="validationErrorHandler">The validation error handler.</param>
        /// <param name="lookups">The lookups.</param>
        public ContPrefType_04Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.ContPrefType_04)
        {
            It.IsNull(validationErrorHandler)
                .AsGuard<ArgumentNullException>(nameof(validationErrorHandler));
        }

        /// <summary>
        /// Determines whether [has pre GDPR merchandising codes] [the specified preference].
        /// </summary>
        /// <param name="preference">The preference.</param>
        /// <returns>
        ///   <c>true</c> if [has pre GDPR merchandising codes] [the specified preference]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasPreGDPRMerchandisingCodes(IContactPreference preference) =>
            It.IsInRange(
                $"{preference.ContPrefType}{preference.ContPrefCode}",
                ContactPreference.NoContactCoursesOrOpportunitiesPreGDPR,
                ContactPreference.NoContactSurveysAndResearchPreGDPR);

        /// <summary>
        /// Determines whether [has post GDPR merchandising codes] [the specified preference].
        /// </summary>
        /// <param name="preference">The preference.</param>
        /// <returns>
        ///   <c>true</c> if [has post GDPR merchandising codes] [the specified preference]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasPostGDPRMerchandisingCodes(IContactPreference preference) =>
            It.IsInRange(
                $"{preference.ContPrefType}{preference.ContPrefCode}",
                ContactPreference.AgreesContactCoursesOrOpportunitiesPostGDPR,
                ContactPreference.AgreesContactSurveysAndResearchPostGDPR);

        /// <summary>
        /// Determines whether [is not valid] [the specified preference].
        /// </summary>
        /// <param name="preferences">The preferences.</param>
        /// <returns>
        ///   <c>true</c> if [is not valid] [the specified preference]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsNotValid(IReadOnlyCollection<IContactPreference> preferences) =>
            preferences.Any(HasPreGDPRMerchandisingCodes)
                && preferences.Any(HasPostGDPRMerchandisingCodes);

        /// <summary>
        /// Validates this learner.
        /// </summary>
        /// <param name="thisLearner">this learner.</param>
        public void Validate(ILearner thisLearner)
        {
            It.IsNull(thisLearner)
                .AsGuard<ArgumentNullException>(nameof(thisLearner));

            var learnRefNumber = thisLearner.LearnRefNumber;

            if (IsNotValid(thisLearner.ContactPreferences.ToReadOnlyCollection()))
            {
                RaiseValidationMessage(learnRefNumber);
            }
        }

        /// <summary>
        /// Raises the validation message.
        /// </summary>
        /// <param name="learnRefNumber">The learn reference number.</param>
        public void RaiseValidationMessage(string learnRefNumber)
        {
            HandleValidationError(learnRefNumber, null, BuildMessageParametersFor());
        }

        /// <summary>
        /// Builds the error message parameters.
        /// </summary>
        /// <returns>
        /// returns a list of message parameters
        /// </returns>
        public IEnumerable<IErrorMessageParameter> BuildMessageParametersFor()
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.ContPrefType, ContactPreference.Types.RestrictedUserInteraction),
                BuildErrorMessageParameter(PropertyNameConstants.ContPrefCode, CompatibilityMessage)
            };
        }
    }
}