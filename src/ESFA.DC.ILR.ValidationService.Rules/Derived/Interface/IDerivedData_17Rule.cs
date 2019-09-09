using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Rules.Derived.Interface
{
    /// <summary>
    /// the derived data rule 17 contract
    /// </summary>
    /// <seealso cref="IDerivedDataRule" />
    public interface IDerivedData_17Rule :
        IDerivedDataRule
    {
        /// <summary>
        /// Determines whether [is TNP more than contribution cap for] [the specified standard].
        /// </summary>
        /// <param name="theStandard">The standard.</param>
        /// <param name="theDeliveries">The deliveries.</param>
        /// <returns>
        ///   <c>true</c> if [is TNP more than contribution cap for] [the specified standard]; otherwise, <c>false</c>.
        /// </returns>
        bool IsTNPMoreThanContributionCapFor(int theStandard, IReadOnlyCollection<ILearningDelivery> theDeliveries);

        /// <summary>
        /// Gets the total TNP price for.
        /// </summary>
        /// <param name="theDeliveries">The deliveries.</param>
        /// <returns>the price</returns>
        int GetTotalTNPPriceFor(IReadOnlyCollection<ILearningDelivery> theDeliveries);

        /// <summary>
        /// Gets the funding contribution cap for.
        /// </summary>
        /// <param name="theStandard">The standard.</param>
        /// <param name="theDeliveries">The deliveries.</param>
        /// <returns>the lars standard government funding contribution cap</returns>
        decimal? GetFundingContributionCapFor(int theStandard, IReadOnlyCollection<ILearningDelivery> theDeliveries);
    }
}
