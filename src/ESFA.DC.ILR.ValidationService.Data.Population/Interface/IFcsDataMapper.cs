using System.Collections.Generic;
using ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface;

namespace ESFA.DC.ILR.ValidationService.Data.Population.Interface
{
    public interface IFcsDataMapper : IMapper
    {
        IReadOnlyDictionary<string, IFcsContractAllocation> MapFcsContractAllocations(IReadOnlyCollection<ReferenceDataService.Model.FCS.FcsContractAllocation> fcsContractAllocations);
    }
}
