using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Internal.AcademicYear.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.DestinationAndProgression.OutStartDate
{
    public class OutStartDate_01Rule : AbstractRule, IRule<ILearnerDestinationAndProgression>
    {
        private readonly IAcademicYearDataService _academicYearDataService;
        private readonly IDateTimeQueryService _dateTimeQueryService;

        public OutStartDate_01Rule(
            IAcademicYearDataService academicYearDataService,
            IDateTimeQueryService dateTimeQueryService,
            IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.OutStartDate_01)
        {
            _academicYearDataService = academicYearDataService;
            _dateTimeQueryService = dateTimeQueryService;
        }

        public void Validate(ILearnerDestinationAndProgression objectToValidate)
        {
            if (objectToValidate?.DPOutcomes != null)
            {
                var academicStartMinus10Years = _dateTimeQueryService.AddYearsToDate(_academicYearDataService.Start(), -10);

                foreach (var dpOutcome in objectToValidate.DPOutcomes)
                {
                    if (ConditionMet(dpOutcome.OutStartDate, academicStartMinus10Years))
                    {
                        HandleValidationError(
                            objectToValidate.LearnRefNumber,
                            errorMessageParameters: BuildErrorMessageParameters(dpOutcome.OutStartDate));
                    }
                }
            }
        }

        public bool ConditionMet(DateTime outStartDate, DateTime academicStartMinus10Years)
        {
            return outStartDate < academicStartMinus10Years;
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(DateTime outStartDate)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.OutStartDate, outStartDate),
            };
        }
    }
}
