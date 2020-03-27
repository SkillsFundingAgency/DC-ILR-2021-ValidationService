using System;
using ESFA.DC.ILR.Constants;
using ESFA.DC.ILR.Desktop.Interface;
using ESFA.DC.ILR.ValidationService.Interface;

namespace ESFA.DC.ILR.ValidationService.Desktop.Context
{
    public class DesktopContextValidationContext : IValidationContext
    {
        private readonly IDesktopContext _desktopContext;

        public DesktopContextValidationContext(IDesktopContext desktopContext)
        {
            _desktopContext = desktopContext;
        }

        public string Filename
        {
            get => _desktopContext.KeyValuePairs[ILRContextKeys.Filename].ToString();
            set => _desktopContext.KeyValuePairs[ILRContextKeys.Filename] = value;
        }

        public string Container => _desktopContext.KeyValuePairs[ILRContextKeys.Container].ToString();

        public string IlrReferenceDataKey => _desktopContext.KeyValuePairs[ILRContextKeys.IlrReferenceData].ToString();

        public string InvalidLearnRefNumbersKey => _desktopContext.KeyValuePairs[ILRContextKeys.InvalidLearnRefNumbers].ToString();

        public string ValidLearnRefNumbersKey => _desktopContext.KeyValuePairs[ILRContextKeys.ValidLearnRefNumbers].ToString();

        public string ValidationErrorsKey => _desktopContext.KeyValuePairs[ILRContextKeys.ValidationErrors].ToString();

        public string ValidationErrorMessageLookupKey => _desktopContext.KeyValuePairs[ILRContextKeys.ValidationErrorLookups].ToString();

        public string JobId => throw new NotImplementedException();

        public int ValidLearnRefNumbersCount
        {
            set => _desktopContext.KeyValuePairs[ILRContextKeys.ValidLearnRefNumbersCount] = value;
        }

        public int InvalidLearnRefNumbersCount
        {
            set => _desktopContext.KeyValuePairs[ILRContextKeys.InvalidLearnRefNumbersCount] = value;
        }

        public int ValidationTotalErrorCount
        {
            set => _desktopContext.KeyValuePairs[ILRContextKeys.ValidationTotalErrorCount] = value;
        }

        public int ValidationTotalWarningCount
        {
            set => _desktopContext.KeyValuePairs[ILRContextKeys.ValidationTotalWarningCount] = value;
        }

        public int ReturnPeriod => int.Parse(_desktopContext.KeyValuePairs[ILRContextKeys.ReturnPeriod].ToString());
    }
}
