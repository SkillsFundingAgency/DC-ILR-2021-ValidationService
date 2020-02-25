using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.HE.QUALENT3
{
    public class QUALENT3_03Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IProvideLookupDetails _provideLookupDetails;
        private readonly ILearningDeliveryFAMQueryService _learningDeliveryFAMQueryService;

        public QUALENT3_03Rule(
            IValidationErrorHandler validationErrorHandler,
            IProvideLookupDetails provideLookupDetails,
            ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService)
            : base(validationErrorHandler, RuleNameConstants.QUALENT3_03)
        {
            _provideLookupDetails = provideLookupDetails;
            _learningDeliveryFAMQueryService = learningDeliveryFAMQueryService;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate == null)
            {
                return;
            }

            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(learningDelivery.LearnStartDate, learningDelivery.LearningDeliveryHEEntity?.QUALENT3, learningDelivery.LearningDeliveryFAMs))
                {
                    HandleValidationError(objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber, BuildErrorMessageParameters(learningDelivery.LearnStartDate, learningDelivery.LearningDeliveryHEEntity?.QUALENT3));
                }
            }
        }

        public bool ConditionMet(DateTime learnStartDate, string qUALENT3, IReadOnlyCollection<ILearningDeliveryFAM> learningDeliveryFAMs)
        {
            return LearningDeliveryHEConditionMet(learnStartDate, qUALENT3)
                && LearningDeliveryFAMsConditionMet(learningDeliveryFAMs);
        }

        public bool LearningDeliveryFAMsConditionMet(IReadOnlyCollection<ILearningDeliveryFAM> learningDeliveryFAMs)
        {
            return !_learningDeliveryFAMQueryService.HasLearningDeliveryFAMType(learningDeliveryFAMs, LearningDeliveryFAMTypeConstants.RES);
        }

        public bool LearningDeliveryHEConditionMet(DateTime learnStartDate, string qUALENT3)
        {
            return !string.IsNullOrEmpty(qUALENT3)
                && !_provideLookupDetails.IsCurrent(TypeOfLimitedLifeLookup.QualEnt3, qUALENT3, learnStartDate);
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(DateTime learnStartDate, string qUALENT3)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, learnStartDate),
                BuildErrorMessageParameter(PropertyNameConstants.QUALENT3, qUALENT3)
            };
        }
    }
}
