using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.AddHours
{
    public class AddHours_02Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IReadOnlyCollection<int> _fundModels = new HashSet<int>()
        {
            FundModels.Age16To19ExcludingApprenticeships,
            FundModels.Other16To19,
            FundModels.CommunityLearning,
            FundModels.NotFundedByESFA
        };

        public AddHours_02Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.AddHours_02)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(
                    learningDelivery.FundModel,
                    learningDelivery.AddHoursNullable))
                {
                    HandleValidationError(objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber, BuildErrorMessageParameters(learningDelivery.FundModel, learningDelivery.AddHoursNullable));
                }
            }
        }

        public bool ConditionMet(int fundModel, int? addHours)
        {
            return addHours.HasValue
                   && _fundModels.Contains(fundModel);
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(int fundModel, int? addHoursNullable)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.FundModel, fundModel),
                BuildErrorMessageParameter(PropertyNameConstants.AddHours, addHoursNullable)
            };
        }
    }
}
