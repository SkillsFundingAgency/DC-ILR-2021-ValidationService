using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.EDRS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.WorkPlaceEmpId
{
    public class WorkPlaceEmpId_01Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IEmployersDataService _edrsDataService;

        public WorkPlaceEmpId_01Rule(IEmployersDataService edrsDataService, IValidationErrorHandler validationErrorHandler)
             : base(validationErrorHandler, RuleNameConstants.WorkPlaceEmpId_01)
        {
            _edrsDataService = edrsDataService;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate.LearningDeliveries == null)
            {
                return;
            }

            foreach (var learningDelivery in objectToValidate.LearningDeliveries.Where(x => x.LearningDeliveryWorkPlacements != null))
            {
                foreach (var learningDeliveryWorkPlacement in learningDelivery.LearningDeliveryWorkPlacements)
                {
                    if (ConditionMet(learningDeliveryWorkPlacement.WorkPlaceEmpIdNullable))
                    {
                        HandleValidationError(objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber, BuildErrorMessageParameters(learningDeliveryWorkPlacement.WorkPlaceEmpIdNullable));
                    }
                }
            }
        }

        public bool ConditionMet(int? workPlaceEmpId)
        {
            return workPlaceEmpId.HasValue
                   && workPlaceEmpId != ValidationConstants.TemporaryEmployerId
                   && !_edrsDataService.IsValid(workPlaceEmpId);
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(int? workPlaceEmpId)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.WorkPlaceEmpId, workPlaceEmpId)
            };
        }
    }
}