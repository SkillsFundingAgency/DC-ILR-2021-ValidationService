using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.Postcode
{
    public class Postcode_15Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IPostcodeQueryService _postcodeQueryService;

        public Postcode_15Rule(
            IPostcodeQueryService postcodeQueryService,
            IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.Postcode_15)
        {
            _postcodeQueryService = postcodeQueryService;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (ConditionMet(objectToValidate.Postcode))
            {
                HandleValidationError(objectToValidate.LearnRefNumber, errorMessageParameters: BuildErrorMessageParameters(objectToValidate.Postcode));
            }
        }

        public bool ConditionMet(string postcode)
        {
            return !string.IsNullOrWhiteSpace(postcode)
                && !_postcodeQueryService.RegexValid(postcode);
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(string postcode)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.Postcode, postcode)
            };
        }
    }
}
