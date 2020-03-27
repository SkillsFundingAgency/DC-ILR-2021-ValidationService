using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.CrossEntity
{
    public class R45Rule : AbstractRule, IRule<ILearner>
    {
        public R45Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.R45)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate?.LLDDAndHealthProblems == null)
            {
                return;
            }

            var duplicates = objectToValidate.LLDDAndHealthProblems
                .GroupBy(les => new { les.LLDDCat })
                .Where(grp => grp.Count() > 1);

            foreach (var duplicate in duplicates)
            {
                HandleValidationError(
                    objectToValidate.LearnRefNumber,
                    null,
                    BuildErrorMessageParameters(duplicate.Key.LLDDCat));
            }
        }

        private IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(int lLDDCat)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.LLDDCat, lLDDCat)
            };
        }
    }
}