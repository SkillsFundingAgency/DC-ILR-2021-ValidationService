using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnDelFAMType
{
    public class LearnDelFAMType_22Rule :
        IRule<ILearner>
    {
        public HashSet<int> _fundModels = new HashSet<int>
        {
            TypeOfFunding.AdultSkills,
            TypeOfFunding.OtherAdult
        };

        public const string Name = RuleNameConstants.LearnDelFAMType_22;

        private readonly IValidationErrorHandler _messageHandler;

        public LearnDelFAMType_22Rule(IValidationErrorHandler validationErrorHandler)
        {
            _messageHandler = validationErrorHandler;
        }

        public string RuleName => Name;

        public bool IsQualifyingFundModel(ILearningDelivery delivery) =>
            _fundModels.Contains(delivery.FundModel);

        public bool HasFullOrCoFundingIndicator(ILearningDeliveryFAM monitor) =>
            monitor.LearnDelFAMType.CaseInsensitiveEquals(Monitoring.Delivery.Types.FullOrCoFunding);

        public bool HasFullOrCoFundingIndicator(ILearningDelivery delivery) =>
            delivery.LearningDeliveryFAMs.NullSafeAny(HasFullOrCoFundingIndicator);

        public bool IsNotValid(ILearningDelivery delivery) =>
            !IsQualifyingFundModel(delivery) && HasFullOrCoFundingIndicator(delivery);

        public void Validate(ILearner objectToValidate)
        {
            var learnRefNumber = objectToValidate.LearnRefNumber;

            objectToValidate.LearningDeliveries
                .NullSafeWhere(IsNotValid)
                .ForEach(x => RaiseValidationMessage(learnRefNumber, x));
        }

        public void RaiseValidationMessage(string learnRefNumber, ILearningDelivery thisDelivery)
        {
            var parameters = new List<IErrorMessageParameter>
            {
                _messageHandler.BuildErrorMessageParameter(PropertyNameConstants.FundModel, thisDelivery.FundModel),
                _messageHandler.BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, Monitoring.Delivery.Types.FullOrCoFunding)
            };

            _messageHandler.Handle(RuleName, learnRefNumber, thisDelivery.AimSeqNumber, parameters);
        }
    }
}
