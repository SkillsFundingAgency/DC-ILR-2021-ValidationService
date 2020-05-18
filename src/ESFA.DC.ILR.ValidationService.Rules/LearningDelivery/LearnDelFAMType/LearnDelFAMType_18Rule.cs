using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnDelFAMType
{
    public class LearnDelFAMType_18Rule : AbstractRule, IRule<ILearner>
    {
        private readonly ILearningDeliveryFAMQueryService _learningDeliveryFAMQueryService;

        private readonly string[] _learnDelfamTypes =
            {
                LearningDeliveryFAMTypeConstants.SOF,
                LearningDeliveryFAMTypeConstants.FFI,
                LearningDeliveryFAMTypeConstants.EEF,
                LearningDeliveryFAMTypeConstants.RES,
                LearningDeliveryFAMTypeConstants.ADL,
                LearningDeliveryFAMTypeConstants.ASL,
                LearningDeliveryFAMTypeConstants.SPP,
                LearningDeliveryFAMTypeConstants.FLN
            };

        public LearnDelFAMType_18Rule(
            IValidationErrorHandler validationErrorHandler,
            ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService)
            : base(validationErrorHandler, RuleNameConstants.LearnDelFAMType_18)
        {
            _learningDeliveryFAMQueryService = learningDeliveryFAMQueryService;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate?.LearningDeliveries == null)
            {
                return;
            }

            foreach (var learningDelivery in objectToValidate.LearningDeliveries.Where(d => d.LearningDeliveryFAMs != null))
            {
                var learnDelFamTypes = learningDelivery.LearningDeliveryFAMs.Select(x => x.LearnDelFAMType).Distinct();

                foreach (var learnDelFamType in learnDelFamTypes)
                {
                    if (ConditionMet(learnDelFamType, learningDelivery.LearningDeliveryFAMs))
                    {
                        HandleValidationError(objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber, BuildErrorMessageParameters(learnDelFamType));
                    }
                }
            }
        }

        public bool ConditionMet(string learnDelFamType, IReadOnlyCollection<ILearningDeliveryFAM> learningDeliveryFaMs)
        {
            return _learnDelfamTypes.Contains(learnDelFamType) &&
                _learningDeliveryFAMQueryService.GetLearningDeliveryFAMsCountByFAMType(
                    learningDeliveryFaMs, learnDelFamType) > 1;
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(string learnDelFamType)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, learnDelFamType)
            };
        }
    }
}
