using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.HE.SOC2000
{
    public class SOC2000_03Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IProvideLookupDetails _provideLookupDetails;

        private readonly DateTime _firstAugust2014 = new DateTime(2014, 08, 01);

        public SOC2000_03Rule(
            IProvideLookupDetails provideLookupDetails,
            IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.SOC2000_03)
        {
            _provideLookupDetails = provideLookupDetails;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate?.LearningDeliveries == null)
            {
                return;
            }

            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (learningDelivery.LearningDeliveryHEEntity?.SOC2000Nullable == null)
                {
                    continue;
                }

                if (ConditionMet(
                    learningDelivery.LearnStartDate,
                    learningDelivery.LearningDeliveryHEEntity.SOC2000Nullable.Value))
                {
                    HandleValidationError(
                        objectToValidate.LearnRefNumber,
                        learningDelivery.AimSeqNumber,
                        BuildErrorMessageParameters(
                            learningDelivery.LearnStartDate,
                            learningDelivery.LearningDeliveryHEEntity.SOC2000Nullable.Value));
                }
            }
        }

        public bool ConditionMet(DateTime learnStartDate, int soc2000)
        {
            return learnStartDate < _firstAugust2014
                   && !_provideLookupDetails.Contains(TypeOfIntegerCodedLookup.SOC2000, soc2000);
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(DateTime learnStartDate, int soc2000)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, learnStartDate),
                BuildErrorMessageParameter(PropertyNameConstants.SOC2000, soc2000)
            };
        }
    }
}