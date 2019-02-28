using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.HE.PCFLDCS
{
    public class PCFLDCS_03Rule : AbstractRule, IRule<ILearner>
    {
        private readonly ILARSDataService _larsDataService;

        public PCFLDCS_03Rule(
            ILARSDataService larsDataService,
            IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.PCFLDCS_03)
        {
            _larsDataService = larsDataService;
        }

        public void Validate(ILearner objectToValidate)
        {
            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (learningDelivery.LearningDeliveryHEEntity != null
                        && ConditionMet(
                            learningDelivery.LearnAimRef,
                            learningDelivery.LearningDeliveryHEEntity))
                {
                    HandleValidationError(
                        objectToValidate.LearnRefNumber,
                        learningDelivery.AimSeqNumber,
                        BuildErrorMessageParameters(
                            learningDelivery.LearningDeliveryHEEntity.PCFLDCSNullable));
                }
            }
        }

        public bool ConditionMet(string learnAimRef, ILearningDeliveryHE learningDeliveryHe)
        {
            return LDCSCodeConditionMet(learnAimRef)
                && LearningDeliveryHEConditionMet(learningDeliveryHe);
        }

        public bool LDCSCodeConditionMet(string learnAimRef)
        {
            return !_larsDataService.LearnDirectClassSystemCode1MatchForLearnAimRef(learnAimRef);
        }

        public bool LearningDeliveryHEConditionMet(ILearningDeliveryHE learningDeliveryHe)
        {
            return learningDeliveryHe?.PCFLDCSNullable != null;
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(decimal? pcfldcs)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.PCFLDCS, pcfldcs)
            };
        }
    }
}
