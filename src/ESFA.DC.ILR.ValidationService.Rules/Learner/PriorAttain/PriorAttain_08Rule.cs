using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.PriorAttain
{
    public class PriorAttain_08Rule : AbstractRule, IRule<ILearner>
    {
        private readonly DateTime _startDate = new DateTime(2020, 08, 01);
        private readonly HashSet<int> _priorAttains = new HashSet<int> { 4, 5, 10, 11, 12, 13, 97, 98 };

        public PriorAttain_08Rule(
            IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.PriorAttain_08)
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

        public bool LearnStartDateConditionMet(DateTime learnStartDate) =>
            learnStartDate >= _startDate;

        public bool FundModelConditionMet(int fundModel) =>
            fundModel == FundModels.AdultSkills;

        public bool PriorAttainConditionMet(int? priorAttain) =>
            priorAttain.HasValue
            && _priorAttains.Contains(priorAttain.Value);

        public bool ProgTypeConditionMet(int? progType) =>
            progType == ProgTypes.Traineeship;

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
