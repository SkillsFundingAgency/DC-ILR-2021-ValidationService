using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnDelFAMType
{
    public class LearnDelFAMType_14Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IEnumerable<int> _fundModels = new HashSet<int>() { FundModels.AdultSkills, FundModels.ApprenticeshipsFrom1May2017, FundModels.OtherAdult };

        private readonly ILearningDeliveryFAMQueryService _learningDeliveryFAMQueryService;
        private readonly IDerivedData_07Rule _dd07;

        public LearnDelFAMType_14Rule(
            IValidationErrorHandler validationErrorHandler,
            ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService,
            IDerivedData_07Rule dd07)
            : base(validationErrorHandler, RuleNameConstants.LearnDelFAMType_14)
        {
            _learningDeliveryFAMQueryService = learningDeliveryFAMQueryService;
            _dd07 = dd07;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate == null)
            {
                return;
            }

            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(learningDelivery.FundModel, learningDelivery.ProgTypeNullable, learningDelivery.LearningDeliveryFAMs))
                {
                    HandleValidationError(objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber, BuildErrorMessageParameters(learningDelivery.FundModel, LearningDeliveryFAMTypeConstants.EEF));
                }
            }
        }

        public bool ConditionMet(int fundModel, int? progType, IReadOnlyCollection<ILearningDeliveryFAM> learningDeliveryFAMs)
        {
            return (FundModelConditionMet(fundModel) || DD07ConditionMet(progType))
                && LearningDeliveryFAMsConditionMet(learningDeliveryFAMs);
        }

        public bool LearningDeliveryFAMsConditionMet(IReadOnlyCollection<ILearningDeliveryFAM> learningDeliveryFAMs)
        {
            return _learningDeliveryFAMQueryService.HasLearningDeliveryFAMType(learningDeliveryFAMs, LearningDeliveryFAMTypeConstants.EEF);
        }

        public bool FundModelConditionMet(int fundModel)
        {
            return !_fundModels.Contains(fundModel);
        }

        public bool DD07ConditionMet(int? progType)
        {
            return !_dd07.IsApprenticeship(progType);
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(int fundModel, string learnDelFAMType)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.FundModel, fundModel),
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, learnDelFAMType)
            };
        }
    }
}
