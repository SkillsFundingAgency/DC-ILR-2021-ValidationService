using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.CrossEntity
{
    public class R49Rule : AbstractRule, IRule<ILearner>
    {
        public R49Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.R49)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate?.ProviderSpecLearnerMonitorings == null)
            {
                return;
            }

            var duplicates = objectToValidate.ProviderSpecLearnerMonitorings
                    .GroupBy(pdm => pdm.ProvSpecLearnMonOccur?.ToUpper())
                    .Where(grp => grp.Count() > 1)
                    .ToList();

            foreach (var duplicate in duplicates)
            {
                HandleValidationError(
                    objectToValidate.LearnRefNumber,
                    null,
                    BuildErrorMessageParameters(duplicate.Key));
            }
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(string provSpecLearnMonOccur)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.ProvSpecLearnMonOccur, provSpecLearnMonOccur ?? string.Empty)
            };
        }
    }
}