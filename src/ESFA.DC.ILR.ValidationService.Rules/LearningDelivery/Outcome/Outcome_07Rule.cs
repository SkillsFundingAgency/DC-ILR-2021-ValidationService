using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.Outcome
{
    public class Outcome_07Rule : AbstractRule, IRule<ILearner>
    {
        private readonly ILearnerDPQueryService _learnerDpQueryService;

        public Outcome_07Rule(ILearnerDPQueryService learnerDpQueryService, IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.Outcome_07)
        {
            _learnerDpQueryService = learnerDpQueryService;
        }

        public void Validate(ILearner learner)
        {
            if (learner?.LearningDeliveries == null)
            {
                return;
            }

            foreach (var learningDelivery in learner.LearningDeliveries)
            {
                if (LearningDeliveryConditionMet(learningDelivery) && DpOutcomeConditionMet(learner.LearnRefNumber))
                {
                    HandleValidationError(
                       learner.LearnRefNumber,
                       learningDelivery.AimSeqNumber,
                       BuildErrorMessageParameters(learner.LearnRefNumber, learningDelivery));
                }
            }
        }

        public bool LearningDeliveryConditionMet(ILearningDelivery learningDelivery)
        {
            return learningDelivery.AimType == AimTypes.ProgrammeAim
                    && (learningDelivery.ProgTypeNullable == ProgTypes.Traineeship)
                    && (learningDelivery.OutcomeNullable == OutcomeConstants.PartialAchievement || learningDelivery.OutcomeNullable == OutcomeConstants.NoAchievement);
        }

        public bool DpOutcomeConditionMet(string learnRefNum)
        {
            var outcomes = _learnerDpQueryService.GetDestinationAndProgressionForLearner(learnRefNum)?.DPOutcomes;

            return !(outcomes != null && outcomes.Any());
        }

        private IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(string learnRefNumber, ILearningDelivery learningDelivery)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.LearnRefNumber, learnRefNumber),
                BuildErrorMessageParameter(PropertyNameConstants.AimType, learningDelivery.AimType),
                BuildErrorMessageParameter(PropertyNameConstants.ProgType, learningDelivery.ProgTypeNullable),
                BuildErrorMessageParameter(PropertyNameConstants.Outcome, learningDelivery.OutcomeNullable)
            };
        }
    }
}
