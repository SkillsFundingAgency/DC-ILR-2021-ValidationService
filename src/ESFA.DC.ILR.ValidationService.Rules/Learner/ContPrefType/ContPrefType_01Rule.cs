﻿using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.ContPrefType
{
    public class ContPrefType_01Rule :
        AbstractRule,
        IRule<ILearner>
    {
        private readonly IProvideLookupDetails _lookups;

        public ContPrefType_01Rule(
            IValidationErrorHandler validationErrorHandler,
            IProvideLookupDetails lookups)
            : base(validationErrorHandler, RuleNameConstants.ContPrefType_01)
        {
            _lookups = lookups;
        }

        public bool HasDisqualifyingContactPreference(IContactPreference preference) =>
            !_lookups.Contains(TypeOfLimitedLifeLookup.ContPrefType, $"{preference.ContPrefType}{preference.ContPrefCode}");

        public bool IsNotValid(IContactPreference preference) =>
            HasDisqualifyingContactPreference(preference);

        public void Validate(ILearner thisLearner)
        {
            var learnRefNumber = thisLearner.LearnRefNumber;

            thisLearner.ContactPreferences
                .ForAny(IsNotValid, x => RaiseValidationMessage(learnRefNumber, x));
        }

        public void RaiseValidationMessage(string learnRefNumber, IContactPreference thisPreference)
        {
            HandleValidationError(learnRefNumber, null, BuildMessageParametersFor(thisPreference));
        }

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