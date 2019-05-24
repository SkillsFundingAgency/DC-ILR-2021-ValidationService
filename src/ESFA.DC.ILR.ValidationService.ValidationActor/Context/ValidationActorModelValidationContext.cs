using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.ValidationActor.Interfaces.Models;
using ESFA.DC.Serialization.Json;

namespace ESFA.DC.ILR.ValidationService.ValidationActor.Context
{
    public class ValidationActorModelValidationContext : IValidationContext
    {
        public ValidationActorModelValidationContext(IEnumerable<string> ignoredRules)
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
