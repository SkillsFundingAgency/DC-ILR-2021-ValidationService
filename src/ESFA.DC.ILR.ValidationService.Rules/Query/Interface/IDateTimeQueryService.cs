using System;

namespace ESFA.DC.ILR.ValidationService.Rules.Query.Interface
{
    public interface IDateTimeQueryService : IQueryService
    {
        int YearsBetween(DateTime start, DateTime end);

        int MonthsBetween(DateTime start, DateTime end);

        double WholeMonthsBetween(DateTime start, DateTime end);

        double DaysBetween(DateTime start, DateTime end);

        double WholeDaysBetween(DateTime start, DateTime end);

        int AgeAtGivenDate(DateTime dateOfBirth, DateTime givenDate);
    }
}
