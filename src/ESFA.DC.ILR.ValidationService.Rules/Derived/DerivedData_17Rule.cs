using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ESFA.DC.ILR.ValidationService.Rules.Derived
{
    /// <summary>
    /// derived data rule 17
    /// </summary>
    /// <seealso cref="IDerivedData_17Rule" />
    public class DerivedData_17Rule :
        IDerivedData_17Rule
    {
        /// <summary>
        /// The lars data (service)
        /// </summary>
        private readonly ILARSDataService _larsData;

        /// <summary>
        /// The check(er, common rule operations provider)
        /// </summary>
        private readonly IProvideRuleCommonOperations _check;

        /// <summary>
        /// The apprenticeship financial record data (service)
        /// </summary>
        private readonly ILearningDeliveryAppFinRecordQueryService _appFinRecordData;

        public DerivedData_17Rule(
            ILARSDataService larsDataService,
            IProvideRuleCommonOperations commonOps,
            ILearningDeliveryAppFinRecordQueryService appFinRecordQueryService)
        {
            It.IsNull(larsDataService)
                .AsGuard<ArgumentNullException>(nameof(larsDataService));
            It.IsNull(commonOps)
                .AsGuard<ArgumentNullException>(nameof(commonOps));
            It.IsNull(appFinRecordQueryService)
                .AsGuard<ArgumentNullException>(nameof(appFinRecordQueryService));

            _larsData = larsDataService;
            _check = commonOps;
            _appFinRecordData = appFinRecordQueryService;
        }

        /// <summary>
        /// Determines whether [is TNP more than contribution cap for] [the specified standard].
        /// </summary>
        /// <param name="theStandard">The standard.</param>
        /// <param name="theDeliveries">The deliveries.</param>
        /// <returns>
        /// <c>true</c> if [is TNP more than contribution cap for] [the specified standard]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsTNPMoreThanContributionCapFor(int theStandard, IReadOnlyCollection<ILearningDelivery> theDeliveries)
        {
            var filtered = theDeliveries
                .SafeWhere(x => IsQualifyingItem(x, theStandard))
                .ToReadOnlyCollection();

            return filtered.Any() ? RunCheck(filtered, theStandard) : false;
        }

        /// <summary>
        /// Runs the check.
        /// </summary>
        /// <param name="theDeliveries">The deliveries.</param>
        /// <param name="theStandard">The standard.</param>
        /// <returns>returns true if has exceeded the capped threshold</returns>
        public bool RunCheck(IReadOnlyCollection<ILearningDelivery> theDeliveries, int theStandard)
        {
            var afinTotal = GetTotalTNPPriceFor(theDeliveries);
            var fundingCap = GetFundingContributionCapFor(theStandard, theDeliveries);

            return HasExceededCappedThreshold(afinTotal, fundingCap);
        }

        /// <summary>
        /// Determines whether [is qualifying item] [the specified delivery].
        /// </summary>
        /// <param name="theDelivery">The delivery.</param>
        /// <param name="stdCode">The standard code.</param>
        /// <returns>
        ///   <c>true</c> if [is qualifying item] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsQualifyingItem(ILearningDelivery theDelivery, int stdCode) =>
            HasQualifyingStdCode(theDelivery, stdCode)
            && IsProgrameAim(theDelivery)
            && IsStandardApprenticeship(theDelivery)
            && HasQualifyingModel(theDelivery);

        /// <summary>
        /// Determines whether [has qualifying std code] [the specified delivery].
        /// </summary>
        /// <param name="theDelivery">The delivery.</param>
        /// <param name="stdCode">The standard code.</param>
        /// <returns>
        ///   <c>true</c> if [has qualifying std code] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasQualifyingStdCode(ILearningDelivery theDelivery, int stdCode) =>
            theDelivery.StdCodeNullable == stdCode;

        /// <summary>
        /// Determines whether [is programe aim] [the specified delivery].
        /// </summary>
        /// <param name="theDelivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [is programe aim] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsProgrameAim(ILearningDelivery theDelivery) =>
            _check.InAProgramme(theDelivery);

        /// <summary>
        /// Determines whether [is standard apprenticeship] [the specified delivery].
        /// </summary>
        /// <param name="theDelivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [is standard apprenticeship] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsStandardApprenticeship(ILearningDelivery theDelivery) =>
            _check.IsStandardApprenticeship(theDelivery);

        /// <summary>
        /// Determines whether [has qualifying model] [the specified delivery].
        /// </summary>
        /// <param name="theDelivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [has qualifying model] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasQualifyingModel(ILearningDelivery theDelivery) =>
            _check.HasQualifyingFunding(theDelivery, TypeOfFunding.OtherAdult);

        /// <summary>
        /// Gets the total TNP price for.
        /// </summary>
        /// <param name="theDeliveries">The deliveries.</param>
        /// <returns>the total negotiated price</returns>
        public int GetTotalTNPPriceFor(IReadOnlyCollection<ILearningDelivery> theDeliveries) =>
            _appFinRecordData.GetTotalTNPPriceForLatestAppFinRecordsForLearning(theDeliveries);

        /// <summary>
        /// Gets the funding contribution cap for.
        /// </summary>
        /// <param name="theStandard">The standard.</param>
        /// <param name="theDeliveries">The deliveries.</param>
        /// <returns>the lars standard government funding contribution cap</returns>
        public decimal? GetFundingContributionCapFor(int theStandard, IReadOnlyCollection<ILearningDelivery> theDeliveries)
        {
            var applicableDate = GetEarliestDateForCapChecking(theDeliveries);
            var standardFunding = GetStandardFundingFor(theStandard, applicableDate);
            return standardFunding?.CoreGovContributionCap;
        }

        /// <summary>
        /// Gets the earliest date for cap checking.
        /// </summary>
        /// <param name="theDeliveries">The deliveries.</param>
        /// <returns>the earliest applicable date</returns>
        public DateTime GetEarliestDateForCapChecking(IReadOnlyCollection<ILearningDelivery> theDeliveries) =>
            theDeliveries
                .Select(x => x.OrigLearnStartDateNullable < x.LearnStartDate ? x.OrigLearnStartDateNullable.Value : x.LearnStartDate)
                .OrderBy(x => x)
                .FirstOrDefault();

        /// <summary>
        /// Gets the standard funding for.
        /// </summary>
        /// <param name="theStandard">The standard.</param>
        /// <param name="theDate">The date.</param>
        /// <returns>the lars standard funding (or null if not found)</returns>
        public ILARSStandardFunding GetStandardFundingFor(int theStandard, DateTime theDate) =>
            _larsData.GetStandardFundingFor(theStandard, theDate);

        /// <summary>
        /// Determines whether [has exceeded capped threshold] [the specified TNP total].
        /// </summary>
        /// <param name="tnpTotal">The TNP total.</param>
        /// <param name="fundingCap">The funding cap.</param>
        /// <returns>
        ///   <c>true</c> if [has exceeded capped threshold] [the specified TNP total]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasExceededCappedThreshold(int tnpTotal, decimal? fundingCap) =>
            (tnpTotal / 3 * 2) > fundingCap;
    }
}
