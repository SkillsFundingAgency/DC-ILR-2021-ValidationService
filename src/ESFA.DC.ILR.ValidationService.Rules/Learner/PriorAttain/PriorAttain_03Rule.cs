using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.PriorAttain
{
    public class PriorAttain_03Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IProvideLookupDetails _provideLookupDetails;

        public PriorAttain_03Rule(IProvideLookupDetails provideLookupDetails, IValidationErrorHandler validationErrorHandler)
           : base(validationErrorHandler, RuleNameConstants.PriorAttain_03)
        {
            _provideLookupDetails = provideLookupDetails;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate != null && objectToValidate.PriorAttainNullable.HasValue)
            {
                if (ConditionMet(objectToValidate.PriorAttainNullable.Value))
                {
                    HandleValidationError(learnRefNumber: objectToValidate.LearnRefNumber, errorMessageParameters: BuildErrorMessageParameters(objectToValidate.PriorAttainNullable.Value));
                }
            }
        }

        public bool ConditionMet(int priorAttainValue)
        {
            return !_provideLookupDetails.Contains(TypeOfIntegerCodedLookup.PriorAttain, priorAttainValue);
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(int priorAttainValue)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.PriorAttain, priorAttainValue)
            };
        }
    }
}