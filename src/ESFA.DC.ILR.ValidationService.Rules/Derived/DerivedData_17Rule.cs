using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Derived
{
    public class DerivedData_17Rule : IDerivedData_17Rule
    {
        private readonly IDateTimeQueryService _dateTimeQueryService;
        private readonly ILARSDataService _larsData;
        private readonly ILearningDeliveryAppFinRecordQueryService _appFinRecordData;

        public DerivedData_17Rule(
            IDateTimeQueryService dateTimeQueryService,
            ILARSDataService larsDataService,
            ILearningDeliveryAppFinRecordQueryService appFinRecordQueryService)
        {
            _dateTimeQueryService = dateTimeQueryService;
            _larsData = larsDataService;
            _appFinRecordData = appFinRecordQueryService;
        }

        public bool IsTNPMoreThanContributionCapFor(int theStandard, IReadOnlyCollection<ILearningDelivery> theDeliveries)
        {
            var filtered = theDeliveries
                .NullSafeWhere(x => IsQualifyingItem(x, theStandard))
                .ToReadOnlyCollection();

            return filtered.Any() ? RunCheck(filtered, theStandard) : false;
        }

        public bool RunCheck(IReadOnlyCollection<ILearningDelivery> theDeliveries, int theStandard)
        {
            var afinTotal = GetTotalTNPPriceFor(theDeliveries);
            var fundingCap = GetFundingContributionCapFor(theStandard, theDeliveries);

            return HasExceededCappedThreshold(afinTotal, fundingCap);
        }

        public bool IsQualifyingItem(ILearningDelivery theDelivery, int stdCode) =>
            HasQualifyingStdCode(theDelivery, stdCode)
            && IsProgrameAim(theDelivery)
            && IsStandardApprenticeship(theDelivery)
            && HasQualifyingModel(theDelivery);

        public bool HasQualifyingStdCode(ILearningDelivery theDelivery, int stdCode) =>
            theDelivery.StdCodeNullable == stdCode;

        public bool IsProgrameAim(ILearningDelivery theDelivery) =>
            theDelivery.AimType == AimTypes.ProgrammeAim;

        public bool IsStandardApprenticeship(ILearningDelivery theDelivery) =>
            theDelivery.ProgTypeNullable == ProgTypes.ApprenticeshipStandard;

        public bool HasQualifyingModel(ILearningDelivery theDelivery) =>
            theDelivery.FundModel == FundModels.OtherAdult;

        public int GetTotalTNPPriceFor(IReadOnlyCollection<ILearningDelivery> theDeliveries) =>
            _appFinRecordData.GetTotalTNPPriceForLatestAppFinRecordsForLearning(theDeliveries);

        public decimal? GetFundingContributionCapFor(int theStandard, IReadOnlyCollection<ILearningDelivery> theDeliveries)
        {
            var applicableDate = GetEarliestDateForCapChecking(theDeliveries);
            var standardFunding = GetStandardFundingFor(theStandard, applicableDate);
            return standardFunding?.CoreGovContributionCap;
        }

        public DateTime GetEarliestDateForCapChecking(IReadOnlyCollection<ILearningDelivery> theDeliveries) =>
            theDeliveries
                .Select(x => x.OrigLearnStartDateNullable < x.LearnStartDate ? x.OrigLearnStartDateNullable.Value : x.LearnStartDate)
                .OrderBy(x => x)
                .FirstOrDefault();

        public bool HasExceededCappedThreshold(int tnpTotal, decimal? fundingCap) =>
            (tnpTotal / 3 * 2) > fundingCap;

        public ILARSStandardFunding GetStandardFundingFor(int standardCode, DateTime startDate)
        {
            var standard = _larsData.GetStandardFor(standardCode);

            return standard?.StandardsFunding
                .NullSafeWhere(sf => _dateTimeQueryService.IsDateBetween(startDate, sf.EffectiveFrom, sf.EffectiveTo ?? DateTime.MaxValue))
                .OrderBy(x => x.EffectiveTo) // get the earliest closure first
                .FirstOrDefault();
        }
    }
}
