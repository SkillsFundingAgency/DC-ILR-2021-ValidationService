﻿using System;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Query
{
    public class DateTimeQueryService : IDateTimeQueryService
    {
        public int YearsBetween(DateTime start, DateTime end)
        {
            var years = end.Year - start.Year;

            return end < start.AddYears(years) ? years - 1 : years;
        }

        public int MonthsBetween(DateTime start, DateTime end)
        {
            int monthsApart = (12 * (start.Year - end.Year)) + start.Month - end.Month;

            return Math.Abs(monthsApart);
        }

        public double WholeMonthsBetween(DateTime start, DateTime end)
        {
            if (start.AddMonths(12).AddDays(-1) == end || (start.AddMonths(12) < end))
            {
                return MonthsBetween(start, end) + 1;
            }

            return MonthsBetween(start, end);
        }

        public double DaysBetween(DateTime start, DateTime end)
        {
            return (end - start).TotalDays;
        }

        public double WholeDaysBetween(DateTime start, DateTime end)
        {
            return Math.Abs(DaysBetween(start, end)) + 1;
        }

        public DateTime AddYearsToDate(DateTime date, int yearsToAdd)
        {
            DateTime dateResult;
            try
            {
                dateResult = date.AddYears(yearsToAdd);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                dateResult = yearsToAdd < 0 ? DateTime.MinValue.Date : DateTime.MaxValue.Date;
            }

            return dateResult;
        }

        public bool IsDateBetween(DateTime candidate, DateTime min, DateTime max, bool includeBoundaries = true)
        {
            return includeBoundaries
               ? candidate >= min && candidate <= max
               : candidate > min && candidate < max;
        }
    }
}
