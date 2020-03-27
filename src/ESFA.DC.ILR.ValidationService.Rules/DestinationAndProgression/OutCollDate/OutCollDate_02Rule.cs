using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Internal.AcademicYear.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.DestinationAndProgression.OutCollDate
{
    public class OutCollDate_02Rule : AbstractRule, IRule<ILearnerDestinationAndProgression>
    {
        private readonly IAcademicYearDataService _academicYearDataService;
        private readonly IDateTimeQueryService _dateTimeQueryService;

        public OutCollDate_02Rule(
            IAcademicYearDataService academicYearDataService,
            IDateTimeQueryService dateTimeQueryService,
            IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.OutCollDate_02)
        {
            _academicYearDataService = academicYearDataService;
            _dateTimeQueryService = dateTimeQueryService;
        }

        public void Validate(ILearnerDestinationAndProgression objectToValidate)
        {
            if (objectToValidate?.DPOutcomes == null)
            {
                return;
            }

            var academicStartMinus10Years = _dateTimeQueryService.AddYearsToDate(_academicYearDataService.Start(), -10);

            foreach (var dpOutcome in objectToValidate.DPOutcomes)
            {
                if (ConditionMet(dpOutcome.OutCollDate, academicStartMinus10Years))
                {
                    HandleValidationError(
                        objectToValidate.LearnRefNumber,
                        errorMessageParameters: BuildErrorMessageParameters(dpOutcome.OutCollDate));
                }
            }
        }

        private bool ConditionMet(DateTime outCollDate, DateTime academicStartMinus10Years)
        {
            return outCollDate < academicStartMinus10Years;
        }

        private IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(DateTime outCollDate)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.OutCollDate, outCollDate),
            };
        }
    }
}
