using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.CrossEntity
{
    public class R59Rule : AbstractRule, IRule<IMessage>
    {
        public R59Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.R59)
        {
        }

        public void Validate(IMessage objectToValidate)
        {
            if (objectToValidate?.Learners == null)
            {
                return;
            }

            var duplicateUlns = objectToValidate.Learners
                .Where(x => x.ULN != ValidationConstants.TemporaryULN)
                .GroupBy(x => x.ULN)
                .Where(x => x.Count() > 1)
                .Select(x => x.Key);

            foreach (var uln in duplicateUlns)
            {
                objectToValidate.Learners.Where(x => x.ULN == uln)
                    .ForEach(learner =>
                        HandleValidationError(
                            learner.LearnRefNumber,
                            null,
                            BuildErrorMessageParameters(objectToValidate.LearningProviderEntity?.UKPRN, uln)));
            }
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(long? ukprn, long uln)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.UKPRN, ukprn),
                BuildErrorMessageParameter(PropertyNameConstants.ULN, uln)
            };
        }
    }
}
