using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.HE.TTACCOM
{
    public class TTACCOM_04Rule : AbstractRule, IRule<ILearner>
    {
        private readonly DateTime _augustFirst2013 = new DateTime(2013, 08, 01);
        private readonly IDerivedData_06Rule _dd06;

        public TTACCOM_04Rule(IDerivedData_06Rule dd06, IValidationErrorHandler validationErrorHandler)
          : base(validationErrorHandler, RuleNameConstants.TTACCOM_04)
        {
            _dd06 = dd06;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate.LearningDeliveries != null
                && objectToValidate.LearnerHEEntity != null
                && LearnStartDateConditionMet(objectToValidate.LearningDeliveries)
                && LearnerHEConditionMet(objectToValidate.LearnerHEEntity))
            {
                foreach (var learningDelivery in objectToValidate.LearningDeliveries)
                {
                    if (learningDelivery.LearningDeliveryHEEntity != null
                        && ConditionMet(learningDelivery.LearningDeliveryHEEntity))
                    {
                        HandleValidationError(
                            objectToValidate.LearnRefNumber,
                            learningDelivery.AimSeqNumber,
                            BuildErrorMessageParameters(
                                learningDelivery.LearningDeliveryHEEntity.MODESTUD));
                    }
                }
            }
        }

        public bool ConditionMet(ILearningDeliveryHE learningDeliveryHE) =>
            LearningDeliveryHEConditionMet(learningDeliveryHE);

        public bool LearnerHEConditionMet(ILearnerHE learnerHE) =>
            !learnerHE.TTACCOMNullable.HasValue;

        public bool LearningDeliveryHEConditionMet(ILearningDeliveryHE learningDeliveryHE) =>
            learningDeliveryHE.MODESTUD == TypeOfMODESTUD.FullTimeAndSandwich;

        public bool LearnStartDateConditionMet(IEnumerable<ILearningDelivery> learningDeliveries) =>
            _dd06.Derive(learningDeliveries) >= _augustFirst2013;

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(int modeSTUD)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.MODESTUD, modeSTUD)
            };
        }
    }
}
