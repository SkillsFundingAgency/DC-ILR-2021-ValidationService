using System;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Data.Internal.AcademicYear.Interface
{
    public static class AcademicYearDateExtensions
    {
        private static readonly Dictionary<AcademicYearDates, AcademicYearDateDefinition> _academicYearDateDefinitions = new Dictionary<AcademicYearDates, AcademicYearDateDefinition>()
        {
            [AcademicYearDates.Commencement] = new AcademicYearDateDefinition(8, 1, x => GetYear(x, 8)),
            [AcademicYearDates.PreviousYearEnd] = new AcademicYearDateDefinition(7, 31, x => GetYear(x, 8)),
            [AcademicYearDates.August31] = new AcademicYearDateDefinition(8, 31, x => GetYear(x, 7)),
            [AcademicYearDates.CurrentYearEnd] = new AcademicYearDateDefinition(7, 31, x => x.Month < 8 ? x.Year : x.Year + 1),
            [AcademicYearDates.NextYearCommencement] = new AcademicYearDateDefinition(8, 1, x => x.Month < 8 ? x.Year : x.Year + 1),
            [AcademicYearDates.TraineeshipsAugust1] = new AcademicYearDateDefinition(8, 1, x => x.Month < 9 ? x.Year : x.Year + 1),
        };

        private static int GetYear(DateTime dateTime, int monthToCompare)
        {
            if (dateTime.Month > monthToCompare)
            {
                return dateTime.Year;
            }
            else
            {
                if (dateTime.Year > 1)
                {
                    return dateTime.Year - 1;
                }
                else
                {
                    return dateTime.Year;
                }
            }
        }

        public static DateTime GetAcademicYearDateFor(this AcademicYearDates forThisDate, DateTime candidate)
        {
            var academicYearDateDefinition = _academicYearDateDefinitions[forThisDate];

            return new DateTime(academicYearDateDefinition.YearCalculation(candidate), academicYearDateDefinition.Month, academicYearDateDefinition.Day);
        }

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
