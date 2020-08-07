using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnDelFAMType
{
    public class LearnDelFamType_86Rule : AbstractRule, IRule<ILearner>
    {
        private readonly HashSet<string> _ldmCodes = new HashSet<string>()
        {
            LearningDeliveryFAMCodeConstants.LDM_376,
        };

        private readonly ILearningDeliveryFAMQueryService _learningDeliveryFAMQueryService;
        private readonly DateTime _cutOffDate = new DateTime(2022, 03, 31);

        public LearnDelFamType_86Rule(
            ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService,
            IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.LearnDelFAMType_86)
        {
            _learningDeliveryFAMQueryService = learningDeliveryFAMQueryService;
        }

        public void Validate(ILearner learner)
        {
            if (learner?.LearningDeliveries == null)
            {
                return;
            }

            foreach (var learningDelivery in learner.LearningDeliveries)
            {
                var filteredFams = _learningDeliveryFAMQueryService.GetLearningDeliveryFAMsForTypeAndCodes(learningDelivery.LearningDeliveryFAMs, LearningDeliveryFAMTypeConstants.LDM, _ldmCodes);

                if (filteredFams == null || !filteredFams.Any())
                {
                    continue;
                }

                if (ConditionMet(learningDelivery))
                {
                    HandleValidationError(
                        learner.LearnRefNumber,
                        learningDelivery.AimSeqNumber,
                        BuildErrorMessageParameters(learningDelivery.LearnPlanEndDate));
                }
            }
        }

        public bool ConditionMet(ILearningDelivery learningDelivery)
        {
            return IsAdultSkillsFundingModel(learningDelivery.FundModel)
                   && LearnPlanEndDateAfterCutoff(learningDelivery.LearnPlanEndDate);
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(DateTime learnPlanEndDate)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, LearningDeliveryFAMTypeConstants.LDM),
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMCode, LearningDeliveryFAMCodeConstants.LDM_376),
                BuildErrorMessageParameter(PropertyNameConstants.FundModel, FundModels.AdultSkills),
                BuildErrorMessageParameter(PropertyNameConstants.LearnPlanEndDate, learnPlanEndDate.ToString(CultureInfo.CurrentUICulture))
            };
        }

        private bool IsAdultSkillsFundingModel(int fundModel)
        {
            return fundModel == FundModels.AdultSkills;
        }

        private bool LearnPlanEndDateAfterCutoff(DateTime learnPlanEndDate)
        {
            return learnPlanEndDate > _cutOffDate;
        }
    }
}
