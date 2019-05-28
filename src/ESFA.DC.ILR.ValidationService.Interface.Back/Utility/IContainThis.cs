using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Utility
{
    public interface IContainThis<TItem> :
        IReadOnlyCollection<TItem>
    {
        /// <summary>
        /// Determines whether [contains] [the specified this item].
        /// </summary>
        /// <param name="thisItem">The this item.</param>
        /// <returns>
        ///   <c>true</c> if [contains] [the specified this item]; otherwise, <c>false</c>.
        /// </returns>
        bool Contains(TItem thisItem);
    }
}
