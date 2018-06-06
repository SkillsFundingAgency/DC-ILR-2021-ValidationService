﻿using ESFA.DC.ILR.ValidationService.Interface;

namespace ESFA.DC.ILR.ValidationService.Stateless.Models
{
    public class PreValidationContext : IPreValidationContext
    {
        public string Input { get; set; }

        public string Output { get; set; }

        public string JobId { get; set; }

        public string InvalidLearnRefNumbersKey { get; set; }

        public string ValidLearnRefNumbersKey { get; set; }

        public string ValidationErrorsKey { get; set; }

        public string ValidationErrorMessageLookupKey { get; set; }
    }
}
