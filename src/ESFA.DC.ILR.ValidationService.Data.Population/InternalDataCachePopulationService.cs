using System;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.ReferenceDataService.Model;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Data.Internal;
using ESFA.DC.ILR.ValidationService.Data.Internal.AcademicYear.Model;
using ESFA.DC.ILR.ValidationService.Data.Population.Interface;
using ESFA.DC.ILR.ValidationService.Interface;

namespace ESFA.DC.ILR.ValidationService.Data.Population
{
    public class InternalDataCachePopulationService : IInternalDataCachePopulationService
    {
        private readonly IInternalDataCache _internalDataCache;
        private readonly ILookupsDataMapper _lookupsDataMapper;

        public InternalDataCachePopulationService(IInternalDataCache internalDataCache, ILookupsDataMapper lookupsDataMapper)
        {
            _internalDataCache = internalDataCache;
            _lookupsDataMapper = lookupsDataMapper;
        }

        public void Populate(ReferenceDataRoot referenceDataRoot)
        {
            var internalDataCache = (InternalDataCache)_internalDataCache;
            var lookupsDictionary = _lookupsDataMapper.BuildLookups(referenceDataRoot.MetaDatas.Lookups);
            var academicYear = BuildAcademicYear();

            internalDataCache.AcademicYear = _lookupsDataMapper.MapAcademicYear(academicYear);
            internalDataCache.IntegerLookups = _lookupsDataMapper.MapIntegerLookups(lookupsDictionary);
            internalDataCache.LimitedLifeLookups = _lookupsDataMapper.MapLimitedLifeLookups(lookupsDictionary);
            internalDataCache.ListItemLookups = _lookupsDataMapper.MapListItemLookups(lookupsDictionary);
            internalDataCache.StringLookups = _lookupsDataMapper.MapStringLookups(lookupsDictionary);
        }

        private AcademicYear BuildAcademicYear()
        {
            return new AcademicYear()
            {
                AugustThirtyFirst = new DateTime(2019, 8, 31),
                End = new DateTime(2020, 7, 31),
                JanuaryFirst = new DateTime(2020, 1, 1),
                JulyThirtyFirst = new DateTime(2020, 7, 31),
                Start = new DateTime(2019, 8, 1),
            };
        }
    }
}
