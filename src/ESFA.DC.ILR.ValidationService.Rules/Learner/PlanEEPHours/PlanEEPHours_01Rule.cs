using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.PlanEEPHours
{
    public class PlanEEPHours_01Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IDerivedData_07Rule _dd07;

        public PlanEEPHours_01Rule(
            IDerivedData_07Rule dd07,
            IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.PlanEEPHours_01)
        {
            _dd07 = dd07;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (!AllLearningAimsClosedExcludeConditionMet(objectToValidate.LearningDeliveries))
            {
                foreach (var learningDelivery in objectToValidate.LearningDeliveries)
                {
                    if (ConditionMet(
                        learningDelivery.FundModel,
                        objectToValidate.PlanEEPHoursNullable,
                        learningDelivery.ProgTypeNullable))
                    {
                        HandleValidationError(
                            objectToValidate.LearnRefNumber,
                            learningDelivery.AimSeqNumber,
                            BuildErrorMessageParameters(learningDelivery.FundModel));
                    }
                }
            }
        }

        public bool ConditionMet(int fundModel, int? planEEPHours, int? progType)
        {
            return FundModelConditionMet(fundModel)
                   && PlanEEPHoursConditionMet(planEEPHours)
                   && !Excluded(progType, fundModel);
        }

        public bool FundModelConditionMet(int fundModel)
        {
            return fundModel == FundModels.Age16To19ExcludingApprenticeships;
        }

        public bool PlanEEPHoursConditionMet(int? planEEPHours)
        {
            return planEEPHours == null;
        }

        public bool Excluded(int? progType, int fundModel)
        {
            return DD07ExcludeConditionMet(progType)
                   || FundModelExcludeConditionMet(fundModel);
        }

        public bool DD07ExcludeConditionMet(int? progType)
        {
            return _dd07.IsApprenticeship(progType);
        }

        public bool FundModelExcludeConditionMet(int fundModel)
        {
            return fundModel == 70;
        }

        public bool AllLearningAimsClosedExcludeConditionMet(IEnumerable<ILearningDelivery> learningDeliveries)
        {
            return learningDeliveries != null
                   && learningDeliveries.All(ld => ld.LearnActEndDateNullable.HasValue);
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(int fundModel)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.FundModel, fundModel)
            };
        }
    }
}
