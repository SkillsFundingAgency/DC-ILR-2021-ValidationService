using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.HE.SPECFEE
{
    public class SPECFEE_02Rule : AbstractRule, IRule<ILearner>
    {
        private readonly DateTime _firstAugust2009 = new DateTime(2009, 08, 01);

        public SPECFEE_02Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.SPECFEE_02)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate?.LearningDeliveries == null)
            {
                return;
            }

            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (learningDelivery.LearningDeliveryHEEntity == null)
                {
                    continue;
                }

                if (ConditionMet(
                    learningDelivery.LearnStartDate,
                    learningDelivery.LearningDeliveryHEEntity.MODESTUD,
                    learningDelivery.LearningDeliveryHEEntity.SPECFEE))
                {
                    HandleValidationError(
                        objectToValidate.LearnRefNumber,
                        learningDelivery.AimSeqNumber,
                        BuildErrorMessageParameters(
                            learningDelivery.LearnStartDate,
                            learningDelivery.LearningDeliveryHEEntity.MODESTUD,
                            learningDelivery.LearningDeliveryHEEntity.SPECFEE));
                }
            }
        }

        public bool ConditionMet(DateTime learnStartDate, int modestud, int specfee)
        {
            return learnStartDate >= _firstAugust2009
                   && modestud == TypeOfMODESTUD.SandwichYearOut
                   && specfee != TypeOfSPECFEE.SandwichPlacement;
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(DateTime learnStartDate, int modestud, int specfee)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, learnStartDate),
                BuildErrorMessageParameter(PropertyNameConstants.MODESTUD, modestud),
                BuildErrorMessageParameter(PropertyNameConstants.SPECFEE, specfee),
            };
        }
    }
}
