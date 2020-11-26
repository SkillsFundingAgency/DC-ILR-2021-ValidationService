using System;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.LearnFAMType
{
    public class LearnFAMType_03Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IDerivedData_06Rule _derivedData06;
        private readonly IProvideLookupDetails _lookupDetails;

        public LearnFAMType_03Rule(
            IDerivedData_06Rule derivedData06,
            IProvideLookupDetails lookupDetails,
            IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.LearnFAMType_03)
        {
            _derivedData06 = derivedData06;
            _lookupDetails = lookupDetails;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate.LearningDeliveries == null || objectToValidate.LearnerFAMs == null)
            {
                return;
            }

            var dd06Date = _derivedData06.Derive(objectToValidate.LearningDeliveries);

            foreach (var learnerFam in objectToValidate.LearnerFAMs)
            {
                if (ConditionMet(learnerFam, dd06Date))
                {
                    HandleValidationError(objectToValidate.LearnRefNumber);
                }
            }
        }

        public bool ConditionMet(ILearnerFAM learnerFam, DateTime dd06Date)
        {
            return _lookupDetails.IsExpired(TypeOfLimitedLifeLookup.LearnFAMType, $"{learnerFam.LearnFAMType}{learnerFam.LearnFAMCode}", dd06Date);
        }
    }
}
