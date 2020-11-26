using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.ProgType
{
    public class ProgType_19Rule : AbstractRule, IRule<ILearner>
    {
        private readonly ILARSDataService _larsDataService;

        public ProgType_19Rule(
            IValidationErrorHandler validationErrorHandler,
            ILARSDataService larsDataService)
            : base(validationErrorHandler, RuleNameConstants.ProgType_19)
        {
            _larsDataService = larsDataService;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate.LearningDeliveries == null)
            {
                return;
            }

            var aim = objectToValidate.LearningDeliveries.FirstOrDefault(l => l.ProgTypeNullable == ProgTypes.TLevel);

            if (aim != null)
            {
                if (!objectToValidate.LearningDeliveries.Any(l =>
                    l.AimType == AimTypes.CoreAim16To19ExcludingApprenticeships &&
                    _larsDataService.GetDeliveryFor(l.LearnAimRef)?.LearnAimRefType == LARSConstants.LearnAimRefTypes.TLevelTechnicalQualification))
                {
                    HandleValidationError(objectToValidate.LearnRefNumber, aim.AimSeqNumber, BuildErrorMessageParameters(_larsDataService.GetDeliveryFor(aim.LearnAimRef).LearnAimRefType, aim.ProgTypeNullable));
                }
            }
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(string learnAimRefType, int? progType)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.LarsLearnAimRefType, learnAimRefType),
                BuildErrorMessageParameter(PropertyNameConstants.ProgType, progType)
            };
        }
    }
}
