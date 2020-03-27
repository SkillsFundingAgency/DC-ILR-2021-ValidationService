using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ESFA.DC.ILR.ValidationService.Rules.HE.MODESTUD
{
    public class MODESTUD_03Rule : AbstractRule, IRule<ILearner>
    {
        private readonly DateTime _dateCondition = new DateTime(2009, 08, 01);
        private readonly int[] _validSpecFeeCodes = { 4, 5 };

    public MODESTUD_03Rule(
            IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.MODESTUD_03)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate.LearningDeliveries == null)
            {
                return;
            }

            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(
                    learningDelivery.LearnStartDate,
                    learningDelivery.LearningDeliveryHEEntity))
                {
                    HandleValidationError(
                        objectToValidate.LearnRefNumber,
                        learningDelivery.AimSeqNumber,
                        BuildErrorMessageParameters(learningDelivery.LearnStartDate, learningDelivery.LearningDeliveryHEEntity.SPECFEE, learningDelivery.LearningDeliveryHEEntity.MODESTUD));
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
                   && _validSpecFeeCodes.Contains(learningDeliveryHe.SPECFEE) && learningDeliveryHe.MODESTUD != 3;
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(DateTime learnStartDate, int specFee, int modeStud)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, learnStartDate),
                BuildErrorMessageParameter(PropertyNameConstants.SPECFEE, specFee),
                BuildErrorMessageParameter(PropertyNameConstants.MODESTUD, modeStud)
            };
        }
    }
}
