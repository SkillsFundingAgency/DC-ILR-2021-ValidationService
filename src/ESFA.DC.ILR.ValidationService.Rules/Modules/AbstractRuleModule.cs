using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Autofac;
using ESFA.DC.ILR.ValidationService.Interface;
using Module = Autofac.Module;

namespace ESFA.DC.ILR.ValidationService.Rules.Modules
{
    public abstract class AbstractRuleModule<T> : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
                .Where(t => t.IsAssignableTo<T>())
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();
        }
    }
}
