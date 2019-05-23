using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Utility;
using System;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.ContPrefType
{
    public class ContPrefType_01Rule :
        AbstractRule,
        IRule<ILearner>
    {
        /// <summary>
        /// The lookup (details provider)
        /// </summary>
        private readonly IProvideLookupDetails _lookups;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContPrefType_01Rule" /> class.
        /// </summary>
        /// <param name="validationErrorHandler">The validation error handler.</param>
        /// <param name="lookups">The lookups.</param>
        public ContPrefType_01Rule(
            IValidationErrorHandler validationErrorHandler,
            IProvideLookupDetails lookups)
            : base(validationErrorHandler, RuleNameConstants.ContPrefType_01)
        {
            It.IsNull(validationErrorHandler)
                .AsGuard<ArgumentNullException>(nameof(validationErrorHandler));
            It.IsNull(lookups)
                .AsGuard<ArgumentNullException>(nameof(lookups));

            _lookups = lookups;
        }

        /// <summary>
        /// Determines whether [has disqualifying contact preference] [the specified preference].
        /// </summary>
        /// <param name="preference">The preference.</param>
        /// <returns>
        ///   <c>true</c> if [has disqualifying contact preference] [the specified preference]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasDisqualifyingContactPreference(IContactPreference preference) =>
            !_lookups.Contains(TypeOfLimitedLifeLookup.ContPrefType, $"{preference.ContPrefType}{preference.ContPrefCode}");

        /// <summary>
        /// Determines whether [is not valid] [the specified preference].
        /// </summary>
        /// <param name="preference">The preference.</param>
        /// <returns>
        ///   <c>true</c> if [is not valid] [the specified preference]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsNotValid(IContactPreference preference) =>
            HasDisqualifyingContactPreference(preference);

        /// <summary>
        /// Validates this learner.
        /// </summary>
        /// <param name="thisLearner">this learner.</param>
        public void Validate(ILearner thisLearner)
        {
            It.IsNull(thisLearner)
                .AsGuard<ArgumentNullException>(nameof(thisLearner));

            var learnRefNumber = thisLearner.LearnRefNumber;

            thisLearner.ContactPreferences
                .ForAny(IsNotValid, x => RaiseValidationMessage(learnRefNumber, x));
        }

        /// <summary>
        /// Raises the validation message.
        /// </summary>
        /// <param name="learnRefNumber">The learn reference number.</param>
        /// <param name="thisPreference">this preference.</param>
        public void RaiseValidationMessage(string learnRefNumber, IContactPreference thisPreference)
        {
            HandleValidationError(learnRefNumber, null, BuildMessageParametersFor(thisPreference));
        }

        /// <summary>
        /// Builds the error message parameters.
        /// </summary>
        /// <param name="thisPreference">this preference.</param>
        /// <returns>
        /// returns a list of message parameters
        /// </returns>
        public IEnumerable<IErrorMessageParameter> BuildMessageParametersFor(IContactPreference thisPreference)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.ContPrefType, thisPreference.ContPrefType),
                BuildErrorMessageParameter(PropertyNameConstants.ContPrefCode, thisPreference.ContPrefCode)
            };
        }
    }
}