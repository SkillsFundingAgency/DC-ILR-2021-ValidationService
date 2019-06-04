using System;
using Autofac;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.CrossEntity;
using ESFA.DC.ILR.ValidationService.Rules.Message.FileLevel.Entity;
using ESFA.DC.ILR.ValidationService.Rules.Message.FileLevel.Header;
using ESFA.DC.ILR.ValidationService.Rules.Message.UKPRN;

namespace ESFA.DC.ILR.ValidationService.Stateless.Modules
{
    public class MessageRuleSetModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var rules = new[]
            {
                typeof(Entity_1Rule),
                typeof(Header_2Rule),
                typeof(Header_3Rule),
                typeof(R06Rule),
                typeof(R59Rule),
                typeof(R69Rule),
                typeof(R71Rule),
                typeof(R85Rule),
                typeof(R107Rule),
                typeof(R108Rule),
                typeof(UKPRN_03Rule)
            };

            builder.RegisterTypes(rules).As<IRule<IMessage>>().InstancePerLifetimeScope();
        }
    }
}
