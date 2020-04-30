using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.CompStatus
{
    public class CompStatus_05Rule : AbstractRule, IRule<ILearner>
    {
        public CompStatus_05Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.CompStatus_05)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (!Excluded(learningDelivery.ProgTypeNullable, learningDelivery.FundModel)
                    && ConditionMet(learningDelivery.OutcomeNullable, learningDelivery.CompStatus))
                {
                    HandleValidationError(objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber, BuildErrorMessageParameters(learningDelivery.CompStatus, learningDelivery.OutcomeNullable));
                }
            }
        }

        public bool ConditionMet(int? outcome, int compStatus)
        {
            return outcome.HasValue && compStatus == CompletionState.IsOngoing;
        }

        public bool Excluded(int? ProgType, int? FundModel) =>
            ProgType == TypeOfLearningProgramme.ApprenticeshipStandard && FundModel == FundModels.ApprenticeshipsFrom1May2017;

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(int compStatus, int? outcome)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.CompStatus, compStatus),
                BuildErrorMessageParameter(PropertyNameConstants.Outcome, outcome),
            };
        }
    }
}
