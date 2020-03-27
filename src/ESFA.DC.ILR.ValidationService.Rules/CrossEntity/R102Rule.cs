using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.CrossEntity
{
    public class R102Rule : AbstractRule, IRule<ILearner>
    {
        private readonly string _famTypeACT = LearningDeliveryFAMTypeConstants.ACT;
        private readonly ILearningDeliveryFAMQueryService _learningDeliveryFAMQueryService;

        public R102Rule(ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService, IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.R102)
        {
            _learningDeliveryFAMQueryService = learningDeliveryFAMQueryService;
        }

        public void Validate(ILearner objectToValidate)
        {
            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(learningDelivery.LearnStartDate, learningDelivery.LearningDeliveryFAMs))
                {
                    HandleValidationError(objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber, BuildErrorMessageParameters(learningDelivery.LearnStartDate, _famTypeACT));
                }
            }
        }

        public bool ConditionMet(DateTime learnStartDate, IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs)
        {
            if (learningDeliveryFAMs != null)
            {
                return _learningDeliveryFAMQueryService.HasLearningDeliveryFAMType(learningDeliveryFAMs, _famTypeACT)
                && !_learningDeliveryFAMQueryService.HasLearningDeliveryFAMTypeForDate(learningDeliveryFAMs, _famTypeACT, learnStartDate);
            }

            return false;
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(DateTime learnStartDate, string famType)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, learnStartDate),
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, famType)
            };
        }
    }
}
