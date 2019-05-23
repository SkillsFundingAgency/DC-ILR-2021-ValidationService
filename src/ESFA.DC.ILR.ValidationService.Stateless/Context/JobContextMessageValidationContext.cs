using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Constants;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.JobContext.Interface;
using ESFA.DC.JobContextManager.Model.Interface;

namespace ESFA.DC.ILR.ValidationService.Stateless.Context
{
    public class JobContextMessageValidationContext : IValidationContext
    {
        private readonly IJobContextMessage _jobContextMessage;

        public JobContextMessageValidationContext(IJobContextMessage jobContextMessage)
        {
            _jobContextMessage = jobContextMessage;
        }

        public string Filename => _jobContextMessage.KeyValuePairs[ILRContextKeys.Filename].ToString();

        public string Container => _jobContextMessage.KeyValuePairs[ILRContextKeys.Container].ToString();

        public string IlrReferenceDataKey => _jobContextMessage.KeyValuePairs[JobContextMessageKey.IlrReferenceData].ToString();

        public string InvalidLearnRefNumbersKey => _jobContextMessage.KeyValuePairs[JobContextMessageKey.InvalidLearnRefNumbers].ToString();

        public string ValidLearnRefNumbersKey => _jobContextMessage.KeyValuePairs[JobContextMessageKey.ValidLearnRefNumbers].ToString();

        public string ValidationErrorsKey => _jobContextMessage.KeyValuePairs[JobContextMessageKey.ValidationErrors].ToString();

        public string ValidationErrorMessageLookupKey => _jobContextMessage.KeyValuePairs[JobContextMessageKey.ValidationErrorLookups].ToString();

        public string JobId => _jobContextMessage.JobId.ToString();

        public int ValidLearnRefNumbersCount
        {
            get => int.Parse(_jobContextMessage.KeyValuePairs[JobContextMessageKey.ValidLearnRefNumbersCount].ToString());
            set => _jobContextMessage.KeyValuePairs[JobContextMessageKey.ValidLearnRefNumbersCount] = value;
        }

        public int InvalidLearnRefNumbersCount
        {
            get => int.Parse(_jobContextMessage.KeyValuePairs[JobContextMessageKey.InvalidLearnRefNumbersCount].ToString());
            set => _jobContextMessage.KeyValuePairs[JobContextMessageKey.InvalidLearnRefNumbersCount] = value;
        }

        public int ValidationTotalErrorCount
        {
            get => int.Parse(_jobContextMessage.KeyValuePairs[JobContextMessageKey.ValidationTotalErrorCount].ToString());
            set => _jobContextMessage.KeyValuePairs[JobContextMessageKey.ValidationTotalErrorCount] = value;
        }

        public int ValidationTotalWarningCount
        {
            get => int.Parse(_jobContextMessage.KeyValuePairs[JobContextMessageKey.ValidationTotalWarningCount].ToString());
            set => _jobContextMessage.KeyValuePairs[JobContextMessageKey.ValidationTotalWarningCount] = value;
        }

        public IEnumerable<string> IgnoredRules => _jobContextMessage.Topics[_jobContextMessage.TopicPointer].Tasks.SelectMany(x => x.Tasks);
    }
}
