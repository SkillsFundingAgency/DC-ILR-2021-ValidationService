using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.HE.MODESTUD
{
    public class MODESTUD_01Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IProvideLookupDetails _provideLookupDetails;

        public MODESTUD_01Rule(IProvideLookupDetails provideLookupDetails, IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.MODESTUD_01)
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
                if (learningDelivery.LearningDeliveryHEEntity != null && ConditionMet(learningDelivery.LearningDeliveryHEEntity.MODESTUD))
                {
                    HandleValidationError(
                        learnRefNumber: objectToValidate.LearnRefNumber,
                        aimSequenceNumber: learningDelivery.AimSeqNumber,
                        errorMessageParameters: BuildErrorMessageParameters(learningDelivery.LearningDeliveryHEEntity.MODESTUD));
                }
            }
        }

        public bool ConditionMet(int modeStud)
        {
            return !_provideLookupDetails.Contains(TypeOfIntegerCodedLookup.ModeStud, modeStud);
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(int modeStud)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.MODESTUD, modeStud)
            };
        }
    }
}
