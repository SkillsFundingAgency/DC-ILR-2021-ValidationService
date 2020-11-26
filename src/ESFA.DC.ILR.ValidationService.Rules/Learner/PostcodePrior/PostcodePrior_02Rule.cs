using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.PostcodePrior
{
    public class PostcodePrior_02Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IPostcodeQueryService _postcodeQueryService;

        public PostcodePrior_02Rule(
            IPostcodeQueryService postcodeQueryService,
            IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.PostcodePrior_02)
        {
            _postcodeQueryService = postcodeQueryService;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (ConditionMet(objectToValidate.PostcodePrior))
            {
                HandleValidationError(objectToValidate.LearnRefNumber, errorMessageParameters: BuildErrorMessageParameters(objectToValidate.PostcodePrior));
            }
        }

        public bool ConditionMet(string postcodePrior)
        {
            return !string.IsNullOrWhiteSpace(postcodePrior)
                && !_postcodeQueryService.RegexValid(postcodePrior);
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(string postcodePrior)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.PostcodePrior, postcodePrior)
            };
        }
    }
}
