namespace ESFA.DC.ILR.ValidationService.Rules.Learner.LearnFAMType
{
    using System.Collections.Generic;
    using ESFA.DC.ILR.Model.Interface;
    using ESFA.DC.ILR.ValidationService.Data.Interface;
    using ESFA.DC.ILR.ValidationService.Interface;
    using ESFA.DC.ILR.ValidationService.Rules.Abstract;
    using ESFA.DC.ILR.ValidationService.Rules.Constants;

    public class LearnFAMType_01Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IProvideLookupDetails _lookupDetails;

        public LearnFAMType_01Rule(IProvideLookupDetails lookupDetails, IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.LearnFAMType_01)
        {
            _lookupDetails = lookupDetails;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate.LearnerFAMs == null)
            {
                return;
            }

            foreach (var learnerFam in objectToValidate.LearnerFAMs)
            {
                if (ConditionMet(learnerFam))
                {
                    HandleValidationError(objectToValidate.LearnRefNumber, errorMessageParameters: BuildErrorMessageParameters(learnerFam.LearnFAMType, learnerFam.LearnFAMCode));
                }
            }
        }

        public bool ConditionMet(ILearnerFAM learnerFam)
        {
            return learnerFam.LearnFAMType != null
                && !_lookupDetails.Contains(TypeOfLimitedLifeLookup.LearnFAMType, $"{learnerFam.LearnFAMType}{learnerFam.LearnFAMCode}");
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(string learnFAMType, int learnFAMCode)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.LearnFAMType, learnFAMType),
                BuildErrorMessageParameter(PropertyNameConstants.LearnFAMCode, learnFAMCode)
            };
        }
    }
}
