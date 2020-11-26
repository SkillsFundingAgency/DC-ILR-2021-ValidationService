using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Data.Internal.AcademicYear.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnAimRef
{
    public class LearnAimRef_89Rule : AbstractRule, IRule<ILearner>
    {
        private readonly ILARSDataService _larsDataService;
        private readonly IDerivedData_ValidityCategory_01 _ddValidityCategory;
        private readonly IDerivedData_CategoryRef_01 _ddCategoryRef;
        private readonly IAcademicYearDataService _academicYearDataService;

        public LearnAimRef_89Rule(
            ILARSDataService larsDataService,
            IDerivedData_ValidityCategory_01 ddValidityCategory,
            IDerivedData_CategoryRef_01 ddCategoryRef,
            IAcademicYearDataService academicYearDataService,
            IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.LearnAimRef_89)
        {
            _larsDataService = larsDataService;
            _ddValidityCategory = ddValidityCategory;
            _ddCategoryRef = ddCategoryRef;
            _academicYearDataService = academicYearDataService;
        }

        public void Validate(ILearner objectToValidate)
        {
            var previousYearEnd = _academicYearDataService.PreviousYearEnd();

            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(learningDelivery, objectToValidate.LearnerEmploymentStatuses, previousYearEnd))
                {
                    HandleValidationError(objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber, BuildErrorMessageParameters(learningDelivery.LearnAimRef));
                }
            }
        }

        public bool ConditionMet(ILearningDelivery learningDelivery, IReadOnlyCollection<ILearnerEmploymentStatus> learnerEmploymentStatuses, DateTime previousYearEnd)
        {
            if (CategoryRefConditionMet(learningDelivery))
            {
                return false;
            }

            var validityCategory = _ddValidityCategory.Derive(learningDelivery, learnerEmploymentStatuses);

            if (validityCategory == null)
            {
                return false;
            }

            return LarsValidityCategoryConditionMet(validityCategory, learningDelivery.LearnAimRef, previousYearEnd);
        }

        public bool CategoryRefConditionMet(ILearningDelivery learningDelivery)
        {
            var ddLookup = _ddCategoryRef.Derive(learningDelivery);
            var larsCategoryRefs = _larsDataService.GetCategoriesFor(learningDelivery.LearnAimRef);

            return larsCategoryRefs.Any(x => x.CategoryRef == ddLookup);
        }

        public bool LarsValidityCategoryConditionMet(string category, string learnAimRef, DateTime previousYearEnd)
        {
            var larsValidities = _larsDataService?.GetValiditiesFor(learnAimRef)
                .Where(v => v.ValidityCategory.CaseInsensitiveEquals(category)) ?? Enumerable.Empty<ILARSLearningDeliveryValidity>();

            if (!larsValidities.Any())
            {
                return true;
            }

            var latestValidity = larsValidities.OrderByDescending(s => s.StartDate).FirstOrDefault();

            return latestValidity == null ? false : latestValidity.EndDate.HasValue && latestValidity.EndDate <= previousYearEnd;
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(string learnAimRef)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.LearnAimRef, learnAimRef)
            };
        }
    }
}
