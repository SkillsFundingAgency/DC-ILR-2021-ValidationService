using System;
using ESFA.DC.ILR.ValidationService.Interface;

namespace ESFA.DC.ILR.ValidationService.Data.Internal.AcademicYear.Interface
{
    /// <summary>
    /// the academic year data service
    /// </summary>
    public interface IAcademicYearDataService : IDataService
    {
        /// <summary>
        /// Gets an academic year of learning date.
        /// </summary>
        /// <param name="candidate">
        /// The candidate, representing a point of reference for
        /// any current academic year where a calcuation can be made
        /// based on the required 'academic year date' operation
        /// </param>
        /// <param name="yearDate">The academic year date (of interest)</param>
        /// <returns>
        /// an academic date for the year of learning
        /// </returns>
        DateTime GetAcademicYearOfLearningDate(DateTime candidate, AcademicYearDates yearDate);

        DateTime AugustThirtyFirst();

        /// <summary>
        /// the academic year end.
        /// </summary>
        /// <returns>an academic year end date</returns>
        DateTime End();

        DateTime JanuaryFirst();

        DateTime JulyThirtyFirst();

        /// <summary>
        /// the academic year start.
        /// </summary>
        /// <returns>an academic year start date</returns>
        DateTime Start();
    }
}