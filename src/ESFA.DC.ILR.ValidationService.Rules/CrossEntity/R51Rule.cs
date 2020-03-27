using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.CrossEntity
{
    public class R51Rule : AbstractRule, IRule<ILearner>
    {
        public R51Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.R51)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate?.LearnerFAMs == null)
            {
                return;
            }

            var duplicates = objectToValidate.LearnerFAMs
                    .GroupBy(fam => new
                    {
                        LearnFAMType = fam.LearnFAMType?.ToUpper(),
                        fam.LearnFAMCode
                    }).
                    Where(grp => grp.Count() > 1)
                    .ToList();

            foreach (var duplicate in duplicates)
            {
                HandleValidationError(
                    objectToValidate.LearnRefNumber,
                    null,
                    BuildErrorMessageParameters(duplicate.Key.LearnFAMType, duplicate.Key.LearnFAMCode));
            }
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(string famType, int famCode)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.LearnFAMType, famType ?? string.Empty),
                BuildErrorMessageParameter(PropertyNameConstants.LearnFAMCode, famCode)
            };
        }
    }
}