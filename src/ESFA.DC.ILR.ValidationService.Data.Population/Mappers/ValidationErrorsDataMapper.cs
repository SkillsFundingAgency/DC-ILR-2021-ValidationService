using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.ValidationService.Data.External.ValidationErrors.Model;
using ESFA.DC.ILR.ValidationService.Data.Population.Interface;
using ESFA.DC.ILR.ValidationService.Interface.Enum;
using SeverityLevel = ESFA.DC.ILR.ReferenceDataService.Model.MetaData.ValidationError.SeverityLevel;

namespace ESFA.DC.ILR.ValidationService.Data.Population.Mappers
{
    public class ValidationErrorsDataMapper : IValidationErrorsDataMapper
    {
        public ValidationErrorsDataMapper()
        {
        }

        public IReadOnlyDictionary<string, ValidationError> MapValidationErrors(IReadOnlyCollection<ReferenceDataService.Model.MetaData.ValidationError> validationErrors)
        {
            return validationErrors
                .ToDictionary(
                ve => ve.RuleName,
                ve => new ValidationError
                {
                    RuleName = ve.RuleName,
                    Severity = MapSeverity(ve.Severity),
                    Message = ve.Message
                }, StringComparer.OrdinalIgnoreCase);
        }

        private Severity MapSeverity(SeverityLevel severityLevel)
        {
            switch (severityLevel)
            {
                case SeverityLevel.Error:
                    return Severity.Error;
                case SeverityLevel.Warning:
                    return Severity.Warning;
                case SeverityLevel.Fail:
                    return Severity.Fail;
                default:
                    throw new ArgumentOutOfRangeException(nameof(severityLevel));
            }
        }
    }
}
