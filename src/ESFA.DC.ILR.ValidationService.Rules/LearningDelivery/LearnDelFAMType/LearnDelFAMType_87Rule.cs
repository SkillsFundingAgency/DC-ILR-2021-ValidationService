using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnDelFAMType
{
    public class LearnDelFAMType_87Rule : AbstractRule, IRule<ILearner>
    {
        public LearnDelFAMType_87Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.LearnDelFAMType_87)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            throw new System.NotImplementedException();
        }

        public bool ConditionMet()
        {
            return false;
        }

        public bool IsAdultSkillsFundingType(ILearningDelivery learningDelivery) =>
            learningDelivery.FundModel == FundModels.AdultSkills;

        public bool
    }
}
