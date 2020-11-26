using System;
using ESFA.DC.ILR.ValidationService.Data.Internal.AcademicYear.Interface;
using ESFA.DC.ILR.ValidationService.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Query.Interface
{
    public interface IAcademicYearQueryService : IQueryService
    {
        DateTime LastFridayInJuneForDateInAcademicYear(DateTime dateTime);

        bool DateIsInPrevAcademicYear(DateTime dateTime, DateTime currentYear);

        DateTime GetAcademicYearOfLearningDate(DateTime candidate, AcademicYearDates yearDate);
    }
}
