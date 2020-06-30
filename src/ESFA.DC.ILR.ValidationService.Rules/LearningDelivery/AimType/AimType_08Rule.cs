using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.AimType
{
    public class AimType_08Rule : AbstractRule, IRule<ILearner>
    {
        private const string _larsLearnAimRefType = "1468";
        private const int _tLevelProgType = 31;
        private const int _coreAimType = 5;

        private readonly ILARSDataService _larsDataService;

        public AimType_08Rule(IValidationErrorHandler validationErrorHandler, ILARSDataService larsDataService)
            : base(validationErrorHandler, RuleNameConstants.AimType_08)
        {
            _larsDataService = larsDataService;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate.LearningDeliveries == null)
            {
                return;
            }

            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(learningDelivery.LearnAimRef, learningDelivery.ProgTypeNullable, learningDelivery.AimType))
                {
                    HandleValidationError(objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber, BuildErrorMessageParameters(learningDelivery.LearnAimRef, learningDelivery.ProgTypeNullable, learningDelivery.AimType));
                }
            }
        }

        public bool ConditionMet(string learnAimRef, int? progType, int aimType)
        {
            return _larsDataService.GetDeliveryFor(learnAimRef)?.LearnAimRefType == _larsLearnAimRefType
                   && progType.HasValue && progType == _tLevelProgType
                   && aimType != _coreAimType;
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(string learnAimRef, int? progType, int aimType)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.LearnAimRef, learnAimRef),
                BuildErrorMessageParameter(PropertyNameConstants.ProgType, progType),
                BuildErrorMessageParameter(PropertyNameConstants.AimType, aimType),
            };
        }
    }
}
