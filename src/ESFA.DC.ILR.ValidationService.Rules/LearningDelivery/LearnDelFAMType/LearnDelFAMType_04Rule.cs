using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnDelFAMType
{
    public class LearnDelFAMType_04Rule :
        IRule<ILearner>
    {
        public const string Name = RuleNameConstants.LearnDelFAMType_04;

        private readonly IValidationErrorHandler _messageHandler;

        private readonly IProvideLookupDetails _lookupDetails;

        public LearnDelFAMType_04Rule(
            IValidationErrorHandler validationErrorHandler,
            IProvideLookupDetails lookupDetails)
        {
            It.IsNull(validationErrorHandler)
                .AsGuard<ArgumentNullException>(nameof(validationErrorHandler));
            It.IsNull(lookupDetails)
                .AsGuard<ArgumentNullException>(nameof(lookupDetails));

            _messageHandler = validationErrorHandler;
            _lookupDetails = lookupDetails;
        }

        public string RuleName => Name;

        public bool IsNotValid(ILearningDeliveryFAM monitor) =>
            !_lookupDetails.Contains(TypeOfLimitedLifeLookup.LearnDelFAMType, $"{monitor.LearnDelFAMType}{monitor.LearnDelFAMCode}");

        public void Validate(ILearner objectToValidate)
        {
            It.IsNull(objectToValidate)
                .AsGuard<ArgumentNullException>(nameof(objectToValidate));

            var learnRefNumber = objectToValidate.LearnRefNumber;

            objectToValidate.LearningDeliveries
                .ToReadOnlyCollection()
                .SelectMany(x => x.LearningDeliveryFAMs.ToReadOnlyCollection())
                .Where(IsNotValid)
                .ForEach(x => RaiseValidationMessage(learnRefNumber, x));
        }

        public void RaiseValidationMessage(string learnRefNumber, ILearningDeliveryFAM thisMonitor)
        {
            var parameters = new List<IErrorMessageParameter>
            {
                _messageHandler.BuildErrorMessageParameter(nameof(thisMonitor.LearnDelFAMType), thisMonitor.LearnDelFAMType),
                _messageHandler.BuildErrorMessageParameter(nameof(thisMonitor.LearnDelFAMCode), thisMonitor.LearnDelFAMCode)
            };

            _messageHandler.Handle(RuleName, learnRefNumber, null, parameters);
        }
    }
}
