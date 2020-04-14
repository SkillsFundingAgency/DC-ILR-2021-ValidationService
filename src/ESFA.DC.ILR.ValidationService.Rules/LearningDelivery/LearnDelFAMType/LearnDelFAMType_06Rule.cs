using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Utility;
using System;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnDelFAMType
{
    public class LearnDelFAMType_06Rule :
        AbstractRule,
        IRule<ILearner>
    {
        private readonly IProvideLookupDetails _lookupDetails;

        private readonly IProvideRuleCommonOperations _check;

        public LearnDelFAMType_06Rule(
            IValidationErrorHandler validationErrorHandler,
            IProvideLookupDetails lookupDetails,
            IProvideRuleCommonOperations commonOperations)
            : base(validationErrorHandler, RuleNameConstants.LearnDelFAMType_06)
        {
            It.IsNull(validationErrorHandler)
                .AsGuard<ArgumentNullException>(nameof(validationErrorHandler));
            It.IsNull(lookupDetails)
                .AsGuard<ArgumentNullException>(nameof(lookupDetails));
            It.IsNull(commonOperations)
                .AsGuard<ArgumentNullException>(nameof(commonOperations));

            _lookupDetails = lookupDetails;
            _check = commonOperations;
        }

        public void Validate(ILearner thisLearner)
        {
            It.IsNull(thisLearner)
                .AsGuard<ArgumentNullException>(nameof(thisLearner));

            thisLearner.LearningDeliveries
                .ForAny(IsQualifyingDelivery, x => CheckDeliveryFAMs(x, y => RaiseValidationMessage(thisLearner.LearnRefNumber, x, y)));
        }

        public bool IsQualifyingDelivery(ILearningDelivery thisDelivery) =>
            !_check.IsRestart(thisDelivery);

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
