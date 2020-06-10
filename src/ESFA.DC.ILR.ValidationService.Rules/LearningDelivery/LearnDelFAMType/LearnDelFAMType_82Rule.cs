using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Data.Internal.AcademicYear.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnDelFAMType
{
    public class LearnDelFAMType_82Rule : AbstractRule, IRule<ILearner>
    {
        private readonly int _larsCategoryRef = 41;

        private readonly IAcademicYearDataService _academicYearDataService;
        private readonly IDerivedData_35Rule _dd35;
        private readonly ILARSDataService _larsDataService;
        private readonly ILearningDeliveryFAMQueryService _learningDeliveryFAMQueryService;

        public LearnDelFAMType_82Rule(
            IValidationErrorHandler validationErrorHandler,
            IAcademicYearDataService academicYearDataService,
            IDerivedData_35Rule dd35,
            ILARSDataService larsDataService,
            ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService)
            : base(validationErrorHandler, RuleNameConstants.LearnDelFAMType_82)
        {
            _academicYearDataService = academicYearDataService;
            _dd35 = dd35;
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
                if (ConditionMet(learningDelivery))
                {
                    HandleErrors(learner.LearnRefNumber, learningDelivery);
                }
            }
        }

        public bool ConditionMet(ILearningDelivery learningDelivery)
        {
            return LearnStartDateConditionMet(learningDelivery.LearnStartDate)
                && DD35ConditionMet(learningDelivery)
                && LarsCategoryConditionMet(learningDelivery.LearnAimRef);
        }

        public bool DD35ConditionMet(ILearningDelivery learningDelivery)
        {
            return !_dd35.IsCombinedAuthorities(learningDelivery);
        }

        public bool LarsCategoryConditionMet(string learnAimRef)
        {
            var larsCategories = _larsDataService.GetCategoriesFor(learnAimRef);

            return larsCategories.Any(x => x.CategoryRef == _larsCategoryRef);
        }

        public bool LearnStartDateConditionMet(DateTime learnStartDate)
        {
            return learnStartDate >= _academicYearDataService.Start();
        }

        public void HandleErrors(string learnRefNumber, ILearningDelivery learningDelivery)
        {
            var sofDeliveryFams = _learningDeliveryFAMQueryService.GetLearningDeliveryFAMsForType(learningDelivery.LearningDeliveryFAMs, LearningDeliveryFAMTypeConstants.SOF);

            if (sofDeliveryFams.Any())
            {
                foreach (var learningDeliveryFAM in sofDeliveryFams)
                {
                    HandleValidationError(learnRefNumber, learningDelivery.AimSeqNumber, BuildErrorMessageParameters(learningDelivery.LearnStartDate, learningDeliveryFAM.LearnDelFAMCode));
                }

                return;
            }

            HandleValidationError(learnRefNumber, learningDelivery.AimSeqNumber, BuildErrorMessageParameters(learningDelivery.LearnStartDate, null));
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(DateTime learnStartDate, string learnDelFamCode)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, learnStartDate),
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, LearningDeliveryFAMTypeConstants.SOF),
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMCode, learnDelFamCode),
                BuildErrorMessageParameter(PropertyNameConstants.LarsCategoryRef, _larsCategoryRef)
            };
        }
    }
}
