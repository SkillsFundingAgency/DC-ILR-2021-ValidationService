using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.HE.MSTUFEE
{
    public class MSTUFEE_01Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IProvideLookupDetails _provideLookupDetails;

        public MSTUFEE_01Rule(IProvideLookupDetails provideLookupDetails, IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.MSTUFEE_01)
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
                if (learningDelivery.LearningDeliveryHEEntity != null && ConditionMet(learningDelivery.LearningDeliveryHEEntity.MSTUFEE))
                {
                    HandleValidationError(
                        learnRefNumber: objectToValidate.LearnRefNumber,
                        aimSequenceNumber: learningDelivery.AimSeqNumber,
                        errorMessageParameters: BuildErrorMessageParameters(learningDelivery.LearningDeliveryHEEntity.MSTUFEE));
                }
            }
        }

        public bool ConditionMet(int mstufeeValue)
        {
            return !_provideLookupDetails.Contains(TypeOfIntegerCodedLookup.MSTuFee, mstufeeValue);
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(int mstufeeValue)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.MSTUFEE, mstufeeValue)
            };
        }
    }
}