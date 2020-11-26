using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnAimRef
{
    public class LearnAimRef_88Rule : AbstractRule, IRule<ILearner>
    {
        private readonly ILARSDataService _larsDataService;
        private readonly IDerivedData_ValidityCategory_01 _ddValidityCategory;
        private readonly IDerivedData_CategoryRef_01 _ddCategoryRef;

        public LearnAimRef_88Rule(
            ILARSDataService larsDataService,
            IDerivedData_ValidityCategory_01 ddValidityCategory,
            IDerivedData_CategoryRef_01 ddCategoryRef,
            IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.LearnAimRef_88)
        {
            _larsDataService = larsDataService;
            _ddValidityCategory = ddValidityCategory;
            _ddCategoryRef = ddCategoryRef;
        }

        public void Validate(ILearner objectToValidate)
        {
            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(learningDelivery, objectToValidate.LearnerEmploymentStatuses))
                {
                    HandleValidationError(objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber, BuildErrorMessageParameters(learningDelivery.LearnStartDate, learningDelivery.LearnAimRef));
                }
            }
        }

        public bool ConditionMet(ILearningDelivery learningDelivery, IReadOnlyCollection<ILearnerEmploymentStatus> learnerEmploymentStatuses)
        {
            var validityCategory = _ddValidityCategory.Derive(learningDelivery, learnerEmploymentStatuses);
            var categoryRef = _ddCategoryRef.Derive(learningDelivery);

            var validityCheck = validityCategory != null ?
                LarsValidityConditionMet(validityCategory, learningDelivery.LearnAimRef, learningDelivery.LearnStartDate) :
                false;

            var categoryCheck = categoryRef != null ?
               LarsCategoryConditionMet(categoryRef.Value, learningDelivery.LearnAimRef, learningDelivery.LearnStartDate) :
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

        public bool LarsCategoryConditionMet(int categoryRef, string learnAimRef, DateTime learnStartDate)
        {
            var larsCategories = _larsDataService?
                .GetCategoriesFor(learnAimRef)
                .Where(x => x.CategoryRef == categoryRef) ?? Enumerable.Empty<ILARSLearningCategory>();

            return !larsCategories.Any() || larsCategories.Any(l => learnStartDate < l.StartDate || learnStartDate > (l.EndDate ?? DateTime.MaxValue));
        }

        public bool LarsValidityConditionMet(string category, string learnAimRef, DateTime learnStartDate)
        {
            var larsValidity = _larsDataService?.GetValiditiesFor(learnAimRef)
                .Where(v => v.ValidityCategory.CaseInsensitiveEquals(category)) ?? Enumerable.Empty<ILARSLearningDeliveryValidity>();

            return !larsValidity.Any() || larsValidity.Any(l => learnStartDate < l.StartDate || learnStartDate > l.LastNewStartDate);
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
