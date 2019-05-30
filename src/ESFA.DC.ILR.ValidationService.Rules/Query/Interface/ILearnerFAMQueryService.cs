using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Query.Interface
{
    public interface ILearnerFAMQueryService : IQueryService
    {
        bool HasAnyLearnerFAMCodesForType(IEnumerable<ILearnerFAM> learnerFAMs, string famType, IEnumerable<int> famCodes);

        bool HasLearnerFAMCodeForType(IEnumerable<ILearnerFAM> learnerFAMs, string famType, int famCode);

        bool HasLearnerFAMType(IEnumerable<ILearnerFAM> learnerFAMs, string famType);

        bool HasAnyLearnerFAMTypes(IEnumerable<ILearnerFAM> learnerFams, IEnumerable<string> famTypes);

        int GetLearnerFAMsCountByFAMType(IEnumerable<ILearnerFAM> learnerFaMs, string famType);
    }
}
