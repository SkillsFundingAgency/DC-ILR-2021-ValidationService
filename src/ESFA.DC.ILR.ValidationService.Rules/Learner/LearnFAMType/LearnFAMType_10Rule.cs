using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.LearnFAMType
{
    public class LearnFAMType_10Rule : AbstractRule, IRule<ILearner>
    {
        private readonly ILearnerFAMQueryService _learnerFAMQueryService;

        public LearnFAMType_10Rule(ILearnerFAMQueryService learnerFamQueryService, IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.LearnFAMType_10)
        {
            _learnerFAMQueryService = learnerFamQueryService;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate?.LearnerFAMs == null)
            {
                return;
            }

            if (ConditionMet(objectToValidate.LearnerFAMs))
            {
                HandleValidationError(objectToValidate.LearnRefNumber, errorMessageParameters: BuildErrorMessageParameters(LearnerFAMTypeConstants.LSR));
            }
        }

        public bool ConditionMet(IEnumerable<ILearnerFAM> learnerFAMs)
        {
            return _learnerFAMQueryService.GetLearnerFAMsCountByFAMType(learnerFAMs, LearnerFAMTypeConstants.LSR) > 4;
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(string learnFAMType)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.LearnFAMType, learnFAMType)
            };
        }
    }
}
