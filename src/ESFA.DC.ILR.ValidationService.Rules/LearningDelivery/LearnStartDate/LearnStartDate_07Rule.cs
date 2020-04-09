using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
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
    /// <summary>
    /// learn start date rule 07
    /// </summary>
    /// <seealso cref="AbstractRule" />
    /// <seealso cref="Interface.IRule{ILearner}" />
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
        /// Validates the specified learner.
        /// </summary>
        /// <param name="theLearner">The learner.</param>
        public void Validate(ILearner theLearner)
        {
            It.IsNull(theLearner)
                .AsGuard<ArgumentNullException>(nameof(theLearner));

            var learnrefNumber = theLearner.LearnRefNumber;
            var deliveries = theLearner.LearningDeliveries.ToReadOnlyCollection();

            deliveries.ForAny(
                x => IsNotValid(x, GetEarliestStartDateFor(x, deliveries)),
                x => RaiseValidationMessage(learnrefNumber, x));
        }

        /// <summary>
        /// Gets the earliest start date for.
        /// </summary>
        /// <param name="theDelivery">The  delivery.</param>
        /// <param name="usingSources">Using sources.</param>
        /// <returns>return the earliest stat date for this aim</returns>
        public DateTime? GetEarliestStartDateFor(ILearningDelivery theDelivery, IReadOnlyCollection<ILearningDelivery> usingSources) =>
            _derivedData04.GetEarliesStartDateFor(theDelivery, usingSources);

        /// <summary>
        /// Determines whether [is not valid] [the delivery].
        /// </summary>
        /// <param name="theDelivery">The delivery.</param>
        /// <param name="earliestStart">The earliest start.</param>
        /// <returns>
        ///   <c>true</c> if [is not valid] [the delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsNotValid(ILearningDelivery theDelivery, DateTime? earliestStart) =>
            !IsExcluded(theDelivery)
                && IsComponentAim(theDelivery)
                && IsApprenticeship(theDelivery)
                && HasEarliestStart(earliestStart)
                && !HasQualifyingFrameworkAim(FilteredFrameworkAimsFor(theDelivery, GetQualifyingFrameworksFor(theDelivery)), x=> IsCurrent(x, earliestStart.Value));

        /// <summary>
        /// Determines whether this delivery is excluded.
        /// </summary>
        /// <param name="theDelivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if this delivery is excluded; otherwise, <c>false</c>.
        /// </returns>
        public bool IsExcluded(ILearningDelivery theDelivery) =>
            IsStandardApprenticeship(theDelivery)
                || IsRestart(theDelivery)
                || IsCommonComponent(GetLARSLearningDeliveryFor(theDelivery));

        /// <summary>
        /// Determines whether [is standard Apprenticeship] [the specified delivery].
        /// </summary>
        /// <param name="theDelivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [is standard Apprenticeship] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsStandardApprenticeship(ILearningDelivery theDelivery) =>
            _check.IsStandardApprenticeship(theDelivery);

        /// <summary>
        /// Determines whether the specified delivery is restart.
        /// </summary>
        /// <param name="theDelivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if the specified delivery is restart; otherwise, <c>false</c>.
        /// </returns>
        public bool IsRestart(ILearningDelivery theDelivery) =>
            _check.IsRestart(theDelivery);

        /// <summary>
        /// Gets the LARS delivery for.
        /// </summary>
        /// <param name="theDelivery">the delivery.</param>
        /// <returns>the LARS delivery</returns>
        public ILARSLearningDelivery GetLARSLearningDeliveryFor(ILearningDelivery theDelivery) =>
            _larsData.GetDeliveryFor(theDelivery.LearnAimRef);

        /// <summary>
        /// Checks if the lars delivery is a common component.
        /// </summary>
        /// <param name="larsDelivery">lars delivery.</param>
        /// <returns>true if common component, false if not</returns>
        public bool IsCommonComponent(ILARSLearningDelivery larsDelivery) =>
            It.IsInRange(larsDelivery?.FrameworkCommonComponent, TypeOfLARSCommonComponent.CommonComponents);

        /// <summary>
        /// Determines whether [is component aim] [the specified delivery].
        /// </summary>
        /// <param name="theDelivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [is component aim] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsComponentAim(ILearningDelivery theDelivery) =>
            _check.IsComponentOfAProgram(theDelivery);

        /// <summary>
        /// Determines whether the specified delivery is apprenticeship.
        /// </summary>
        /// <param name="theDelivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if the specified delivery is apprenticeship; otherwise, <c>false</c>.
        /// </returns>
        public bool IsApprenticeship(ILearningDelivery theDelivery) =>
            _check.InApprenticeship(theDelivery);

        /// <summary>
        /// Determines whether [has earliest start] [the specified earliest start].
        /// </summary>
        /// <param name="earliestStart">The earliest start.</param>
        /// <returns>
        ///   <c>true</c> if [has earliest start] [the specified earliest start]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasEarliestStart(DateTime? earliestStart) =>
            It.Has(earliestStart);

        /// <summary>
        /// Gets the qualifying frameworks for.
        /// </summary>
        /// <param name="theDelivery">The delivery.</param>
        /// <returns>the filtered list of framework aims</returns>
        public IReadOnlyCollection<ILARSFrameworkAim> GetQualifyingFrameworksFor(ILearningDelivery theDelivery) =>
            _larsData.GetFrameworkAimsFor(theDelivery.LearnAimRef);

        /// <summary>
        /// Filtered framework aims for.
        /// </summary>
        /// <param name="theDelivery">The delivery.</param>
        /// <param name="usingTheseAims">using these aims.</param>
        /// <returns>
        /// the list filtered on programme type, framework code and pathway code
        /// </returns>
        public IReadOnlyCollection<ILARSFrameworkAim> FilteredFrameworkAimsFor(ILearningDelivery theDelivery, IReadOnlyCollection<ILARSFrameworkAim> usingTheseAims) =>
            usingTheseAims
                .SafeWhere(fa => fa.ProgType == theDelivery.ProgTypeNullable
                    && fa.FworkCode == theDelivery.FworkCodeNullable
                    && fa.PwayCode == theDelivery.PwayCodeNullable)
                .ToReadOnlyCollection();

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
        public bool HasQualifyingFrameworkAim(IReadOnlyCollection<ILARSFrameworkAim> frameworkAims, Func<IReadOnlyCollection<ILARSFrameworkAim>, bool> isCurrent) =>
            IsOutOfScope(frameworkAims) 
            || isCurrent(frameworkAims);

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

        public bool IsCurrent(IReadOnlyCollection<ILARSFrameworkAim> frameworkAims, DateTime earliestStart) =>
            frameworkAims.Any(fa => IsCurrent(fa, earliestStart));

        public bool IsCurrent(ILARSFrameworkAim frameworkAim, DateTime candidateStart) =>
            It.IsBetween(candidateStart, DateTime.MinValue, frameworkAim.EndDate ?? DateTime.MaxValue);

        /// <summary>
        /// Raises the validation message.
        /// </summary>
        /// <param name="learnRefNumber">The learn reference number.</param>
        /// <param name="thisDelivery">this delivery.</param>
        public void RaiseValidationMessage(string learnRefNumber, ILearningDelivery thisDelivery) =>
            HandleValidationError(learnRefNumber, thisDelivery.AimSeqNumber, BuildMessageParametersFor(thisDelivery));

        /// <summary>
        /// Builds the message parameters for.
        /// </summary>
        /// <param name="thisDelivery">this delivery.</param>
        /// <returns>a collection of message parameters</returns>
        public IEnumerable<IErrorMessageParameter> BuildMessageParametersFor(ILearningDelivery thisDelivery) => new[]
        {
            BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, thisDelivery.LearnStartDate),
            BuildErrorMessageParameter(PropertyNameConstants.PwayCode, thisDelivery.PwayCodeNullable),
            BuildErrorMessageParameter(PropertyNameConstants.ProgType, thisDelivery.ProgTypeNullable),
            BuildErrorMessageParameter(PropertyNameConstants.FworkCode, thisDelivery.FworkCodeNullable),
        };
    }
}
