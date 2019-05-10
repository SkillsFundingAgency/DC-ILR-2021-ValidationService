using System.Collections.Generic;
using ESFA.DC.ILR.ValidationService.Data.External.ValidationErrors.Model;

namespace ESFA.DC.ILR.ValidationService.Data.Population.Interface
{
    public interface IValidationErrorsDataMapper
    {
        IReadOnlyDictionary<string, ValidationError> MapValidationErrors(IReadOnlyCollection<ReferenceDataService.Model.MetaData.ValidationError> validationErrors);
    }
}
