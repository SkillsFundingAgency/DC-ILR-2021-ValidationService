using System;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnDelFAMType
{
    public class LearnDelFAMType_06Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IProvideLookupDetails _lookupDetails;
        private readonly ILearningDeliveryFAMQueryService _learningDeliveryFAMQueryService;

        public LearnDelFAMType_06Rule(
            IValidationErrorHandler validationErrorHandler,
            IProvideLookupDetails lookupDetails,
            ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService)
            : base(validationErrorHandler, RuleNameConstants.LearnDelFAMType_06)
        {
            _lookupDetails = lookupDetails;
            _learningDeliveryFAMQueryService = learningDeliveryFAMQueryService;
        }

        public void Validate(ILearner thisLearner)
        {
            thisLearner.LearningDeliveries
                .ForAny(IsQualifyingDelivery, x => CheckDeliveryFAMs(x, y => RaiseValidationMessage(thisLearner.LearnRefNumber, x, y)));
        }

        public bool IsQualifyingDelivery(ILearningDelivery thisDelivery) =>
            !_learningDeliveryFAMQueryService.HasLearningDeliveryFAMType(thisDelivery.LearningDeliveryFAMs, LearningDeliveryFAMTypeConstants.RES);

        public void CheckDeliveryFAMs(ILearningDelivery learningDelivery, Action<ILearningDeliveryFAM> raiseMessage)
        {
            learningDelivery.LearningDeliveryFAMs
                .ForAny(x => IsNotCurrent(x, learningDelivery.LearnStartDate), raiseMessage);
        }

        public bool IsNotCurrent(ILearningDeliveryFAM monitor, DateTime referenceDate)
        {
            return !_lookupDetails.IsVaguelyCurrent(
                TypeOfLimitedLifeLookup.LearnDelFAMType,
                $"{monitor.LearnDelFAMType}{monitor.LearnDelFAMCode}",
                referenceDate);
        }

        private void RaiseValidationMessage(string learnRefNumber, ILearningDelivery thisDelivery, ILearningDeliveryFAM andMonitor)
        {
            var parameters = new IErrorMessageParameter[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, thisDelivery.LearnStartDate),
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, andMonitor.LearnDelFAMType),
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMCode, andMonitor.LearnDelFAMCode)
            };

            HandleValidationError(learnRefNumber, thisDelivery.AimSeqNumber, parameters);
        }
    }
}
