﻿using System.Reflection;
using Autofac;
using ESFA.DC.ILR.ValidationService.Interface;
using Module = Autofac.Module;

namespace ESFA.DC.ILR.ValidationService.Rules.Modules
{
    public class RuleSetModule<TRule, T> : Module
        where TRule : IValidationRule<T>
        where T : class
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
                .Where(t => t.IsAssignableTo<TRule>())
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();
        }
    }
}
