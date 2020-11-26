using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.HE.MSTUFEE
{
    public class MSTUFEE_04Rule : AbstractRule, IRule<ILearner>
    {
        private readonly int[] _mstufeeCodes = { 2, 3 };

        public MSTUFEE_04Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.MSTUFEE_04)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(learningDelivery.LearningDeliveryHEEntity))
                {
                    HandleValidationError(
                        objectToValidate.LearnRefNumber,
                        learningDelivery.AimSeqNumber,
                        BuildErrorMessageParameters(
                            learningDelivery.LearningDeliveryHEEntity.MSTUFEE,
                            learningDelivery.LearningDeliveryHEEntity.DOMICILE));
                }
            }
        }

        public bool ConditionMet(ILearningDeliveryHE learningDeliveryHe)
        {
            return learningDeliveryHe != null
                   && _mstufeeCodes.Contains(learningDeliveryHe.MSTUFEE)
                   && learningDeliveryHe.DOMICILE == "XG";
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(int mstufee, string domicile)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.MSTUFEE, mstufee),
                BuildErrorMessageParameter(PropertyNameConstants.DOMICILE, domicile)
            };
        }
    }
}
