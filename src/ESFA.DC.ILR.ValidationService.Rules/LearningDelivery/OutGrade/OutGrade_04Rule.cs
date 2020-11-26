using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.OutGrade
{
    public class OutGrade_04Rule : AbstractRule, IRule<ILearner>
    {
        public OutGrade_04Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.OutGrade_04)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(
                    learningDelivery.OutGrade,
                    learningDelivery.OutcomeNullable))
                {
                    HandleValidationError(
                        objectToValidate.LearnRefNumber,
                        learningDelivery.AimSeqNumber,
                        BuildErrorMessageParameters(learningDelivery.OutcomeNullable, learningDelivery.OutGrade));
                }
            }
        }

        public bool ConditionMet(string outGrade, int? outcome)
        {
            return !string.IsNullOrWhiteSpace(outGrade) && (outcome == 8 || !outcome.HasValue);
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(int? outcome, string outGrade)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.Outcome, outcome),
                BuildErrorMessageParameter(PropertyNameConstants.OutGrade, outGrade)
            };
        }
    }
}
