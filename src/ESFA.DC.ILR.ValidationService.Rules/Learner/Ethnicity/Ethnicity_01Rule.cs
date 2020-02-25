using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.Ethnicity
{
    public class Ethnicity_01Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IProvideLookupDetails _provideLookupDetails;

        public Ethnicity_01Rule(
            IValidationErrorHandler validationErrorHandler,
            IProvideLookupDetails provideLookupDetails)
            : base(validationErrorHandler, RuleNameConstants.Ethnicity_01)
        {
            _provideLookupDetails = provideLookupDetails;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate == null)
            {
                return;
            }

            if (!_provideLookupDetails.Contains(TypeOfIntegerCodedLookup.Ethnicity, objectToValidate.Ethnicity))
            {
                HandleValidationError(learnRefNumber: objectToValidate.LearnRefNumber, errorMessageParameters: BuildErrorMessageParameters(objectToValidate.Ethnicity));
            }
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(int ethnicity)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.Ethnicity, ethnicity)
            };
        }
    }
}
