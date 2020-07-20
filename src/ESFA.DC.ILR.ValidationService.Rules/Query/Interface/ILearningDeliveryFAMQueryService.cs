using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Query.Interface
{
    public interface ILearningDeliveryFAMQueryService : IQueryService
    {
        bool HasAnyLearningDeliveryFAMCodesForType(IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs, string famType, IEnumerable<string> famCodes);

        bool HasLearningDeliveryFAMCodeForType(IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs, string famType, string famCode);

        bool HasLearningDeliveryFAMType(IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs, string famType);

        bool HasLearningDeliveryFAMTypeForDate(IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs, string famType, DateTime date);

        int GetLearningDeliveryFAMsCountByFAMType(IReadOnlyCollection<ILearningDeliveryFAM> learningDeliveryFAMs, string famType);

        IEnumerable<ILearningDeliveryFAM> GetLearningDeliveryFAMsForType(IEnumerable<ILearningDeliveryFAM> learningDeliveryFams, string famType);

        IEnumerable<ILearningDeliveryFAM> GetOverLappingLearningDeliveryFAMsForType(IEnumerable<ILearningDeliveryFAM> learningDeliveryFams, string famType);

        IEnumerable<ILearningDeliveryFAM> GetLearningDeliveryFAMsForTypeAndCodes(IEnumerable<ILearningDeliveryFAM> learningDeliveryFams, string famType, IEnumerable<string> famCodes);
    }
}
