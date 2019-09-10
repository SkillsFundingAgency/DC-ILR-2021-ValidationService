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
    /// learning delivery funding amd monitoring rule 09
    /// </summary>
    /// <seealso cref="AbstractRule" />
    /// <seealso cref="Interface.IRule{ILearner}" />
    public class LearnDelFAMType_74Rule :
        AbstractRule,
        IRule<ILearner>
    {
        /// <summary>
        /// The check (rule common operations provider)
        /// </summary>
        private readonly IProvideRuleCommonOperations _check;

        /// <summary>
        /// Initializes a new instance of the <see cref="LearnDelFAMType_74Rule" /> class.
        /// </summary>
        /// <param name="validationErrorHandler">The validation error handler.</param>
        /// <param name="commonOperations">The common operations.</param>
        public LearnDelFAMType_74Rule(
            IValidationErrorHandler validationErrorHandler,
            IProvideRuleCommonOperations commonOperations)
            : base(validationErrorHandler, RuleNameConstants.LearnDelFAMType_74)
        {
            It.IsNull(validationErrorHandler)
                .AsGuard<ArgumentNullException>(nameof(validationErrorHandler));
            It.IsNull(commonOperations)
                .AsGuard<ArgumentNullException>(nameof(commonOperations));

            _check = commonOperations;
        }

        /// <summary>
        /// Gets the last inviable date.
        /// </summary>
        public static DateTime LastInviableDate => new DateTime(2019, 07, 31);

        /// <summary>
        /// Determines whether [has qualifying start] [the specified the delivery].
        /// </summary>
        /// <param name="theDelivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [has qualifying start] [the specified the delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasQualifyingStart(ILearningDelivery theDelivery) =>
            theDelivery.LearnStartDate > LastInviableDate;

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
                TypeOfFunding.ApprenticeshipsFrom1May2017,
                TypeOfFunding.EuropeanSocialFund,
                TypeOfFunding.OtherAdult);

        /// <summary>
        /// Determines whether [has traineeship funding] [the specified delivery].
        /// </summary>
        /// <param name="theDelivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [has traineeship funding] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasTraineeshipFunding(ILearningDelivery theDelivery) =>
            HasQualifyingTraineeshipModel(theDelivery)
            && IsTraineeship(theDelivery);

        /// <summary>
        /// Determines whether [has qualifying traineeship model] [the specified delivery].
        /// </summary>
        /// <param name="theDelivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [has qualifying traineeship model] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasQualifyingTraineeshipModel(ILearningDelivery theDelivery) =>
            _check.HasQualifyingFunding(theDelivery, TypeOfFunding.AdultSkills);

        /// <summary>
        /// Determines whether the specified delivery is traineeship.
        /// </summary>
        /// <param name="theDelivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if the specified delivery is traineeship; otherwise, <c>false</c>.
        /// </returns>
        public bool IsTraineeship(ILearningDelivery theDelivery) =>
            _check.IsTraineeship(theDelivery);

        /// <summary>
        /// Determines whether [is not valid] [the specified delivery].
        /// </summary>
        /// <param name="theDelivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [is not valid] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsNotValid(ILearningDelivery theDelivery) =>
            HasQualifyingStart(theDelivery)
            && (HasQualifyingFunding(theDelivery) || HasTraineeshipFunding(theDelivery))
            && HasDisqualifyingMonitor(theDelivery);

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
        /// <param name="thisDelivery">this delivery.</param>
        public void RaiseValidationMessage(string learnRefNumber, ILearningDelivery thisDelivery) =>
            HandleValidationError(learnRefNumber, thisDelivery.AimSeqNumber, BuildMessageParametersFor(thisDelivery));

        /// <summary>
        /// Builds the message parameters for (this delivery).
        /// </summary>
        /// <param name="theDelivery">The delivery.</param>
        /// <returns>
        /// returns a list of message parameters
        /// </returns>
        public IEnumerable<IErrorMessageParameter> BuildMessageParametersFor(ILearningDelivery theDelivery) => new[]
        {
            BuildErrorMessageParameter(PropertyNameConstants.FundModel, theDelivery.FundModel),
            BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, Monitoring.Delivery.Types.SourceOfFunding),
            BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMCode, LearningDeliveryFAMCodeConstants.SOF_ESFA_Adult)
        };
    }
}
