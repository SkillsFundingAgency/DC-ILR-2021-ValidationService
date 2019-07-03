using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.ValidationService.Data.External.ValidationRules.Model;
using ESFA.DC.ILR.ValidationService.Data.Population.Interface;

namespace ESFA.DC.ILR.ValidationService.Data.Population.Mappers
{
    public class ValidationRulesDataMapper : IValidationRulesDataMapper
    {
        public IReadOnlyCollection<ValidationRule> MapValidationRules(IReadOnlyCollection<ReferenceDataService.Model.MetaData.ValidationRule> validationRules)
        {
            return validationRules
                ?.Select(vr => new ValidationRule
                {
                    RuleName = vr.RuleName,
                    Online = vr.Online,
                    Desktop = vr.Desktop
                }).ToList();
        }
    }
}
