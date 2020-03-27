using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.HE.STULOAD
{
    public class STULOAD_04Rule : AbstractRule, IRule<ILearner>
    {
        private readonly DateTime learnStartDateCondition = new DateTime(2013, 08, 01);

        public STULOAD_04Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.STULOAD_04)
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
                        BuildErrorMessageParameters(learningDelivery.LearnStartDate));
                }
            }
        }

        public bool ConditionMet(DateTime learnStartDate, ILearningDeliveryHE learningDeliveryHe)
        {
            return LearnStartDateConditionMet(learnStartDate)
                   && STULOADConditionMet(learningDeliveryHe);
        }

        public bool LearnStartDateConditionMet(DateTime learnStartDate)
        {
            return learnStartDate >= learnStartDateCondition;
        }

        public bool STULOADConditionMet(ILearningDeliveryHE learningDeliveryHe)
        {
            return learningDeliveryHe != null
                   && !learningDeliveryHe.STULOADNullable.HasValue;
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(DateTime learnStartDate)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, learnStartDate)
            };
        }
    }
}
