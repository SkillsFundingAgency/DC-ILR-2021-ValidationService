using System.Collections.Generic;
using ESFA.DC.ILR.ValidationService.Interface.Enum;

namespace ESFA.DC.ILR.ValidationService.Interface
{
    public interface IValidationError
    {
        string LearnerReferenceNumber { get; }

        long? AimSequenceNumber { get; }

        string RuleName { get; }

        Severity? Severity { get; }

        IEnumerable<IErrorMessageParameter> ErrorMessageParameters { get; }
    }
}
