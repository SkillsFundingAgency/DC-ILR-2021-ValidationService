using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.CrossEntity
{
    public class R125Rule : AbstractRule, IRule<ILearner>
    {
        private readonly HashSet<int> _aimTypes = new HashSet<int>()
        {
            AimTypes.ComponentAimInAProgramme,
            AimTypes.CoreAim16To19ExcludingApprenticeships
        };

        private readonly HashSet<int?> _progTypes = new HashSet<int?>()
        {
            ProgTypes.TLevelTransition,
            ProgTypes.TLevel
        };

        public R125Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.R125)
        {
        }

        public void Validate(ILearner learner)
        {
            if (learner?.LearningDeliveries == null)
            {
                return;
            }

            var tLevelDeliveries = learner.LearningDeliveries.Where(ld => _aimTypes.Contains(ld.AimType) && _progTypes.Contains(ld.ProgTypeNullable));

            foreach (var tLevelDelivery in tLevelDeliveries.Where(tLevelDelivery => ConditionMet(learner.LearningDeliveries, tLevelDelivery)))
            {
                HandleValidationError(
                    learner.LearnRefNumber,
                    tLevelDelivery.AimSeqNumber,
                    BuildErrorMessageParameters(tLevelDelivery.AimType, tLevelDelivery.ProgTypeNullable, tLevelDelivery.FundModel));
            }
        }

        public bool ConditionMet(IEnumerable<ILearningDelivery> learningDeliveries, ILearningDelivery tLevelDelivery)
        {
            return !learningDeliveries.Any(ld => ld.AimType == AimTypes.ProgrammeAim
                                                         && tLevelDelivery.ProgTypeNullable == ld.ProgTypeNullable
                                                         && tLevelDelivery.FundModel == ld.FundModel);
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(int aimType, int? progType, int fundModel)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.AimType, aimType),
                BuildErrorMessageParameter(PropertyNameConstants.ProgType, progType),
                BuildErrorMessageParameter(PropertyNameConstants.FundModel, fundModel)
            };
        }
    }
}
