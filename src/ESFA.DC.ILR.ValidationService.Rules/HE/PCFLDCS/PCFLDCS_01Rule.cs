using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.HE.PCFLDCS
{
    public class PCFLDCS_01Rule : AbstractRule, IRule<ILearner>
    {
        private const int _expectedTotal = 100;
        private readonly DateTime _firstAugust2009 = new DateTime(2009, 08, 01);

        public PCFLDCS_01Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.PCFLDCS_01)
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
                    learningDelivery.LearningDeliveryHEEntity.PCFLDCSNullable,
                    learningDelivery.LearningDeliveryHEEntity.PCSLDCSNullable,
                    learningDelivery.LearningDeliveryHEEntity.PCTLDCSNullable))
                {
                    HandleValidationError(
                        objectToValidate.LearnRefNumber,
                        learningDelivery.AimSeqNumber,
                        BuildErrorMessageParameters(
                            learningDelivery.LearningDeliveryHEEntity.PCFLDCSNullable,
                            learningDelivery.LearningDeliveryHEEntity.PCSLDCSNullable,
                            learningDelivery.LearningDeliveryHEEntity.PCTLDCSNullable));
                }
            }
        }

        public bool ConditionMet(DateTime learnStartDate, decimal? pcfldcs, decimal? pcsldcs, decimal? pctldcs)
        {
            return learnStartDate >= _firstAugust2009
                   && LDCSConditionMet(pcfldcs, pcsldcs, pctldcs);
        }

        public bool LDCSConditionMet(decimal? pcfldcs, decimal? pcsldcs, decimal? pctldcs)
        {
            if (!pcfldcs.HasValue && !pcsldcs.HasValue && !pctldcs.HasValue)
            {
                return false;
            }

            decimal total = pcfldcs.GetValueOrDefault(0)
                            + pcsldcs.GetValueOrDefault(0)
                            + pctldcs.GetValueOrDefault(0);

            return total != _expectedTotal;
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(decimal? pcfldcs, decimal? pcsldcs, decimal? pctldcs)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.PCFLDCS, pcfldcs),
                BuildErrorMessageParameter(PropertyNameConstants.PCSLDCS, pcsldcs),
                BuildErrorMessageParameter(PropertyNameConstants.PCTLDCS, pctldcs)
            };
        }
    }
}
