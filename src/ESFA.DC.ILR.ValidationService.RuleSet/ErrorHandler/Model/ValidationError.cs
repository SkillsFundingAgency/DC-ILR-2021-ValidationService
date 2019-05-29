using System.Collections.Generic;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Interface.Enum;

namespace ESFA.DC.ILR.ValidationService.RuleSet.ErrorHandler.Model
{
    public struct ValidationError : IValidationError
    {
        public string LearnerReferenceNumber { get; set; }

        public long? AimSequenceNumber { get; set; }

        public string RuleName { get; set; }

        public Severity? Severity { get; set; }

        public IEnumerable<IErrorMessageParameter> ErrorMessageParameters { get; set; }
    }
}