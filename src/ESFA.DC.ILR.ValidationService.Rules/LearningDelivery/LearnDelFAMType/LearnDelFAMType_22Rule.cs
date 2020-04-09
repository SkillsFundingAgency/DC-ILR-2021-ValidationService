using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Utility;
using System;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnDelFAMType
{
    public class LearnDelFAMType_22Rule :
        IRule<ILearner>
    {
        public const string MessagePropertyName = PropertyNameConstants.LearnDelFAMType;

        public const string Name = RuleNameConstants.LearnDelFAMType_22;

        private readonly IValidationErrorHandler _messageHandler;

        public LearnDelFAMType_22Rule(IValidationErrorHandler validationErrorHandler)
        {
            It.IsNull(validationErrorHandler)
                .AsGuard<ArgumentNullException>(nameof(validationErrorHandler));

            _messageHandler = validationErrorHandler;
        }

        public string RuleName => Name;

        public bool IsQualifyingFundModel(ILearningDelivery delivery) =>
            It.IsInRange(
                delivery.FundModel,
                TypeOfFunding.AdultSkills,
                TypeOfFunding.OtherAdult);

        public bool HasFullOrCoFundingIndicator(ILearningDeliveryFAM monitor) =>
            It.IsInRange(monitor.LearnDelFAMType, Monitoring.Delivery.Types.FullOrCoFunding);

        public bool HasFullOrCoFundingIndicator(ILearningDelivery delivery) =>
            delivery.LearningDeliveryFAMs.SafeAny(HasFullOrCoFundingIndicator);

        public bool IsNotValid(ILearningDelivery delivery) =>
            !IsQualifyingFundModel(delivery) && HasFullOrCoFundingIndicator(delivery);

        public void Validate(ILearner objectToValidate)
        {
            It.IsNull(objectToValidate)
                .AsGuard<ArgumentNullException>(nameof(objectToValidate));

            var learnRefNumber = objectToValidate.LearnRefNumber;

            objectToValidate.LearningDeliveries
                .SafeWhere(IsNotValid)
                .ForEach(x => RaiseValidationMessage(learnRefNumber, x));
        }

        public void RaiseValidationMessage(string learnRefNumber, ILearningDelivery thisDelivery)
        {
            var parameters = new List<IErrorMessageParameter>
            {
                _messageHandler.BuildErrorMessageParameter(PropertyNameConstants.FundModel, thisDelivery.FundModel),
                _messageHandler.BuildErrorMessageParameter(MessagePropertyName, Monitoring.Delivery.Types.FullOrCoFunding)
            };

            _messageHandler.Handle(RuleName, learnRefNumber, thisDelivery.AimSeqNumber, parameters);
        }
    }
}
