using System;
using ESFA.DC.ILR.ValidationService.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Query.Interface
{
    public interface IDateTimeQueryService : IQueryService
    {
        int YearsBetween(DateTime start, DateTime end);

        int MonthsBetween(DateTime start, DateTime end);

        double WholeMonthsBetween(DateTime start, DateTime end);

        double DaysBetween(DateTime start, DateTime end);

        double WholeDaysBetween(DateTime start, DateTime end);

        DateTime AddYearsToDate(DateTime date, int yearsToAdd);

        bool IsDateBetween(DateTime candidate, DateTime min, DateTime max, bool includeBoundaries = true);
    }
}
