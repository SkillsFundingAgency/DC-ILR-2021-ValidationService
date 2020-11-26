using System;
using ESFA.DC.ILR.ValidationService.Interface;

namespace ESFA.DC.ILR.ValidationService.Data.Internal.AcademicYear.Interface
{
    public interface IAcademicYearDataService : IDataService
    {
        DateTime AugustThirtyFirst();

        DateTime End();

        DateTime JanuaryFirst();

        DateTime JulyThirtyFirst();

        DateTime Start();

        DateTime PreviousYearEnd();

        int ReturnPeriod();
    }
}