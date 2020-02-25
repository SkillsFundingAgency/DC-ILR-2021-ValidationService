using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.HE.QUALENT3
{
    public class QUALENT3_02Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IProvideLookupDetails _provideLookupDetails;

        public QUALENT3_02Rule(IProvideLookupDetails provideLookupDetails, IValidationErrorHandler validationErrorHandler)
          : base(validationErrorHandler, RuleNameConstants.QUALENT3_02)
        {
            _provideLookupDetails = provideLookupDetails;
        }

        public void Validate(ILearner objectToValidate)
        {
            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(learningDelivery.LearningDeliveryHEEntity))
                {
                    HandleValidationError(objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber, BuildErrorMessageParameters(learningDelivery.LearningDeliveryHEEntity.QUALENT3));
                }
            }
        }

        public bool ConditionMet(ILearningDeliveryHE learningDeliveryHE)
        {
            return LearningDeliveryHEConditionMet(learningDeliveryHE)
                && QUALENT3ValidConditionMet(learningDeliveryHE.QUALENT3);
        }

        public bool LearningDeliveryHEConditionMet(ILearningDeliveryHE learningDeliveryHE)
        {
            return learningDeliveryHE != null && learningDeliveryHE.QUALENT3 != null;
        }

        public bool QUALENT3ValidConditionMet(string qualent3)
        {
            return !_provideLookupDetails.Contains(TypeOfLimitedLifeLookup.QualEnt3, qualent3);
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(string qualent3)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.QUALENT3, qualent3)
            };
        }
    }
}
