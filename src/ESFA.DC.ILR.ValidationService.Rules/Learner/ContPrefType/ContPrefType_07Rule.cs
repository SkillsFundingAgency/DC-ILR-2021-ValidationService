using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.ContPrefType
{
    public class ContPrefType_07Rule : AbstractRule, IRule<ILearner>
    {
        public ContPrefType_07Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.ContPrefType_07)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate?.ContactPreferences == null)
            {
                return;
            }

            if (ConditionMet(objectToValidate.ContactPreferences))
            {
                HandleValidationError(
                    objectToValidate.LearnRefNumber,
                    errorMessageParameters: BuildErrorMessageParameters(ContactPreference.Types.PreferredMethodOfContact));
            }
        }

        public bool ConditionMet(IEnumerable<IContactPreference> contactPreferences)
        {
            if (contactPreferences.Count(cp => cp.ContPrefType.CaseInsensitiveEquals(ContactPreference.Types.PreferredMethodOfContact)) > 3)
            {
                return true;
            }

            return false;
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(string contPrefType)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.ContPrefType, contPrefType),
            };
        }
    }
}
