using System.Collections.Generic;
using ESFA.DC.ILR.ReferenceDataService.Model.LARS;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Model;

namespace ESFA.DC.ILR.ValidationService.Data.Population.Interface
{
    public interface ILarsDataMapper : IMapper
    {
        IReadOnlyCollection<ILARSStandard> MapLarsStandards(IReadOnlyCollection<ReferenceDataService.Model.LARS.LARSStandard> larsStandards);

        IReadOnlyCollection<ILARSStandardValidity> MapLarsStandardValidities(IReadOnlyCollection<ReferenceDataService.Model.LARS.LARSStandard> larsStandards);

        IReadOnlyDictionary<string, LearningDelivery> MapLarsLearningDeliveries(IReadOnlyCollection<LARSLearningDelivery> larsLearningDeliveries);

      //  IReadOnlyCollection<Framework> MapLarsFrameworks(IReadOnlyCollection<LARSLearningDelivery> larsStandards);
    }
}
