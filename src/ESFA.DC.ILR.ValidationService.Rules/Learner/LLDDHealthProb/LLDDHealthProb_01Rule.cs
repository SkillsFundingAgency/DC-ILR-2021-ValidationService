using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.LLDDHealthProb
{
    public class LLDDHealthProb_01Rule : AbstractRule, IRule<ILearner>
    {
        private readonly int[] _validCodes = { 1, 2, 9 };

        public LLDDHealthProb_01Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.LLDDHealthProb_01)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            if (ConditionMet(objectToValidate.LLDDHealthProb))
            {
                HandleValidationError(objectToValidate.LearnRefNumber, errorMessageParameters: BuildErrorMessageParameters(objectToValidate.LLDDHealthProb));
            }
        }

        public bool ConditionMet(int llddHealthProb)
        {
            return !_validCodes.Contains(llddHealthProb);
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(int llddHealthProb)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.LLDDHealthProb, llddHealthProb)
            };
        }
    }
}
