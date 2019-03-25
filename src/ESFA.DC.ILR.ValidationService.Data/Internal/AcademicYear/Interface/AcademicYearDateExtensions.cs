using System;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Data.Internal.AcademicYear.Interface
{
    /// <summary>
    /// the academic year date format helper
    /// </summary>
    public static class AcademicYearDateExtensions
    {
        /// <summary>
        /// Academic Year Date Definitions
        /// </summary>
        private static readonly Dictionary<AcademicYearDates, AcademicYearDateDefinition> _academicYearDateDefinitions = new Dictionary<AcademicYearDates, AcademicYearDateDefinition>()
        {
            [AcademicYearDates.Commencement] = new AcademicYearDateDefinition(8, 1, x => x.Month > 8 ? x.Year : x.Year - 1),
            [AcademicYearDates.PreviousYearEnd] = new AcademicYearDateDefinition(7, 31, x => x.Month > 8 ? x.Year : x.Year - 1),
            [AcademicYearDates.August31] = new AcademicYearDateDefinition(8, 31, x => x.Month > 7 ? x.Year : x.Year - 1),
            [AcademicYearDates.CurrentYearEnd] = new AcademicYearDateDefinition(7, 31, x => x.Month < 8 ? x.Year : x.Year + 1),
            [AcademicYearDates.NextYearCommencement] = new AcademicYearDateDefinition(8, 1, x => x.Month < 8 ? x.Year : x.Year + 1),
        };

        /// <summary>
        /// Gets the Academic Year Date For the date in academic year with a selective shift on the Year Property.
        /// </summary>
        /// <param name="forThisDate"></param>
        /// <param name="candidate"></param>
        /// <returns>Date in Academic Year of Candidate</returns>
        public static DateTime GetAcademicYearDateFor(this AcademicYearDates forThisDate, DateTime candidate)
        {
            var academicYearDateDefinition = _academicYearDateDefinitions[forThisDate];

            return new DateTime(academicYearDateDefinition.YearCalculation(candidate), academicYearDateDefinition.Month, academicYearDateDefinition.Day);
        }

        /// <summary>
        /// Year Agnostic Date Definition
        /// </summary>
        private struct AcademicYearDateDefinition
        {
            public AcademicYearDateDefinition(int month, int day, Func<DateTime, int> yearCalculation)
            {
                Month = month;
                Day = day;
                YearCalculation = yearCalculation;
            }

            public int Month { get; }

            public int Day { get; }

            public Func<DateTime, int> YearCalculation { get; }
        }
    }
}
