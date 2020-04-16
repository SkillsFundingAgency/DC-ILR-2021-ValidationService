using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.HE.SPECFEE
{
    public class SPECFEE_01Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IProvideLookupDetails _provideLookupDetails;

        public SPECFEE_01Rule(IProvideLookupDetails provideLookupDetails, IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.SPECFEE_01)
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
                if (learningDelivery.LearningDeliveryHEEntity != null && ConditionMet(learningDelivery.LearningDeliveryHEEntity.SPECFEE))
                {
                    HandleValidationError(
                        learnRefNumber: objectToValidate.LearnRefNumber,
                        aimSequenceNumber: learningDelivery.AimSeqNumber,
                        errorMessageParameters: BuildErrorMessageParameters(learningDelivery.LearningDeliveryHEEntity.SPECFEE));
                }
            }
        }

        public bool ConditionMet(int specfeeValue)
        {
            return !_provideLookupDetails.Contains(TypeOfIntegerCodedLookup.SpecFee, specfeeValue);
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(int specfeeValue)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.SPECFEE, specfeeValue)
            };
        }
    }
}