using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.HE.PCOLAB
{
    public class PCOLAB_01Rule : AbstractRule, IRule<ILearner>
    {
        private readonly DateTime _firstAugust2013 = new DateTime(2013, 08, 01);

        public PCOLAB_01Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.PCOLAB_01)
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
                    learningDelivery.PartnerUKPRNNullable,
                    learningDelivery.LearningDeliveryHEEntity.PCOLABNullable))
                {
                    HandleValidationError(
                        objectToValidate.LearnRefNumber,
                        learningDelivery.AimSeqNumber,
                        BuildErrorMessageParameters(
                            learningDelivery.LearnStartDate,
                            learningDelivery.PartnerUKPRNNullable));
                }
            }
        }

        public bool ConditionMet(DateTime learnStartDate, int? partnerUkprn, decimal? pcolab)
        {
            return learnStartDate >= _firstAugust2013
                   && partnerUkprn.HasValue
                   && !pcolab.HasValue;
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(DateTime learnStartDate, int? partnerUkprn)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, learnStartDate),
                BuildErrorMessageParameter(PropertyNameConstants.PartnerUKPRN, partnerUkprn)
            };
        }
    }
}
