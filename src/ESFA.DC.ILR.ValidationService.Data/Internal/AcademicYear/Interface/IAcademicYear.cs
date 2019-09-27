using System;

namespace ESFA.DC.ILR.ValidationService.Data.Internal.AcademicYear.Interface
{
    public interface IAcademicYear
    {
        DateTime AugustThirtyFirst { get; }

        DateTime End { get; }

        DateTime JanuaryFirst { get; }

        DateTime JulyThirtyFirst { get; }

        DateTime Start { get; }

        DateTime PreviousYearEnd { get; }
    }
}
