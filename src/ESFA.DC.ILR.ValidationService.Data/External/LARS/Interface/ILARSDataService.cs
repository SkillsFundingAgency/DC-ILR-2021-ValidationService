using System;
using System.Collections.Generic;
using ESFA.DC.ILR.ValidationService.Interface;

namespace ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface
{
    public interface ILARSDataService : IDataService
    {
        ILARSLearningDelivery GetDeliveryFor(string thisAimRef);

        IReadOnlyCollection<ILARSLearningCategory> GetCategoriesFor(string thisAimRef);

        IReadOnlyCollection<ILARSLearningDeliveryValidity> GetValiditiesFor(string thisAimRef);

        IReadOnlyCollection<ILARSAnnualValue> GetAnnualValuesFor(string thisAimRef);

        IReadOnlyCollection<ILARSFrameworkAim> GetFrameworkAimsFor(string thisAimRef);

        IReadOnlyCollection<ILARSStandardValidity> GetStandardValiditiesFor(int thisStandardCode);

        ILARSStandard GetStandardFor(int standardCode);

        bool ContainsStandardFor(int thisStandardCode);

        bool HasKnownLearnDirectClassSystemCode3For(string thisLearnAimRef);

        string GetNotionalNVQLevelv2ForLearnAimRef(string learnAimRef);

        bool EffectiveDatesValidforLearnAimRef(string learnAimRef, DateTime date);

        bool EnglishPrescribedIdsExistsforLearnAimRef(string learnAimRef, HashSet<int?> engPrscIDs);

        bool FrameworkCodeExistsForFrameworkAims(string learnAimRef, int? progType, int? fworkCode, int? pwayCode);

        bool FrameWorkComponentTypeExistsInFrameworkAims(string learnAimRef, HashSet<int?> frameworkTypeComponents);

        bool FrameworkCodeExistsForFrameworkAimsAndFrameworkComponentTypes(string learnAimRef, int? progType, int? fworkCode, int? pwayCode, HashSet<int?> frameworkTypeComponents, DateTime startDate);

        bool FrameworkCodeExistsForCommonComponent(string learnAimRef, int? progType, int? fworkCode, int? pwayCode);

        bool LearnAimRefExists(string learnAimRef);

        bool LearnAimRefExistsForLearningDeliveryCategoryRef(string learnAimRef, int categoryRef);

        bool NotionalNVQLevelMatchForLearnAimRef(string learnAimRef, string level);

        bool NotionalNVQLevelV2MatchForLearnAimRefAndLevel(string learnAimRef, string level);

        bool NotionalNVQLevelV2MatchForLearnAimRefAndLevels(string learnAimRef, IEnumerable<string> levels);

        bool FullLevel2EntitlementCategoryMatchForLearnAimRef(string learnAimRef, int level);

        bool FullLevel2PercentForLearnAimRefAndDateAndPercentValue(string learnAimRef, DateTime effectiveFromDate, decimal percentValue);

        bool FullLevel2PercentForLearnAimRefNotMatchPercentValue(string learnAimRef, DateTime effectiveFromDate, decimal percentValue);

        bool FullLevel3EntitlementCategoryMatchForLearnAimRef(string learnAimRef, int level);

        bool FullLevel3PercentForLearnAimRefAndDateAndPercentValue(string learnAimRef, DateTime learnStartDate, decimal percentValue);

        bool LearnDirectClassSystemCode1MatchForLearnAimRef(string learnAimRef);

        bool LearnDirectClassSystemCode2MatchForLearnAimRef(string learnAimRef);

        bool BasicSkillsMatchForLearnAimRef(string learnAimRef, int basicSkills);

        bool BasicSkillsMatchForLearnAimRefAndStartDate(IEnumerable<int> basicSkillsType, string learnAimRef, DateTime learnStartDate);

        bool BasicSkillsTypeMatchForLearnAimRef(IEnumerable<int> basicSkillsTypes, string learnAimRef);

        bool LearnStartDateGreaterThanFrameworkEffectiveTo(string learnAimRef, DateTime learnStartDate, int? progType, int? fWorkCode, int? pwayCode);

        bool OrigLearnStartDateBetweenStartAndEndDateForValidityCategory(DateTime origLearnStartDate, string learnAimRef, string validityCategory);

        bool LearnStartDateGreaterThanStandardsEffectiveTo(int stdCode, DateTime learnStartDate);

        bool HasAnyLearningDeliveryForLearnAimRefAndTypes(string learnAimRef, IEnumerable<string> types);

        bool IsCurrentAndNotWithdrawn(ISupportFundingWithdrawal source, DateTime candidate, DateTime? optionalEnding = null);

        bool OrigLearnStartDateBetweenStartAndEndDateForAnyValidityCategory(
            DateTime origLearnStartDate,
            string learnAimRef,
            IEnumerable<string> categoriesHashSet);
    }
}
