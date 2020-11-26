using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.HE.SEC
{
    public class SEC_02Rule : AbstractRule, IRule<ILearner>
    {
        private readonly DateTime _firstAugust2013 = new DateTime(2013, 08, 01);
        private readonly string[] _validDomiciles = { "XF", "XG", "XH", "XI", "XK" };

        public SEC_02Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.SEC_02)
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
                    learningDelivery.LearningDeliveryHEEntity.UCASAPPID,
                    learningDelivery.LearningDeliveryHEEntity.DOMICILE,
                    learningDelivery.LearningDeliveryHEEntity.SECNullable))
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

        public bool ConditionMet(DateTime learnStartDate, string ucasAppId, string domicile, int? sec)
        {
            return learnStartDate >= _firstAugust2013
                   && !string.IsNullOrEmpty(ucasAppId)
                   && _validDomiciles.Any(domicile.CaseInsensitiveEquals)
                   && !sec.HasValue;
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
