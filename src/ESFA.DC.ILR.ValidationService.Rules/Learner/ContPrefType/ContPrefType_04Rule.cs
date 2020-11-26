using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.ContPrefType
{
    public class ContPrefType_04Rule :
        AbstractRule,
        IRule<ILearner>
    {
        private const string CompatibilityMessage = "(incompatible combination)";

        public ContPrefType_04Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.ContPrefType_04)
        {
        }

        public bool HasPreGDPRMerchandisingCodes(IContactPreference preference)
        {
            var code = $"{preference.ContPrefType}{preference.ContPrefCode}";

            return code.CaseInsensitiveEquals(ContactPreference.NoContactCoursesOrOpportunitiesPreGDPR)
                            || code.CaseInsensitiveEquals(ContactPreference.NoContactSurveysAndResearchPreGDPR);
        }

        public bool HasPostGDPRMerchandisingCodes(IContactPreference preference)
        {
            var code = $"{preference.ContPrefType}{preference.ContPrefCode}";

            return code.CaseInsensitiveEquals(ContactPreference.AgreesContactCoursesOrOpportunitiesPostGDPR)
                || code.CaseInsensitiveEquals(ContactPreference.AgreesContactSurveysAndResearchPostGDPR);
        }

        public bool IsNotValid(IReadOnlyCollection<IContactPreference> preferences) =>
            preferences.Any(HasPreGDPRMerchandisingCodes)
                && preferences.Any(HasPostGDPRMerchandisingCodes);

        public void Validate(ILearner thisLearner)
        {
            var learnRefNumber = thisLearner.LearnRefNumber;

            if (IsNotValid(thisLearner.ContactPreferences.ToReadOnlyCollection()))
            {
                RaiseValidationMessage(learnRefNumber);
            }
        }

        public void RaiseValidationMessage(string learnRefNumber)
        {
            HandleValidationError(learnRefNumber, null, BuildMessageParametersFor());
        }

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