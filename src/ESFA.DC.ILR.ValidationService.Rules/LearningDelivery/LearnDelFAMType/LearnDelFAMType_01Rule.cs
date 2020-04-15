using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using System;
using System.Linq;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnDelFAMType
{
    public class LearnDelFAMType_01Rule :
        IRule<ILearner>
    {
        public const string MessagePropertyName = "LearnDelFAMType";

        public const string Name = RuleNameConstants.LearnDelFAMType_01;

        private readonly IValidationErrorHandler _messageHandler;

        public LearnDelFAMType_01Rule(IValidationErrorHandler validationErrorHandler)
        {
            _messageHandler = validationErrorHandler;
        }

        public string RuleName => Name;

        public bool IsFunded(ILearningDelivery delivery) =>
            TypeOfFunding.AsAFundedSet.Contains(delivery.FundModel);

        public void Validate(ILearner objectToValidate)
        {
            var learnRefNumber = objectToValidate.LearnRefNumber;

            objectToValidate.LearningDeliveries
                .NullSafeWhere(IsFunded)
                .ForEach(x =>
                {
                    var failedValidation = !x.LearningDeliveryFAMs.NullSafeAny(y => ConditionMet(y));

                    if (failedValidation)
                    {
                        RaiseValidationMessage(learnRefNumber, x);
                    }
                });
        }

        public bool ConditionMet(ILearningDeliveryFAM famRecord)
        {
            return famRecord != null
                ? famRecord.LearnDelFAMType.CaseInsensitiveEquals(Monitoring.Delivery.Types.SourceOfFunding)
                : true;
        }

        public void RaiseValidationMessage(string learnRefNumber, ILearningDelivery thisDelivery)
        {
            _messageHandler.Handle(RuleName, learnRefNumber, thisDelivery.AimSeqNumber);
        }
    }
}
