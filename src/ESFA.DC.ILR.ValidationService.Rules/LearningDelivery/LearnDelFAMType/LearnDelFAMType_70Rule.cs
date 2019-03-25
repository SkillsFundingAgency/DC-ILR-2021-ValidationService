using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnDelFAMType
{
    public class LearnDelFAMType_70Rule : AbstractRule, IRule<ILearner>
    {
        private readonly ILearningDeliveryFAMQueryService _learningDeliveryFAMQueryService;

        public LearnDelFAMType_70Rule(
            IValidationErrorHandler validationErrorHandler,
            ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService)
            : base(validationErrorHandler, RuleNameConstants.LearnDelFAMType_70)
        {
            _learningDeliveryFAMQueryService = learningDeliveryFAMQueryService;
        }

        public DateTime LastViableLearnStartDate => new DateTime(2018, 01, 01);

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate?.LearningDeliveries == null)
            {
                return;
            }

            foreach (var learningDelivery in objectToValidate.LearningDeliveries.Where(d => d.LearningDeliveryFAMs != null))
            {
                if (ConditionMet(learningDelivery.LearnStartDate, learningDelivery.LearningDeliveryFAMs))
                {
                    HandleValidationError(
                        objectToValidate.LearnRefNumber,
                        learningDelivery.AimSeqNumber,
                        BuildErrorMessageParameters(learningDelivery.LearnStartDate, LearningDeliveryFAMTypeConstants.LDM, LearningDeliveryFAMCodeConstants.LDM_Pilot));
                }
            }
        }

        public bool ConditionMet(DateTime learnStartDate, IReadOnlyCollection<ILearningDeliveryFAM> learningDeliveryFAMs)
        {
            return LearningDeliveryFAMCodeConditionMet(learningDeliveryFAMs) && LearnStartDateConditionMet(learnStartDate);
        }

        public bool LearningDeliveryFAMCodeConditionMet(IReadOnlyCollection<ILearningDeliveryFAM> learningDeliveryFAMs)
        {
            return _learningDeliveryFAMQueryService.HasLearningDeliveryFAMCodeForType(
                learningDeliveryFAMs,
                LearningDeliveryFAMTypeConstants.LDM,
                LearningDeliveryFAMCodeConstants.LDM_Pilot);
        }

        public bool LearnStartDateConditionMet(DateTime learnStartDate)
        {
            return learnStartDate < LastViableLearnStartDate;
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(DateTime learnStartDate, string learnDelFAMType, string learnDelFAMCode)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, learnStartDate),
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, learnDelFAMType),
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMCode, learnDelFAMCode)
            };
        }
    }
}
