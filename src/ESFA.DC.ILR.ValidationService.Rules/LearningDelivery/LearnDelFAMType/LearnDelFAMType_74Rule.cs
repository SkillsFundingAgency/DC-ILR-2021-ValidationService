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
        /// Determines whether [has qualifying monitor] [the specified monitor].
        /// there can only be one SOF code on a delivery
        /// </summary>
        /// <param name="monitor">The monitor.</param>
        /// <returns>
        ///   <c>true</c> if [has qualifying monitor] [the specified monitor]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasQualifyingMonitor(ILearningDeliveryFAM monitor) =>
            It.IsOutOfRange(monitor.LearnDelFAMType, Monitoring.Delivery.Types.SourceOfFunding)
            || It.IsInRange($"{monitor.LearnDelFAMType}{monitor.LearnDelFAMCode}", 
                    Monitoring.Delivery.ESFAAdultFunding,
                    Monitoring.Delivery.CambridgeshireAndPeterboroughCombinedAuthority,
                    Monitoring.Delivery.GreaterLondonAuthority,
                    Monitoring.Delivery.GreaterManchesterCombinedAuthority,
                    Monitoring.Delivery.LiverpoolCityRegionCombinedAuthority,
                    Monitoring.Delivery.TeesValleyCombinedAuthority,
                    Monitoring.Delivery.WestMidlandsCombinedAuthority,
                    Monitoring.Delivery.WestOfEnglandCombinedAuthority);

        /// <summary>
        /// Determines whether [has qualifying monitor] [the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [has qualifying monitor] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasQualifyingMonitor(ILearningDelivery delivery) =>
            _check.CheckDeliveryFAMs(delivery, HasQualifyingMonitor);

        /// <summary>
        /// Determines whether the specified delivery has monitors.
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if the specified delivery has monitors; otherwise, <c>false</c>.
        /// </returns>
        public bool HasMonitors(ILearningDelivery delivery) =>
            It.HasValues(delivery.LearningDeliveryFAMs);

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
            && (!HasMonitors(delivery) || !HasQualifyingMonitor(delivery));

        /// <summary>
        /// Validates the specified object.
        /// </summary>
        /// <param name="objectToValidate">The object to validate.</param>
        public void Validate(ILearner objectToValidate)
        {
            It.IsNull(objectToValidate)
                .AsGuard<ArgumentNullException>(nameof(objectToValidate));

            var learnRefNumber = objectToValidate.LearnRefNumber;

            objectToValidate.LearningDeliveries
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
        /// Builds the message parameters for (this delivery).
        /// </summary>
        /// <param name="thisDelivery">this delivery.</param>
        /// <returns>
        /// returns a list of message parameters
        /// </returns>
        public IEnumerable<IErrorMessageParameter> BuildMessageParametersFor(ILearningDelivery thisDelivery)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.FundModel, thisDelivery.FundModel),
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, LearningDeliveryFAMTypeConstants.SOF),
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMCode, LearningDeliveryFAMCodeConstants.SOF_ESFA_Adult)
            };
        }
    }
}
