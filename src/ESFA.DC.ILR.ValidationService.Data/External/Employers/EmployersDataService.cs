using ESFA.DC.ILR.ValidationService.Data.External.EDRS.Interface;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using System.Linq;

namespace ESFA.DC.ILR.ValidationService.Data.External.EDRS
{
    /// <summary>
    /// i provide EDRS data operations implementation
    /// </summary>
    /// <seealso cref="IEmployersDataService" />
    public class EmployersDataService : IEmployersDataService
    {
        private readonly IExternalDataCache _referenceDataCache;

        public EmployersDataService(IExternalDataCache referenceDataCache)
        {
            _referenceDataCache = referenceDataCache;
        }

        /// <summary>
        /// Returns true if ... is valid.
        /// </summary>
        /// <param name="thisEmployer">this employer (reference number).</param>
        /// <returns>
        /// <c>true</c> if the specified this employer is valid; otherwise, <c>false</c>.
        /// </returns>
        public bool IsValid(int? empId)
        {
            return empId.HasValue && _referenceDataCache.ERNs.Contains(empId.Value);
        }
    }
}
