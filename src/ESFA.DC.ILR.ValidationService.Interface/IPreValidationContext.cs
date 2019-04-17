using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Interface
{
    public interface IPreValidationContext
    {
        string Input { get; set; }

        string Output { get; }

        string Container { get; set; }

        string IlrReferenceDataKey { get; }

        string InvalidLearnRefNumbersKey { get; }

        string ValidLearnRefNumbersKey { get; }

        string ValidationErrorsKey { get; }

        string ValidationErrorMessageLookupKey { get; }

        string JobId { get; }

        int ValidLearnRefNumbersCount { get; set; }

        int InvalidLearnRefNumbersCount { get; set; }

        int ValidationTotalErrorCount { get; set; }

        int ValidationTotalWarningCount { get; set; }

        IEnumerable<string> Tasks { get; set; }
    }
}
