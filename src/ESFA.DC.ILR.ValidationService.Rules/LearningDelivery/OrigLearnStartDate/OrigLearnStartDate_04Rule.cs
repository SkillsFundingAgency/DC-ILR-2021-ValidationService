using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Utility;
using System;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.OrigLearnStartDate
{
    public class OrigLearnStartDate_04Rule :
        IRule<ILearner>
    {
        public const string MessagePropertyName = "OrigLearnStartDate";

        public const string Name = RuleNameConstants.OrigLearnStartDate_04;

        private readonly IValidationErrorHandler _messageHandler;

        private readonly HashSet<int> fundModels = new HashSet<int> { 35, 36, 81, 99 };

        public OrigLearnStartDate_04Rule(IValidationErrorHandler validationErrorHandler)
        {
            It.IsNull(validationErrorHandler)
                .AsGuard<ArgumentNullException>(nameof(validationErrorHandler));

            _messageHandler = validationErrorHandler;
        }

        public string RuleName => Name;

        public bool HasOriginalLearningStartDate(ILearningDelivery delivery) =>
            It.Has(delivery.OrigLearnStartDateNullable);

        public bool HasRestartIndicator(ILearningDeliveryFAM monitor) =>
            It.IsInRange(monitor.LearnDelFAMType, Monitoring.Delivery.Types.Restart);

        public bool HasRestartIndicator(ILearningDelivery delivery) =>
            delivery.LearningDeliveryFAMs.NullSafeAny(HasRestartIndicator);

        public bool IsNotValid(ILearningDelivery delivery) =>
            HasOriginalLearningStartDate(delivery) &&
            HasValidFundModel(delivery) &&
            !HasRestartIndicator(delivery);

        public bool HasValidFundModel(ILearningDelivery delivery) =>
            fundModels.Contains(delivery.FundModel);

        public void Validate(ILearner objectToValidate)
        {
            It.IsNull(objectToValidate)
                .AsGuard<ArgumentNullException>(nameof(objectToValidate));

            var learnRefNumber = objectToValidate.LearnRefNumber;

            objectToValidate.LearningDeliveries
                .NullSafeWhere(IsNotValid)
                .ForEach(x => RaiseValidationMessage(learnRefNumber, x));
        }

        public void RaiseValidationMessage(string learnRefNumber, ILearningDelivery thisDelivery)
        {
            var parameters = new List<IErrorMessageParameter>
            {
                _messageHandler.BuildErrorMessageParameter(MessagePropertyName, thisDelivery.OrigLearnStartDateNullable.Value)
            };

            _messageHandler.Handle(RuleName, learnRefNumber, thisDelivery.AimSeqNumber, parameters);
        }
    }
}
