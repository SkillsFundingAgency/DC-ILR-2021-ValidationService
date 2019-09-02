using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.Organisation.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.Postcodes.Interface;
using ESFA.DC.ILR.ValidationService.Data.File.FileData.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LSDPostcode
{
    /// <summary>
    /// learning start date postcode rule 02
    /// </summary>
    /// <seealso cref="AbstractRule" />
    /// <seealso cref="Interface.IRule{ILearner}" />
    public class LSDPostcode_02Rule :
        AbstractRule,
        IRule<ILearner>
    {
        /// <summary>
        /// The check(er, rule common operations provider)
        /// </summary>
        private readonly IProvideRuleCommonOperations _check;

        /// <summary>
        /// The postcode data (service)
        /// </summary>
        private readonly IPostcodesDataService _postcodeData;

        /// <summary>
        /// Initializes a new instance of the <see cref="LSDPostcode_02Rule"/> class.
        /// </summary>
        /// <param name="validationErrorHandler">The validation error handler.</param>
        /// <param name="commonOps">The common rule operations provider.</param>
        /// <param name="organisationDataService">The organisation data service.</param>
        /// <param name="fileDataService">The file data service.</param>
        /// <param name="postcodesDataService">The postcodes data service.</param>
        public LSDPostcode_02Rule(
            IValidationErrorHandler validationErrorHandler,
            IProvideRuleCommonOperations commonOps,
            IOrganisationDataService organisationDataService,
            IFileDataService fileDataService,
            IPostcodesDataService postcodesDataService)
            : base(validationErrorHandler, RuleNameConstants.LSDPostcode_02)
        {
            It.IsNull(validationErrorHandler)
                .AsGuard<ArgumentNullException>(nameof(validationErrorHandler));
            It.IsNull(commonOps)
                .AsGuard<ArgumentNullException>(nameof(commonOps));
            It.IsNull(organisationDataService)
                .AsGuard<ArgumentNullException>(nameof(organisationDataService));
            It.IsNull(fileDataService)
                .AsGuard<ArgumentNullException>(nameof(fileDataService));
            It.IsNull(postcodesDataService)
                .AsGuard<ArgumentNullException>(nameof(postcodesDataService));

            var providerUKPRN = fileDataService.UKPRN();

            OrganisationType = organisationDataService.GetLegalOrgTypeForUkprn(providerUKPRN);
            FirstViableStart = new DateTime(2019, 08, 01);

            _check = commonOps;
            _postcodeData = postcodesDataService;
        }

        /// <summary>
        /// Gets the first viable start, which is 1st August 2019
        /// </summary>
        public DateTime FirstViableStart { get; }

        /// <summary>
        /// Gets the type of organisation.
        /// </summary>
        public string OrganisationType { get; }

        /// <summary>
        /// Validates the specified the learner.
        /// </summary>
        /// <param name="theLearner">The learner.</param>
        public void Validate(ILearner theLearner)
        {
            It.IsNull(theLearner)
                .AsGuard<ArgumentNullException>(nameof(theLearner));

            if (!IsSpecialistDesignatedCollege())
            {
                var learnRefNumber = theLearner.LearnRefNumber;

                theLearner.LearningDeliveries
                    .ForAny(IsNotValid, x => RaiseValidationMessage(learnRefNumber, x));
            }
        }

        /// <summary>
        /// Determines whether [is specialist designated college].
        /// </summary>
        /// <returns>
        ///   <c>true</c> if [is specialist designated college]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsSpecialistDesignatedCollege() =>
            It.IsInRange(OrganisationType, LegalOrgTypeConstants.USDC);

        /// <summary>
        /// Determines whether [is not valid] [the specified delivery].
        /// </summary>
        /// <param name="theDelivery">The delivery.</param>
        /// <param name="devolvedPostcodes">The devolved postcodes.</param>
        /// <returns>
        ///   <c>true</c> if [is not valid] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsNotValid(ILearningDelivery theDelivery) =>
            !IsExcluded(theDelivery)
            && HasQualifyingModel(theDelivery)
            && HasQualifyingStart(theDelivery)
            && !HasValidSourceOfFunding(theDelivery);

        /// <summary>
        /// Determines whether the specified the delivery is excluded.
        /// </summary>
        /// <param name="theDelivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if the specified the delivery is excluded; otherwise, <c>false</c>.
        /// </returns>
        public bool IsExcluded(ILearningDelivery theDelivery) =>
            IsTraineeship(theDelivery)
            || IsRestart(theDelivery)
            || IsPostcodeValidationExclusion(theDelivery);

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
        /// Determines whether the specified the delivery is restart.
        /// </summary>
        /// <param name="theDelivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if the specified the delivery is restart; otherwise, <c>false</c>.
        /// </returns>
        public bool IsRestart(ILearningDelivery theDelivery) =>
            _check.IsRestart(theDelivery);

        /// <summary>
        /// Determines whether [is postcode validation exclusion] [the specified delivery].
        /// </summary>
        /// <param name="theDelivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [is postcode validation exclusion] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsPostcodeValidationExclusion(ILearningDelivery theDelivery) =>
            _check.CheckDeliveryFAMs(theDelivery, IsPostcodeValidationExclusion);

        /// <summary>
        /// Determines whether [is postcode validation exclusion] [the specified monitor].
        /// </summary>
        /// <param name="theMonitor">The monitor.</param>
        /// <returns>
        ///   <c>true</c> if [is postcode validation exclusion] [the specified monitor]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsPostcodeValidationExclusion(ILearningDeliveryFAM theMonitor) =>
            It.IsInRange($"{theMonitor.LearnDelFAMType}{theMonitor.LearnDelFAMCode}", Monitoring.Delivery.PostcodeValidationExclusion);

        /// <summary>
        /// Determines whether [has qualifying model] [the specified delivery].
        /// </summary>
        /// <param name="theDelivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [has qualifying model] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasQualifyingModel(ILearningDelivery theDelivery) =>
            _check.HasQualifyingFunding(theDelivery, TypeOfFunding.AdultSkills, TypeOfFunding.CommunityLearning);

        /// <summary>
        /// Determines whether [has qualifying start] [the specified delivery].
        /// </summary>
        /// <param name="theDelivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [has qualifying start] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasQualifyingStart(ILearningDelivery theDelivery) =>
            _check.HasQualifyingStart(theDelivery, FirstViableStart);

        /// <summary>
        /// Determines whether [has valid source of funding] [the specified delivery].
        /// </summary>
        /// <param name="theDelivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [has valid source of funding] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasValidSourceOfFunding(ILearningDelivery theDelivery)
        {
            var devolvedPCs = GetDevolvedPostcodes(theDelivery);
            var ldFamSofs = GetDeliveryFundingCodes(theDelivery);
            var devolvedPCsForSof = GetDevolvedPostcodesForSoF(devolvedPCs, x => HasQualifyingFundingCode(x, ldFamSofs));

            return HasValidSourceOfFunding(devolvedPCsForSof, x => HasQualifyingEffectiveStart(x, theDelivery));
        }

        /// <summary>
        /// Gets the devolved postcodes.
        /// </summary>
        /// <param name="theDelivery">The delivery.</param>
        /// <returns>a collection of devolved postcodes</returns>
        public IReadOnlyCollection<IDevolvedPostcode> GetDevolvedPostcodes(ILearningDelivery theDelivery) =>
            _postcodeData.GetDevolvedPostcodes(theDelivery.LSDPostcode);

        /// <summary>
        /// Gets the delivery funding codes.
        /// </summary>
        /// <param name="theDelivery">The delivery.</param>
        /// <returns>a collection of delivery monitor (source of) funding codes</returns>
        public IContainThis<string> GetDeliveryFundingCodes(ILearningDelivery theDelivery) =>
            theDelivery.LearningDeliveryFAMs
                .SafeWhere(HasQualifyingFundingType)
                .Select(GetFundingCode)
                .AsSafeDistinctKeySet();

        /// <summary>
        /// Determines whether [has qualifying funding type] [the specified monitor].
        /// </summary>
        /// <param name="theMonitor">The monitor.</param>
        /// <returns>
        ///   <c>true</c> if [has qualifying funding type] [the specified monitor]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasQualifyingFundingType(ILearningDeliveryFAM theMonitor) =>
            theMonitor.LearnDelFAMType == Monitoring.Delivery.Types.SourceOfFunding;

        /// <summary>
        /// Gets the funding code.
        /// </summary>
        /// <param name="theMonitor">The monitor.</param>
        /// <returns>the montior funding code</returns>
        public string GetFundingCode(ILearningDeliveryFAM theMonitor) =>
            theMonitor.LearnDelFAMCode;

        /// <summary>
        /// Gets the devolved postcodes for sof.
        /// </summary>
        /// <param name="devolvedPCs">The devolved p cs.</param>
        /// <param name="hasQualifyingCode">The has qualifying code.</param>
        /// <returns></returns>
        public IReadOnlyCollection<IDevolvedPostcode> GetDevolvedPostcodesForSoF(
            IReadOnlyCollection<IDevolvedPostcode> devolvedPCs, 
            Func<IDevolvedPostcode, bool> hasQualifyingCode) =>
                devolvedPCs
                    .SafeWhere(hasQualifyingCode)
                    .AsSafeReadOnlyList();

        /// <summary>
        /// Determines whether [has qualifying funding code] [the specified devolved pc].
        /// </summary>
        /// <param name="devolvedPC">The devolved pc.</param>
        /// <param name="deliveryFundingCodes">The delivery funding codes.</param>
        /// <returns>
        ///   <c>true</c> if [has qualifying funding code] [the specified devolved pc]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasQualifyingFundingCode(IDevolvedPostcode devolvedPC, IContainThis<string> deliveryFundingCodes) =>
            deliveryFundingCodes.Contains(devolvedPC.SourceOfFunding);

        /// <summary>
        /// Determines whether [has valid source of funding] [the specified devolved postcodes].
        /// </summary>
        /// <param name="devolvedPCs">The devolved p cs.</param>
        /// <param name="hasQualifyingEffectiveStart">The has qualifying effective start.</param>
        /// <returns>
        ///   <c>true</c> if [has valid source of funding] [the specified devolved postcodes]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasValidSourceOfFunding(
            IReadOnlyCollection<IDevolvedPostcode> devolvedPCs, 
            Func<IDevolvedPostcode, bool> hasQualifyingEffectiveStart) =>
                devolvedPCs.SafeAny(hasQualifyingEffectiveStart);

        /// <summary>
        /// Determines whether [has qualifying effective start] [the specified devolved postcode].
        /// </summary>
        /// <param name="devolvedPC">The devolved pc.</param>
        /// <param name="theDelivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [has qualifying effective start] [the specified devolved postcode]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasQualifyingEffectiveStart(IDevolvedPostcode devolvedPC, ILearningDelivery theDelivery) =>
            theDelivery.LearnStartDate >= devolvedPC.EffectiveFrom;

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
        /// <returns>a collection of message parameters</returns>
        public IEnumerable<IErrorMessageParameter> BuildMessageParametersFor(ILearningDelivery theDelivery) => new[]
        {
            BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, theDelivery.LearnStartDate),
            BuildErrorMessageParameter(PropertyNameConstants.FundModel, theDelivery.FundModel),
            BuildErrorMessageParameter(PropertyNameConstants.LSDPostcode, theDelivery.LSDPostcode),
            BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, Monitoring.Delivery.Types.SourceOfFunding)
        };
    }
}
