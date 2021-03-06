﻿using ESFA.DC.ILR.Constants;
using ESFA.DC.ILR.ValidationService.Interface;
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

        public string Filename
        {
            get => _jobContextMessage.KeyValuePairs[ILRContextKeys.Filename].ToString();
            set => _jobContextMessage.KeyValuePairs[ILRContextKeys.Filename] = value;
        }

        public string Container => _jobContextMessage.KeyValuePairs[ILRContextKeys.Container].ToString();

        public string IlrReferenceDataKey => _jobContextMessage.KeyValuePairs[ILRContextKeys.IlrReferenceData].ToString();

        public string LearnerReferenceDataKey => _jobContextMessage.KeyValuePairs[ILRContextKeys.LearnerReferenceData].ToString();

        public string InvalidLearnRefNumbersKey => _jobContextMessage.KeyValuePairs[ILRContextKeys.InvalidLearnRefNumbers].ToString();

        public string ValidLearnRefNumbersKey => _jobContextMessage.KeyValuePairs[ILRContextKeys.ValidLearnRefNumbers].ToString();

        public string ValidationErrorsKey => _jobContextMessage.KeyValuePairs[ILRContextKeys.ValidationErrors].ToString();

        public string ValidationErrorMessageLookupKey => _jobContextMessage.KeyValuePairs[ILRContextKeys.ValidationErrorLookups].ToString();

        public string JobId => _jobContextMessage.JobId.ToString();

        public int ValidLearnRefNumbersCount
        {
            set => _jobContextMessage.KeyValuePairs[ILRContextKeys.ValidLearnRefNumbersCount] = value;
        }

        public int InvalidLearnRefNumbersCount
        {
            set => _jobContextMessage.KeyValuePairs[ILRContextKeys.InvalidLearnRefNumbersCount] = value;
        }

        public int ValidationTotalErrorCount
        {
            set => _jobContextMessage.KeyValuePairs[ILRContextKeys.ValidationTotalErrorCount] = value;
        }

        public int ValidationTotalWarningCount
        {
            set => _jobContextMessage.KeyValuePairs[ILRContextKeys.ValidationTotalWarningCount] = value;
        }

        public int ReturnPeriod => int.Parse(_jobContextMessage.KeyValuePairs[ILRContextKeys.ReturnPeriod].ToString());
    }
}
