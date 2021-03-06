﻿using System;
using System.Diagnostics;
using System.Threading;
using Autofac;
using Autofac.Integration.ServiceFabric;
using ESFA.DC.FileService.Config;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Stateless.Context;
using ESFA.DC.ILR.ValidationService.Stateless.Handlers;
using ESFA.DC.ILR.ValidationService.Stateless.Modules;
using ESFA.DC.IO.AzureStorage.Config.Interfaces;
using ESFA.DC.JobContextManager;
using ESFA.DC.JobContextManager.Interface;
using ESFA.DC.JobContextManager.Model;
using ESFA.DC.JobContextManager.Model.Interface;
using ESFA.DC.Mapping.Interface;
using ESFA.DC.ServiceFabric.Common.Config;
using ESFA.DC.ServiceFabric.Common.Modules;
using ESFA.DC.ServiceFabric.Helpers;

namespace ESFA.DC.ILR.ValidationService.Stateless
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
                // The ServiceManifest.XML file defines one or more service type names.
                // Registering a service maps a service type name to a .NET type.
                // When Service Fabric creates an instance of this service type,
                // an instance of the class is created in this host process.
                var builder = BuildContainer();

                // Register the Autofac magic for Service Fabric support.
                builder.RegisterServiceFabricSupport();

                // Register the stateless service.
                builder.RegisterStatelessService<ESFA.DC.ServiceFabric.Common.Stateless>("ESFA.DC.ILR2021.ValidationService.StatelessType");

                using (var container = builder.Build())
                {
                    ServiceEventSource.Current.ServiceTypeRegistered(Process.GetCurrentProcess().Id, typeof(ESFA.DC.ServiceFabric.Common.Stateless).Name);

                    // Prevents this host process from terminating so services keep running.
                    Thread.Sleep(Timeout.Infinite);
                }
            }
            catch (Exception e)
            {
                ServiceEventSource.Current.ServiceHostInitializationFailed(e.ToString());
                throw;
            }
        }

        private static ContainerBuilder BuildContainer()
        {
            var configHelper = new ConfigurationHelper();
            var containerBuilder = new ContainerBuilder();

            var serviceFabricConfigurationService = new ServiceFabricConfigurationService();

            var statelessServiceConfiguration = serviceFabricConfigurationService.GetConfigSectionAsStatelessServiceConfiguration();
            var azureStorageFileServiceConfiguration = serviceFabricConfigurationService.GetConfigSectionAs<AzureStorageFileServiceConfiguration>("AzureStorageFileServiceConfiguration");

            containerBuilder.RegisterModule(new StatelessServiceModule(statelessServiceConfiguration));
            containerBuilder.RegisterModule<SerializationModule>();

            containerBuilder.RegisterModule<PreValidationServiceModule>();
            containerBuilder.RegisterModule(new IOModule(azureStorageFileServiceConfiguration));

            containerBuilder.RegisterType<DefaultJobContextMessageMapper<JobContextMessage>>().As<IMapper<JobContextMessage, JobContextMessage>>();
            containerBuilder.RegisterType<MessageHandler>().As<IMessageHandler<JobContextMessage>>();
            containerBuilder.RegisterType<JobContextMessageValidationContextFactory>().As<IValidationContextFactory<IJobContextMessage>>();

            containerBuilder.RegisterType<JobContextManager<JobContextMessage>>().As<IJobContextManager<JobContextMessage>>().InstancePerLifetimeScope();

            return containerBuilder;
        }
    }
}
