using System;
using IAcademicYear = ESFA.DC.ILR.ValidationService.Data.Internal.AcademicYear.Interface.IAcademicYear;

namespace ESFA.DC.ILR.ValidationService.Data.Internal.AcademicYear.Model
{
    public class AcademicYear : IAcademicYear
    {
        public DateTime AugustThirtyFirst { get; set; }

        public DateTime End { get; set; }

        public DateTime JanuaryFirst { get; set; }

        public DateTime JulyThirtyFirst { get; set; }

        public DateTime Start { get; set; }

       public DateTime PreviousYearEnd { get; set; }
    }
}
