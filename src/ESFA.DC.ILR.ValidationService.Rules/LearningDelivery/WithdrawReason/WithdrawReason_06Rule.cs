using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Utility;
using System;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.WithdrawReason
{
    /// <summary>
    /// withdraw reason 06 (introduced 1920)
    /// </summary>
    /// <seealso cref="Interface.IRule{ILearner}" />
    public class WithdrawReason_06Rule :
        AbstractRule,
        IRule<ILearner>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WithdrawReason_03Rule"/> class.
        /// </summary>
        /// <param name="validationErrorHandler">The validation error handler.</param>
        public WithdrawReason_06Rule(
            IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.WithdrawReason_06)
        {
        }

        /// <summary>
        /// Determines whether the specified delivery has withdrawn.
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if the specified delivery has withdrawn; otherwise, <c>false</c>.
        /// </returns>
        public bool HasWithdrawn(ILearningDelivery delivery) =>
            It.IsInRange(delivery.CompStatus, CompletionState.HasWithdrawn);

        /// <summary>
        /// Determines whether [has withdrew as industrial placement learner] [the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [has withdrew as industrial placement learner] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasWithdrewAsIndustrialPlacementLearner(ILearningDelivery delivery) =>
            It.IsInRange(delivery.WithdrawReasonNullable, ReasonForWithdrawal.IndustrialPlacementLearnerWithdrew);

        /// <summary>
        /// Determines whether [has qualifying aim] [the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [has qualifying aim] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasQualifyingAim(ILearningDelivery delivery) =>
            It.IsInRange(delivery.LearnAimRef, TypeOfAim.References.IndustryPlacement);

        /// <summary>
        /// Determines whether [is not valid] [the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [is not valid] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsNotValid(ILearningDelivery delivery) =>
            HasWithdrawn(delivery)
            && HasWithdrewAsIndustrialPlacementLearner(delivery)
            && !HasQualifyingAim(delivery);

        /// <summary>
        /// Validates the specified object.
        /// </summary>
        /// <param name="theLearner">The object to validate.</param>
        public void Validate(ILearner theLearner)
        {
            It.IsNull(theLearner)
                .AsGuard<ArgumentNullException>(nameof(theLearner));

            var learnRefNumber = theLearner.LearnRefNumber;

            theLearner.LearningDeliveries
                .ForAny(IsNotValid, x => RaiseValidationMessage(learnRefNumber, x));
        }

        /// <summary>
        /// Raises the validation message.
        /// </summary>
        /// <param name="learnRefNumber">The learn reference number.</param>
        /// <param name="thisDelivery">this delivery.</param>
        public void RaiseValidationMessage(string learnRefNumber, ILearningDelivery thisDelivery)
        {
            HandleValidationError(learnRefNumber, thisDelivery.AimSeqNumber, BuildMessageParametersFor(thisDelivery));
        }

        /// <summary>
        /// Builds the message parameters for..
        /// </summary>
        /// <param name="thisDelivery">The this delivery.</param>
        /// <returns>
        /// returns a list of message parameters
        /// </returns>
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
