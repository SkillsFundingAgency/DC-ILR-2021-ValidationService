using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.WithdrawReason
{
    public class WithdrawReason_03Rule :
        AbstractRule,
        IRule<ILearner>
    {
        public WithdrawReason_03Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.WithdrawReason_03)
        {
        }

        public bool HasWithdrawn(ILearningDelivery delivery) =>
            delivery.CompStatus == CompletionState.HasWithdrawn;

        public bool HasWithdrawReason(ILearningDelivery delivery) =>
            delivery.WithdrawReasonNullable.HasValue;

        public void Validate(ILearner objectToValidate)
        {
            var learnRefNumber = objectToValidate.LearnRefNumber;

            objectToValidate.LearningDeliveries
                .NullSafeWhere(HasWithdrawn)
                .ForEach(x =>
                {
                    var failedValidation = !HasWithdrawReason(x);

                    if (failedValidation)
                    {
                        RaiseValidationMessage(learnRefNumber, x);
                    }
                });
        }

        public void RaiseValidationMessage(string learnRefNumber, ILearningDelivery thisDelivery)
        {
            HandleValidationError(learnRefNumber, thisDelivery.AimSeqNumber, BuildMessageParametersFor(thisDelivery));
        }

        public IEnumerable<IErrorMessageParameter> BuildMessageParametersFor(ILearningDelivery thisDelivery)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.CompStatus, thisDelivery.CompStatus.ToString())
            };
        }
    }
}
