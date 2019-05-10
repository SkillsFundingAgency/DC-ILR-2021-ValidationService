using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Data.Internal.Model
{
    public class IlrLookup
    {
        public string Name { get; set; }

        public string Code { get; set; }

        public ValidityPeriods ValidityPeriods { get; set; }

        public IReadOnlyCollection<IlrSubLookup> SubLookup { get; set; }
    }
}
