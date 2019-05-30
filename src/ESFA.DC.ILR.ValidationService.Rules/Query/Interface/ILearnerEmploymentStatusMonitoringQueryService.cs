using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Query.Interface
{
    public interface ILearnerEmploymentStatusMonitoringQueryService : IQueryService
    {
        bool HasAnyEmploymentStatusMonitoringTypeAndCodeForLearnerEmploymentStatus(IEnumerable<ILearnerEmploymentStatus> learnerEmploymentStatuses, string esmType, int esmCode);

        bool HasAnyEmploymentStatusMonitoringTypeMoreThanOnce(IEnumerable<IEmploymentStatusMonitoring> employmentStatusMonitorings, IEnumerable<string> esmTypes);

        IEnumerable<string> GetDuplicatedEmploymentStatusMonitoringTypesForTypes(IEnumerable<IEmploymentStatusMonitoring> employmentStatusMonitorings, IEnumerable<string> esmTypes);

        bool HasAnyEmploymentStatusMonitoringTypeAndCodesForEmploymentStatus(ILearnerEmploymentStatus learnerEmploymentStatus, string esmType, IEnumerable<int> esmCodes);

        bool HasAnyEmploymentStatusMonitoringTypeAndCodeForEmploymentStatus(ILearnerEmploymentStatus learnerEmploymentStatus, string esmType, int esmCode);
    }
}
