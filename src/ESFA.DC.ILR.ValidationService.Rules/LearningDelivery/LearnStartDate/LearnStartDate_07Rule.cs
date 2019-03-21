using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnStartDate
{
    public class LearnStartDate_07Rule :
        AbstractRule,
        IRule<ILearner>
    {
        /// <summary>
        /// The derived data 04 (rule)
        /// </summary>
        private readonly IDerivedData_04Rule _derivedData04;

        /// <summary>
        /// The lars data (service)
        /// </summary>
        private readonly ILARSDataService _larsData;

        /// <summary>
        /// The check (rule common operations provider)
        /// </summary>
        private readonly IProvideRuleCommonOperations _check;

        /// <summary>
        /// Initializes a new instance of the <see cref="LearnStartDate_07Rule"/> class.
        /// </summary>
        /// <param name="validationErrorHandler">The validation error handler.</param>
        /// <param name="derivedData07">The derived data07.</param>
        /// <param name="derivedData04">The derived data04.</param>
        /// <param name="larsData">The lars data.</param>
        /// <param name="commonOperations">The common operations.</param>
        public LearnStartDate_07Rule(
            IValidationErrorHandler validationErrorHandler,
            IDerivedData_04Rule derivedData04,
            ILARSDataService larsData,
            IProvideRuleCommonOperations commonOperations)
                : base(validationErrorHandler, RuleNameConstants.LearnStartDate_07)
        {
            It.IsNull(validationErrorHandler)
                .AsGuard<ArgumentNullException>(nameof(validationErrorHandler));
            It.IsNull(derivedData04)
                .AsGuard<ArgumentNullException>(nameof(derivedData04));
            It.IsNull(larsData)
                .AsGuard<ArgumentNullException>(nameof(larsData));
            It.IsNull(commonOperations)
                .AsGuard<ArgumentNullException>(nameof(commonOperations));

            _derivedData04 = derivedData04;
            _larsData = larsData;
            _check = commonOperations;
        }

        /// <summary>
        /// Validates this learner.
        /// </summary>
        /// <param name="thisLearner">this learner.</param>
        public void Validate(ILearner thisLearner)
        {
            It.IsNull(thisLearner)
                .AsGuard<ArgumentNullException>(nameof(thisLearner));

            var learnrefNumber = thisLearner.LearnRefNumber;
            var deliveries = thisLearner.LearningDeliveries.AsSafeReadOnlyList();

            deliveries.ForAny(
                x => IsNotValid(x, GetEarliestStartDateFor(x, deliveries)),
                x => RaiseValidationMessage(learnrefNumber, x));
        }

        /// <summary>
        /// Determines whether [is not valid] [this delivery].
        /// </summary>
        /// <param name="thisDelivery">this delivery.</param>
        /// <param name="earliestStart">The earliest start.</param>
        /// <returns>
        ///   <c>true</c> if [is not valid] [this delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsNotValid(ILearningDelivery thisDelivery, DateTime? earliestStart) =>
            !IsExcluded(thisDelivery)
                && _check.IsComponentOfAProgram(thisDelivery)
                && _check.InApprenticeship(thisDelivery)
                && !It.IsEmpty(earliestStart)
                && !HasQualifyingFrameworkAim(FilteredFrameworkAimsFor(thisDelivery, GetQualifyingFrameworksFor(thisDelivery)), earliestStart.Value);

        /// <summary>
        /// Gets the earliest start date for.
        /// </summary>
        /// <param name="thisDelivery">The this delivery.</param>
        /// <param name="usingSources">The using sources.</param>
        /// <returns>return the earliest stat date for this aim</returns>
        public DateTime? GetEarliestStartDateFor(ILearningDelivery thisDelivery, IReadOnlyCollection<ILearningDelivery> usingSources) =>
            _derivedData04.GetEarliesStartDateFor(thisDelivery, usingSources);

        /// <summary>
        /// Determines whether this delivery is excluded.
        /// </summary>
        /// <param name="thisDelivery">this delivery.</param>
        /// <returns>
        ///   <c>true</c> if this delivery is excluded; otherwise, <c>false</c>.
        /// </returns>
        public bool IsExcluded(ILearningDelivery thisDelivery) =>
            _check.IsStandardApprencticeship(thisDelivery)
            || _check.IsRestart(thisDelivery);

        /// <summary>
        /// Gets the qualifying frameworks for.
        /// </summary>
        /// <param name="thisDelivery">this delivery.</param>
        /// <returns>the filtered list of framework aims</returns>
        public IReadOnlyCollection<ILARSFrameworkAim> GetQualifyingFrameworksFor(ILearningDelivery thisDelivery) =>
            _larsData.GetFrameworkAimsFor(thisDelivery.LearnAimRef);

        /// <summary>
        /// Filtereds the framework aims for.
        /// </summary>
        /// <param name="thisDelivery">this delivery.</param>
        /// <param name="usingTheseAims">using these aims.</param>
        /// <returns>
        /// the list filtered on programme type, framework code and pathway code
        /// </returns>
        public IReadOnlyCollection<ILARSFrameworkAim> FilteredFrameworkAimsFor(ILearningDelivery thisDelivery, IReadOnlyCollection<ILARSFrameworkAim> usingTheseAims)
        {
            return usingTheseAims
                .SafeWhere(fa => fa.ProgType == thisDelivery.ProgTypeNullable
                    && fa.FworkCode == thisDelivery.FworkCodeNullable
                    && fa.PwayCode == thisDelivery.PwayCodeNullable)
                .AsSafeReadOnlyList();
        }

        /// <summary>
        /// Determines whether [has qualifying framework aim] [this delivery].
        /// TODO: back log item => restore full date range checks for 19/20 rollover processing
        /// IsOutOfScope(frameworkAims) || frameworkAims.Any(fa => fa.IsCurrent(earliestStart));
        /// </summary>
        /// <param name="frameworkAims">The framework aims.</param>
        /// <param name="earliestStart">The earliest start.</param>
        /// <returns>
        ///   <c>true</c> if [has qualifying framework aim] [this delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasQualifyingFrameworkAim(IReadOnlyCollection<ILARSFrameworkAim> frameworkAims, DateTime earliestStart) =>
            IsOutOfScope(frameworkAims) || frameworkAims.Any(fa => It.IsBetween(earliestStart, DateTime.MinValue, fa.EndDate ?? DateTime.MaxValue));

        /// <summary>
        /// Determines whether [is out of scope] [the specified framework aims].
        /// if the <paramref name="frameworkAims"/> come back empty after being filtered then it is
        ///  assumed the delivery aim is a framework common commponent
        /// </summary>
        /// <param name="frameworkAims">The framework aims.</param>
        /// <returns>
        ///   <c>true</c> if [is out of scope] [the specified framework aims]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsOutOfScope(IReadOnlyCollection<ILARSFrameworkAim> frameworkAims) =>
            It.IsEmpty(frameworkAims);

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
        /// Builds the message parameters for.
        /// </summary>
        /// <param name="thisDelivery">this delivery.</param>
        /// <returns>a collection of message parameters</returns>
        public IEnumerable<IErrorMessageParameter> BuildMessageParametersFor(ILearningDelivery thisDelivery)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, thisDelivery.LearnStartDate),
                BuildErrorMessageParameter(PropertyNameConstants.PwayCode, thisDelivery.PwayCodeNullable),
                BuildErrorMessageParameter(PropertyNameConstants.ProgType, thisDelivery.ProgTypeNullable),
                BuildErrorMessageParameter(PropertyNameConstants.FworkCode, thisDelivery.FworkCodeNullable),
            };
        }
    }
}
