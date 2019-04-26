using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using ESFA.DC.ILR.ReferenceDataService.Model;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Data.Internal;
using ESFA.DC.ILR.ValidationService.Data.Internal.AcademicYear.Model;
using ESFA.DC.ILR.ValidationService.Data.Internal.Model;
using ESFA.DC.ILR.ValidationService.Data.Population.Interface;
using ESFA.DC.ILR.ValidationService.Utility;

namespace ESFA.DC.ILR.ValidationService.Data.Population
{
    public class InternalDataCachePopulationService : IInternalDataCachePopulationService
    {
        private readonly IInternalDataCache _internalDataCache;
        private readonly ICache<ReferenceDataRoot> _referenceDataCache;
        private readonly ILookupsDataMapper _lookupsDataMapper;

        public InternalDataCachePopulationService(IInternalDataCache internalDataCache, ICache<ReferenceDataRoot> referenceDataCache, ILookupsDataMapper lookupsDataMapper)
        {
            _internalDataCache = internalDataCache;
            _referenceDataCache = referenceDataCache;
            _lookupsDataMapper = lookupsDataMapper;
        }

        public async Task PopulateAsync(CancellationToken cancellationToken)
        {
            var internalDataCache = (InternalDataCache)_internalDataCache;
            var referenceDataCache = _referenceDataCache.Item;
            var academicYear = BuildAcademicYear();

            internalDataCache.AcademicYear = _lookupsDataMapper.MapAcademicYear(academicYear);
            internalDataCache.IntegerLookups = _lookupsDataMapper.MapIntegerLookups(referenceDataCache.MetaDatas.Lookups);
            internalDataCache.LimitedLifeLookups = _lookupsDataMapper.MapLimitedLifeLookups(referenceDataCache.MetaDatas.Lookups);
            internalDataCache.ListItemLookups = _lookupsDataMapper.MapListItemLookups(referenceDataCache.MetaDatas.Lookups);
            internalDataCache.StringLookups = _lookupsDataMapper.MapStringLookups(referenceDataCache.MetaDatas.Lookups);
        }

        private AcademicYear BuildAcademicYear()
        {
            return new AcademicYear()
            {
                AugustThirtyFirst = new DateTime(2018, 8, 31),
                End = new DateTime(2019, 7, 31),
                JanuaryFirst = new DateTime(2019, 1, 1),
                JulyThirtyFirst = new DateTime(2019, 7, 31),
                Start = new DateTime(2018, 8, 1),
            };
        }

        private DateTime GetMinimumDate(string candidate) =>
            It.IsEmpty(candidate) ? DateTime.MinValue : DateTime.Parse(candidate);

        private DateTime GetMaximumDate(string candidate) =>
            It.IsEmpty(candidate) ? DateTime.MaxValue : DateTime.Parse(candidate);

        private string GetAttributeValue(XAttribute thisAttribute) =>
            thisAttribute?.Value;
    }
}
