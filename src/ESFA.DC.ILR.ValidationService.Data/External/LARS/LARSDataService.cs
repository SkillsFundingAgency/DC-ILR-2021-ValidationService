using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ESFA.DC.ILR.ValidationService.Data.External.LARS
{
    public class LARSDataService : ILARSDataService
    {
        private readonly IExternalDataCache _externalDataCache;

        /// <summary>
        /// The case de-sensitised lars deliveries
        /// </summary>
        private readonly IReadOnlyDictionary<string, Model.LearningDelivery> _larsDeliveries;

        /// <summary>
        /// Initializes a new instance of the <see cref="LARSDataService"/> class.
        /// </summary>
        /// <param name="externalDataCache">The external data cache.</param>
        public LARSDataService(IExternalDataCache externalDataCache)
        {
            It.IsNull(externalDataCache)
                .AsGuard<ArgumentNullException>(nameof(externalDataCache));

            _externalDataCache = externalDataCache;

            // de-sensitise the lars deliveries
            _larsDeliveries = externalDataCache.LearningDeliveries.ToCaseInsensitiveDictionary();
        }

        /// <summary>
        /// Gets the deliveries for.
        /// this routine has been deprecated on the contract
        /// so don't use it; use 'get delivery for' instead...
        /// </summary>
        /// <param name="thisAimRef">this aim reference.</param>
        /// <returns>a collection of lars learning deliveries for this learning aim reference</returns>
        public IReadOnlyCollection<ILARSLearningDelivery> GetDeliveriesFor(string thisAimRef)
        {
            var delivery = GetDeliveryFor(thisAimRef);

            return It.Has(delivery)
                ? new[] { delivery }
                : Collection.EmptyAndReadOnly<ILARSLearningDelivery>();
        }

        /// <summary>
        /// Gets the (lars) delivery for (this aim reference).
        /// </summary>
        /// <param name="thisAimRef">this aim reference.</param>
        /// <returns>
        /// a lars learning delivery for this learning aim reference
        /// </returns>
        public ILARSLearningDelivery GetDeliveryFor(string thisAimRef)
        {
            _larsDeliveries.TryGetValue(thisAimRef, out var learningDelivery);

            return learningDelivery;
        }

        /// <summary>
        /// Gets a collection of (lars) learning categories for (this aim reference).
        ///  i should never return null
        /// </summary>
        /// <param name="thisAimRef">this aim reference.</param>
        /// <returns>
        /// a collection of lars learning categories for this learning aim reference
        /// </returns>
        public IReadOnlyCollection<ILARSLearningCategory> GetCategoriesFor(string thisAimRef)
        {
            var delivery = GetDeliveryFor(thisAimRef);

            return delivery?.Categories
                ?? Collection.EmptyAndReadOnly<ILARSLearningCategory>();
        }

        /// <summary>
        /// Gets a collection of (lars) learning delivery periods of validity for (this aim reference).
        ///  i should never return null
        /// </summary>
        /// <param name="thisAimRef">this aim reference.</param>
        /// <returns>
        /// a collection of lars learing delivery periods of validity for this learning aim reference
        /// </returns>
        public IReadOnlyCollection<ILARSLearningDeliveryValidity> GetValiditiesFor(string thisAimRef)
        {
            var delivery = GetDeliveryFor(thisAimRef);

            return delivery?.Validities
                ?? Collection.EmptyAndReadOnly<ILARSLearningDeliveryValidity>();
        }

        /// <summary>
        /// Gets the (lars) annual values for (this aim reference).
        ///  i should never return null
        /// </summary>
        /// <param name="thisAimRef">The this aim reference.</param>
        /// <returns>
        /// a collection of lars 'annual values' for this learning aim reference
        /// </returns>
        public IReadOnlyCollection<ILARSAnnualValue> GetAnnualValuesFor(string thisAimRef)
        {
            var delivery = GetDeliveryFor(thisAimRef);

            return delivery?.AnnualValues
                ?? Collection.EmptyAndReadOnly<ILARSAnnualValue>();
        }

        /// <summary>
        /// Gets the (lars) framework aims for (this aim reference).
        /// </summary>
        /// <param name="thisAimRef">The this aim reference.</param>
        /// <returns>
        /// a collection of lars 'framework aims' for this learning aim reference
        /// </returns>
        public IReadOnlyCollection<ILARSFrameworkAim> GetFrameworkAimsFor(string thisAimRef)
        {
            var delivery = GetDeliveryFor(thisAimRef);

            return delivery?.FrameworkAims
                ?? Collection.EmptyAndReadOnly<ILARSFrameworkAim>();
        }

        /// <summary>
        /// Gets the collection of (lars) standard periods of validity for (this standard code).
        ///  i should never return null
        /// </summary>
        /// <param name="thisStandardCode">this standard code.</param>
        /// <returns>
        /// a collection of lars standard periods of validity for this standard code
        /// </returns>
        public IReadOnlyCollection<ILARSStandardValidity> GetStandardValiditiesFor(int thisStandardCode)
        {
            return _externalDataCache.StandardValidities
                .SafeWhere(x => x.StandardCode == thisStandardCode)
                .AsSafeReadOnlyList();
        }

        /// <summary>
        /// Contains the (lars) standard for (this standard code).
        /// </summary>
        /// <param name="thisStandardCode">The this standard code.</param>
        /// <returns>
        ///   <c>true</c> if [contains standard for] [the specified this standard code]; otherwise, <c>false</c>.
        /// </returns>
        public bool ContainsStandardFor(int thisStandardCode)
        {
            return _externalDataCache.Standards
                .Any(x => x.StandardCode == thisStandardCode);
        }

        /// <summary>
        /// Learn aim reference exists (but probably pointless as this should be done in the rule...)
        /// </summary>
        /// <param name="learnAimRef">The learn aim reference.</param>
        /// <returns>true if it does</returns>
        public bool LearnAimRefExists(string learnAimRef)
        {
            return GetDeliveryFor(learnAimRef) != null;
        }

        // TODO: this should happen in the rule
        public string GetNotionalNVQLevelv2ForLearnAimRef(string learnAimRef)
        {
            return GetDeliveryFor(learnAimRef)?.NotionalNVQLevelv2;
        }

        // TODO: this should happen in the rule
        public bool EffectiveDatesValidforLearnAimRef(string learnAimRef, DateTime date)
        {
            var validities = GetValiditiesFor(learnAimRef);

            return validities.Any(x => x.IsCurrent(date));
        }

        // TODO: this should happen in the rule
        public bool EnglishPrescribedIdsExistsforLearnAimRef(string learnAimRef, HashSet<int?> engPrscIDs)
        {
            return engPrscIDs.Contains(GetDeliveryFor(learnAimRef)?.EnglPrscID);
        }

        // TODO: this should happen in the rule
        public bool FrameworkCodeExistsForFrameworkAims(string learnAimRef, int? progType, int? fworkCode, int? pwayCode)
        {
            var frameworkAims = GetFrameworkAimsFor(learnAimRef);

            return frameworkAims.Any(
                fa => fa.ProgType == progType
                && fa.FworkCode == fworkCode
                && fa.PwayCode == pwayCode);
        }

        // TODO: this should happen in the rule
        public bool FrameWorkComponentTypeExistsInFrameworkAims(string learnAimRef, HashSet<int?> frameworkTypeComponents)
        {
            var frameworkAims = GetFrameworkAimsFor(learnAimRef);

            return frameworkAims.Any(fa => frameworkTypeComponents.Contains(fa.FrameworkComponentType));
        }

        public bool FrameworkCodeExistsForFrameworkAimsAndFrameworkComponentTypes(string learnAimRef, int? progType, int? fworkCode, int? pwayCode, HashSet<int?> frameworkTypeComponents, DateTime startDate)
        {
            var frameworkAims = GetFrameworkAimsFor(learnAimRef);

            return frameworkAims.Any(
                fa => fa.ProgType == progType
                      && fa.FworkCode == fworkCode
                      && fa.PwayCode == pwayCode
                      && frameworkTypeComponents.Contains(fa.FrameworkComponentType)
                      && startDate >= fa.StartDate
                      && (!fa.EndDate.HasValue || startDate <= fa.EndDate));
        }

        // TODO: needs to be thought out, this isn't right either...
        public bool FrameworkCodeExistsForCommonComponent(string learnAimRef, int? progType, int? fworkCode, int? pwayCode)
        {
            var learningDelivery = GetDeliveryFor(learnAimRef);

            return learningDelivery != null
                   && _externalDataCache.Frameworks
                        .SafeWhere(f => f.FrameworkCommonComponents != null)
                        .SelectMany(f => f.FrameworkCommonComponents)
                        .Any(fa => fa.ProgType == progType
                            && fa.FworkCode == fworkCode
                            && fa.PwayCode == pwayCode
                            && fa.CommonComponent == learningDelivery.FrameworkCommonComponent);
        }

        // TODO: not descriptive of it's actual operation, and should be done in the rule
        public bool LearnAimRefExistsForLearningDeliveryCategoryRef(string learnAimRef, int categoryRef)
        {
            var categories = GetCategoriesFor(learnAimRef);

            return categories.Any(cr => cr.CategoryRef == categoryRef);
        }

        // TODO: this should happen in the rule
        public bool NotionalNVQLevelMatchForLearnAimRef(string learnAimRef, string level)
        {
            var learningDelivery = GetDeliveryFor(learnAimRef);

            return learningDelivery != null
                && learningDelivery.NotionalNVQLevel == level;
        }

        // TODO: this should happen in the rule
        public bool NotionalNVQLevelV2MatchForLearnAimRefAndLevel(string learnAimRef, string level)
        {
            var learningDelivery = GetDeliveryFor(learnAimRef);

            return learningDelivery != null
                && learningDelivery.NotionalNVQLevelv2 == level;
        }

        // TODO: this should happen in the rule
        public bool NotionalNVQLevelV2MatchForLearnAimRefAndLevels(string learnAimRef, IEnumerable<string> levels)
        {
            var learningDelivery = GetDeliveryFor(learnAimRef);

            return learningDelivery != null
                && levels.AsSafeDistinctKeySet().Contains(learningDelivery.NotionalNVQLevelv2);
        }

        // TODO: this should happen in the rule
        public bool FullLevel2EntitlementCategoryMatchForLearnAimRef(string learnAimRef, int level)
        {
            var values = GetAnnualValuesFor(learnAimRef);

            return values.Any(av => av.FullLevel2EntitlementCategory == level);
        }

        // TODO: this should happen in the rule
        public bool FullLevel3EntitlementCategoryMatchForLearnAimRef(string learnAimRef, int level)
        {
            var values = GetAnnualValuesFor(learnAimRef);

            return values.Any(av => av.FullLevel3EntitlementCategory == level);
        }

        // TODO: this should happen in the rule
        public bool FullLevel3PercentForLearnAimRefAndDateAndPercentValue(string learnAimRef, DateTime learnStartDate, decimal percentValue)
        {
            var values = GetAnnualValuesFor(learnAimRef);

            return values.Any(av => av.FullLevel3Percent == percentValue && av.IsCurrent(learnStartDate));
        }

        // TODO: this should happen in the rule
        public bool HasKnownLearnDirectClassSystemCode3For(string thisLearnAimRef)
        {
            var delivery = GetDeliveryFor(thisLearnAimRef);

            return It.Has(delivery)
                && delivery.LearnDirectClassSystemCode3.IsKnown();
        }

        // TODO: this should happen in the rule
        public bool LearnDirectClassSystemCode1MatchForLearnAimRef(string thisLearnAimRef)
        {
            var delivery = GetDeliveryFor(thisLearnAimRef);

            return It.Has(delivery)
                && delivery.LearnDirectClassSystemCode1.IsKnown();
        }

        // TODO: this should happen in the rule
        public bool LearnDirectClassSystemCode2MatchForLearnAimRef(string thisLearnAimRef)
        {
            var delivery = GetDeliveryFor(thisLearnAimRef);

            return It.Has(delivery)
                && delivery.LearnDirectClassSystemCode2.IsKnown();
        }

        // TODO: this should happen in the rule
        public bool BasicSkillsMatchForLearnAimRef(string learnAimRef, int basicSkills)
        {
            var values = GetAnnualValuesFor(learnAimRef);

            return values.Any(av => av.BasicSkills == basicSkills);
        }

        // TODO: this should happen in the rule
        public bool BasicSkillsTypeMatchForLearnAimRef(IEnumerable<int> basicSkillsTypes, string learnAimRef)
        {
            if (basicSkillsTypes == null || string.IsNullOrEmpty(learnAimRef))
            {
                return false;
            }

            var values = GetAnnualValuesFor(learnAimRef);
            return values.Any(a =>
                a.BasicSkillsType.HasValue && basicSkillsTypes.Contains((int)a.BasicSkillsType));
        }

        // TODO: this should happen in the rule
        public bool BasicSkillsMatchForLearnAimRefAndStartDate(IEnumerable<int> basicSkillsType, string learnAimRef, DateTime learnStartDate)
        {
            var values = GetAnnualValuesFor(learnAimRef);
            var safeSkillsSet = basicSkillsType.AsSafeDistinctKeySet();

            return values.SafeAny(a => a.BasicSkillsType.HasValue
                        && safeSkillsSet.Contains((int)a.BasicSkillsType)
                        && a.IsCurrent(learnStartDate));
        }

        // TODO: this should happen in the rule
        public bool LearnStartDateGreaterThanFrameworkEffectiveTo(DateTime learnStartDate, int? progType, int? fWorkCode, int? pwayCode)
        {
            return _externalDataCache.Frameworks
                .SafeAny(f =>
                    f.ProgType == progType
                    && f.FworkCode == fWorkCode
                    && f.PwayCode == pwayCode

                    // && f.EffectiveTo != null <= not needed with uplifting operators
                    && learnStartDate > f.EffectiveTo);
        }

        // TODO: this should happen in the rule
        public bool OrigLearnStartDateBetweenStartAndEndDateForValidityCategory(DateTime origLearnStartDate, string learnAimRef, string validityCategory)
        {
            return OrigLearnStartDateBetweenStartAndEndDateForAnyValidityCategory(
                origLearnStartDate,
                learnAimRef,
                new List<string>() { validityCategory });
        }

        // TODO: this should happen in the rule
        public bool OrigLearnStartDateBetweenStartAndEndDateForAnyValidityCategory(DateTime origLearnStartDate, string learnAimRef, IEnumerable<string> categoriesHashSet)
        {
            var validities = GetValiditiesFor(learnAimRef);
            var caseInsensitveCategoriesHashSet = categoriesHashSet.ToCaseInsensitiveHashSet();

            return validities.Any(lv =>
                caseInsensitveCategoriesHashSet.Contains(lv.ValidityCategory)
                && lv.IsCurrent(origLearnStartDate));
        }

        // TODO: this should happen in the rule, but may require an accessor => GetStandard(s)For(int thisStandardCode)
        // and what is the point of sending in a 'key' that might null?!?
        public bool LearnStartDateGreaterThanStandardsEffectiveTo(int stdCode, DateTime learnStartDate)
        {
            var validities = GetStandardValiditiesFor(stdCode);

            return validities.Any(s => s.IsCurrent(learnStartDate));
        }

        // TODO: this should happen in the rule
        public bool HasAnyLearningDeliveryForLearnAimRefAndTypes(string learnAimRef, IEnumerable<string> learnAimRefTypes)
        {
            var learningDelivery = GetDeliveryFor(learnAimRef);

            return It.Has(learningDelivery)
                && learnAimRefTypes.ToCaseInsensitiveHashSet().Contains(learningDelivery.LearnAimRefType);
        }

        public ILARSStandardFunding GetStandardFundingForCodeOnDate(int standardCode, DateTime startDate)
        {
            var standard = _externalDataCache.Standards?.FirstOrDefault(x => x.StandardCode == standardCode);

            if (standard?.StandardsFunding != null)
            {
                return standard.StandardsFunding.FirstOrDefault(sf => startDate >= sf.EffectiveFrom &&
                                                               (!sf.EffectiveTo.HasValue ||
                                                                startDate <= sf.EffectiveTo));
            }

            return null;
        }
    }
}
