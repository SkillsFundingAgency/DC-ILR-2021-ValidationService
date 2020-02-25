using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.WorkPlaceMode
{
    public class WorkPlaceMode_01Rule : AbstractRule, IRule<ILearner>
    {
        private readonly ILearningDeliveryWorkPlacementQueryService _learningDeliveryWorkPlacementQueryService;

        public WorkPlaceMode_01Rule(
            ILearningDeliveryWorkPlacementQueryService learningDeliveryWorkPlacementQueryService,
            IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.WorkPlaceMode_01)
        {
            _learningDeliveryWorkPlacementQueryService = learningDeliveryWorkPlacementQueryService;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate.LearningDeliveries == null)
            {
                return;
            }

            foreach (var learningDelivery in objectToValidate.LearningDeliveries.Where(d => d.LearningDeliveryWorkPlacements != null))
            {
                foreach (var learningDeliveryWorkPlacement in learningDelivery.LearningDeliveryWorkPlacements)
                {
                    if (ConditionMet(learningDeliveryWorkPlacement))
                    {
                        HandleValidationError(objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber, BuildErrorMessageParameters(learningDeliveryWorkPlacement));
                    }
                }
            }
        }

        public bool ConditionMet(ILearningDeliveryWorkPlacement learningDeliveryWorkPlacement)
        {
            return !_learningDeliveryWorkPlacementQueryService.IsValidWorkPlaceMode(learningDeliveryWorkPlacement.WorkPlaceMode);
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(ILearningDeliveryWorkPlacement learningDeliveryWorkPlacement)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.WorkPlaceMode, learningDeliveryWorkPlacement.WorkPlaceMode)
            };
        }
    }
}
