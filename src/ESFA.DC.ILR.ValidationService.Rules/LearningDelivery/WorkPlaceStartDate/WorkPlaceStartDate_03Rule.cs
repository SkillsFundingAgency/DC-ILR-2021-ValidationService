using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using System.Collections.Generic;
using System.Linq;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.WorkPlaceStartDate
{
    public class WorkPlaceStartDate_03Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IEnumerable<string> _learnAimRefs = new HashSet<string>
        {
            TypeOfAim.References.WorkPlacement0To49Hours,
            TypeOfAim.References.WorkPlacement50To99Hours,
            TypeOfAim.References.WorkPlacement100To199Hours,
            TypeOfAim.References.WorkPlacement200To499Hours,
            TypeOfAim.References.WorkPlacement500PlusHours,
            TypeOfAim.References.SupportedInternship16To19,
            TypeOfAim.References.WorkExperience,
            TypeOfAim.References.IndustryPlacement,
        }.ToCaseInsensitiveHashSet();

        public WorkPlaceStartDate_03Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.WorkPlaceStartDate_03)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate == null)
            {
                return;
            }

            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(learningDelivery.LearnAimRef, learningDelivery.LearningDeliveryWorkPlacements))
                {
                    HandleValidationError(objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber, BuildErrorMessageParameters(learningDelivery.LearnAimRef));
                }
            }
        }

        public bool ConditionMet(string learnAimRef, IReadOnlyCollection<ILearningDeliveryWorkPlacement> learningDeliveryWorkPlacements)
        {
            return LearnAimRefConditionMet(learnAimRef)
                && WorkPlacementsConditionMet(learningDeliveryWorkPlacements);
        }

        public bool WorkPlacementsConditionMet(IReadOnlyCollection<ILearningDeliveryWorkPlacement> learningDeliveryWorkPlacements) => learningDeliveryWorkPlacements?.Any() ?? false;

        public bool LearnAimRefConditionMet(string learnAimRef) => !_learnAimRefs.Contains(learnAimRef);

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(string learnAimRef)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.LearnAimRef, learnAimRef)
            };
        }
    }
}
