using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Query.Interface
{
    public interface ILearningDeliveryAppFinRecordQueryService : IQueryService
    {
        bool HasAnyLearningDeliveryAFinCodesForType(IEnumerable<IAppFinRecord> appFinRecords, string aFinType, IEnumerable<int> aFinCodes);

        bool HasAnyLearningDeliveryAFinCodeForType(IEnumerable<IAppFinRecord> appFinRecords, string aFinType, int? aFinCode);

        bool HasAnyLearningDeliveryAFinCodes(IEnumerable<IAppFinRecord> appFinRecords, IEnumerable<int> aFinCodes);

        IAppFinRecord GetLatestAppFinRecord(IReadOnlyCollection<IAppFinRecord> appFinRecords, string appFinType, int appFinCode);

        int GetTotalTNPPriceForLatestAppFinRecordsForLearning(IEnumerable<ILearningDelivery> learningDeliveries);

        IEnumerable<IAppFinRecord> GetAppFinRecordsForType(IEnumerable<IAppFinRecord> appFinRecords, string aFinType);
    }
}
