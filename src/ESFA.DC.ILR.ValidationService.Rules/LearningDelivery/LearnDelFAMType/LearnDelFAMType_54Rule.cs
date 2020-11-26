using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnDelFAMType
{
    public class LearnDelFAMType_54Rule : AbstractRule, IRule<ILearner>
    {
        private readonly int _fundModel = FundModels.ApprenticeshipsFrom1May2017;

        private readonly ILearningDeliveryFAMQueryService _learningDeliveryFAMQueryService;

        public LearnDelFAMType_54Rule(
            IValidationErrorHandler validationErrorHandler,
            ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService)
            : base(validationErrorHandler, RuleNameConstants.LearnDelFAMType_54)
        {
            _learningDeliveryFAMQueryService = learningDeliveryFAMQueryService;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate?.LearningDeliveries == null)
            {
                return;
            }

            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(
                    learningDelivery.FundModel,
                    learningDelivery.ProgTypeNullable,
                    learningDelivery.LearningDeliveryFAMs))
                {
                    HandleValidationError(
                        objectToValidate.LearnRefNumber,
                        learningDelivery.AimSeqNumber,
                        BuildErrorMessageParameters(
                            learningDelivery.ProgTypeNullable,
                            LearningDeliveryFAMTypeConstants.EEF,
                            LearningDeliveryFAMCodeConstants.ACT_ContractESFA));
                }
            }
        }

        public bool ConditionMet(int fundModel, int? progType, IReadOnlyCollection<ILearningDeliveryFAM> learningDeliveryFAMs)
        {
            return !(FundModelConditionMet(fundModel)
                || ProgTypeConditionMet(progType))
                && LearningDeliveryFAMsCondtionMet(learningDeliveryFAMs);
        }

        public bool LearningDeliveryFAMsCondtionMet(IReadOnlyCollection<ILearningDeliveryFAM> learningDeliveryFAMs)
        {
            return _learningDeliveryFAMQueryService
                    .HasLearningDeliveryFAMCodeForType(
                        learningDeliveryFAMs,
                        LearningDeliveryFAMTypeConstants.EEF,
                        LearningDeliveryFAMCodeConstants.ACT_ContractESFA)
                && _learningDeliveryFAMQueryService
                    .HasLearningDeliveryFAMCodeForType(
                        learningDeliveryFAMs,
                        LearningDeliveryFAMTypeConstants.FFI,
                        LearningDeliveryFAMCodeConstants.ACT_ContractESFA);
        }

        public bool ProgTypeConditionMet(int? progType) => progType == ProgTypes.ApprenticeshipStandard;

        public bool FundModelConditionMet(int fundModel) => fundModel == _fundModel;

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(int? progTypeNullable, string learnDelFAMType, string learnDelFAMCode)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.ProgType, progTypeNullable),
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, learnDelFAMType),
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMCode, learnDelFAMCode)
            };
        }
    }
}
