using System;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface
{
    /// <summary>
    /// the lars data service
    /// </summary>
    public interface ILARSDataService
    {
        /// <summary>
        /// Gets the deliveries for.
        /// </summary>
        /// <param name="forThisAimRef">this aim reference.</param>
        /// <returns>a collection of lars learning deliveries for this learning aim reference</returns>
        [Obsolete("this method is deprecated; use 'GetDeliveryFor' instead", false)]
        IReadOnlyCollection<ILARSLearningDelivery> GetDeliveriesFor(string forThisAimRef);

        /// <summary>
        /// Gets the (lars) delivery for (this aim reference).
        /// </summary>
        /// <param name="thisAimRef">this aim reference.</param>
        /// <returns>
        /// a lars learning delivery for this learning aim reference
        /// </returns>
        ILARSLearningDelivery GetDeliveryFor(string thisAimRef);

        /// <summary>
        /// Gets a collection of (lars) learning categories for (this aim reference).
        ///  i should never return null
        /// </summary>
        /// <param name="thisAimRef">this aim reference.</param>
        /// <returns>a collection of lars learning categories for this learning aim reference</returns>
        IReadOnlyCollection<ILARSLearningCategory> GetCategoriesFor(string thisAimRef);

        /// <summary>
        /// Gets a collection of (lars) learning delivery periods of validity for (this aim reference).
        ///  i should never return null
        /// </summary>
        /// <param name="thisAimRef">this aim reference.</param>
        /// <returns>a collection of lars learing delivery periods of validity for this learning aim reference</returns>
        IReadOnlyCollection<ILARSLearningDeliveryValidity> GetValiditiesFor(string thisAimRef);

        /// <summary>
        /// Gets the (lars) annual values for (this aim reference).
        ///  i should never return null
        /// </summary>
        /// <param name="thisAimRef">The this aim reference.</param>
        /// <returns>a collection of lars 'annula values' for this learning aim reference</returns>
        IReadOnlyCollection<ILARSAnnualValue> GetAnnualValuesFor(string thisAimRef);

        /// <summary>
        /// Gets the (lars) framework aims for (this aim reference).
        ///  i should never return null
        /// </summary>
        /// <param name="thisAimRef">The this aim reference.</param>
        /// <returns>
        /// a collection of lars 'framework aims' for this learning aim reference
        /// </returns>
        IReadOnlyCollection<ILARSFrameworkAim> GetFrameworkAimsFor(string thisAimRef);

        /// <summary>
        /// Gets the collection of (lars) standard periods of validity for (this standard code).
        ///  i should never return null
        /// </summary>
        /// <param name="thisStandardCode">this standard code.</param>
        /// <returns>a collection of lars standard periods of validity for this standard code</returns>
        IReadOnlyCollection<ILARSStandardValidity> GetStandardValiditiesFor(int thisStandardCode);

        /// <summary>
        /// Contains the (lars) standard for (this standard code).
        /// </summary>
        /// <param name="thisStandardCode">The this standard code.</param>
        /// <returns>
        ///   <c>true</c> if [contains standard for] [the specified this standard code]; otherwise, <c>false</c>.
        /// </returns>
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

        bool FullLevel3EntitlementCategoryMatchForLearnAimRef(string learnAimRef, int level);

        bool FullLevel3PercentForLearnAimRefAndDateAndPercentValue(string learnAimRef, DateTime learnStartDate, decimal percentValue);

        bool LearnDirectClassSystemCode1MatchForLearnAimRef(string learnAimRef);

        bool LearnDirectClassSystemCode2MatchForLearnAimRef(string learnAimRef);

        bool BasicSkillsMatchForLearnAimRef(string learnAimRef, int basicSkills);

        bool BasicSkillsMatchForLearnAimRefAndStartDate(IEnumerable<int> basicSkillsType, string learnAimRef, DateTime learnStartDate);

        bool BasicSkillsTypeMatchForLearnAimRef(IEnumerable<int> basicSkillsTypes, string learnAimRef);

        bool LearnStartDateGreaterThanFrameworkEffectiveTo(DateTime learnStartDate, int? progType, int? fWorkCode, int? pwayCode);

        bool OrigLearnStartDateBetweenStartAndEndDateForValidityCategory(DateTime origLearnStartDate, string learnAimRef, string validityCategory);

        bool LearnStartDateGreaterThanStandardsEffectiveTo(int stdCode, DateTime learnStartDate);

        bool HasAnyLearningDeliveryForLearnAimRefAndTypes(string learnAimRef, IEnumerable<string> types);

        bool OrigLearnStartDateBetweenStartAndEndDateForAnyValidityCategory(
            DateTime origLearnStartDate,
            string learnAimRef,
            IEnumerable<string> categoriesHashSet);

        ILARSStandardFunding GetStandardFundingForCodeOnDate(int standardCode, DateTime startDate);
    }
}
