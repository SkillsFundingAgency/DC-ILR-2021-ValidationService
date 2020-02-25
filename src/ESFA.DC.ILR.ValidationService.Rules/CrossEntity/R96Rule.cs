using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.CrossEntity
{
    public class R96Rule : AbstractRule, IRule<ILearner>
    {
        public R96Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.R96)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate?
                .LearningDeliveries == null)
            {
                return;
            }

            var workPlaceStartDates = objectToValidate
                .LearningDeliveries
                .Where(w => w.LearningDeliveryWorkPlacements != null)?
                .SelectMany(w => w.LearningDeliveryWorkPlacements)?.ToList()
                .GroupBy(w => w.WorkPlaceStartDate)
                .Where(w => w.Count() > 1)?
                .Select(g => g.Key);

            if ((workPlaceStartDates?.Count() ?? 0) == 0)
            {
                return;
            }

            foreach (var workPlaceStartDate in workPlaceStartDates)
            {
                HandleValidationError(
                    learnRefNumber: objectToValidate.LearnRefNumber,
                    errorMessageParameters: BuildErrorMessageParameters(workPlaceStartDate));
            }
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(DateTime workPlaceStartDate)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.WorkPlaceStartDate, workPlaceStartDate)
            };
        }
    }
}
