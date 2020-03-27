using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.PriorAttain
{
    public class PriorAttain_07Rule : AbstractRule, IRule<ILearner>
    {
        public PriorAttain_07Rule(
            IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.PriorAttain_07)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(
                    learningDelivery.LearnStartDate,
                    learningDelivery.FundModel,
                    objectToValidate.PriorAttainNullable,
                    learningDelivery.ProgTypeNullable))
                {
                    HandleValidationError(objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber, BuildErrorMessageParameters(
                        learningDelivery.LearnStartDate,
                        learningDelivery.FundModel,
                        objectToValidate.PriorAttainNullable,
                        learningDelivery.ProgTypeNullable));
                }
            }
        }

        public bool ConditionMet(DateTime learnStartDate, int fundModel, int? priorAttain, int? progType)
        {
            return LearnStartDateConditionMet(learnStartDate)
                   && FundModelConditionMet(fundModel)
                   && PriorAttainConditionMet(priorAttain)
                   && ProgTypeConditionMet(progType);
        }

        public bool LearnStartDateConditionMet(DateTime learnStartDate)
        {
            var conditionStartDate = new DateTime(2016, 07, 31);

            return learnStartDate > conditionStartDate;
        }

        public bool FundModelConditionMet(int fundModel)
        {
            return fundModel == 35;
        }

        public bool PriorAttainConditionMet(int? priorAttain)
        {
            var priorAttains = new[] { 3, 4, 5, 10, 11, 12, 13, 97, 98 };

            return priorAttain.HasValue
                && priorAttains.Contains(priorAttain.Value);
        }

        public bool ProgTypeConditionMet(int? progType)
        {
            return progType == 24;
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(DateTime learnStartDate, int fundModel, int? priorAttain, int? progType)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, learnStartDate),
                BuildErrorMessageParameter(PropertyNameConstants.FundModel, fundModel),
                BuildErrorMessageParameter(PropertyNameConstants.PriorAttain, priorAttain),
                BuildErrorMessageParameter(PropertyNameConstants.ProgType, progType),
            };
        }
    }
}
