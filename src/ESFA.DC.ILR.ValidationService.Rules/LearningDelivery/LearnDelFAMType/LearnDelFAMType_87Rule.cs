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
    public class LearnDelFAMType_87Rule : AbstractRule, IRule<ILearner>
    {
        private readonly ILearningDeliveryFAMQueryService _learningDeliveryFAMQueryService;

        private readonly DateTime cutOffEndDate = new DateTime(2022, 07, 31);
        private readonly HashSet<string> _ldmCodes = new HashSet<string>()
        {
            LearningDeliveryFAMCodeConstants.LDM_376
        };

        public LearnDelFAMType_87Rule(
            ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService,
            IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.LearnDelFAMType_87)
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
                                            BuildErrorMessageParameters(learningDelivery.LearnActEndDateNullable));
                }
            }
        }

        public bool ConditionMet(ILearningDelivery learningDelivery) =>
            IsAdultSkillsFundingModel(learningDelivery.FundModel)
            && EndDateCheck(learningDelivery.LearnActEndDateNullable);

        public bool IsAdultSkillsFundingModel(int fundModel) =>
            fundModel == FundModels.AdultSkills;

        public bool EndDateCheck(DateTime? endDate) =>
            endDate > cutOffEndDate;

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(DateTime? learnActEndDate)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, LearningDeliveryFAMTypeConstants.LDM),
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMCode, LearningDeliveryFAMCodeConstants.LDM_376),
                BuildErrorMessageParameter(PropertyNameConstants.FundModel, FundModels.AdultSkills),
                BuildErrorMessageParameter(PropertyNameConstants.LearnActEndDate, learnActEndDate.ToString())
            };
        }
    }
}
