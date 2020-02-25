using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.WorkPlaceEmpId
{
    public class WorkPlaceEmpId_02Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IDerivedData_14Rule _derivedData14Rule;

        public WorkPlaceEmpId_02Rule(IDerivedData_14Rule derivedData14Rule, IValidationErrorHandler validationErrorHandler)
             : base(validationErrorHandler, RuleNameConstants.WorkPlaceEmpId_02)
        {
            _derivedData14Rule = derivedData14Rule;
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
            if (!workPlaceEmpId.HasValue)
            {
                return false; // null rule is handled as part of WorkPlaceEmpId_01 rule.
            }

            return workPlaceEmpId != ValidationConstants.TemporaryEmployerId && !HasValidChecksum(workPlaceEmpId.Value);
        }

        public bool HasValidChecksum(int workPlaceEmpId)
        {
            var checkSum = _derivedData14Rule.GetWorkPlaceEmpIdChecksum(workPlaceEmpId);

            if (checkSum.Equals(_derivedData14Rule.InvalidLengthChecksum))
            {
                return false;
            }

            if (checkSum != workPlaceEmpId.ToString().ElementAt(8))
            {
                return false;
            }

            return true;
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(int? workPlaceEmpId)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.WorkPlaceEmpId, workPlaceEmpId.GetValueOrDefault())
            };
        }
    }
}