using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Utility;
using System;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.ContPrefType
{
    public class ContPrefType_02Rule :
        IRule<ILearner>
    {
        /// <summary>
        /// The (rule) name
        /// </summary>
        public const string Name = RuleNameConstants.ContPrefType_02;

        /// <summary>
        /// the message handler
        /// </summary>
        private readonly IValidationErrorHandler _messageHandler;

        public ContPrefType_02Rule(IValidationErrorHandler validationErrorHandler)
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
        /// Determines whether [has disqualifying contact indicator] [the specified preference].
        /// </summary>
        /// <param name="preference">The preference.</param>
        /// <returns>
        ///   <c>true</c> if [has disqualifying contact indicator] [the specified preference]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasDisqualifyingContactIndicator(IContactPreference preference) =>
            It.IsInRange(
                $"{preference.ContPrefType}{preference.ContPrefCode}",
                ContactPreference.NoContactByPostPreGDPR,
                ContactPreference.NoContactByPhonePreGDPR,
                ContactPreference.NoContactByEmailPreGDPR,
                ContactPreference.AgreesContactByPostPostGDPR,
                ContactPreference.AgreesContactByPhonePostGDPR,
                ContactPreference.AgreesContactByEmailPostGDPR,
                ContactPreference.NoContactCoursesOrOpportunitiesPreGDPR,
                ContactPreference.NoContactSurveysAndResearchPreGDPR,
                ContactPreference.AgreesContactCoursesOrOpportunitiesPostGDPR,
                ContactPreference.AgreesContactSurveysAndResearchPostGDPR);

        /// <summary>
        /// Determines whether [has disqualifying contact indicator] [the specified this learner].
        /// </summary>
        /// <param name="thisLearner">The this learner.</param>
        /// <returns>
        ///   <c>true</c> if [has disqualifying contact indicator] [the specified this learner]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasDisqualifyingContactIndicator(ILearner thisLearner) =>
            thisLearner.ContactPreferences.SafeAny(HasDisqualifyingContactIndicator);

        /// <summary>
        /// Determines whether [has restricted contact indicator] [the specified preference].
        /// </summary>
        /// <param name="preference">The preference.</param>
        /// <returns>
        ///   <c>true</c> if [has restricted contact indicator] [the specified preference]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasRestrictedContactIndicator(IContactPreference preference) =>
            It.IsInRange(
                $"{preference.ContPrefType}{preference.ContPrefCode}",
                ContactPreference.NoContactIllnessOrDied_ValidTo20130731,
                ContactPreference.NoContactDueToIllness,
                ContactPreference.NoContactDueToDeath);

        /// <summary>
        /// Determines whether [has restricted contact indicator] [the specified this learner].
        /// </summary>
        /// <param name="thisLearner">The this learner.</param>
        /// <returns>
        ///   <c>true</c> if [has restricted contact indicator] [the specified this learner]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasRestrictedContactIndicator(ILearner thisLearner) =>
            thisLearner.ContactPreferences.SafeAny(HasRestrictedContactIndicator);

        /// <summary>
        /// Determines whether [has conflicting contact indicators] [the specified this learner].
        /// </summary>
        /// <param name="thisLearner">The this learner.</param>
        /// <returns>
        ///   <c>true</c> if [has conflicting contact indicators] [the specified this learner]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasConflictingContactIndicators(ILearner thisLearner) =>
            HasRestrictedContactIndicator(thisLearner) && HasDisqualifyingContactIndicator(thisLearner);

        /// <summary>
        /// Validates the specified object to validate.
        /// </summary>
        /// <param name="objectToValidate">The object to validate.</param>
        public void Validate(ILearner objectToValidate)
        {
            It.IsNull(objectToValidate)
                .AsGuard<ArgumentNullException>(nameof(objectToValidate));

            var learnRefNumber = objectToValidate.LearnRefNumber;

            if (HasConflictingContactIndicators(objectToValidate))
            {
                objectToValidate.ContactPreferences
                    .SafeWhere(HasDisqualifyingContactIndicator)
                    .ForEach(x => RaiseValidationMessage(learnRefNumber, x));
            }
        }

        /// <summary>
        /// Raises the validation message.
        /// </summary>
        /// <param name="learnRefNumber">The learn reference number.</param>
        /// <param name="thisPreference">this preference.</param>
        public void RaiseValidationMessage(string learnRefNumber, IContactPreference thisPreference)
        {
            var parameters = Collection.Empty<IErrorMessageParameter>();
            parameters.Add(_messageHandler.BuildErrorMessageParameter(nameof(thisPreference.ContPrefType), thisPreference.ContPrefType));
            parameters.Add(_messageHandler.BuildErrorMessageParameter(nameof(thisPreference.ContPrefCode), thisPreference.ContPrefCode));

            _messageHandler.Handle(RuleName, learnRefNumber, null, parameters);
        }
    }
}