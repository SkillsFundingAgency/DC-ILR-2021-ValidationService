using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.OutGrade
{
    public class OutGrade_06Rule : AbstractRule, IRule<ILearner>
    {
        private readonly string[] _validOutGrades = { "FL", "U", "N", "X", "Y" };

        public OutGrade_06Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.OutGrade_06)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(
                    learningDelivery.OutcomeNullable,
                    learningDelivery.OutGrade))
                {
                    HandleValidationError(
                        objectToValidate.LearnRefNumber,
                        learningDelivery.AimSeqNumber,
                        BuildErrorMessageParameters(learningDelivery.OutcomeNullable, learningDelivery.OutGrade));
                }
            }
        }

        public bool ConditionMet(int? outcome, string outGrade)
        {
            return OutcomeConditionMet(outcome)
                   && OutGradeConditionMet(outGrade);
        }

        public bool OutcomeConditionMet(int? outcome)
        {
            return outcome == 3;
        }

        public bool OutGradeConditionMet(string outGrade)
        {
            return !_validOutGrades.Contains(outGrade) && outGrade != null;
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
