using System;
using System.Threading;
using Autofac;
using Autofac.Integration.ServiceFabric;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Modules;
using ESFA.DC.ILR.ValidationService.Modules.Actor;
using ESFA.DC.ILR.ValidationService.ValidationActor.Context;
using ESFA.DC.ILR.ValidationService.ValidationActor.Interfaces.Models;
using ESFA.DC.ServiceFabric.Helpers;
using Microsoft.ServiceFabric.Actors.Runtime;
using LoggerOptions = ESFA.DC.ILR.ValidationService.ValidationActor.Configuration.LoggerOptions;

namespace ESFA.DC.ILR.ValidationService.ValidationActor
{
    internal static class Program
    {
        /// <summary>
        /// This is the entry point of the service host process.
        /// </summary>
        private static void Main()
        {
            try
            {
                var builder = BuildContainer();

                // Register the Autofac magic for Service Fabric support.
                builder.RegisterServiceFabricSupport();

                // Register the actor service.
                builder.RegisterActor<ValidationActor>(settings: new ActorServiceSettings
                {
                    ActorGarbageCollectionSettings = new ActorGarbageCollectionSettings(30, 30)
                });

                using (var container = builder.Build())
                {
                    Thread.Sleep(Timeout.Infinite);
                }
            }
            catch (Exception e)
            {
                ActorEventSource.Current.ActorHostInitializationFailed(e.ToString());
                throw;
            }
        }

        private static ContainerBuilder BuildContainer()
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterModule<ActorValidationModule>();

            containerBuilder.RegisterType<ValidationActorModelValidationContextFactory>().As<IValidationContextFactory<ValidationActorModel>>();

            // register logger
            var configHelper = new ConfigurationHelper();
            var loggerOptions = configHelper.GetSectionValues<LoggerOptions>("LoggerSection");

            containerBuilder.RegisterModule(new LoggerModule(loggerOptions));

            return containerBuilder;
        }
    }
}
