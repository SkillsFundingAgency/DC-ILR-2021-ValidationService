using System.Collections.Concurrent;
using System.Collections.Generic;
using ESFA.DC.ILR.ValidationService.Interface;

namespace ESFA.DC.ILR.ValidationService.RuleSet.ErrorHandler
{
    public class ValidationErrorCache<T> : IValidationErrorCache<T>
    {
        private readonly ConcurrentBag<T> _validationErrors = new ConcurrentBag<T>();

        public IReadOnlyCollection<T> ValidationErrors
        {
            get { return _validationErrors; }
        }

        public void Add(T validationError)
        {
            _validationErrors.Add(validationError);
        }
    }
}
