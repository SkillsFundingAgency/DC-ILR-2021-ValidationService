using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

using System;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.WithdrawReason
{
    public class WithdrawReason_06Rule :
        AbstractRule,
        IRule<ILearner>
    {
        public WithdrawReason_06Rule(
            IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.WithdrawReason_06)
        {
        }

        public bool HasWithdrawn(ILearningDelivery delivery) =>
            delivery.CompStatus == CompletionState.HasWithdrawn;

        public bool HasWithdrewAsIndustrialPlacementLearner(ILearningDelivery delivery) =>
           delivery.WithdrawReasonNullable == ReasonForWithdrawal.IndustrialPlacementLearnerWithdrew;

        public bool HasQualifyingAim(ILearningDelivery delivery) =>
            delivery.LearnAimRef.CaseInsensitiveEquals(TypeOfAim.References.IndustryPlacement);

        public bool IsNotValid(ILearningDelivery delivery) =>
            HasWithdrawn(delivery)
            && HasWithdrewAsIndustrialPlacementLearner(delivery)
            && !HasQualifyingAim(delivery);

        public void Validate(ILearner theLearner)
        {
            var learnRefNumber = theLearner.LearnRefNumber;

            theLearner.LearningDeliveries
                .ForAny(IsNotValid, x => RaiseValidationMessage(learnRefNumber, x));
        }

        public void RaiseValidationMessage(string learnRefNumber, ILearningDelivery thisDelivery)
        {
            HandleValidationError(learnRefNumber, thisDelivery.AimSeqNumber, BuildMessageParametersFor(thisDelivery));
        }

        public IEnumerable<IErrorMessageParameter> BuildMessageParametersFor(ILearningDelivery thisDelivery)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.LearnAimRef, thisDelivery.LearnAimRef),
                BuildErrorMessageParameter(PropertyNameConstants.WithdrawReason, thisDelivery.WithdrawReasonNullable)
            };
        }
    }
}
