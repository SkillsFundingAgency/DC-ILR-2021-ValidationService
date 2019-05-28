using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Interface
{
    public interface IValidationErrorHandler
    {
        void Handle(string ruleName, string learnRefNumber = null, long? aimSequenceNumber = null, IEnumerable<IErrorMessageParameter> errorMessageParameters = null);

        IErrorMessageParameter BuildErrorMessageParameter(string propertyName, object value);
    }
}