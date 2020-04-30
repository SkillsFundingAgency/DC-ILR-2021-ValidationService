using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Derived
{
    /// <summary>
    /// derived data 07 - viable apprenticeship codes
    /// </summary>
    /// <seealso cref="IDerivedData_07Rule" />
    public class DerivedData_07Rule : IDerivedData_07Rule
    {
        /// <summary>
        /// The allowed programme types
        /// </summary>
        private static readonly IEnumerable<int?> _allowedProgTypes = new HashSet<int?>()
        {
            ProgTypes.AdvancedLevelApprenticeship,
            ProgTypes.IntermediateLevelApprenticeship,
            ProgTypes.HigherApprenticeshipLevel4,
            ProgTypes.HigherApprenticeshipLevel5,
            ProgTypes.HigherApprenticeshipLevel6,
            ProgTypes.HigherApprenticeshipLevel7Plus,
            ProgTypes.ApprenticeshipStandard
        };

        /// <summary>
        /// Determines whether the specified prog type is apprenticeship.
        /// </summary>
        /// <param name="progType">Type of the prog.</param>
        /// <returns>
        ///   <c>true</c> if the specified prog type is apprenticeship; otherwise, <c>false</c>.
        /// </returns>
        public bool IsApprenticeship(int? progType)
        {
            return _allowedProgTypes.Contains(progType);
        }
    }
}
