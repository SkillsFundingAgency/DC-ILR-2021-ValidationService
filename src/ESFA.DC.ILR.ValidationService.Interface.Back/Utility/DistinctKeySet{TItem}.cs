using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Utility
{
    public class DistinctKeySet<TItem> :
        HashSet<TItem>,
        IContainThis<TItem>
    {
        public DistinctKeySet()
            : base()
        {
        }

        public DistinctKeySet(IEnumerable<TItem> theseItems)
            : base(theseItems)
        {
        }
    }
}
