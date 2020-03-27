using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.WorkPlaceEmpId
{
    public class WorkPlaceEmpId_05Rule : AbstractRule, IRule<ILearner>
    {
        private readonly ILearningDeliveryWorkPlacementQueryService _learningDeliveryWorkPlacementQueryService;

        public WorkPlaceEmpId_05Rule(
            ILearningDeliveryWorkPlacementQueryService learningDeliveryWorkPlacementQueryService,
            IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.WorkPlaceEmpId_05)
        {
            _learningDeliveryWorkPlacementQueryService = learningDeliveryWorkPlacementQueryService;
        }

        public void Validate(ILearner objectToValidate)
        {
            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(
                    learningDelivery.ProgTypeNullable,
                    learningDelivery.LearningDeliveryWorkPlacements))
                {
                    HandleValidationError(
                        objectToValidate.LearnRefNumber,
                        learningDelivery.AimSeqNumber,
                        errorMessageParameters: BuildErrorMessageParameters(learningDelivery.ProgTypeNullable));
                }
            }
        }

        public bool ConditionMet(int? progType, IEnumerable<ILearningDeliveryWorkPlacement> learningDeliveryWorkPlacements)
        {
            return ProgTypeConditionMet(progType)
                   && WorkPlacementConditionMet(learningDeliveryWorkPlacements);
        }

        public bool ProgTypeConditionMet(int? progType)
        {
            return progType == 24;
        }

        public bool WorkPlacementConditionMet(IEnumerable<ILearningDeliveryWorkPlacement> learningDeliveryWorkPlacements)
        {
            return _learningDeliveryWorkPlacementQueryService.HasAnyEmpIdNullAndStartDateNotNull(learningDeliveryWorkPlacements);
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(int? progType)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.ProgType, progType),
            };
        }
    }
}
