using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.HE.UCASAPPID
{
    public class UCASAPPID_01Rule : AbstractRule, IRule<ILearner>
    {
        private readonly DateTime _firstAugust2013 = new DateTime(2013, 08, 01);

        public UCASAPPID_01Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.UCASAPPID_01)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate?.LearningDeliveries == null || objectToValidate.LearnerHEEntity?.UCASPERID == null)
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
                    learningDelivery.LearningDeliveryHEEntity.UCASAPPID))
                {
                    HandleValidationError(
                        objectToValidate.LearnRefNumber,
                        learningDelivery.AimSeqNumber,
                        BuildErrorMessageParameters(
                            objectToValidate.LearnerHEEntity.UCASPERID,
                            learningDelivery.LearnStartDate));
                }
            }
        }

        public bool ConditionMet(DateTime learnStartDate, string ucasAppId)
        {
            return learnStartDate >= _firstAugust2013
                   && string.IsNullOrEmpty(ucasAppId);
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(string ucasPerId, DateTime learnStartDate)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.UCASPERID, ucasPerId),
                BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, learnStartDate)
            };
        }
    }
}
