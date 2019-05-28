using System;
using System.Collections.Generic;
using ESFA.DC.ILR.ValidationService.Interface;

namespace ESFA.DC.ILR.ValidationService.ValidationDPActor.Context
{
    public class ValidationDPActorModelValidationContext : IValidationContext
    {
        public ValidationDPActorModelValidationContext(IEnumerable<string> ignoredRules)
        {
            IgnoredRules = ignoredRules;
        }

        public string Filename => throw new NotImplementedException();

        public string Container => throw new NotImplementedException();

        public string IlrReferenceDataKey => throw new NotImplementedException();

        public string InvalidLearnRefNumbersKey => throw new NotImplementedException();

        public string ValidLearnRefNumbersKey => throw new NotImplementedException();

        public string ValidationErrorsKey => throw new NotImplementedException();

        public string ValidationErrorMessageLookupKey => throw new NotImplementedException();

        public string JobId => throw new NotImplementedException();

        public int ValidLearnRefNumbersCount
        {
            set => throw new NotImplementedException();
        }

        public int InvalidLearnRefNumbersCount
        {
            set => throw new NotImplementedException();
        }

        public int ValidationTotalErrorCount
        {
            set => throw new NotImplementedException();
        }

        public int ValidationTotalWarningCount
        {
            set => throw new NotImplementedException();
        }

        public IEnumerable<string> IgnoredRules { get; set; }
    }
}
