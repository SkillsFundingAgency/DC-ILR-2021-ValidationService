﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.ILR.ValidationService.Interface
{
    public interface IRuleSetOrchestrationService<TRule, T>
        where T : class
    {
        Task<IEnumerable<IValidationError>> ExecuteAsync(IEnumerable<T> validationItems, CancellationToken cancellationToken);

        Task<IEnumerable<IValidationError>> ExecuteAsync(T validationItem, CancellationToken cancellationToken);
    }
}
