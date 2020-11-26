using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.Accom
{
    public class Accom_01Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IProvideLookupDetails _provideLookupDetails;

        public Accom_01Rule(IProvideLookupDetails provideLookupDetails, IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.Accom_01)
        {
            _provideLookupDetails = provideLookupDetails;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate != null && objectToValidate.AccomNullable.HasValue)
            {
                if (ConditionMet(objectToValidate.AccomNullable.Value))
                {
                    HandleValidationError(learnRefNumber: objectToValidate.LearnRefNumber, errorMessageParameters: BuildErrorMessageParameters(objectToValidate.AccomNullable.Value));
                }
            }
        }

        public bool ConditionMet(int accomValue)
        {
            return !_provideLookupDetails.Contains(TypeOfIntegerCodedLookup.Accom, accomValue);
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(int accom)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.Accom, accom)
            };
        }
    }
}