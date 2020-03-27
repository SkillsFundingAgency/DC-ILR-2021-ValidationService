using System;

namespace ESFA.DC.ILR.ValidationService.Providers.Utils
{
    public class ValidationSeverityFailException : Exception
    {
        public ValidationSeverityFailException(string message)
            : base(message)
        {
        }
    }
}
