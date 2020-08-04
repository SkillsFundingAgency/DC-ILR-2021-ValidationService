using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnDelFAMType
{
    public class LearnDelFamType_85Rule : AbstractRule, IRule<ILearner>
    {
        private readonly HashSet<string> _ldmCodes = new HashSet<string>()
        {
            LearningDeliveryFAMCodeConstants.LDM_376,
        };

        private readonly ILearningDeliveryFAMQueryService _learningDeliveryFAMQueryService;
        private readonly ILARSDataService _larsDataService;

        public LearnDelFamType_85Rule(
            ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService,
            IValidationErrorHandler validationErrorHandler,
            ILARSDataService larsDataService)
            : base(validationErrorHandler, RuleNameConstants.LearnDelFAMType_85)
        {
            _learningDeliveryFAMQueryService = learningDeliveryFAMQueryService;
            _larsDataService = larsDataService;
        }

        public void Validate(ILearner learner)
        {
            if (learner?.LearningDeliveries == null)
            {
                return;
            }

            foreach (var learningDelivery in learner.LearningDeliveries)
            {
                if (!_learningDeliveryFAMQueryService.GetLearningDeliveryFAMsForTypeAndCodes(learningDelivery.LearningDeliveryFAMs, LearningDeliveryFAMTypeConstants.LDM, _ldmCodes).Any())
                {
                    continue;
                }

                var larsCategories = _larsDataService.GetCategoriesFor(learningDelivery.LearnAimRef);

                if (ConditionMet(learningDelivery, larsCategories))
                {
                    HandleValidationError(
                        learner.LearnRefNumber,
                        learningDelivery.AimSeqNumber,
                        BuildErrorMessageParameters(larsCategories));
                }
            }
        }

        public bool ConditionMet(ILearningDelivery learningDelivery, IEnumerable<ILARSLearningCategory> larsLearningCategories)
        {
            return IsAdultSkillsFundingModel(learningDelivery.FundModel)
                   && !IsCovid19SkillsOffer(larsLearningCategories);
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(IEnumerable<ILARSLearningCategory> larsLearningCategories)
        {
            var errorList = new List<IErrorMessageParameter>
            {
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, LearningDeliveryFAMTypeConstants.LDM),
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMCode, LearningDeliveryFAMCodeConstants.LDM_376),
                BuildErrorMessageParameter(PropertyNameConstants.FundModel, FundModels.AdultSkills)
            };

            larsLearningCategories.ForEach(lc => errorList.Add(BuildErrorMessageParameter(PropertyNameConstants.LarsCategoryRef, lc.CategoryRef)));

            return errorList;
        }

        private bool IsCovid19SkillsOffer(IEnumerable<ILARSLearningCategory> larsLearningCategories)
        {
            return larsLearningCategories != null && larsLearningCategories.Any(l => l.CategoryRef == LARSConstants.Categories.Covid19SkillsOffer);
        }

        private bool IsAdultSkillsFundingModel(int fundModel)
        {
            return fundModel == FundModels.AdultSkills;
        }
    }
}
