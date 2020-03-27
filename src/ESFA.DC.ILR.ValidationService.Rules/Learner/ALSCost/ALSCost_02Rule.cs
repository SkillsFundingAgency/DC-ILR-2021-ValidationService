using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.ALSCost
{
    public class ALSCost_02Rule : AbstractRule, IRule<ILearner>
    {
        private readonly ILearnerFAMQueryService _learnerFamQueryService;

        public ALSCost_02Rule(ILearnerFAMQueryService learnerFamQueryService, IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.ALSCost_02)
        {
            _learnerFamQueryService = learnerFamQueryService;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate == null)
            {
                return;
            }

            if (ConditionMet(objectToValidate.ALSCostNullable, objectToValidate.LearnerFAMs))
            {
                HandleValidationError(objectToValidate.LearnRefNumber, null, BuildErrorMessageParameters(objectToValidate.ALSCostNullable));
            }
        }

        public bool ConditionMet(long? aLsCostNullable, IReadOnlyCollection<ILearnerFAM> learnerFams)
        {
            return aLsCostNullable.HasValue
                && !_learnerFamQueryService.HasLearnerFAMType(learnerFams, LearnerFAMTypeConstants.HNS);
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(long? alsCost)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.ALSCost, alsCost),
                BuildErrorMessageParameter(PropertyNameConstants.LearnFAMType, LearnerFAMTypeConstants.HNS),
            };
        }
    }
}