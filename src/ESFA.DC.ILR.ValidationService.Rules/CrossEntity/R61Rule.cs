using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.CrossEntity
{
    public class R61Rule : AbstractRule, IRule<ILearner>
    {
        private readonly string _famTypeLSF = Monitoring.Delivery.Types.LearningSupportFunding;

        private readonly ILearningDeliveryFAMQueryService _learningDeliveryFAMQueryService;

        public R61Rule(ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService, IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.R61)
        {
            _learningDeliveryFAMQueryService = learningDeliveryFAMQueryService;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate.LearningDeliveries == null)
            {
                return;
            }

            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                var overlappingLearningDeliveryFAMs =
                    _learningDeliveryFAMQueryService
                    .GetOverLappingLearningDeliveryFAMsForType(learningDelivery.LearningDeliveryFAMs, _famTypeLSF);

                foreach (var learningDeliveryFAM in overlappingLearningDeliveryFAMs)
                {
                    HandleValidationError(
                    objectToValidate.LearnRefNumber,
                    learningDelivery.AimSeqNumber,
                    errorMessageParameters: BuildErrorMessageParameters(
                        _famTypeLSF,
                        learningDeliveryFAM.LearnDelFAMDateFromNullable,
                        learningDeliveryFAM.LearnDelFAMDateToNullable));
                }
            }
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(string famType, DateTime? learnDelFamDateFrom, DateTime? learnDelFamDateTo)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, famType),
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMDateFrom, learnDelFamDateFrom),
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMDateTo, learnDelFamDateTo)
            };
        }
    }
}
