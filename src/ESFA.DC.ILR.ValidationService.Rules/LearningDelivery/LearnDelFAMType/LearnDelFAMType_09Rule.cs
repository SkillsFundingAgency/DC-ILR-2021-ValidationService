using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Utility;
using System;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnDelFAMType
{
    /// <summary>
    /// learning delivery funding and monitoring rule 09
    /// </summary>
    /// <seealso cref="AbstractRule" />
    /// <seealso cref="Interface.IRule{ILearner}" />
    public class LearnDelFAMType_09Rule :
        AbstractRule,
        IRule<ILearner>
    {
        /// <summary>
        /// The check (rule common operations provider)
        /// </summary>
        private readonly IProvideRuleCommonOperations _check;

        /// <summary>
        /// Initializes a new instance of the <see cref="LearnDelFAMType_09Rule" /> class.
        /// </summary>
        /// <param name="validationErrorHandler">The validation error handler.</param>
        /// <param name="commonOperations">The common operations.</param>
        public LearnDelFAMType_09Rule(
            IValidationErrorHandler validationErrorHandler,
            IProvideRuleCommonOperations commonOperations)
            : base(validationErrorHandler, RuleNameConstants.LearnDelFAMType_09)
        {
            It.IsNull(validationErrorHandler)
                .AsGuard<ArgumentNullException>(nameof(validationErrorHandler));
            It.IsNull(commonOperations)
                .AsGuard<ArgumentNullException>(nameof(commonOperations));

            _check = commonOperations;
        }

        /// <summary>
        /// Gets the first inviable date.
        /// </summary>
        public static DateTime FirstInviableDate => new DateTime(2019, 08, 01);

        /// <summary>
        /// Determines whether [has qualifying start] [the specified the delivery].
        /// </summary>
        /// <param name="theDelivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [has qualifying start] [the specified the delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasQualifyingStart(ILearningDelivery theDelivery) =>
            theDelivery.LearnStartDate < FirstInviableDate;

        /// <summary>
        /// Determines whether [has disqualifying monitor] [the specified monitor].
        /// there can only be one SOF code on a delivery
        /// </summary>
        /// <param name="monitor">The monitor.</param>
        /// <returns>
        ///   <c>true</c> if [has disqualifying monitor] [the specified monitor]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasDisqualifyingMonitor(ILearningDeliveryFAM monitor) =>
            It.IsInRange(monitor.LearnDelFAMType, Monitoring.Delivery.Types.SourceOfFunding)
            && It.IsOutOfRange($"{monitor.LearnDelFAMType}{monitor.LearnDelFAMCode}", Monitoring.Delivery.ESFAAdultFunding);

        /// <summary>
        /// Determines whether [has disqualifying monitor] [the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [has disqualifying monitor] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasDisqualifyingMonitor(ILearningDelivery delivery) =>
            _check.CheckDeliveryFAMs(delivery, HasDisqualifyingMonitor);

        /// <summary>
        /// Determines whether [has qualifying funding] [the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [has qualifying funding] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasQualifyingFunding(ILearningDelivery delivery) =>
            _check.HasQualifyingFunding(
                delivery,
                TypeOfFunding.CommunityLearning,
                TypeOfFunding.AdultSkills,
                TypeOfFunding.ApprenticeshipsFrom1May2017,
                TypeOfFunding.EuropeanSocialFund,
                TypeOfFunding.OtherAdult);

        /// <summary>
        /// Determines whether [is not valid] [the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [is not valid] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsNotValid(ILearningDelivery delivery) =>
            HasQualifyingStart(delivery)
            && HasQualifyingFunding(delivery)
            && HasDisqualifyingMonitor(delivery);

        /// <summary>
        /// Validates the specified learner.
        /// </summary>
        /// <param name="theLearner">The learner.</param>
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
        /// <param name="theDelivery">the delivery.</param>
        public void RaiseValidationMessage(string learnRefNumber, ILearningDelivery theDelivery) =>
            HandleValidationError(learnRefNumber, theDelivery.AimSeqNumber, BuildMessageParametersFor(theDelivery));

        /// <summary>
        /// Builds the message parameters for (this delivery).
        /// </summary>
        /// <param name="thisDelivery">this delivery.</param>
        /// <returns>
        /// returns a list of message parameters
        /// </returns>
        public IEnumerable<IErrorMessageParameter> BuildMessageParametersFor(ILearningDelivery thisDelivery) => new[]
        {
            BuildErrorMessageParameter(PropertyNameConstants.FundModel, thisDelivery.FundModel),
            BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, Monitoring.Delivery.Types.SourceOfFunding),
            BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMCode, LearningDeliveryFAMCodeConstants.SOF_ESFA_Adult)
        };
    }
}
