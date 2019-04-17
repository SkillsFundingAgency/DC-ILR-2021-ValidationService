using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using ESFA.DC.ILR.ValidationService.Data.Population.Interface;
using ESFA.DC.ILR.ValidationService.Data.Population.Mappers;

namespace ESFA.DC.ILR.ValidationService.Modules
{
    public class DataCacheMapperModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<UlnDataMapper>().As<IUlnDataMapper>().InstancePerLifetimeScope();
        }
    }
}
