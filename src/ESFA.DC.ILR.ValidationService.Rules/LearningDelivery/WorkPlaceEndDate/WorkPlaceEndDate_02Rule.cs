using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.WorkPlaceEndDate
{
    public class WorkPlaceEndDate_02Rule : AbstractRule, IRule<ILearner>
    {
        private readonly ILearningDeliveryWorkPlacementQueryService _learningDeliveryWorkPlacementQueryService;

        public WorkPlaceEndDate_02Rule(
            ILearningDeliveryWorkPlacementQueryService learningDeliveryWorkPlacementQueryService,
            IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.WorkPlaceEndDate_02)
        {
            _learningDeliveryWorkPlacementQueryService = learningDeliveryWorkPlacementQueryService;
        }

        public void Validate(ILearner objectToValidate)
        {
            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(
                    learningDelivery.LearnActEndDateNullable,
                    learningDelivery.LearningDeliveryWorkPlacements))
                {
                    HandleValidationError(
                        objectToValidate.LearnRefNumber,
                        learningDelivery.AimSeqNumber,
                        errorMessageParameters: BuildErrorMessageParameters(learningDelivery.LearnActEndDateNullable));
                }
            }
        }

        public bool ConditionMet(DateTime? learnActEndDate, IEnumerable<ILearningDeliveryWorkPlacement> workPlacements)
        {
            return LearnActEndDateConditionMet(learnActEndDate)
                   && WorkPlaceEndDateConditionMet(learnActEndDate, workPlacements);
        }

        public bool LearnActEndDateConditionMet(DateTime? learnActEndDate)
        {
            return learnActEndDate.HasValue;
        }

        public bool WorkPlaceEndDateConditionMet(DateTime? learnActEndDate, IEnumerable<ILearningDeliveryWorkPlacement> workPlacements)
        {
            return _learningDeliveryWorkPlacementQueryService.HasAnyWorkPlaceEndDatesGreaterThanLearnActEndDate(workPlacements, learnActEndDate);
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(DateTime? learnActEndDate)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.LearnActEndDate, learnActEndDate)
            };
        }
    }
}
