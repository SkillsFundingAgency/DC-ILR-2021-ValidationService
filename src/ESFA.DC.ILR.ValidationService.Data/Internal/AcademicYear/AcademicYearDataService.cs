using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Data.Internal.AcademicYear.Interface;
using System;

namespace ESFA.DC.ILR.ValidationService.Data.Internal.AcademicYear
{
    public class AcademicYearDataService : IAcademicYearDataService
    {
        private readonly IInternalDataCache _internalDataCache;
        private readonly IExternalDataCache _externalDataCache;

        public AcademicYearDataService(IInternalDataCache internalDataCache, IExternalDataCache externalDataCache)
        {
            _internalDataCache = internalDataCache;
            _externalDataCache = externalDataCache;
        }

        public DateTime AugustThirtyFirst()
        {
            return _internalDataCache.AcademicYear.AugustThirtyFirst;
        }

        public DateTime End()
        {
            return _internalDataCache.AcademicYear.End;
        }

        public DateTime JanuaryFirst()
        {
            return _internalDataCache.AcademicYear.JanuaryFirst;
        }

        public DateTime JulyThirtyFirst()
        {
            return _internalDataCache.AcademicYear.JulyThirtyFirst;
        }

        public DateTime Start()
        {
            return _internalDataCache.AcademicYear.Start;
        }

        public DateTime PreviousYearEnd()
        {
            return _internalDataCache.AcademicYear.PreviousYearEnd;
        }

        public int ReturnPeriod()
        {
            return _externalDataCache.ReturnPeriod;
        }
    }
}
