using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.CrossYear
{
    public class FRM_04Rule : AbstractRule, ICrossYearRule<ILearner>
    {
        private readonly HashSet<int> _fundModels = new HashSet<int>
        {
            FundModels.Age16To19ExcludingApprenticeships, FundModels.AdultSkills, FundModels.ApprenticeshipsFrom1May2017
        };

        private readonly ILearningDeliveryFAMQueryService _learningDeliveryFamQueryService;
        private readonly IDerivedData_39Rule _dd39;

        public FRM_04Rule(
            IValidationErrorHandler validationErrorHandler,
            ILearningDeliveryFAMQueryService learningDeliveryFamQueryService,
            IDerivedData_39Rule dd39)
            : base(validationErrorHandler, RuleNameConstants.FRM_04)
        {
            _learningDeliveryFamQueryService = learningDeliveryFamQueryService;
            _dd39 = dd39;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate.LearningDeliveries == null)
            {
                return;
            }

            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                var previousMatchingAim =
                    _dd39.GetMatchingLearningAimFromPreviousYear(objectToValidate, learningDelivery);

                if (previousMatchingAim != null)
                {
                    if (ConditionMet(learningDelivery, previousMatchingAim))
                    {
                        HandleValidationError(
                            objectToValidate.LearnRefNumber,
                            learningDelivery.AimSeqNumber,
                            BuildErrorMessageParameters(learningDelivery.FundModel, learningDelivery.LearnActEndDateNullable));
                    }
                }
            }
        }

        public bool ConditionMet(ILearningDelivery learningDelivery,  ILearnerReferenceData learnerReferenceData)
        {
            return
                FundedAimCondition(learningDelivery.FundModel, learningDelivery.ProgTypeNullable, learningDelivery.LearningDeliveryFAMs)
                && LearnActEndDateCondition(learningDelivery.LearnActEndDateNullable, learnerReferenceData.LearnActEndDate);
        }

        public bool LearnActEndDateCondition(DateTime? currentLearnActEndDate, DateTime? previousLearnActEndDate)
        {
            if (previousLearnActEndDate.HasValue)
            {
                return !currentLearnActEndDate.HasValue || currentLearnActEndDate != previousLearnActEndDate;
            }

            return false;
        }

        public bool FundedAimCondition(int fundModel, int? progType, IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs)
        {
            return _fundModels.Contains(fundModel)
                   || (fundModel == FundModels.OtherAdult && progType == ProgTypes.ApprenticeshipStandard)
                   || (fundModel == FundModels.NotFundedByESFA
                       && _learningDeliveryFamQueryService.HasLearningDeliveryFAMCodeForType(
                           learningDeliveryFAMs,
                           LearningDeliveryFAMTypeConstants.ADL,
                           LearningDeliveryFAMCodeConstants.ADL_Code));
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(int fundModel, DateTime? learnActEndDate)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.FundModel, fundModel),
                BuildErrorMessageParameter(PropertyNameConstants.LearnActEndDate, learnActEndDate)
            };
        }
    }
}
