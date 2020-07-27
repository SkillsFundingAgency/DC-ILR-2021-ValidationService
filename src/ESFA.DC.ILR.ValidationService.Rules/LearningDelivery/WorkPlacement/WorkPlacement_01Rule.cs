using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.WorkPlacement
{
    public class WorkPlacement_01Rule : AbstractRule, IRule<ILearner>
    {
        public WorkPlacement_01Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.WorkPlacement_01)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate.LearningDeliveries == null)
            {
                return;
            }

            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(learningDelivery.LearnAimRef, learningDelivery.LearningDeliveryWorkPlacements))
                {
                    HandleValidationError(
                        objectToValidate.LearnRefNumber,
                        learningDelivery.AimSeqNumber,
                        BuildErrorMessageParameters());
                    return;
                }
            }
        }

        public bool ConditionMet(string learnAimRef, IReadOnlyCollection<ILearningDeliveryWorkPlacement> workPlacements)
        {
            return learnAimRef.CaseInsensitiveEquals(AimTypes.References.TLevelWorkExperience)
                && (workPlacements == null || !workPlacements.Any());
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters()
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.LearnAimRef, AimTypes.References.TLevelWorkExperience)
            };
        }
    }
}
