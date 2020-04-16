using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.HE.PCSLDCS
{
    public class PCSLDCS_02Rule : AbstractRule, IRule<ILearner>
    {
        private readonly ILARSDataService _larsDataService;

        public PCSLDCS_02Rule(
            ILARSDataService larsDataService,
            IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.PCSLDCS_02)
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
                    learningDelivery.LearningDeliveryHEEntity.PCSLDCSNullable))
                {
                    HandleValidationError(
                        objectToValidate.LearnRefNumber,
                        learningDelivery.AimSeqNumber,
                        BuildErrorMessageParameters(learningDelivery.LearningDeliveryHEEntity.PCSLDCSNullable));
                }
            }
        }

        public bool ConditionMet(string learnAimRef, decimal? pcsldcs)
        {
            return pcsldcs.HasValue
                   && !_larsDataService.LearnDirectClassSystemCode2MatchForLearnAimRef(learnAimRef);
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(decimal? pcsldcs)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.PCSLDCS, pcsldcs)
            };
        }
    }
}