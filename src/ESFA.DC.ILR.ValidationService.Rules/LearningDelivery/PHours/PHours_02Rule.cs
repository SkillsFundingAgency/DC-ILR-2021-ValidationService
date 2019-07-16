using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Internal.AcademicYear.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.PHours
{
    public class PHours_02Rule : AbstractRule, IRule<ILearner>
    {
        private const int _minPlannedHours = 278;
        private readonly ILearningDeliveryFAMQueryService _learningDeliveryFAMQueryService;

        public PHours_02Rule(ILearningDeliveryFAMQueryService learningDeliveryFamQueryService, IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.PHours_02)
        {
            _learningDeliveryFAMQueryService = learningDeliveryFamQueryService;
        }

        public PHours_02Rule()
            : base(null, null)
        {

        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate.LearningDeliveries != null)
            {
                foreach (var learningDelivery in objectToValidate.LearningDeliveries)
                {
                    if (ConditionMet(learningDelivery.PHoursNullable, learningDelivery.LearningDeliveryFAMs))
                    {
                        HandleValidationError(
                                 objectToValidate.LearnRefNumber,
                                 learningDelivery.AimSeqNumber,
                                 BuildErrorMessageParameters(learningDelivery.PHoursNullable));
                        return;
                    }
                }
            }
        }

        public bool ConditionMet(int? plannedHours, IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs)
        {
            return PlannedHoursConditionMet(plannedHours)
                && LearningDeliveryFAMsConditionMet(learningDeliveryFAMs);
        }

        public virtual bool PlannedHoursConditionMet(int? plannedHours)
        {
            return plannedHours.HasValue && plannedHours < _minPlannedHours;
        }

        public virtual bool LearningDeliveryFAMsConditionMet(IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs)
        {
            return !_learningDeliveryFAMQueryService.HasLearningDeliveryFAMType(learningDeliveryFAMs, LearningDeliveryFAMTypeConstants.RES);
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(int? plannedHours)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.PHours, plannedHours)
            };
        }
    }
}
