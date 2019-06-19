using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Interface;

namespace ESFA.DC.ILR.ValidationService.Desktop
{
    public class DisabledRulesProvider : IDisabledRulesProvider
    {
        private readonly IExternalDataCache _externalDataCache;

        public DisabledRulesProvider(IExternalDataCache externalDataCache)
        {
            _externalDataCache = externalDataCache;
        }

        public ICollection<string> Provide()
        {
            return _externalDataCache.ValidationRules.Where(vr => !vr.Desktop).Select(vr => vr.RuleName).ToList();
        }
    }
}
