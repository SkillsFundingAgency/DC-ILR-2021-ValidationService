using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.OutGrade
{
    public class OutGrade_01Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IProvideLookupDetails _provideLookupDetails;

        public OutGrade_01Rule(IValidationErrorHandler validationErrorHandler, IProvideLookupDetails provideLookupDetails)
            : base(validationErrorHandler, RuleNameConstants.OutGrade_01)
        {
            _provideLookupDetails = provideLookupDetails;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate.LearningDeliveries == null)
            {
                return;
            }

            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(learningDelivery.OutGrade))
                {
                    HandleValidationError(objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber, errorMessageParameters: BuildErrorMessageParameters(learningDelivery.OutGrade));
                }
            }
        }

        public bool ConditionMet(string outGrade)
        {
            return !string.IsNullOrWhiteSpace(outGrade) && !_provideLookupDetails.Contains(TypeOfStringCodedLookup.OutGrade, outGrade);
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(string outGrade)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.OutGrade, outGrade)
            };
        }
    }
}
