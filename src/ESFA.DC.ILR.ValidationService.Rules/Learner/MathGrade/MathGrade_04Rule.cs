using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.MathGrade
{
    public class MathGrade_04Rule : AbstractRule, IRule<ILearner>
    {
        private readonly ILearnerFAMQueryService _learnerFamQueryService;

        public MathGrade_04Rule(
            ILearnerFAMQueryService learnerFamQueryService,
            IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.MathGrade_04)
        {
            _learnerFamQueryService = learnerFamQueryService;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (ConditionMet(
                objectToValidate.MathGrade,
                objectToValidate.LearnerFAMs))
            {
                HandleValidationError(objectToValidate.LearnRefNumber, errorMessageParameters: BuildErrorMessageParameters(objectToValidate.MathGrade));
            }
        }

        public bool ConditionMet(string mathGrade, IEnumerable<ILearnerFAM> learnerFAMs)
        {
            return MathGradeConditionMet(mathGrade)
                   && LearnerFAMConditionMet(learnerFAMs);
        }

        public bool MathGradeConditionMet(string mathGrade)
        {
            return !string.IsNullOrWhiteSpace(mathGrade)
                   && !mathGrade.CaseInsensitiveEquals(ValidationConstants.None);
        }

        public bool LearnerFAMConditionMet(IEnumerable<ILearnerFAM> learnerFAMs)
        {
            var famType = "MCF";
            var famCodes = new[] { 2, 3, 4 };

            return _learnerFamQueryService.HasAnyLearnerFAMCodesForType(learnerFAMs, famType, famCodes);
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(string mathGrade)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.MathGrade, mathGrade),
            };
        }
    }
}
