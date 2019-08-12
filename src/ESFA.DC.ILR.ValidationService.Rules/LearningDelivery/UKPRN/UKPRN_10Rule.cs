using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface;
using ESFA.DC.ILR.ValidationService.Data.File.FileData.Interface;
using ESFA.DC.ILR.ValidationService.Data.Internal.AcademicYear.Interface;
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
    /// the UKPRN rule 10
    /// </summary>
    /// <seealso cref="AbstractRule" />
    /// <seealso cref="Interface.IRule{ILearner}" />
    public class UKPRN_10Rule :
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
        /// Initializes a new instance of the <see cref="UKPRN_10Rule"/> class.
        /// </summary>
        /// <param name="validationErrorHandler">The validation error handler.</param>
        /// <param name="fileDataService">The file data service.</param>
        /// <param name="academicYearDataService">The academic year data service.</param>
        /// <param name="commonOps">The common ops.</param>
        /// <param name="fcsDataService">The FCS data service.</param>
        public UKPRN_10Rule(
            IValidationErrorHandler validationErrorHandler,
            IFileDataService fileDataService,
            IAcademicYearDataService academicYearDataService,
            IProvideRuleCommonOperations commonOps,
            IFCSDataService fcsDataService)
            : base(validationErrorHandler, RuleNameConstants.UKPRN_10)
        {
            It.IsNull(validationErrorHandler)
                .AsGuard<ArgumentNullException>(nameof(validationErrorHandler));
            It.IsNull(fileDataService)
                .AsGuard<ArgumentNullException>(nameof(fileDataService));
            It.IsNull(academicYearDataService)
                .AsGuard<ArgumentNullException>(nameof(academicYearDataService));
            It.IsNull(commonOps)
                .AsGuard<ArgumentNullException>(nameof(commonOps));
            It.IsNull(fcsDataService)
                .AsGuard<ArgumentNullException>(nameof(fcsDataService));

            FundingStreams = new CaseInsensitiveDistinctKeySet
            {
                FundingStreamPeriodCodeConstants.LEVY1799,
                FundingStreamPeriodCodeConstants.NONLEVY2019
            };
            FirstViableStart = new DateTime(2017, 05, 01);
            AcademicYearStartDate = academicYearDataService.Start();
            ProviderUKPRN = fileDataService.UKPRN();

            _check = commonOps;
            _fcsData = fcsDataService;
        }

        /// <summary>
        /// Gets the funding streams.
        /// </summary>
        public CaseInsensitiveDistinctKeySet FundingStreams { get; }

        /// <summary>
        /// Gets the first viable start, which is 1st May 2017
        /// </summary>
        public DateTime FirstViableStart { get; }

        /// <summary>
        /// Gets the academic year start date.
        /// </summary>
        public DateTime AcademicYearStartDate { get; }

        /// <summary>
        /// Gets the provider ukprn.
        /// </summary>
        public int ProviderUKPRN { get; }

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
        /// Determines whether [is not valid] [the specified the delivery].
        /// </summary>
        /// <param name="theDelivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [is not valid] [the specified the delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsNotValid(ILearningDelivery theDelivery) =>
            !IsExcluded(theDelivery)
                && HasQualifyingModel(theDelivery)
                && HasQualifyingStart(theDelivery)
                && HasQualifyingMonitor(theDelivery)
                && !HasFundingRelationship();

        /// <summary>
        /// Determines whether the specified the delivery is excluded.
        /// </summary>
        /// <param name="theDelivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if the specified the delivery is excluded; otherwise, <c>false</c>.
        /// </returns>
        public bool IsExcluded(ILearningDelivery theDelivery) =>
            HasDisqualifyingEndDate(theDelivery);

        /// <summary>
        /// Determines whether [has disqualifying end date] [the specified the delivery].
        /// </summary>
        /// <param name="theDelivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [has disqualifying end date] [the specified the delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasDisqualifyingEndDate(ILearningDelivery theDelivery) =>
            AcademicYearStartDate > theDelivery.LearnActEndDateNullable;

        /// <summary>
        /// Determines whether [has qualifying model] [the specified the delivery].
        /// </summary>
        /// <param name="theDelivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [has qualifying model] [the specified the delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasQualifyingModel(ILearningDelivery theDelivery) =>
            _check.HasQualifyingFunding(theDelivery, TypeOfFunding.ApprenticeshipsFrom1May2017);

        /// <summary>
        /// Determines whether [has qualifying start] [the specified the delivery].
        /// </summary>
        /// <param name="theDelivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [has qualifying start] [the specified the delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasQualifyingStart(ILearningDelivery theDelivery) =>
            _check.HasQualifyingStart(theDelivery, FirstViableStart);

        /// <summary>
        /// Determines whether [has qualifying monitor] [the specified the monitor].
        /// </summary>
        /// <param name="theMonitor">The monitor.</param>
        /// <returns>
        ///   <c>true</c> if [has qualifying monitor] [the specified the monitor]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasQualifyingMonitor(ILearningDeliveryFAM theMonitor) =>
            It.IsInRange($"{theMonitor.LearnDelFAMType}{theMonitor.LearnDelFAMCode}", Monitoring.Delivery.ApprenticeshipFundedThroughAContractForServicesWithEmployer);

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
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, LearningDeliveryFAMTypeConstants.ACT),
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMCode, "1")
            };
    }
}
