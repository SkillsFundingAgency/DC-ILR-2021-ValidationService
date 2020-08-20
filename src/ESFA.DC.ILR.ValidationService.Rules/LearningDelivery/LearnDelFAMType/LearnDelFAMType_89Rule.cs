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
    public class LearnDelFAMType_89Rule : AbstractRule, IRule<ILearner>
    {
        private readonly ILearningDeliveryFAMQueryService _learningDeliveryFAMQueryService;
        private readonly ILARSDataService _larsDataService;

        private readonly HashSet<string> _ldmCodes = new HashSet<string>()
        {
            LearningDeliveryFAMCodeConstants.LDM_376
        };

        public LearnDelFAMType_89Rule(
            ILARSDataService larsDataService,
            ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService,
            IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.LearnDelFAMType_89)
        {
            _larsDataService = larsDataService;
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
                var larsCatrgories = _larsDataService.GetCategoriesFor(learningDelivery.LearnAimRef);

                if (ConditionMet(learningDelivery, larsCatrgories))
                {
                    HandleValidationError(learner.LearnRefNumber, learningDelivery.AimSeqNumber, BuildErrorMessageParameters(larsCatrgories));
                }
            }
        }

        public bool ConditionMet(ILearningDelivery learningDelivery, IEnumerable<ILARSLearningCategory> larsLearningCategories) =>
            IsAdultSkills(learningDelivery)
            && IsCovid19SkillsOffer(larsLearningCategories)
            && !HasClassroomBased(learningDelivery);

        public bool IsCovid19SkillsOffer(IEnumerable<ILARSLearningCategory> larsLearningCategories)
        {
            return larsLearningCategories != null && larsLearningCategories.Any(l => l.CategoryRef == LARSConstants.Categories.Covid19SkillsOffer);
        }

        public bool IsAdultSkills(ILearningDelivery learningDelivery) =>
            learningDelivery.FundModel == FundModels.AdultSkills;

        public bool HasClassroomBased(ILearningDelivery learningDelivery) =>
            _learningDeliveryFAMQueryService.GetLearningDeliveryFAMsForTypeAndCodes(learningDelivery.LearningDeliveryFAMs, LearningDeliveryFAMTypeConstants.LDM, _ldmCodes).Any();

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(IEnumerable<ILARSLearningCategory> larsLearningCategories)
        {
            var errorList = new List<IErrorMessageParameter>
            {
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, LearningDeliveryFAMTypeConstants.LDM),
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMCode, LearningDeliveryFAMCodeConstants.LDM_376),
                BuildErrorMessageParameter(PropertyNameConstants.FundModel, FundModels.AdultSkills),
            };

            larsLearningCategories.ForEach(lc => errorList.Add(BuildErrorMessageParameter(PropertyNameConstants.LarsCategoryRef, lc.CategoryRef)));

            return errorList;
        }
    }
}
