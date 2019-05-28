using System.Collections.Generic;
using Autofac;
using ESFA.DC.ILR.ValidationService.Interface.Configuration;
using ESFA.DC.Logging;
using ESFA.DC.Logging.Config;
using ESFA.DC.Logging.Config.Interfaces;
using ESFA.DC.Logging.Enums;
using ESFA.DC.Logging.Interfaces;

namespace ESFA.DC.ILR.ValidationService.ValidationActor.Modules
{
    public class LoggerModule : Module
    {
        private readonly ILoggerOptions _loggerOptions;

        public LoggerModule(ILoggerOptions loggerOptions)
        {
            _loggerOptions = loggerOptions;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(c => new ApplicationLoggerSettings
            {
                ApplicationLoggerOutputSettingsCollection = new List<IApplicationLoggerOutputSettings>()
                {
                    new MsSqlServerApplicationLoggerOutputSettings()
                    {
                        MinimumLogLevel = LogLevel.Verbose,
                        ConnectionString = _loggerOptions.LoggerConnectionstring
                    },
                    new ConsoleApplicationLoggerOutputSettings()
                    {
                        MinimumLogLevel = LogLevel.Verbose
                    }
                }
            }).As<IApplicationLoggerSettings>().SingleInstance();

            builder.RegisterType<ExecutionContext>().As<IExecutionContext>().InstancePerLifetimeScope();
            builder.RegisterType<SerilogLoggerFactory>().As<ISerilogLoggerFactory>().InstancePerLifetimeScope();
            builder.RegisterType<SeriLogger>().As<ILogger>().InstancePerLifetimeScope();
        }
    }
}
