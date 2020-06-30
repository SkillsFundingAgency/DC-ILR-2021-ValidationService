using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.ProgType
{
    public class ProgType_16Rule : AbstractRule, IRule<ILearner>
    {
        private readonly ILARSDataService _larsDataService;
        private readonly string[] _validLearnAimRefTypes = { LARSConstants.LearnAimRefTypes.TLevelTechnicalQualification };

        public ProgType_16Rule(IValidationErrorHandler validationErrorHandler, ILARSDataService larsDataService)
            : base(validationErrorHandler, RuleNameConstants.ProgType_16)
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
                if (ConditionMet(learningDelivery.LearnAimRef, learningDelivery.ProgTypeNullable))
                {
                    HandleValidationError(objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber, BuildErrorMessageParameters(learningDelivery.LearnAimRef, learningDelivery.ProgTypeNullable));
                }
            }
        }

        public bool ConditionMet(string learnAimRef, int? progType)
        {
            return ProgTypeConditionMet(progType)
                   && LARSConditionMet(learnAimRef);
        }

        public bool ProgTypeConditionMet(int? progType)
        {
            return progType == null || progType != ProgTypes.TLevel;
        }

        public bool LARSConditionMet(string learnAimRef)
        {
            return _larsDataService.HasAnyLearningDeliveryForLearnAimRefAndTypes(learnAimRef, _validLearnAimRefTypes);
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(string learnAimRef, int? progType)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.LearnAimRef, learnAimRef),
                BuildErrorMessageParameter(PropertyNameConstants.ProgType, progType)
            };
        }
    }
}