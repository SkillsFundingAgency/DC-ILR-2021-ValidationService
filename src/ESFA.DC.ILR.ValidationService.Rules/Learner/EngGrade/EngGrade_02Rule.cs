using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.EngGrade
{
    public class EngGrade_02Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IProvideLookupDetails _lookupProviderDetails;

        public EngGrade_02Rule(
            IValidationErrorHandler validationErrorHandler,
            IProvideLookupDetails provideLookupDetails)
            : base(validationErrorHandler, RuleNameConstants.EngGrade_02)
        {
            _lookupProviderDetails = provideLookupDetails;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate == null
                || !EngGradeSuppliedAndNotNone(objectToValidate?.EngGrade))
            {
                return;
            }

            if (EngGradeConditionMet(objectToValidate.EngGrade))
            {
                HandleValidationError(
                    learnRefNumber: objectToValidate.LearnRefNumber,
                    errorMessageParameters: BuildErrorMessageParameters(objectToValidate.EngGrade));
            }
        }

        public bool EngGradeConditionMet(string engGrade) => !_lookupProviderDetails.Contains(TypeOfStringCodedLookup.GCSEGrade, engGrade);

        public bool EngGradeSuppliedAndNotNone(string engGrade) => !string.IsNullOrEmpty(engGrade)
                && !engGrade.CaseInsensitiveEquals(Grades.NONE);

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(string engGrade)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.EngGrade, engGrade)
            };
        }
    }
}
