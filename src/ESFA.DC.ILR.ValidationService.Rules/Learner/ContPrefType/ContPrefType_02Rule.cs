using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Utility;
using System;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.ContPrefType
{
    public class ContPrefType_02Rule :
        IRule<ILearner>
    {
        public const string Name = RuleNameConstants.ContPrefType_02;
        private readonly HashSet<string> _contactPreferences = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            ContactPreference.NoContactByPostPreGDPR,
            ContactPreference.NoContactByPhonePreGDPR,
            ContactPreference.NoContactByEmailPreGDPR,
            ContactPreference.AgreesContactByPostPostGDPR,
            ContactPreference.AgreesContactByPhonePostGDPR,
            ContactPreference.AgreesContactByEmailPostGDPR,
            ContactPreference.NoContactCoursesOrOpportunitiesPreGDPR,
            ContactPreference.NoContactSurveysAndResearchPreGDPR,
            ContactPreference.AgreesContactCoursesOrOpportunitiesPostGDPR,
            ContactPreference.AgreesContactSurveysAndResearchPostGDPR
        };

        private readonly HashSet<string> _contactRestrictions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
             ContactPreference.NoContactIllnessOrDied_ValidTo20130731,
             ContactPreference.NoContactDueToIllness,
             ContactPreference.NoContactDueToDeath
        };


        private readonly IValidationErrorHandler _messageHandler;

        public ContPrefType_02Rule(IValidationErrorHandler validationErrorHandler)
        {
            It.IsNull(validationErrorHandler)
                .AsGuard<ArgumentNullException>(nameof(validationErrorHandler));

            _messageHandler = validationErrorHandler;
        }

        public string RuleName => Name;

        public bool HasDisqualifyingContactIndicator(IContactPreference preference) =>
            _contactPreferences.Contains($"{preference.ContPrefType}{preference.ContPrefCode}");

        public bool HasDisqualifyingContactIndicator(ILearner thisLearner) =>
            thisLearner.ContactPreferences.NullSafeAny(HasDisqualifyingContactIndicator);

        public bool HasRestrictedContactIndicator(IContactPreference preference) =>
            _contactRestrictions.Contains($"{preference.ContPrefType}{preference.ContPrefCode}");

        public bool HasRestrictedContactIndicator(ILearner thisLearner) =>
            thisLearner.ContactPreferences.NullSafeAny(HasRestrictedContactIndicator);

        public bool HasConflictingContactIndicators(ILearner thisLearner) =>
            HasRestrictedContactIndicator(thisLearner) && HasDisqualifyingContactIndicator(thisLearner);

        public void Validate(ILearner objectToValidate)
        {
            It.IsNull(objectToValidate)
                .AsGuard<ArgumentNullException>(nameof(objectToValidate));

            var learnRefNumber = objectToValidate.LearnRefNumber;

            if (HasConflictingContactIndicators(objectToValidate))
            {
                objectToValidate.ContactPreferences
                    .NullSafeWhere(HasDisqualifyingContactIndicator)
                    .ForEach(x => RaiseValidationMessage(learnRefNumber, x));
            }
        }

        public void RaiseValidationMessage(string learnRefNumber, IContactPreference thisPreference)
        {
            var parameters = new List<IErrorMessageParameter>
            {
                _messageHandler.BuildErrorMessageParameter(nameof(thisPreference.ContPrefType), thisPreference.ContPrefType),
                _messageHandler.BuildErrorMessageParameter(nameof(thisPreference.ContPrefCode), thisPreference.ContPrefCode)
            };

            _messageHandler.Handle(RuleName, learnRefNumber, null, parameters);
        }
    }
}