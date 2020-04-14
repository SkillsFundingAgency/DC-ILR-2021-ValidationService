using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Utility;
using System;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.ContPrefType
{
    public class ContPrefType_03Rule :
        AbstractRule,
        IRule<ILearner>
    {
        /// <summary>
        /// The derived data rule 06
        /// </summary>
        private readonly IDerivedData_06Rule _derivedData;

        /// <summary>
        /// The lookup (details provider)
        /// </summary>
        private readonly IProvideLookupDetails _lookups;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContPrefType_03Rule" /> class.
        /// </summary>
        /// <param name="validationErrorHandler">The validation error handler.</param>
        /// <param name="derivedData">The derived data rule 06</param>
        /// <param name="lookups">The lookups.</param>
        public ContPrefType_03Rule(
            IValidationErrorHandler validationErrorHandler,
            IDerivedData_06Rule derivedData,
            IProvideLookupDetails lookups)
            : base(validationErrorHandler, RuleNameConstants.ContPrefType_03)
        {
            It.IsNull(validationErrorHandler)
                .AsGuard<ArgumentNullException>(nameof(validationErrorHandler));
            It.IsNull(derivedData)
                .AsGuard<ArgumentNullException>(nameof(derivedData));
            It.IsNull(lookups)
                .AsGuard<ArgumentNullException>(nameof(lookups));

            _derivedData = derivedData;
            _lookups = lookups;
        }

        public DateTime GetQualifyingStartDate(IReadOnlyCollection<ILearningDelivery> usingSources) =>
            _derivedData.Derive(usingSources);

        /// <summary>
        /// Determines whether [has disqualifying contact preference] [the specified preference].
        /// </summary>
        /// <param name="preference">The preference.</param>
        /// <param name="candidate">The candidate.</param>
        /// <returns>
        ///   <c>true</c> if [has disqualifying contact preference] [the specified preference]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasDisqualifyingContactPreference(IContactPreference preference, DateTime candidate) =>
            !_lookups.IsCurrent(TypeOfLimitedLifeLookup.ContPrefType, $"{preference.ContPrefType}{preference.ContPrefCode}", candidate);

        /// <summary>
        /// Determines whether [is not valid] [the specified preference].
        /// </summary>
        /// <param name="preference">The preference.</param>
        /// <param name="earliestStart">The earliest start.</param>
        /// <returns>
        ///   <c>true</c> if [is not valid] [the specified preference]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsNotValid(IContactPreference preference, DateTime earliestStart) =>
            HasDisqualifyingContactPreference(preference, earliestStart);

        /// <summary>
        /// Validates this learner.
        /// </summary>
        /// <param name="thisLearner">this learner.</param>
        public void Validate(ILearner thisLearner)
        {
            It.IsNull(thisLearner)
                .AsGuard<ArgumentNullException>(nameof(thisLearner));

            var learnRefNumber = thisLearner.LearnRefNumber;
            var earliestStart = GetQualifyingStartDate(thisLearner.LearningDeliveries.ToReadOnlyCollection());

            thisLearner.ContactPreferences
                .ForAny(x => IsNotValid(x, earliestStart), x => RaiseValidationMessage(learnRefNumber, x, earliestStart));
        }

        /// <summary>
        /// Raises the validation message.
        /// </summary>
        /// <param name="learnRefNumber">The learn reference number.</param>
        /// <param name="thisPreference">this preference.</param>
        /// <param name="disqualifyingStart">The disqualifying start.</param>
        public void RaiseValidationMessage(string learnRefNumber, IContactPreference thisPreference, DateTime disqualifyingStart)
        {
            HandleValidationError(learnRefNumber, null, BuildMessageParametersFor(thisPreference, disqualifyingStart));
        }

        /// <summary>
        /// Builds the error message parameters.
        /// </summary>
        /// <param name="thisPreference">this preference.</param>
        /// <param name="disqualifyingStart">The disqualifying start.</param>
        /// <returns>
        /// returns a list of message parameters
        /// </returns>
        public IEnumerable<IErrorMessageParameter> BuildMessageParametersFor(IContactPreference thisPreference, DateTime disqualifyingStart)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.DerivedData_06, disqualifyingStart),
                BuildErrorMessageParameter(PropertyNameConstants.ContPrefType, thisPreference.ContPrefType),
                BuildErrorMessageParameter(PropertyNameConstants.ContPrefCode, thisPreference.ContPrefCode)
            };
        }
    }
}