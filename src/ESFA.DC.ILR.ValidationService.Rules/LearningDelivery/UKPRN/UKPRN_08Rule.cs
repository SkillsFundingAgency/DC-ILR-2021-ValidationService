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
    public class UKPRN_08Rule : AbstractRule, IRule<ILearner>
    {
        /// <summary>
        /// The file data service
        /// </summary>
        private readonly IFileDataService _fileDataService;

        /// <summary>
        /// The academic year data service
        /// </summary>
        private readonly IAcademicYearDataService _academicYearDataService;

        /// <summary>
        /// The checker (common operations provider)
        /// </summary>
        private readonly IProvideRuleCommonOperations _check;

        /// <summary>
        /// The FCS data service
        /// </summary>
        private readonly IFCSDataService _fcsDataService;

        /// <summary>
        /// Initializes a new instance of the <see cref="UKPRN_08Rule"/> class.
        /// </summary>
        /// <param name="validationErrorHandler">The validation error handler.</param>
        /// <param name="fileDataService">The file data service.</param>
        /// <param name="academicYearDataService">The academic year data service.</param>
        /// <param name="commonOperations">The common operations.</param>
        /// <param name="fcsDataService">The FCS data service.</param>
        public UKPRN_08Rule(
            IValidationErrorHandler validationErrorHandler,
            IFileDataService fileDataService,
            IAcademicYearDataService academicYearDataService,
            IProvideRuleCommonOperations commonOperations,
            IFCSDataService fcsDataService)
            : base(validationErrorHandler, RuleNameConstants.UKPRN_08)
        {
            // this check should be in the base class
            It.IsNull(validationErrorHandler)
                .AsGuard<ArgumentNullException>(nameof(validationErrorHandler));

            It.IsNull(fileDataService)
                .AsGuard<ArgumentNullException>(nameof(fileDataService));
            It.IsNull(academicYearDataService)
                .AsGuard<ArgumentNullException>(nameof(academicYearDataService));
            It.IsNull(commonOperations)
                .AsGuard<ArgumentNullException>(nameof(commonOperations));
            It.IsNull(fcsDataService)
                .AsGuard<ArgumentNullException>(nameof(fcsDataService));

            _fileDataService = fileDataService;
            _academicYearDataService = academicYearDataService;
            _check = commonOperations;
            _fcsDataService = fcsDataService;
        }

        /// <summary>
        /// Determines whether this delivery is excluded.
        /// </summary>
        /// <param name="thisDelivery">this delivery.</param>
        /// <returns>
        ///   <c>true</c> if this delivery is excluded; otherwise, <c>false</c>.
        /// </returns>
        public bool IsExcluded(ILearningDelivery thisDelivery) =>
            IsNotPartOfThisYear(thisDelivery);

        /// <summary>
        /// Gets the current academic year commencement date.
        /// </summary>
        /// <returns>a date time</returns>
        public DateTime GetCurrentAcademicYearCommencementDate() =>
            _academicYearDataService.GetAcademicYearOfLearningDate(_academicYearDataService.Today, AcademicYearDates.Commencement);

        /// <summary>
        /// Determines whether [is not part of this year] [this delivery].
        /// this delivery cannot be null
        /// </summary>
        /// <param name="thisDelivery">this delivery.</param>
        /// <returns>
        ///   <c>true</c> if [is not part of this year] [this delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsNotPartOfThisYear(ILearningDelivery thisDelivery) =>
            It.Has(thisDelivery.LearnActEndDateNullable)
                && thisDelivery.LearnActEndDateNullable < GetCurrentAcademicYearCommencementDate();

        /// <summary>
        /// Determines whether [has qualifying provider identifier] [the specified allocation].
        /// </summary>
        /// <param name="allocation">The allocation.</param>
        /// <param name="providerID">The provider identifier.</param>
        /// <returns>
        ///   <c>true</c> if [has qualifying provider identifier] [the specified allocation]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasQualifyingProviderID(IFcsContractAllocation allocation, int providerID) =>
            allocation.DeliveryUKPRN == providerID;

        /// <summary>
        /// Determines whether [has qualifying funding stream] [the specified allocation].
        /// </summary>
        /// <param name="allocation">The allocation.</param>
        /// <returns>
        ///   <c>true</c> if [has qualifying funding stream] [the specified allocation]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasQualifyingFundingStream(IFcsContractAllocation allocation) =>
            It.IsInRange(
                allocation.FundingStreamPeriodCode,
                FundingStreamPeriodCodeConstants.ALLB1920,
                FundingStreamPeriodCodeConstants.ALLBC1920);

        /// <summary>
        /// Determines whether [has funding relationship] [this delivery].
        /// </summary>
        /// <param name="thisDelivery">this delivery.</param>
        /// <returns>
        ///   <c>true</c> if [has funding relationship] [this delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasFundingRelationship(ILearningDelivery thisDelivery)
        {
            var allocations = _fcsDataService.GetContractAllocationsFor(_fileDataService.UKPRN());

            return allocations.SafeAny(HasQualifyingFundingStream);
        }

        /// <summary>
        /// Determines whether [is not valid] [this delivery].
        /// </summary>
        /// <param name="thisDelivery">this delivery.</param>
        /// <returns>
        ///   <c>true</c> if [is not valid] [this delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsNotValid(ILearningDelivery thisDelivery)
        {
            return !IsExcluded(thisDelivery)
                && _check.IsLoansBursary(thisDelivery)
                && !HasFundingRelationship(thisDelivery);
        }

        /// <summary>
        /// Validates this learner.
        /// </summary>
        /// <param name="thisLearner">this learner.</param>
        public void Validate(ILearner thisLearner)
        {
            It.IsNull(thisLearner)
                .AsGuard<ArgumentNullException>(nameof(thisLearner));

            var learnRefNumber = thisLearner.LearnRefNumber;

            thisLearner.LearningDeliveries
                .ForAny(IsNotValid, x => RaiseValidationMessage(learnRefNumber, x));
        }

        /// <summary>
        /// Raises the validation message.
        /// </summary>
        /// <param name="learnRefNumber">The learn reference number.</param>
        /// <param name="thisDelivery">this delivery.</param>
        public void RaiseValidationMessage(string learnRefNumber, ILearningDelivery thisDelivery)
        {
            HandleValidationError(learnRefNumber, thisDelivery.AimSeqNumber, BuildMessageParameters());
        }

        /// <summary>
        /// Builds the message parameters.
        /// </summary>
        /// <returns>
        /// returns a collection of message parameters
        /// </returns>
        public IEnumerable<IErrorMessageParameter> BuildMessageParameters()
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.UKPRN, _fileDataService.UKPRN()),
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, Monitoring.Delivery.Types.AdvancedLearnerLoansBursaryFunding)
            };
        }
    }
}
