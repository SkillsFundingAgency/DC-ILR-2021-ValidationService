using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnAimRef
{
    public class LearnAimRef_86Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IValidationErrorHandler _messageHandler;
        private readonly ILearningDeliveryFAMQueryService _learningDeliveryFAMQueryService;

        public LearnAimRef_86Rule(
            IValidationErrorHandler validationErrorHandler,
            ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService)
            : base(validationErrorHandler, RuleNameConstants.LearnAimRef_86)
        {
            _messageHandler = validationErrorHandler;
            _learningDeliveryFAMQueryService = learningDeliveryFAMQueryService;
        }

        public bool IsWorkExperience(ILearningDelivery delivery) =>
            delivery.LearnAimRef.CaseInsensitiveEquals(AimTypes.References.WorkExperience) ||
            delivery.LearnAimRef.CaseInsensitiveEquals(AimTypes.References.IndustryPlacement) ||
            delivery.LearnAimRef.CaseInsensitiveEquals(AimTypes.References.TLevelWorkExperience);

        public bool LearningDeliveryFAMExclusion(IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs) =>
             _learningDeliveryFAMQueryService.HasLearningDeliveryFAMCodeForType(
                learningDeliveryFAMs,
                LearningDeliveryFAMTypeConstants.LDM,
                LearningDeliveryFAMCodeConstants.LDM_SteelRedundancy);

        public bool ProgTypeCondition(int? progType) => progType != ProgTypes.Traineeship;

        public bool FundModelCondition(int fundModel) => fundModel == FundModels.AdultSkills;

        public bool ConditionMet(ILearningDelivery delivery) =>
            FundModelCondition(delivery.FundModel)
            && ProgTypeCondition(delivery.ProgTypeNullable)
            && IsWorkExperience(delivery)
            && !LearningDeliveryFAMExclusion(delivery.LearningDeliveryFAMs);

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate.LearningDeliveries == null)
            {
                return;
            }

            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(learningDelivery))
                {
                    HandleValidationError(
                        objectToValidate.LearnRefNumber,
                        learningDelivery.AimSeqNumber,
                        BuildErrorMessageParameters(learningDelivery.LearnAimRef, learningDelivery.FundModel, learningDelivery.ProgTypeNullable));
                }
            }
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(string learnAimRef, int fundModel, int? progType)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.LearnAimRef, learnAimRef),
                BuildErrorMessageParameter(PropertyNameConstants.FundModel, fundModel),
                BuildErrorMessageParameter(PropertyNameConstants.ProgType, progType)
            };
        }
    }
}
