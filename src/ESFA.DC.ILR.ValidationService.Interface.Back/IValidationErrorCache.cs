using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Interface
{
    public interface IValidationErrorCache<T>
    {
        IReadOnlyCollection<T> ValidationErrors { get; }

        void Add(T validationError);
    }
}
