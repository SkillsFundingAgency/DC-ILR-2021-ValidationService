using System.Collections.Generic;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using System.Linq;

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
            TypeOfLearningProgramme.AdvancedLevelApprenticeship,
            TypeOfLearningProgramme.IntermediateLevelApprenticeship,
            TypeOfLearningProgramme.HigherApprenticeshipLevel4,
            TypeOfLearningProgramme.HigherApprenticeshipLevel5,
            TypeOfLearningProgramme.HigherApprenticeshipLevel6,
            TypeOfLearningProgramme.HigherApprenticeshipLevel7Plus,
            TypeOfLearningProgramme.ApprenticeshipStandard
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
