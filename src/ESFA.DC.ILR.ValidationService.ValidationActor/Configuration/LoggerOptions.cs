﻿using ESFA.DC.ILR.ValidationService.Interface.Configuration;

namespace ESFA.DC.ILR.ValidationService.ValidationActor.Configuration
{
    public class LoggerOptions : ILoggerOptions
    {
        public string LoggerConnectionstring { get; set; }
    }
}
