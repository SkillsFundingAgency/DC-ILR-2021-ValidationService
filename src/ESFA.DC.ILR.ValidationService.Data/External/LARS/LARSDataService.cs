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

        private readonly IReadOnlyDictionary<string, Model.LearningDelivery> _larsDeliveries;

        private readonly IReadOnlyDictionary<int, ILARSStandard> _larsStandards;

        public LARSDataService(IExternalDataCache externalDataCache)
        {
            It.IsNull(externalDataCache)
                .AsGuard<ArgumentNullException>(nameof(externalDataCache));

            _externalDataCache = externalDataCache;

            // de-sensitise the lars deliveries
            _larsDeliveries = externalDataCache.LearningDeliveries.ToCaseInsensitiveDictionary();

            if (externalDataCache.Standards != null)
            {
                _larsStandards = externalDataCache.Standards.ToDictionary(k => k.StandardCode, v => v);
            }
        }

        public ILARSLearningDelivery GetDeliveryFor(string thisAimRef) =>
            _larsDeliveries.GetValueOrDefault(thisAimRef);

        public ILARSStandard GetStandardFor(int standardCode) =>
            _larsStandards.GetValueOrDefault(standardCode);

        public ILARSStandardFunding GetStandardFundingFor(int standardCode, DateTime startDate)
        {
            var standard = GetStandardFor(standardCode);

            return standard?.StandardsFunding
                .NullSafeWhere(sf => It.IsBetween(startDate, sf.EffectiveFrom, sf.EffectiveTo ?? DateTime.MaxValue))
                .OrderBy(x => x.EffectiveTo) // get the earliest closure first
                .FirstOrDefault();
        }

        public IReadOnlyCollection<ILARSLearningCategory> GetCategoriesFor(string thisAimRef)
        {
            return GetDeliveryFor(thisAimRef)?.Categories ?? Array.Empty<ILARSLearningCategory>();
        }

        public IReadOnlyCollection<ILARSLearningDeliveryValidity> GetValiditiesFor(string thisAimRef)
        {
            return GetDeliveryFor(thisAimRef)?.Validities ?? Array.Empty<ILARSLearningDeliveryValidity>();
        }

        public IReadOnlyCollection<ILARSAnnualValue> GetAnnualValuesFor(string thisAimRef)
        {
            return GetDeliveryFor(thisAimRef)?.AnnualValues ?? Array.Empty<ILARSAnnualValue>();
        }

        public IReadOnlyCollection<ILARSFrameworkAim> GetFrameworkAimsFor(string thisAimRef)
        {
            var delivery = GetDeliveryFor(thisAimRef);

            return delivery?.Frameworks
                .NullSafeWhere(f => f.FrameworkAim != null)
                .Select(f => f.FrameworkAim)
                .ToArray() ?? Array.Empty<ILARSFrameworkAim>();
        }

        public IReadOnlyCollection<ILARSStandardValidity> GetStandardValiditiesFor(int thisStandardCode)
        {
            return _externalDataCache.StandardValidities
                .NullSafeWhere(x => x.StandardCode == thisStandardCode)
                .ToArray() ?? Array.Empty<ILARSStandardValidity>();
        }

        public bool ContainsStandardFor(int thisStandardCode)
        {
            return _externalDataCache.Standards
                .Any(x => x.StandardCode == thisStandardCode);
        }

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
                   && learningDelivery.Frameworks
                        .Any(f => f.ProgType == progType
                            && f.FworkCode == fworkCode
                            && f.PwayCode == pwayCode
                            && f.FrameworkCommonComponents.Any(cc => cc.CommonComponent == learningDelivery.FrameworkCommonComponent));
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
                && levels.Any(x => x.CaseInsensitiveEquals(learningDelivery.NotionalNVQLevelv2));
        }

        // TODO: this should happen in the rule
        public bool FullLevel2EntitlementCategoryMatchForLearnAimRef(string learnAimRef, int level)
        {
            var values = GetAnnualValuesFor(learnAimRef);

            return values.Any(av => av.FullLevel2EntitlementCategory == level);
        }

        public bool FullLevel2PercentForLearnAimRefAndDateAndPercentValue(string learnAimRef, DateTime effectiveFromDate, decimal percentValue)
        {
            var values = GetAnnualValuesFor(learnAimRef);

            return values.Any(av => av.FullLevel2Percent == percentValue && av.IsCurrent(effectiveFromDate));
        }

        public bool FullLevel2PercentForLearnAimRefNotMatchPercentValue(string learnAimRef, DateTime effectiveFromDate, decimal percentValue)
        {
            var values = GetAnnualValuesFor(learnAimRef);

            return values.Any(av => (!av.FullLevel2Percent.HasValue || av.FullLevel2Percent.Value != percentValue) && av.IsCurrent(effectiveFromDate));
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
        public bool BasicSkillsMatchForLearnAimRefAndStartDate(IEnumerable<int> basicSkillsTypes, string learnAimRef, DateTime learnStartDate)
        {
            if (basicSkillsTypes == null || string.IsNullOrEmpty(learnAimRef))
            {
                return false;
            }

            var values = GetAnnualValuesFor(learnAimRef);
            return values.NullSafeAny(a => a.BasicSkillsType.HasValue
                        && basicSkillsTypes.Contains((int)a.BasicSkillsType)
                        && a.IsCurrent(learnStartDate));
        }

        // TODO: this should happen in the rule
        public bool LearnStartDateGreaterThanFrameworkEffectiveTo(string learnAimRef, DateTime learnStartDate, int? progType, int? fworkCode, int? pwayCode)
        {
            var learningDelivery = GetDeliveryFor(learnAimRef);

            return learningDelivery != null
                   && learningDelivery.Frameworks
                        .Any(f => f.ProgType == progType
                            && f.FworkCode == fworkCode
                            && f.PwayCode == pwayCode
                            && learnStartDate > f.EffectiveTo);
        }

        // TODO: this should happen in the rule
        public bool OrigLearnStartDateBetweenStartAndEndDateForValidityCategory(DateTime origLearnStartDate, string learnAimRef, string validityCategory)
        {
            var validities = GetValiditiesFor(learnAimRef);

            return validities.Any(
                lv => lv.ValidityCategory.CaseInsensitiveEquals(validityCategory)
                && lv.IsCurrent(origLearnStartDate));
        }

        // TODO: this should happen in the rule
        public bool OrigLearnStartDateBetweenStartAndEndDateForAnyValidityCategory(DateTime origLearnStartDate, string learnAimRef, IEnumerable<string> categoriesHashSet)
        {
            var validities = GetValiditiesFor(learnAimRef);

            return validities.Any(lv =>
                categoriesHashSet.Any(x => x.CaseInsensitiveEquals(lv.ValidityCategory))
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
                && learnAimRefTypes.Any(x => x.CaseInsensitiveEquals(learningDelivery.LearnAimRefType));
        }
    }
}
