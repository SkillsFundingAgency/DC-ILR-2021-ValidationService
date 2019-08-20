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
    /// united kingdom provider number rule 17
    /// </summary>
    /// <seealso cref="AbstractRule" />
    /// <seealso cref="Interface.IRule{ILearner}" />
    public class UKPRN_17Rule : 
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

        /// <summary>
        /// Initializes a new instance of the <see cref="UKPRN_17Rule"/> class.
        /// </summary>
        /// <param name="validationErrorHandler">The validation error handler.</param>
        /// <param name="fileDataService">The file data service.</param>
        /// <param name="commonOps">The common ops.</param>
        /// <param name="fcsDataService">The FCS data service.</param>
        public UKPRN_17Rule(
            IValidationErrorHandler validationErrorHandler,
            IFileDataService fileDataService,
            IProvideRuleCommonOperations commonOps,
            IFCSDataService fcsDataService)
            : base(validationErrorHandler, RuleNameConstants.UKPRN_17)
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
                FundingStreamPeriodCodeConstants.C16_18TRN1920
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
            HasQualifyingModel(theDelivery)
                && IsTraineeship(theDelivery)
                && HasQualifyingMonitor(theDelivery)
                && HasFundingRelationship()
                && HasStartedAfterStopDate(theDelivery);

        /// <summary>
        /// Determines whether [has qualifying model] [the specified the delivery].
        /// </summary>
        /// <param name="theDelivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [has qualifying model] [the specified the delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasQualifyingModel(ILearningDelivery theDelivery) =>
            _check.HasQualifyingFunding(theDelivery, TypeOfFunding.Age16To19ExcludingApprenticeships);

        /// <summary>
        /// Determines whether the specified the delivery is traineeship.
        /// </summary>
        /// <param name="theDelivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if the specified the delivery is traineeship; otherwise, <c>false</c>.
        /// </returns>
        public bool IsTraineeship(ILearningDelivery theDelivery) =>
            _check.IsTraineeship(theDelivery);

        /// <summary>
        /// Determines whether [has qualifying monitor] [the specified the monitor].
        /// </summary>
        /// <param name="theMonitor">The monitor.</param>
        /// <returns>
        ///   <c>true</c> if [has qualifying monitor] [the specified the monitor]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasQualifyingMonitor(ILearningDeliveryFAM theMonitor) =>
            It.IsInRange($"{theMonitor.LearnDelFAMType}{theMonitor.LearnDelFAMCode}", Monitoring.Delivery.ESFAAdultFunding);

        /// <summary>
        /// Determines whether [has qualifying monitor] [the specified the delivery].
        /// </summary>
        /// <param name="theDelivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [has qualifying monitor] [the specified the delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasQualifyingMonitor(ILearningDelivery theDelivery) =>
            _check.CheckDeliveryFAMs(theDelivery, HasQualifyingMonitor);

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
        /// <param name="theDelivery">The delivery.</param>
        /// <returns></returns>
        public IEnumerable<IErrorMessageParameter> BuildMessageParametersFor(ILearningDelivery theDelivery) =>
            new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.UKPRN, ProviderUKPRN),
                BuildErrorMessageParameter(PropertyNameConstants.FundModel, theDelivery.FundModel),
                BuildErrorMessageParameter(PropertyNameConstants.ProgType, theDelivery.ProgTypeNullable),
                BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, theDelivery.LearnStartDate),
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, LearningDeliveryFAMTypeConstants.SOF),
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMCode, LearningDeliveryFAMCodeConstants.SOF_ESFA_Adult)
            };
    }
}