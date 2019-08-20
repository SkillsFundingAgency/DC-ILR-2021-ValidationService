using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface;
using ESFA.DC.ILR.ValidationService.Data.File.FileData.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Utility;
using System;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.UKPRN
{
    /// <summary>
    /// united kingdom provider number rule 18
    /// </summary>
    /// <seealso cref="AbstractRule" />
    /// <seealso cref="Interface.IRule{ILearner}" />
    public class UKPRN_18Rule :
        AbstractRule,
        IRule<ILearner>
    {
        /// <summary>
        /// The check(er, common rule operations provider)
        /// </summary>
        private readonly IProvideRuleCommonOperations _check;

        /// <summary>
        /// The FCS data (service)
        /// </summary>
        private readonly IFCSDataService _fcsData;

        public UKPRN_18Rule(
            IValidationErrorHandler validationErrorHandler,
            IFileDataService fileDataService,
            IProvideRuleCommonOperations commonOps,
            IFCSDataService fcsDataService)
            : base(validationErrorHandler, RuleNameConstants.UKPRN_18)
        {
            It.IsNull(validationErrorHandler)
                .AsGuard<ArgumentNullException>(nameof(validationErrorHandler));
            It.IsNull(fileDataService)
                .AsGuard<ArgumentNullException>(nameof(fileDataService));
            It.IsNull(commonOps)
                .AsGuard<ArgumentNullException>(nameof(commonOps));
            It.IsNull(fcsDataService)
                .AsGuard<ArgumentNullException>(nameof(fcsDataService));

            ProviderUKPRN = fileDataService.UKPRN();
            FundingStreams = new CaseInsensitiveDistinctKeySet
            {
                FundingStreamPeriodCodeConstants.AEBC_19TRN1920,
                FundingStreamPeriodCodeConstants.AEBC_ASCL1920
            };

            _check = commonOps;
            _fcsData = fcsDataService;
        }

        /// <summary>
        /// Gets the funding streams.
        /// </summary>
        public CaseInsensitiveDistinctKeySet FundingStreams { get; }

        /// <summary>
        /// Gets the provider ukprn.
        /// </summary>
        public int ProviderUKPRN { get; }

        /// <summary>
        /// Validates the specified the learner.
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
        /// Determines whether [is not valid] [the specified the delivery].
        /// </summary>
        /// <param name="theDelivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [is not valid] [the specified the delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsNotValid(ILearningDelivery theDelivery) =>
            !IsExcluded(theDelivery)
                && HasQualifyingModel(theDelivery)
                && HasQualifyingMonitor(theDelivery)
                && HasFundingRelationship()
                && HasStartedAfterStopDate(theDelivery);

        /// <summary>
        /// Determines whether the specified the delivery is excluded.
        /// </summary>
        /// <param name="theDelivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if the specified the delivery is excluded; otherwise, <c>false</c>.
        /// </returns>
        public bool IsExcluded(ILearningDelivery theDelivery) =>
            HasDisqualifyingMonitor(theDelivery);

        /// <summary>
        /// Determines whether [has disqualifying monitor] [the specified delivery].
        /// </summary>
        /// <param name="theDelivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [has disqualifying monitor] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasDisqualifyingMonitor(ILearningDelivery theDelivery) =>
            _check.CheckDeliveryFAMs(theDelivery, IsAdultEducationBudgets);

        /// <summary>
        /// Determines whether [is adult education budgets] [the specified monitor].
        /// </summary>
        /// <param name="theMonitor">The monitor.</param>
        /// <returns>
        ///   <c>true</c> if [is adult education budgets] [the specified monitor]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsAdultEducationBudgets(ILearningDeliveryFAM theMonitor) =>
            It.IsInRange($"{theMonitor.LearnDelFAMType}{theMonitor.LearnDelFAMCode}", Monitoring.Delivery.AdultEducationBudgets);

        /// <summary>
        /// Determines whether [has qualifying model] [the specified the delivery].
        /// </summary>
        /// <param name="theDelivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [has qualifying model] [the specified the delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasQualifyingModel(ILearningDelivery theDelivery) =>
            _check.HasQualifyingFunding(theDelivery, TypeOfFunding.AdultSkills);

        /// <summary>
        /// Determines whether [is esfa adult funding] [the specified monitor].
        /// </summary>
        /// <param name="theMonitor">The monitor.</param>
        /// <returns>
        ///   <c>true</c> if [is esfa adult funding] [the specified monitor]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsESFAAdultFunding(ILearningDeliveryFAM theMonitor) =>
            It.IsInRange($"{theMonitor.LearnDelFAMType}{theMonitor.LearnDelFAMCode}", Monitoring.Delivery.ESFAAdultFunding);

        /// <summary>
        /// Determines whether [has qualifying monitor] [the specified the delivery].
        /// </summary>
        /// <param name="theDelivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [has qualifying monitor] [the specified the delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasQualifyingMonitor(ILearningDelivery theDelivery) =>
            _check.CheckDeliveryFAMs(theDelivery, IsESFAAdultFunding);

        /// <summary>
        /// Determines whether [has funding relationship].
        /// </summary>
        /// <returns>
        ///   <c>true</c> if [has funding relationship]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasFundingRelationship() =>
            _fcsData
                .GetContractAllocationsFor(ProviderUKPRN)
                .SafeAny(x => FundingStreams.Contains(x.FundingStreamPeriodCode));

        /// <summary>
        /// Determines whether [has started after stop date] [the specified delivery].
        /// </summary>
        /// <param name="theDelivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [has started after stop date] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasStartedAfterStopDate(ILearningDelivery theDelivery) =>
            _fcsData
                .GetContractAllocationsFor(ProviderUKPRN)
                .SafeAny(x => HasStartedAfterStopDate(x, theDelivery));

        /// <summary>
        /// Determines whether [has started after stop date] [the specified allocation].
        /// </summary>
        /// <param name="theAllocation">The allocation.</param>
        /// <param name="theDelivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [has started after stop date] [the specified allocation]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasStartedAfterStopDate(IFcsContractAllocation theAllocation, ILearningDelivery theDelivery) =>
            theDelivery.LearnStartDate >= theAllocation.StopNewStartsFromDate;

        /// <summary>
        /// Raises the validation message.
        /// </summary>
        /// <param name="learnRefNumber">The learn reference number.</param>
        /// <param name="theDelivery">The delivery.</param>
        public void RaiseValidationMessage(string learnRefNumber, ILearningDelivery theDelivery) =>
            HandleValidationError(learnRefNumber, theDelivery.AimSeqNumber, BuildMessageParametersFor(theDelivery));

        /// <summary>
        /// Builds the message parameters for.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IErrorMessageParameter> BuildMessageParametersFor(ILearningDelivery theDelivery) => new[]
        {
            BuildErrorMessageParameter(PropertyNameConstants.UKPRN, ProviderUKPRN),
            BuildErrorMessageParameter(PropertyNameConstants.FundModel, theDelivery.FundModel),
            BuildErrorMessageParameter(PropertyNameConstants.ProgType, theDelivery.ProgTypeNullable),
            BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, theDelivery.LearnStartDate),
        };
    }
}
