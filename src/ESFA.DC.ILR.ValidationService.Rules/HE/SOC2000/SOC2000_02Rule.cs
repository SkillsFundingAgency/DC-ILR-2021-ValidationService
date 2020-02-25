using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ESFA.DC.ILR.ValidationService.Rules.HE.SOC2000
{
    public class SOC2000_02Rule : AbstractRule, IRule<ILearner>
    {
        private readonly DateTime _dateCondition = new DateTime(2013, 08, 01);
        private readonly string[] _domicileCodes = { "XF", "XG", "XH", "XI", "XK" };

        public SOC2000_02Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.SOC2000_02)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(
                    learningDelivery.LearnStartDate,
                    learningDelivery.LearningDeliveryHEEntity))
                {
                    HandleValidationError(
                        objectToValidate.LearnRefNumber,
                        learningDelivery.AimSeqNumber,
                        BuildErrorMessageParameters(
                            learningDelivery.LearnStartDate,
                            learningDelivery.LearningDeliveryHEEntity.DOMICILE,
                            learningDelivery.LearningDeliveryHEEntity.UCASAPPID));
                }
            }
        }

        public bool ConditionMet(DateTime learnStartDate, ILearningDeliveryHE learningDeliveryHe)
        {
            return LearnStartDateConditionMet(learnStartDate)
                   && LearningDeliveryHEConditionMet(learningDeliveryHe);
        }

        public bool LearnStartDateConditionMet(DateTime learnStartDate)
        {
            return learnStartDate >= _dateCondition;
        }

        public bool LearningDeliveryHEConditionMet(ILearningDeliveryHE learningDeliveryHe)
        {
            return learningDeliveryHe != null
                   && !string.IsNullOrWhiteSpace(learningDeliveryHe.UCASAPPID)
                   && _domicileCodes.Contains(learningDeliveryHe.DOMICILE)
                   && !learningDeliveryHe.SOC2000Nullable.HasValue;
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(DateTime learnStartDate, string domicile, string ucasAppId)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, learnStartDate),
                BuildErrorMessageParameter(PropertyNameConstants.DOMICILE, domicile),
                BuildErrorMessageParameter(PropertyNameConstants.UCASAPPID, ucasAppId)
            };
        }
    }
}
