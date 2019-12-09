using System;
using System.Collections.Generic;
using System.Globalization;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Internal.AcademicYear.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.OrigLearnStartDate
{
    public class OrigLearnStartDate_01Rule : AbstractRule, IRule<ILearner>
    {
        private const int _yearsToAdd = -10;
        private readonly HashSet<int> _fundModels = new HashSet<int> { 35, 36, 81, 99 };

        private readonly IDateTimeQueryService _dateTimeQueryService;

        public OrigLearnStartDate_01Rule(IValidationErrorHandler validationErrorHandler, IDateTimeQueryService dateTimeQueryService)
            : base(validationErrorHandler, RuleNameConstants.OrigLearnStartDate_01)
        {
            _dateTimeQueryService = dateTimeQueryService;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate?.LearningDeliveries == null)
            {
                return;
            }

            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(
                    learningDelivery.LearnStartDate,
                    learningDelivery.OrigLearnStartDateNullable,
                    learningDelivery.FundModel))
                {
                    HandleValidationError(
                        objectToValidate.LearnRefNumber,
                        learningDelivery.AimSeqNumber,
                        BuildErrorMessageParameters(learningDelivery.LearnStartDate, learningDelivery.OrigLearnStartDateNullable));
                }
            }
        }

        public bool ConditionMet(DateTime learnStartDate, DateTime? originalLearnStartDate, int fundModel)
        {
            return FundModelConditionMet(fundModel) &&
                   OriginalLearnStartDateConditionMet(learnStartDate, originalLearnStartDate);
        }

        public bool OriginalLearnStartDateConditionMet(DateTime learnStartDate, DateTime? originalLearnStartDate)
        {
            return originalLearnStartDate.HasValue &&
                   originalLearnStartDate < _dateTimeQueryService.AddYearsToDate(learnStartDate, _yearsToAdd);
        }

        public bool FundModelConditionMet(int fundModel)
        {
            return _fundModels.Contains(fundModel);
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(DateTime learnStartDate, DateTime? originalLearnStartDate)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, learnStartDate),
                BuildErrorMessageParameter(PropertyNameConstants.OrigLearnStartDate, originalLearnStartDate)
            };
        }
    }
}
