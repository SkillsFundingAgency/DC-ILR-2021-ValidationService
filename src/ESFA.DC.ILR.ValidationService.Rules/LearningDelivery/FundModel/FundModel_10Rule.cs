using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.FundModel
{
    public class FundModel_10Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IDerivedData_35Rule _derivedData35;

        public FundModel_10Rule(
            IDerivedData_35Rule derivedData35,
            IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.FundModel_10)
        {
            _derivedData35 = derivedData35;
        }

        public void Validate(ILearner learner)
        {
            if (learner?.LearningDeliveries == null)
            {
                return;
            }

            foreach (var learningDelivery in learner.LearningDeliveries)
            {
                if (!_derivedData35.IsCombinedAuthorities(learningDelivery))
                {
                    continue;
                }

                if (ConditionMet(learningDelivery.FundModel))
                {
                    HandleValidationError(
                        learner.LearnRefNumber,
                        learningDelivery.AimSeqNumber,
                        BuildErrorMessageParameters(learningDelivery.FundModel, learningDelivery.LearningDeliveryFAMs.Where(x => x.LearnDelFAMType.ToUpper()== LearningDeliveryFAMTypeConstants.SOF).FirstOrDefault().LearnDelFAMCode));
                }
            }
        }

        public bool ConditionMet(int fundModel) =>
            fundModel != TypeOfFunding.AdultSkills && fundModel != TypeOfFunding.CommunityLearning;

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(int fundModel, string learnDelFAMCode)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, LearningDeliveryFAMTypeConstants.SOF),
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMCode, learnDelFAMCode),
                BuildErrorMessageParameter(PropertyNameConstants.FundModel, fundModel)

            };
        }
    }
}






