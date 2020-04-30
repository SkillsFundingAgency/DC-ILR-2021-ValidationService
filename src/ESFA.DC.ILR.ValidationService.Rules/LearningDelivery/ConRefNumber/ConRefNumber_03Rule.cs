using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.ConRefNumber
{
    public class ConRefNumber_03Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IEnumerable<int> _fundModels = new HashSet<int>()
        {
            FundModels.Age16To19ExcludingApprenticeships,
            FundModels.Other16To19,
            FundModels.AdultSkills,
            FundModels.ApprenticeshipsFrom1May2017,
            FundModels.OtherAdult,
            FundModels.CommunityLearning,
            FundModels.NotFundedByESFA,
        };

        public ConRefNumber_03Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.ConRefNumber_03)
        {
        }

        public ConRefNumber_03Rule()
            : base(null, RuleNameConstants.ConRefNumber_03)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(learningDelivery.FundModel, learningDelivery.ConRefNumber))
                {
                    HandleValidationError(objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber, BuildErrorMessageParameters(learningDelivery.FundModel, learningDelivery.ConRefNumber));
                }
            }
        }

        public bool ConditionMet(int fundModel, string conRefNumber)
        {
            return ConRefNumberConditionMet(conRefNumber) && FundModelConditionMet(fundModel);
        }

        public virtual bool FundModelConditionMet(int fundModel)
        {
            return _fundModels.Contains(fundModel);
        }

        public virtual bool ConRefNumberConditionMet(string conRefNumber)
        {
            return !string.IsNullOrWhiteSpace(conRefNumber);
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(int fundModel, string conRefNumber)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.FundModel, fundModel),
                BuildErrorMessageParameter(PropertyNameConstants.ConRefNumber, conRefNumber),
            };
        }
    }
}
