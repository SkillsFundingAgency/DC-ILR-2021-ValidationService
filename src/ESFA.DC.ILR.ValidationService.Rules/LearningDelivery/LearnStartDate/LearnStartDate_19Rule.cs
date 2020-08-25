using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnStartDate
{
    public class LearnStartDate_19Rule : AbstractRule, IRule<ILearner>
    {
        private readonly ILARSDataService _larsDataService;
        private readonly ILearningDeliveryFAMQueryService _learningDeliveryFAMQueryService;

        public LearnStartDate_19Rule(
            ILARSDataService larsDataService,
            ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService,
            IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.LearnStartDate_19)
        {
            _larsDataService = larsDataService;
            _learningDeliveryFAMQueryService = learningDeliveryFAMQueryService;
        }

        public void Validate(ILearner learner)
        {
            if (learner.LearningDeliveries == null)
            {
                return;
            }

            foreach (var learningDelivery in learner.LearningDeliveries)
            {
                if (ConditionMet(learningDelivery))
                {
                    HandleValidationError(learner.LearnRefNumber, learningDelivery.AimSeqNumber, BuildMessageParametersFor(learningDelivery));
                }
            }
        }

        public bool ConditionMet(ILearningDelivery learningDelivery)
        {
            return learningDelivery.ProgTypeNullable == ProgTypes.ApprenticeshipStandard
                && learningDelivery.AimType == AimTypes.ComponentAimInAProgramme
                && !_learningDeliveryFAMQueryService.HasLearningDeliveryFAMType(learningDelivery.LearningDeliveryFAMs, LearningDeliveryFAMTypeConstants.RES)
                && LARSConditionMet(learningDelivery.StdCodeNullable.Value, learningDelivery.LearnStartDate);
        }

        public bool LARSConditionMet(int stdCode, DateTime learnStartDate)
        {
            var larsStandard = _larsDataService.GetStandardFor(stdCode);

            if (larsStandard != null)
            {
                return learnStartDate > larsStandard.LastDateStarts;
            }

            return false;
        }

        public IEnumerable<IErrorMessageParameter> BuildMessageParametersFor(ILearningDelivery thisDelivery)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.AimType, thisDelivery.AimType),
                BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, thisDelivery.LearnStartDate),
                BuildErrorMessageParameter(PropertyNameConstants.ProgType, thisDelivery.ProgTypeNullable),
                BuildErrorMessageParameter(PropertyNameConstants.StdCode, thisDelivery.StdCodeNullable)
            };
        }
    }
}
