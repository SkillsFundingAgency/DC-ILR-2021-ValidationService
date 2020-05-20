using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Query
{
    public class LearnerEmploymentStatusMonitoringQueryService : ILearnerEmploymentStatusMonitoringQueryService
    {
        public bool HasAnyEmploymentStatusMonitoringTypeAndCodeForLearnerEmploymentStatus(IEnumerable<ILearnerEmploymentStatus> learnerEmploymentStatuses, string esmType, int esmCode)
        {
            return learnerEmploymentStatuses?
                       .Select(les => les.EmploymentStatusMonitorings?
                           .Any(esmt => esmt.ESMType.CaseInsensitiveEquals(esmType) && esmt.ESMCode == esmCode)).FirstOrDefault() ?? false;
        }

        public bool HasAnyEmploymentStatusMonitoringTypeMoreThanOnce(IEnumerable<IEmploymentStatusMonitoring> employmentStatusMonitorings, IEnumerable<string> esmTypes)
        {
            return employmentStatusMonitorings != null
                   && employmentStatusMonitorings
                       .Where(esm => esmTypes.Any(x => x.CaseInsensitiveEquals(esm.ESMType)))
                       .GroupBy(x => x.ESMType.ToUpperInvariant())
                       .Any(g => g.Count() > 1);
        }

        public IEnumerable<string> GetDuplicatedEmploymentStatusMonitoringTypesForTypes(IEnumerable<IEmploymentStatusMonitoring> employmentStatusMonitorings, IEnumerable<string> esmTypes)
        {
            if (employmentStatusMonitorings == null || esmTypes == null)
            {
                return null;
            }

            return employmentStatusMonitorings
                .Where(esm => esmTypes.Any(x => x.CaseInsensitiveEquals(esm.ESMType)))
                .GroupBy(x => x.ESMType.ToUpperInvariant())
                .Where(g => g.Count() > 1)
                .Select(s => s.Key);
        }

        public bool HasAnyEmploymentStatusMonitoringTypeAndCodesForEmploymentStatus(ILearnerEmploymentStatus learnerEmploymentStatus, string esmType, IEnumerable<int> esmCodes)
        {
            return learnerEmploymentStatus?.EmploymentStatusMonitorings != null
                   && learnerEmploymentStatus.EmploymentStatusMonitorings
                           .Any(esmt => esmt.ESMType.CaseInsensitiveEquals(esmType) && esmCodes.Contains(esmt.ESMCode));
        }

        public bool HasAnyEmploymentStatusMonitoringTypeAndCodeForEmploymentStatus(ILearnerEmploymentStatus learnerEmploymentStatus, string esmType, int esmCode)
        {
            return learnerEmploymentStatus?.EmploymentStatusMonitorings != null
                   && learnerEmploymentStatus.EmploymentStatusMonitorings
                           .Any(esmt => esmt.ESMType.CaseInsensitiveEquals(esmType) && esmCode == esmt.ESMCode);
        }
    }
}
