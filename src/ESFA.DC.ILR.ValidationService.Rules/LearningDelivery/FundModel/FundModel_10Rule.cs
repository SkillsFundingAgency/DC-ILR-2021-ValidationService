using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
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
                if (ConditionMet(learningDelivery))
                {
                    var errorMessageParameters = BuildErrorMessageParameters(learningDelivery.FundModel, learningDelivery.LearningDeliveryFAMs.FirstOrDefault(x => x.LearnDelFAMType.CaseInsensitiveEquals(LearningDeliveryFAMTypeConstants.SOF))?.LearnDelFAMCode);
                    HandleValidationError(learner.LearnRefNumber, learningDelivery.AimSeqNumber, errorMessageParameters);
                }
            }
        }

        public bool ConditionMet(ILearningDelivery learningDelivery) =>
            DD35ConditionMet(learningDelivery) && FundModelConditionMet(learningDelivery.FundModel);

        public bool DD35ConditionMet(ILearningDelivery learningDelivery) => _derivedData35.IsCombinedAuthorities(learningDelivery);

        public bool FundModelConditionMet(int fundModel) => fundModel != TypeOfFunding.AdultSkills && fundModel != TypeOfFunding.CommunityLearning;

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






