using System;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Utility
{
    public class CaseInsensitiveDistinctKeySet :
        HashSet<string>,
        IContainThis<string>
    {
        public CaseInsensitiveDistinctKeySet()
            : base(StringComparer.OrdinalIgnoreCase)
        {
        }

        public CaseInsensitiveDistinctKeySet(IEnumerable<string> theseItems)
            : base(theseItems, StringComparer.OrdinalIgnoreCase)
        {
        }
    }
}
