using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.HE.SEC
{
    public class SEC_01Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IProvideLookupDetails _provideLookupDetails;

        public SEC_01Rule(IProvideLookupDetails provideLookupDetails, IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.SEC_01)
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
                if (learningDelivery.LearningDeliveryHEEntity != null && learningDelivery.LearningDeliveryHEEntity.SECNullable.HasValue && ConditionMet(learningDelivery.LearningDeliveryHEEntity.SECNullable.Value))
                {
                    HandleValidationError(
                        learnRefNumber: objectToValidate.LearnRefNumber,
                        aimSequenceNumber: learningDelivery.AimSeqNumber,
                        errorMessageParameters: BuildErrorMessageParameters(learningDelivery.LearningDeliveryHEEntity.SECNullable.Value));
                }
            }
        }

        public bool ConditionMet(int secValue)
        {
            return !_provideLookupDetails.Contains(TypeOfIntegerCodedLookup.SEC, secValue);
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(int secValue)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.SEC, secValue)
            };
        }
    }
}