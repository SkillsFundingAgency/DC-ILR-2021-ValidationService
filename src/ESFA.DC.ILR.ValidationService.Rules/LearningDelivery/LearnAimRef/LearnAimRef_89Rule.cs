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
        private readonly IDerivedData_ValidityCategory _ddValidityCategory;
        private readonly IAcademicYearDataService _academicYearDataService;

        public LearnAimRef_89Rule(
            ILARSDataService larsDataService,
            IDerivedData_ValidityCategory ddValidityCategory,
            IAcademicYearDataService academicYearDataService,
            IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.LearnAimRef_89)
        {
            _larsDataService = larsDataService;
            _ddValidityCategory = ddValidityCategory;
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
            var category = _ddValidityCategory.Derive(learningDelivery, learnerEmploymentStatuses);

            if (category == null)
            {
                return false;
            }

            return LarsConditionMet(category, learningDelivery.LearnAimRef, previousYearEnd);
        }

        public bool LarsConditionMet(string category, string learnAimRef, DateTime previousYearEnd)
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
