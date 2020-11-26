using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.Sex
{
    public class Sex_01Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IProvideLookupDetails _provideLookupDetails;

        public Sex_01Rule(
            IValidationErrorHandler validationErrorHandler,
            IProvideLookupDetails provideLookupDetails)
                : base(validationErrorHandler, RuleNameConstants.Sex_01)
        {
            _provideLookupDetails = provideLookupDetails;
        }

        public void Validate(ILearner theLearner)
        {
            if (!IsValidSex(theLearner.Sex))
            {
                HandleValidationError(theLearner.LearnRefNumber, null, BuildErrorMessageParameters(theLearner.Sex));
            }
        }

        public bool IsValidSex(string sex) =>
            _provideLookupDetails.Contains(TypeOfStringCodedLookup.Sex, sex);

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(string sex)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.Sex, sex)
            };
        }
    }
}