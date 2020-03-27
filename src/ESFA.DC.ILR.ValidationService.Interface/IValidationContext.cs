namespace ESFA.DC.ILR.ValidationService.Interface
{
    public interface IValidationContext
    {
        string Filename { get; set; }

        string Container { get; }

        string IlrReferenceDataKey { get; }

        string InvalidLearnRefNumbersKey { get; }

        string ValidLearnRefNumbersKey { get; }

        string ValidationErrorsKey { get; }

        string ValidationErrorMessageLookupKey { get; }

        string JobId { get; }

        int ValidLearnRefNumbersCount { set; }

        int InvalidLearnRefNumbersCount { set; }

        int ValidationTotalErrorCount { set; }

        int ValidationTotalWarningCount { set; }

        int ReturnPeriod { get; }
    }
}
