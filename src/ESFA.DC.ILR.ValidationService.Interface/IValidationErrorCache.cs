using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Interface
{
    public interface IValidationErrorCache
    {
        IReadOnlyCollection<IValidationError> ValidationErrors { get; }

        void Add(IValidationError validationError);
    }
}
