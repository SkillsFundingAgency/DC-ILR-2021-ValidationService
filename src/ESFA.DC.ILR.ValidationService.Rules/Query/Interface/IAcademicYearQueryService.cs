using System;

namespace ESFA.DC.ILR.ValidationService.Rules.Query.Interface
{
    public interface IAcademicYearQueryService : IQueryService
    {
        DateTime LastFridayInJuneForDateInAcademicYear(DateTime dateTime);

        bool DateIsInPrevAcademicYear(DateTime dateTime, DateTime currentYear);
    }
}
