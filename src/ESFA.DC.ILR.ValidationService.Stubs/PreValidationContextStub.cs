using System.Collections.Generic;
using ESFA.DC.ILR.ValidationService.Interface;

namespace ESFA.DC.ILR.ValidationService.Stubs
{
    public class PreValidationContextStub : IValidationContext
    {
        public string Filename { get; set; }

        public string Output { get; set; }

        public string Container { get; set; }

        public string IlrReferenceDataKey { get; }

        public string JobId { get; set; }

        public int ValidLearnRefNumbersCount { get; set; }

        public int InvalidLearnRefNumbersCount { get; set; }

        public int ValidationTotalErrorCount { get; set; }

        public int ValidationTotalWarningCount { get; set; }

        public string InvalidLearnRefNumbersKey { get; set; }

        public string ValidLearnRefNumbersKey { get; set; }

        public string ValidationErrorsKey { get; set; }

        public string ValidationErrorMessageLookupKey { get; set; }

        public IEnumerable<string> Tasks { get; set; }
    }
}
