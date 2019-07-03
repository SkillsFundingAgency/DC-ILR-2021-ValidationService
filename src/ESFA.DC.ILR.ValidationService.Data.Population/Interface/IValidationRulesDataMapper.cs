using System.Collections.Generic;
using ESFA.DC.ILR.ValidationService.Data.External.ValidationRules.Model;

namespace ESFA.DC.ILR.ValidationService.Data.Population.Interface
{
    public interface IValidationRulesDataMapper : IMapper
    {
        IReadOnlyCollection<ValidationRule> MapValidationRules(IReadOnlyCollection<ReferenceDataService.Model.MetaData.ValidationRule> validationRules);
    }
}
