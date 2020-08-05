using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Data.File.FileData.Interface;
using ESFA.DC.ILR.ValidationService.Data.Internal.AcademicYear.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnAimRef
{
    public class LearnAimRef_92Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IAcademicYearDataService _academicYearDataService;
        private readonly ILARSDataService _larsDataService;
        private readonly IFileDataService _fileDataService;
        private readonly IDerivedData_ValidityCategory_02 _ddValidityCategory;
        private readonly IDerivedData_CategoryRef_02 _ddCategoryRef;

        public LearnAimRef_92Rule(
            IAcademicYearDataService academicYearDataService,
            ILARSDataService larsDataService,
            IFileDataService fileDataService,
            IDerivedData_ValidityCategory_02 ddValidityCategory,
            IDerivedData_CategoryRef_02 ddCategoryRef,
            IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.LearnAimRef_92)
        {
            _academicYearDataService = academicYearDataService;
            _larsDataService = larsDataService;
            _fileDataService = fileDataService;
            _ddValidityCategory = ddValidityCategory;
            _ddCategoryRef = ddCategoryRef;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate?.LearningDeliveries == null)
            {
                return;
            }

            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (LearnActEndDateCondition(learningDelivery.LearnActEndDateNullable))
                {
                    return;
                }

                if (ConditionMet(learningDelivery, objectToValidate.LearnerEmploymentStatuses))
                {
                    HandleValidationError(objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber, BuildErrorMessageParameters(learningDelivery.LearnStartDate, learningDelivery.LearnAimRef));
                }
            }
        }

        public bool LearnActEndDateCondition(DateTime? learnActEndDate)
        {
            return learnActEndDate == null || learnActEndDate < _academicYearDataService.Start();
        }

        public bool ConditionMet(ILearningDelivery learningDelivery, IReadOnlyCollection<ILearnerEmploymentStatus> learnerEmploymentStatuses)
        {
            var validityCategory = _ddValidityCategory.Derive(learningDelivery, learnerEmploymentStatuses);
            var categoryRef = _ddCategoryRef.Derive(learningDelivery);

            if (validityCategory == null && categoryRef == null)
            {
                return true;
            }

            var validityCheck = validityCategory != null ?
                LarsValidityConditionMet(validityCategory, learningDelivery.LearnAimRef, learningDelivery.LearnActEndDateNullable) :
                false;

            var categoryCheck = categoryRef != null ?
               LarsCategoryConditionMet(categoryRef.Value, learningDelivery.LearnAimRef, learningDelivery.LearnActEndDateNullable) :
               false;

            return TriggerOnValidityCategory(validityCategory, validityCheck) ? TriggerOnCategoryRef(categoryRef, categoryCheck) : false;
        }

        public bool TriggerOnCategoryRef(int? categoryRef, bool categoryCheck)
        {
            return categoryRef == null || (categoryRef != null && categoryCheck) ? true : false;
        }

        public bool TriggerOnValidityCategory(string validityCategory, bool validityCheck)
        {
            return validityCategory != null && validityCheck;
        }

        public bool LarsValidityConditionMet(string category, string learnAimRef, DateTime? learnStartDate)
        {
            var larsValidity = _larsDataService?.GetValiditiesFor(learnAimRef)
                .Where(v => v.ValidityCategory.CaseInsensitiveEquals(category)) ?? Enumerable.Empty<ILARSLearningDeliveryValidity>();

            return !larsValidity.Any() || (learnStartDate == null && larsValidity.Any(l => _fileDataService.FilePreparationDate() > l.EndDate));
        }

        public bool LarsCategoryConditionMet(int categoryRef, string learnAimRef, DateTime? learnActEndDate)
        {
            var larsCategories = _larsDataService?
               .GetCategoriesFor(learnAimRef)
               .Where(x => x.CategoryRef == categoryRef) ?? Enumerable.Empty<ILARSLearningCategory>();

            return !larsCategories.Any() || (learnActEndDate == null && larsCategories.Any(l => _fileDataService.FilePreparationDate() > l.EndDate));
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(DateTime learnStartDate, string learnAimRef)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, learnStartDate),
                BuildErrorMessageParameter(PropertyNameConstants.LearnAimRef, learnAimRef)
            };
        }
    }
}
