using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.WorkPlaceStartDate
{
    public class WorkPlaceStartDate_03Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IEnumerable<string> _learnAimRefs = new HashSet<string>
        {
            AimTypes.References.WorkPlacement0To49Hours,
            AimTypes.References.WorkPlacement50To99Hours,
            AimTypes.References.WorkPlacement100To199Hours,
            AimTypes.References.WorkPlacement200To499Hours,
            AimTypes.References.WorkPlacement500PlusHours,
            AimTypes.References.SupportedInternship16To19,
            AimTypes.References.WorkExperience,
            AimTypes.References.IndustryPlacement,
            AimTypes.References.TLevelWorkExperience,
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
