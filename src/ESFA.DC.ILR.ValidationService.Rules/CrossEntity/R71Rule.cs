using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.CrossEntity
{
    public class R71Rule : AbstractRule, IRule<IMessage>
    {
        public R71Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.R71)
        {
        }

        public void Validate(IMessage objectToValidate)
        {
            if (objectToValidate?.LearnerDestinationAndProgressions == null)
            {
                return;
            }

            var duplicates = objectToValidate.LearnerDestinationAndProgressions
                .GroupBy(ldp => ldp.LearnRefNumber.ToUpper())
                .Where(grp => grp.Count() > 1);

            foreach (var duplicate in duplicates)
            {
                HandleValidationError(duplicate.Key);
            }
        }
    }
}