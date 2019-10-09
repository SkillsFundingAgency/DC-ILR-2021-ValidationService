using Autofac;
using ESFA.DC.ILR.ValidationService.Rules.Derived;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;

namespace ESFA.DC.ILR.ValidationService.Stateless.Modules
{
    internal class DerivedDataModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<DerivedData_01Rule>().As<IDerivedData_01Rule>().InstancePerLifetimeScope();
            builder.RegisterType<DerivedData_04Rule>().As<IDerivedData_04Rule>().InstancePerLifetimeScope();
            builder.RegisterType<DerivedData_06Rule>().As<IDerivedData_06Rule>().InstancePerLifetimeScope();
            builder.RegisterType<DerivedData_07Rule>().As<IDerivedData_07Rule>().InstancePerLifetimeScope();
            builder.RegisterType<DerivedData_05Rule>().As<IDerivedData_05Rule>().InstancePerLifetimeScope();
            builder.RegisterType<DerivedData_11Rule>().As<IDerivedData_11Rule>().InstancePerLifetimeScope();
            builder.RegisterType<DerivedData_12Rule>().As<IDerivedData_12Rule>().InstancePerLifetimeScope();
            builder.RegisterType<DerivedData_14Rule>().As<IDerivedData_14Rule>().InstancePerLifetimeScope();
            builder.RegisterType<DerivedData_15Rule>().As<IDerivedData_15Rule>().InstancePerLifetimeScope();
            builder.RegisterType<DerivedData_17Rule>().As<IDerivedData_17Rule>().InstancePerLifetimeScope();
            builder.RegisterType<DerivedData_18Rule>().As<IDerivedData_18Rule>().InstancePerLifetimeScope();
            builder.RegisterType<DerivedData_19Rule>().As<IDerivedData_19Rule>().InstancePerLifetimeScope();
            builder.RegisterType<DerivedData_21Rule>().As<IDerivedData_21Rule>().InstancePerLifetimeScope();
            builder.RegisterType<DerivedData_22Rule>().As<IDerivedData_22Rule>().InstancePerLifetimeScope();
            builder.RegisterType<DerivedData_23Rule>().As<IDerivedData_23Rule>().InstancePerLifetimeScope();
            builder.RegisterType<DerivedData_25Rule>().As<IDerivedData_25Rule>().InstancePerLifetimeScope();
            builder.RegisterType<DerivedData_26Rule>().As<IDerivedData_26Rule>().InstancePerLifetimeScope();
            builder.RegisterType<DerivedData_27Rule>().As<IDerivedData_27Rule>().InstancePerLifetimeScope();
            builder.RegisterType<DerivedData_28Rule>().As<IDerivedData_28Rule>().InstancePerLifetimeScope();
            builder.RegisterType<DerivedData_29Rule>().As<IDerivedData_29Rule>().InstancePerLifetimeScope();
            builder.RegisterType<DerivedData_31Rule>().As<IDerivedData_31Rule>().InstancePerLifetimeScope();
            builder.RegisterType<DerivedData_32Rule>().As<IDerivedData_32Rule>().InstancePerLifetimeScope();
            builder.RegisterType<DerivedData_ValidityCategory>().As<IDerivedData_ValidityCategory>().InstancePerLifetimeScope();
        }
    }
}
