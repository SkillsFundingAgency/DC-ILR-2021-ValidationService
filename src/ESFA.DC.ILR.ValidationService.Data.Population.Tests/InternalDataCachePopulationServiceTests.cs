using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.ReferenceDataService.Model;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Data.Internal;
using ESFA.DC.ILR.ValidationService.Data.Population.Interface;
using FluentAssertions;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Data.Population.Tests
{
    public class InternalDataCachePopulationServiceTests
    {
        private InternalDataCachePopulationService NewService(
            IInternalDataCache internalDataCache = null,
            ICache<ReferenceDataRoot> referenceDataCache = null,
            ILookupsDataMapper lookupsDataMapper = null)
        {
            return new InternalDataCachePopulationService(internalDataCache, referenceDataCache, lookupsDataMapper);
        }
    }
}
