using System.Collections.Concurrent;
using System.Collections.Generic;
using ESFA.DC.ILR.ValidationService.Interface;

namespace ESFA.DC.ILR.ValidationService.RuleSet.ErrorHandler
{
    public class ValidationErrorCache : ConcurrentBag<IValidationError>, IValidationErrorCache
    {
        public IReadOnlyCollection<IValidationError> ValidationErrors => this;
    }
}
