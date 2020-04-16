using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.HE.PCTLDCS
{
    public class PCTLDCS_02Rule : AbstractRule, IRule<ILearner>
    {
        private readonly ILARSDataService _larsDataService;

        public PCTLDCS_02Rule(
            ILARSDataService larsDataService,
            IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.PCTLDCS_02)
        {
            _larsDataService = larsDataService;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate?.LearningDeliveries == null)
            {
                return;
            }

            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (learningDelivery.LearningDeliveryHEEntity == null)
                {
                    continue;
                }

                if (ConditionMet(
                    learningDelivery.LearnAimRef,
                    learningDelivery.LearningDeliveryHEEntity.PCTLDCSNullable))
                {
                    HandleValidationError(
                        objectToValidate.LearnRefNumber,
                        learningDelivery.AimSeqNumber,
                        BuildErrorMessageParameters(learningDelivery.LearningDeliveryHEEntity.PCTLDCSNullable));
                }
            }
        }

        public bool ConditionMet(string learnAimRef, decimal? pctldcs)
        {
            return pctldcs.HasValue
                   && !_larsDataService.HasKnownLearnDirectClassSystemCode3For(learnAimRef);
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(decimal? pctldcs)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.PCTLDCS, pctldcs)
            };
        }
    }
}