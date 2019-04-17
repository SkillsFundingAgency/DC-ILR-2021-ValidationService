using System.Collections.Generic;
using ESFA.DC.ILR.ValidationService.Interface;

namespace ESFA.DC.ILR.ValidationService.Stateless.Models
{
    public class PreValidationContext : IPreValidationContext
    {
        public string Input { get; set; }

        public string Output { get; set; }

        public string Container { get; set; }

        public string IlrReferenceDataKey { get; set; }

        public string JobId { get; set; }

        public string InvalidLearnRefNumbersKey { get; set; }

        public string ValidLearnRefNumbersKey { get; set; }

        public string ValidationErrorsKey { get; set; }

        public string ValidationErrorMessageLookupKey { get; set; }

        public int ValidLearnRefNumbersCount { get; set; }

        public int InvalidLearnRefNumbersCount { get; set; }

        public int ValidationTotalErrorCount { get; set; }

        public int ValidationTotalWarningCount { get; set; }

        public IEnumerable<string> Tasks { get; set; }
    }
}
