using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Data.Internal.AcademicYear.Interface;
using System;

namespace ESFA.DC.ILR.ValidationService.Data.Internal.AcademicYear
{
    /// <summary>
    /// the academic year data service
    /// </summary>
    /// <seealso cref="IAcademicYearDataService" />
    public class AcademicYearDataService :
        IAcademicYearDataService
    {
        /// <summary>
        /// The internal data cache
        /// </summary>
        private readonly IInternalDataCache _internalDataCache;

        /// <summary>
        /// Initializes a new instance of the <see cref="AcademicYearDataService"/> class.
        /// </summary>
        /// <param name="internalDataCache">The internal data cache.</param>
        public AcademicYearDataService(IInternalDataCache internalDataCache)
        {
            _internalDataCache = internalDataCache;
        }

        /// <summary>
        /// Gets an academic year of learning date.
        /// </summary>
        /// <param name="candidate">The candidate, representing a point of reference for
        /// any current academic year where a calcuation can be made
        /// based on the required 'academic year date' operation</param>
        /// <param name="yearDate">The academic year date (of interest)</param>
        /// <returns>
        /// an academic date for the year of learning
        /// </returns>
        public DateTime GetAcademicYearOfLearningDate(DateTime candidate, AcademicYearDates yearDate) => yearDate.GetAcademicYearDateFor(candidate);

        public DateTime AugustThirtyFirst()
        {
            return _internalDataCache.AcademicYear.AugustThirtyFirst;
        }

        /// <summary>
        /// the academic year end.
        /// </summary>
        /// <returns>
        /// an academic year end date
        /// </returns>
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

        /// <summary>
        /// the academic year start.
        /// </summary>
        /// <returns>
        /// an academic year start date
        /// </returns>
        public DateTime Start()
        {
            return _internalDataCache.AcademicYear.Start;
        }
    }
}
