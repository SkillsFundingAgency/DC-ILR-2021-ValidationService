using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.PlanLearnHours
{
    public class PlanLearnHours_01Rule : AbstractRule, IRule<ILearner>
    {
        private readonly HashSet<int> _excludedFundModels = new HashSet<int> { FundModels.EuropeanSocialFund, FundModels.Other16To19 };

        private readonly IDerivedData_07Rule _dd07;

        public PlanLearnHours_01Rule(IDerivedData_07Rule dd07, IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.PlanLearnHours_01)
        {
            _dd07 = dd07;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate.LearningDeliveries == null)
            {
                return;
            }

            if (PlanLearnHoursConditionMet(objectToValidate.PlanLearnHoursNullable) && HasOpenLearningDeliveries(objectToValidate.LearningDeliveries))
            {
                foreach (var learningDelivery in objectToValidate.LearningDeliveries)
                {
                    if (!Excluded(learningDelivery.FundModel, learningDelivery.ProgTypeNullable))
                    {
                        HandleValidationError(objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber, errorMessageParameters: BuildErrorMessageParameters(objectToValidate.PlanLearnHoursNullable, learningDelivery.FundModel));
                        return;
                    }
                }
            }
        }

        public bool HasOpenLearningDeliveries(IReadOnlyCollection<ILearningDelivery> learningDeliveries)
        {
            return !learningDeliveries.All(ld => ld.LearnActEndDateNullable.HasValue);
        }

        public bool PlanLearnHoursConditionMet(int? planLearnHours) => !planLearnHours.HasValue;

        public bool FundModelExclusionConditionMet(int fundModel) => _excludedFundModels.Contains(fundModel);

        public bool DD07ConditionMet(int? progType) => _dd07.IsApprenticeship(progType);

        public bool TLevelProgrammeExclusion(int fundModel, int? progType) => fundModel == FundModels.Age16To19ExcludingApprenticeships && progType == ProgTypes.TLevel;

        public bool Excluded(int fundModel, int? progType)
        {
            return DD07ConditionMet(progType)
                || FundModelExclusionConditionMet(fundModel)
                || TLevelProgrammeExclusion(fundModel, progType);
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(int? planLearnHours, int fundModel)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.PlanLearnHours, planLearnHours),
                BuildErrorMessageParameter(PropertyNameConstants.FundModel, fundModel)
            };
        }
    }
}