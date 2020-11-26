using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.CrossEntity
{
    public class R126Rule : AbstractRule, IRule<ILearner>
    {
        private readonly HashSet<int> _progTypes = new HashSet<int>() { 30, 31 };
        private readonly HashSet<int> _aimTypes = new HashSet<int>() { AimTypes.ComponentAimInAProgramme, AimTypes.CoreAim16To19ExcludingApprenticeships };

        public R126Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.R126)
        {
        }

        public void Validate(ILearner learner)
        {
            if (learner.LearningDeliveries == null)
            {
                return;
            }

            var learningDeliveries = FilterAims(learner.LearningDeliveries).ToList();

            foreach (var learningDelivery in learningDeliveries)
            {
                if (ConditionMet(learningDelivery, learner.LearningDeliveries))
                {
                    RaiseValidationMessage(learner.LearnRefNumber, learningDelivery);
                }
            }
        }

        public bool ConditionMet(ILearningDelivery comparisonLearningDelivery, IEnumerable<ILearningDelivery> learningDeliveries)
        {
            return !learningDeliveries.Any(x => _aimTypes.Contains(x.AimType) &&
                                               x.ProgTypeNullable == comparisonLearningDelivery.ProgTypeNullable &&
                                               x.FundModel == comparisonLearningDelivery.FundModel);
        }

        public IEnumerable<ILearningDelivery> FilterAims(IEnumerable<ILearningDelivery> learningDeliveries) =>
            learningDeliveries.Where(ld => ld.AimType == AimTypes.ProgrammeAim &&
                                           ld.ProgTypeNullable.HasValue && _progTypes.Contains(ld.ProgTypeNullable.Value));

        public void RaiseValidationMessage(string learnRefNumber, ILearningDelivery theDelivery) =>
            HandleValidationError(learnRefNumber, theDelivery.AimSeqNumber, BuildMessageParametersFor(theDelivery));

        public IEnumerable<IErrorMessageParameter> BuildMessageParametersFor(ILearningDelivery learningDelivery) => new[]
        {
            BuildErrorMessageParameter(PropertyNameConstants.AimType, learningDelivery.ProgTypeNullable),
            BuildErrorMessageParameter(PropertyNameConstants.AimType, learningDelivery.AimType),
            BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, learningDelivery.FundModel)
        };
    }
}
