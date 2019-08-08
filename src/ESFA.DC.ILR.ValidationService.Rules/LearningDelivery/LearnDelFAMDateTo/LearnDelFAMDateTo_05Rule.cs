using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Utility;
using System;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnDelFAMDateTo
{
    /// <summary>
    /// the learning delivery funding and monitoring "date to" rule 3
    /// </summary>
    /// <seealso cref="AbstractRule" />
    /// <seealso cref="Interface.IRule{ILearner}" />
    public class LearnDelFAMDateTo_05Rule :
        AbstractRule,
        IRule<ILearner>
    {
        /// <summary>
        /// The check(er)
        /// </summary>
        private readonly IProvideRuleCommonOperations _check;

        /// <summary>
        /// Initializes a new instance of the <see cref="LearnDelFAMDateTo_05Rule"/> class.
        /// </summary>
        /// <param name="validationErrorHandler">The validation error handler.</param>
        /// <param name="commonOps">The common op(eration)s (provider).</param>
        public LearnDelFAMDateTo_05Rule(
            IValidationErrorHandler validationErrorHandler,
            IProvideRuleCommonOperations commonOps)
            : base(validationErrorHandler, RuleNameConstants.LearnDelFAMDateTo_05)
        {
            It.IsNull(validationErrorHandler)
                .AsGuard<ArgumentNullException>(nameof(validationErrorHandler));
            It.IsNull(commonOps)
                .AsGuard<ArgumentNullException>(nameof(commonOps));

            _check = commonOps;
        }

        /// <summary>
        /// Validates the specified learner.
        /// </summary>
        /// <param name="theLearner">The learner.</param>
        public void Validate(ILearner theLearner)
        {
            It.IsNull(theLearner)
                .AsGuard<ArgumentNullException>(nameof(theLearner));

            theLearner.LearningDeliveries
                .ForEach(x => RunChecksFor(x, y => RaiseValidationMessage(theLearner.LearnRefNumber, x, y)));
        }

        /// <summary>
        /// Runs the checks for.
        /// </summary>
        /// <param name="theDelivery">The delivery.</param>
        /// <param name="doRaiseMessage">do raise message.</param>
        public void RunChecksFor(ILearningDelivery theDelivery, Action<ILearningDeliveryFAM> doRaiseMessage)
        {
            if (HasQualifyingFunding(theDelivery))
            {
                theDelivery.LearningDeliveryFAMs.ForAny(x => IsNotValid(theDelivery, x), doRaiseMessage);
            }
        }

        /// <summary>
        /// Determines whether [has qualifying funding] [the specified the delivery].
        /// </summary>
        /// <param name="theDelivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [has qualifying funding] [the specified the delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasQualifyingFunding(ILearningDelivery theDelivery) =>
            _check.HasQualifyingFunding(theDelivery,
                TypeOfFunding.ApprenticeshipsFrom1May2017);

        /// <summary>
        /// Determines whether [is not valid] [the specified the delivery].
        /// </summary>
        /// <param name="theDelivery">The delivery.</param>
        /// <param name="theMonitor">The monitor.</param>
        /// <returns>
        ///   <c>true</c> if [is not valid] [the specified the delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsNotValid(ILearningDelivery theDelivery, ILearningDeliveryFAM theMonitor) =>
            IsQualifyingMonitor(theMonitor)
            && HasDisqualifyingDates(theDelivery, theMonitor);

        /// <summary>
        /// Determines whether [is qualifying monitor] [the specified the monitor].
        /// </summary>
        /// <param name="theMonitor">The monitor.</param>
        /// <returns>
        ///   <c>true</c> if [is qualifying monitor] [the specified the monitor]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsQualifyingMonitor(ILearningDeliveryFAM theMonitor) =>
            It.IsInRange(theMonitor.LearnDelFAMType, Monitoring.Delivery.Types.ApprenticeshipContract);

        /// <summary>
        /// Determines whether [has disqualifying dates] [the specified the delivery and monitor].
        /// </summary>
        /// <param name="theDelivery">The delivery.</param>
        /// <param name="theMonitor">The monitor.</param>
        /// <returns>
        ///   <c>true</c> if [has disqualifying dates] [the specified the delivery and monitor]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasDisqualifyingDates(ILearningDelivery theDelivery, ILearningDeliveryFAM theMonitor) =>
            It.Has(theMonitor.LearnDelFAMDateToNullable) && theMonitor.LearnDelFAMDateToNullable > theDelivery.AchDateNullable;

        /// <summary>
        /// Raises the validation message.
        /// </summary>
        /// <param name="learnRefNumber">The learn reference number.</param>
        /// <param name="theDelivery">The delivery.</param>
        /// <param name="theInvalidMonitor">The invalid monitor.</param>
        public void RaiseValidationMessage(string learnRefNumber, ILearningDelivery theDelivery, ILearningDeliveryFAM theInvalidMonitor)
        {
            HandleValidationError(learnRefNumber, theDelivery.AimSeqNumber, BuildMessageParametersFor(theDelivery, theInvalidMonitor));
        }

        /// <summary>
        /// Builds the message parameters for.
        /// </summary>
        /// <param name="theDelivery">The delivery.</param>
        /// <param name="theInvalidMonitor">The invalid monitor.</param>
        /// <returns>a collection of message parameters</returns>
        public IEnumerable<IErrorMessageParameter> BuildMessageParametersFor(ILearningDelivery theDelivery, ILearningDeliveryFAM theInvalidMonitor)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.AchDate, theDelivery.AchDateNullable),
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMDateTo, theInvalidMonitor.LearnDelFAMDateToNullable)
            };
        }
    }
}
