using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.LearnFAMType
{
    public class LearnFAMType_09Rule : AbstractRule, IRule<ILearner>
    {
        private readonly ILearnerFAMQueryService _learnerFAMQueryService;
        private readonly string[] _learnFamTypes =
        {
            LearnerFAMTypeConstants.HNS,
            LearnerFAMTypeConstants.EHC,
            LearnerFAMTypeConstants.DLA,
            LearnerFAMTypeConstants.SEN,
            LearnerFAMTypeConstants.MCF,
            LearnerFAMTypeConstants.ECF,
            LearnerFAMTypeConstants.FME
        };

        public LearnFAMType_09Rule(ILearnerFAMQueryService learnerFamQueryService, IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.LearnFAMType_09)
        {
            _learnerFAMQueryService = learnerFamQueryService;
        }

        public void Validate(ILearner objectToValidate)
       {
            if (objectToValidate.LearnerFAMs == null)
            {
                return;
            }

            var learnFamTypes = objectToValidate.LearnerFAMs.Select(x => x.LearnFAMType).Distinct();

            foreach (var learnerFamType in learnFamTypes)
            {
                if (ConditionMet(learnerFamType, objectToValidate.LearnerFAMs))
                {
                    HandleValidationError(objectToValidate.LearnRefNumber, errorMessageParameters: BuildErrorMessageParameters(learnerFamType));
                }
            }
        }

        public bool ConditionMet(string learnerFamType, IEnumerable<ILearnerFAM> learnerFAMs)
        {
            return _learnFamTypes.Contains(learnerFamType) &&
                   _learnerFAMQueryService.GetLearnerFAMsCountByFAMType(learnerFAMs, learnerFamType) > 1;
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
