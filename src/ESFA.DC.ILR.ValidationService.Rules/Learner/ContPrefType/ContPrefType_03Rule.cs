using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using System;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.ContPrefType
{
    public class ContPrefType_03Rule :
        AbstractRule,
        IRule<ILearner>
    {
        private readonly IDerivedData_06Rule _derivedData;

        private readonly IProvideLookupDetails _lookups;

        public ContPrefType_03Rule(
            IValidationErrorHandler validationErrorHandler,
            IDerivedData_06Rule derivedData,
            IProvideLookupDetails lookups)
            : base(validationErrorHandler, RuleNameConstants.ContPrefType_03)
        {
            _derivedData = derivedData;
            _lookups = lookups;
        }

        public DateTime GetQualifyingStartDate(IReadOnlyCollection<ILearningDelivery> usingSources) =>
            _derivedData.Derive(usingSources);

        public bool HasDisqualifyingContactPreference(IContactPreference preference, DateTime candidate) =>
            !_lookups.IsCurrent(TypeOfLimitedLifeLookup.ContPrefType, $"{preference.ContPrefType}{preference.ContPrefCode}", candidate);

        public bool IsNotValid(IContactPreference preference, DateTime earliestStart) =>
            HasDisqualifyingContactPreference(preference, earliestStart);

        public void Validate(ILearner thisLearner)
        {
            var learnRefNumber = thisLearner.LearnRefNumber;
            var earliestStart = GetQualifyingStartDate(thisLearner.LearningDeliveries.ToReadOnlyCollection());

            thisLearner.ContactPreferences
                .ForAny(x => IsNotValid(x, earliestStart), x => RaiseValidationMessage(learnRefNumber, x, earliestStart));
        }

        public void RaiseValidationMessage(string learnRefNumber, IContactPreference thisPreference, DateTime disqualifyingStart)
        {
            HandleValidationError(learnRefNumber, null, BuildMessageParametersFor(thisPreference, disqualifyingStart));
        }

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