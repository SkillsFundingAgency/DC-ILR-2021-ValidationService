using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnDelFAMType
{
    public class LearnDelFAMType_44Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IEnumerable<int> _fundModels = new HashSet<int>() { FundModels.AdultSkills, FundModels.EuropeanSocialFund, FundModels.OtherAdult };
        private readonly IEnumerable<int> _aimTypes = new HashSet<int>() { 3, 5 };
        private readonly DateTime _firstAugust2015 = new DateTime(2015, 08, 01);

        private readonly ILearningDeliveryFAMQueryService _learningDeliveryFAMQueryService;

        public LearnDelFAMType_44Rule(
            ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService,
            IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.LearnDelFAMType_44)
        {
            _learningDeliveryFAMQueryService = learningDeliveryFAMQueryService;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate.LearningDeliveries == null)
            {
                return;
            }

            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(
                    learningDelivery.FundModel,
                    learningDelivery.LearnStartDate,
                    learningDelivery.LearningDeliveryFAMs,
                    learningDelivery.AimType,
                    learningDelivery.ProgTypeNullable))
                {
                    HandleValidationError(
                        objectToValidate.LearnRefNumber,
                        learningDelivery.AimSeqNumber,
                        BuildErrorMessageParameters(
                            learningDelivery.AimType,
                            learningDelivery.LearnStartDate,
                            learningDelivery.FundModel,
                            learningDelivery.ProgTypeNullable,
                            LearningDeliveryFAMTypeConstants.LDM,
                            "034"));
                }
            }
        }

        public bool ConditionMet(
            int fundModel,
            DateTime learnStartDate,
            IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs,
            int aimType,
            int? progType)
        {
            return FundModelConditionMet(fundModel)
                && LearningDeliveryStartDateConditionMet(learnStartDate)
                && LearningDeliveryFAMsConditionMet(learningDeliveryFAMs)
                && LearningDeliveryAimTypeConditionMet(aimType)
                && LearningDeliveryProgTypeConditionMet(progType);
        }

        public bool FundModelConditionMet(int fundModel)
        {
            return _fundModels.Contains(fundModel);
        }

        public bool LearningDeliveryStartDateConditionMet(DateTime learnStartDate)
        {
            return learnStartDate >= _firstAugust2015;
        }

        public bool LearningDeliveryFAMsConditionMet(IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs)
        {
            return !_learningDeliveryFAMQueryService.HasLearningDeliveryFAMType(learningDeliveryFAMs, LearningDeliveryFAMTypeConstants.HHS)
                && !_learningDeliveryFAMQueryService.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, LearningDeliveryFAMTypeConstants.LDM, "034");
        }

        public bool LearningDeliveryAimTypeConditionMet(int aimType)
        {
            return !_aimTypes.Contains(aimType);
        }

        public bool LearningDeliveryProgTypeConditionMet(int? progType)
        {
            return !progType.HasValue
                || !(progType.HasValue && progType == 25);
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(int aimType, DateTime learnStartDate, int fundModel, int? progType, string learnDelFAMType, string learnDelFAMCode)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.AimType, aimType),
                BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, learnStartDate),
                BuildErrorMessageParameter(PropertyNameConstants.FundModel, fundModel),
                BuildErrorMessageParameter(PropertyNameConstants.ProgType, progType),
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, learnDelFAMType),
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMCode, learnDelFAMCode)
            };
        }
    }
}
